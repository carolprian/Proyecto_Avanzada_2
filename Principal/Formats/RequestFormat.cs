using AutoGens;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.Arm;

partial class Program
{
    public static void RequestFormat(string StudentID)
    {
        // Pide plantel si no es colomos no lo deja continuar
        string Plantel = WritePlantel();
        // La fecha en la que pide el permiso
        DateTime CurrentDate = DateTime.Now;
        // Regresa el ID del classroom seleccionado
        int ClassroomId = AddClassroom();
        // Busca la materia y regresa el ID
        string SubjectId = "";
        SubjectId = SearchSubjectsByName(SubjectId, 1);
        // Permite una busqueda de nombre por del profesor y regresa el ID
        string ProfessorId ="z";
        ProfessorId = SearchProfessorByName(ProfessorId, 0, 0);
        //selecciona el primer storer por defecto
        string? StorerId = AddStorer();
        if(StorerId is null)
        {
            WriteLine("There's not a storer to do your request. Please contact the coordinator or the storer");
            WriteLine("Going back to the menu...");
            return;
        }
        // Pide la fecha del prestamo
        DateTime RequestDate = AddDate(CurrentDate);
        // Pide los horarios de la clase
        var Times = AddTimes(RequestDate);
        List<string> EquipmentsId = new List<string>();
        List<byte?> StatusEquipments = new List<byte?>();
        // Agrega el registro a la linking table 
        var request = AddRequest(ClassroomId, ProfessorId, StudentID, StorerId, SubjectId);
        // Busca los equipos por el nombre y los agrega a una lista
        var equipments = SearchEquipmentsRecursive(EquipmentsId, StatusEquipments, RequestDate, Times.Item1, Times.Item2, request.requestId, 4, true);
        // Se pone 0 para establecer que no han sido aprobado el permiso
        int professorNip = 0;
        // Se pone este if, si se menciono en searchEquipments que quiere terminar el permiso y eliminarlo
        if(equipments.i == 1){
            return;
        } else {
            // Si se agrego el registro en Request
            if(request.affected > 0){
                // Agrega los datos a la tabla Request Details
                var requestDetailsId = AddRequestDetails(request.requestId, equipments.equipmentsId, professorNip, Times.Item1, Times.Item2, RequestDate, CurrentDate, equipments.statusEquipments);
                // Verifica que se haya agregado correctamente
                if(requestDetailsId.Affected.Count() >= 1){
                    WriteLine("Request added");
                } else
                {
                    WriteLine("The request was not added. Try again");
                }
            } // Si no se agrego el registro en Request
            else {
                WriteLine("The request couldnt be added. Try again.");
            }
        }
    }

    public static (List<int> Affected, List<int> RequestDetailsId) AddRequestDetails(int RequestId, List<string>? EquipmentsId, int ProfessorNip, DateTime InitTime, DateTime EndTime, DateTime RequestedDate, DateTime CurrentDate, List<byte?>? StatusEquipments){
        // Variable para establecer el índice de las listas de los nombres de los equipos y los status de estos
        int i=0;
        // Lista que almacena los requestDetails Id de los registros nuevos agregados en la tabla
        List<int>? RequestDetailsId = new List<int>();
        // Lista que almacena cuantas filas fueron afectadas en la tabla de Request Details al momento de re
        List<int>? Affecteds = new List<int>();
        // Verifica que la lista de equipmentId y statusId sean del mismo tamaño y no tengan valores nulos para poder usarlos
        if (EquipmentsId == null || StatusEquipments == null || EquipmentsId.Count != StatusEquipments.Count)
        {
            // Manejar el caso donde las listas no son válidas
            // Si no son validas muestra el mensaje y retorna los valores
            WriteLine("The equipment selected was not correctly added. Try again.");
            DeleteRequest(RequestId);
            return (Affecteds, RequestDetailsId);
        } else {
            using (bd_storage db = new()){
                // Si la tabla no existe se lanza una excepcion
                if(db.RequestDetails is null){ 
                    throw new InvalidOperationException ("The table Request Details does not exist. Verify it");
                }
                // Por cada equipo registrado en la tabla se genera una fila de datos en Request Details, donde se llenan los campos 
                //con los datos recibidos por los parametros
                foreach(var e in EquipmentsId){
                    RequestDetail rD = new() {
                        RequestId = RequestId,
                        EquipmentId = EquipmentsId[i],
                        StatusId = StatusEquipments[i],
                        ProfessorNip = ProfessorNip,
                        DispatchTime = InitTime,
                        ReturnTime = EndTime,
                        RequestedDate = RequestedDate,
                        CurrentDate = CurrentDate
                    };
                    // Se añaden los datos a la tabla
                    EntityEntry<RequestDetail> entity = db.RequestDetails.Add(rD);
                    // Se guardan los datos en la bd y el valor de las filas affectadas se guarda en la lista de affecteds
                    Affecteds.Add(db.SaveChanges());
                    // Se guardan los ID generados en la lista de ID's
                    RequestDetailsId.Add(rD.RequestDetailsId);
                    // Aumenta el indice de las listas
                    i++;
                }
            }
        }
        return (Affecteds, RequestDetailsId);
    }

    public static string WritePlantel()
    {
        WriteLine("Insert the plantel: ");
        string Plantel = ReadNonEmptyLine();
        bool Ban = false;
        do{
            Ban = VerifyPlantel(Plantel);
        }while(Ban==false);
        return Plantel;
    }

    public static bool VerifyPlantel(string Plantel){
        if(Plantel.ToLower()=="colomos"){
            return true;
        } else 
        {
            WriteLine("Plantel not registered. Try again");
            return false;
        }
    }

    public static int ListClassrooms(){
        // Indice de la lista
        int i = 0;
        using (bd_storage db = new()){
            // verifica que exista la tabla de Classroom
            if( db.Classrooms is null){
                throw new InvalidOperationException("The table does not exist.");
            } else {
                IQueryable<Classroom> Classrooms = db.Classrooms;
                foreach (var cl in Classrooms)
                {
                    WriteLine($"{i}. {cl.Clave}");
                    i++;
                }
                return Classrooms.Count();
            }
        }
    }
    public static int AddClassroom(){
        int classId=0;
        bool ban=true;
        do{
            using (bd_storage db = new()){
                int Count = ListClassrooms();
                WriteLine("Select the number of the classroom: ");
                classId = TryParseStringaEntero(ReadNonEmptyLine());
                IQueryable<Classroom> classroomsId = db.Classrooms.Where(c => c.ClassroomId==classId);
                if(classroomsId is null || !classroomsId.Any())
                {
                    WriteLine("Not a valid key. Try again");
                } else {
                    //WriteLine($"Classroom {classroomsId.Clave}");
                    ban=false;
                    return classId;
                }
            }
        }while(ban==true);
        return classId;
    }

    public static string? AddStorer(){
        string? storerId = "";
        using(bd_storage db= new()){
            IQueryable<Storer> storers = db.Storers;
            if(storers is not null && storers.Any()){
                storerId = storers.First().StorerId;
            } else {
                storerId = null;
            }
        }
        return storerId;
    }

    public static (int affected, int requestId) AddRequest(int ClassroomId, string ProfessorId, string StudentId, string StorerId, string SubjectId){
        using(bd_storage db = new()){
            if(db.Requests is null){ return(0, -1);}
            Request r  = new Request()
            {
                ClassroomId = ClassroomId,
                ProfessorId = ProfessorId,
                StudentId = StudentId,
                StorerId = StorerId,
                SubjectId = SubjectId
            };

            EntityEntry<Request> entity = db.Requests.Add(r);
            int affected = db.SaveChanges();
            return (affected, r.RequestId);
        }
    }

    public static DateTime AddDate(DateTime CurrentDate)
    {
        DateTime dateValue= DateTime.MinValue;
        bool valideDate = false;
        while (valideDate==false)
        {
            WriteLine("Insert the date of the class: yyyy/MM/dd");
            string dateInput = ReadNonEmptyLine();
            if (DateTime.TryParseExact(dateInput, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            {
                if(dateValue > CurrentDate.Date && CurrentDate.AddDays(14) >= dateValue.Date){
                    if (dateValue.DayOfWeek != DayOfWeek.Saturday && dateValue.DayOfWeek != DayOfWeek.Sunday )
                    {
                        valideDate = true;
                    } else {
                        WriteLine("The request must be between monday to friday");
                    }
                }else{
                    WriteLine("It must be one day appart of the minimum and 2 weeks maximum. Try again");
                }
            } else{
                WriteLine("The format must be yyyy/mm/dd. Try again.");
            }
        }
        return dateValue;
    }

    public static (DateTime, DateTime) AddTimes(DateTime DateValue){
        int scheduleIdInitI = 0, scheduleIdEndI = 0, offset=0, take=9;
        DateTime initTimeValue=DateTime.MinValue, endTimeValue=DateTime.MinValue;
        DayOfWeek day = DateValue.DayOfWeek;
        string dayString = day.ToString(), scheduleIdInit, scheduleIdEnd;
        bool valideHours = false;
        bool initEnd=false; //falso para init, true para end
        while(valideHours==false){
            using (bd_storage db = new()){
                IQueryable<Schedule> schedules = db.Schedules;
                while(initEnd==false){
                    switch(day){
                        case DayOfWeek.Monday:{
                            offset=0;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{sch.ScheduleId.ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class start hour");
                            scheduleIdInit = ReadNonEmptyLine();
                            scheduleIdInitI = TryParseStringaEntero(scheduleIdInit);
                            scheduleIdInitI += offset;
                            if(scheduleIdInitI >= offset && (offset+10) >= scheduleIdInitI){
                                WriteLine("Class start hour added");
                                initEnd=true;
                            } else {
                                WriteLine("Option not valide. Try again.");
                            }
                        }break;
                        case DayOfWeek.Tuesday:{
                            offset=10;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{(sch.ScheduleId-offset).ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class start hour");
                            scheduleIdInit = ReadNonEmptyLine();
                            scheduleIdInitI = TryParseStringaEntero(scheduleIdInit);
                            scheduleIdInitI += offset;
                            if((scheduleIdInitI >= offset) && ((offset+10) >= scheduleIdInitI)){
                                WriteLine("Class start hour added");
                                initEnd=true;
                            }
                        }break;
                        case DayOfWeek.Wednesday:{
                            offset=20;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{(sch.ScheduleId-offset).ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class start hour");
                            scheduleIdInit = ReadNonEmptyLine();
                            scheduleIdInitI = TryParseStringaEntero(scheduleIdInit);
                            scheduleIdInitI += offset;
                            if(scheduleIdInitI >= offset && (offset+10) >= scheduleIdInitI){
                                WriteLine("Class start hour added");
                                initEnd=true;
                            }
                        }break;
                        case DayOfWeek.Thursday:{
                            offset=30;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{(sch.ScheduleId-offset).ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class start hour");
                            scheduleIdInit = ReadNonEmptyLine();
                            scheduleIdInitI = TryParseStringaEntero(scheduleIdInit);
                            scheduleIdInitI += offset;
                            if(scheduleIdInitI >= offset && (offset+10) >= scheduleIdInitI){
                                WriteLine("Class start hour added");
                                initEnd=true;
                            }
                        }break;
                        case DayOfWeek.Friday:{
                            offset=40;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{(sch.ScheduleId-offset).ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class start hour");
                            scheduleIdInit = ReadNonEmptyLine();
                            scheduleIdInitI = TryParseStringaEntero(scheduleIdInit);
                            scheduleIdInitI += offset;
                            if(scheduleIdInitI >= offset && (offset+10) >= scheduleIdInitI){
                                WriteLine("Class start hour added");
                                initEnd=true;
                            }
                        }break;
                    }
                }
                while(initEnd==true){
                    switch(day){
                        case DayOfWeek.Monday:{
                            offset = 1;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{sch.ScheduleId.ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class end hour");
                            scheduleIdEnd = ReadNonEmptyLine();
                            scheduleIdEndI = TryParseStringaEntero(scheduleIdEnd);
                            scheduleIdEndI += offset;
                            if(scheduleIdEndI >= offset && (offset+take) >= scheduleIdEndI){
                                WriteLine("Class end hour added");
                                initEnd=false;
                            }
                        }break;
                        case DayOfWeek.Tuesday:{
                            offset=11;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{(sch.ScheduleId-offset).ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class end hour");
                            scheduleIdEnd = ReadNonEmptyLine();
                            scheduleIdEndI = TryParseStringaEntero(scheduleIdEnd);
                            scheduleIdEndI += offset;
                            if(scheduleIdEndI >= offset && (offset+take) >= scheduleIdEndI){
                                WriteLine("Class end hour added");
                                initEnd=false;
                            }
                        }break;
                        case DayOfWeek.Wednesday:{
                            offset=21;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{(sch.ScheduleId-offset).ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class end hour");
                            scheduleIdEnd = ReadNonEmptyLine();
                            scheduleIdEndI= TryParseStringaEntero(scheduleIdEnd) + offset;
                            if(scheduleIdEndI >= offset && (offset+take) >= scheduleIdEndI){
                                WriteLine("Class end hour added");
                                initEnd=false;
                            }
                        }break;
                        case DayOfWeek.Thursday:{
                            offset=31;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{(sch.ScheduleId-offset).ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class end hour");
                            scheduleIdEnd = ReadNonEmptyLine();
                            scheduleIdEndI= TryParseStringaEntero(scheduleIdEnd) + offset;
                            if(scheduleIdEndI >= offset && (offset+take) >= scheduleIdEndI){
                                WriteLine("Class end hour added");
                                initEnd=false;
                            }
                        }break;
                        case DayOfWeek.Friday:{
                            offset=41;
                            var saltos = schedules.Skip(offset).Take(take);
                            foreach(var sch in saltos){
                                WriteLine($"{(sch.ScheduleId-offset).ToString()}. {sch.InitTime.ToString("HH:mm")}");
                            }
                            WriteLine($"Select the number to choose the class end hour");
                            scheduleIdEnd = ReadNonEmptyLine();
                            scheduleIdEndI= TryParseStringaEntero(scheduleIdEnd) + offset;
                            if(scheduleIdEndI >= offset && (offset+take) >= scheduleIdEndI){
                                WriteLine("Class end hour added");
                                initEnd=false;
                            }
                        }break;
                    }
                }

                if (scheduleIdInitI > scheduleIdEndI){
                    WriteLine($"It can't be first the end of the class. Try again.");
                } else if((scheduleIdEndI - scheduleIdInitI) < 1 || (scheduleIdEndI - scheduleIdInitI) > 5 ) {
                    WriteLine($"The bare minium of loans are 50 min and maximun are 3 hours and 20 minuts"); 
                } else {
                    IQueryable<Schedule>? startHour = db.Schedules.Where(s => s.ScheduleId == scheduleIdInitI);
                    if(startHour is null || !startHour.Any()){
                        WriteLine("Wrong start hour. Try again");
                        initEnd=false;
                    } else 
                    {
                        initTimeValue = DateValue.Date + startHour.FirstOrDefault().InitTime.TimeOfDay;

                        IQueryable<Schedule> finHour = db.Schedules.Where(s => s.ScheduleId == scheduleIdEndI);
                        if(finHour is null || !finHour.Any()){
                            WriteLine("Wrong end hour. Try again");
                            initEnd=false;
                        } else {
                            endTimeValue = DateValue.Date + finHour.FirstOrDefault().InitTime.TimeOfDay;
                            valideHours=true;
                        }
                    }
                }
            }
        }
        return (initTimeValue, endTimeValue);
    }

    public static void DeleteRequest(int? requestId)
    {
        using (bd_storage db = new())
        {
            var request = db.Requests
                    .Where(r => r.RequestId == requestId)
                    .FirstOrDefault();

            if (request != null)
            {
                db.Requests.Remove(request);
                int affected = db.SaveChanges();

                if (affected > 0)
                {
                    WriteLine("Request successfully deleted");
                }
                else
                {
                    WriteLine("Request couldn't be deleted");
                }
            }
            else
            {
                WriteLine("Request ID not found in the database");
            }
        }
    }

    public static void DeleteRequestFormat(string Username)
    {
        WriteLine("Here's a list of all the request format that has not been accepted yet. ");
        ViewRequestFormatNotAcceptedYet(Username);

        while (true)
        {
            WriteLine();
            WriteLine("Provide the ID of the request that you want to delete (check the list): ");
            int detailsId = Convert.ToInt32(ReadLine());

            using (bd_storage db = new())
            {
                var requestDetail = db.RequestDetails
                .FirstOrDefault(e => e.RequestDetailsId == detailsId);

                // Obtén el RequestId asociado
                int? requestId = requestDetail.RequestId;

                var request = db.Requests
                .FirstOrDefault(r => r.RequestId == requestId);


                if (requestDetail == null)
                {
                    WriteLine("That request ID doesn't exist in the database, try again");
                    continue; // Vuelve al inicio del bucle
                }
                else
                {
                    // Elimina el registro de RequestDetails
                    db.RequestDetails.Remove(requestDetail);
                    int affectedDetails = db.SaveChanges();

                    if (affectedDetails > 0)
                    {
                        WriteLine("RequestDetail successfully deleted");
                        db.Requests.Remove(request);
                        int affectedRquest= db.SaveChanges();

                        if (affectedRquest == 1)
                        {
                            WriteLine("Request successfully deleted");
                        }
                        else
                        {
                            WriteLine("Request couldn't be deleted");
                        }
                        
                    }
                    else
                    {
                        WriteLine("RequestDetail couldn't be deleted");
                    }

                }
            }

            return;
        }
    }

    public static void UpdateRequestFormat(string Username){
        int i=1, affected = 0, op=0;
        bool validateRequest=false;
        DateTime request=DateTime.Today;
        WriteLine("Here's a list of all the request format that has not been accepted yet. ");
        ViewRequestFormatNotAcceptedYet(Username);
        WriteLine();
        WriteLine("Provide the ID of the request that you want to modify (check the list): ");
        int requestID = Convert.ToInt32(ReadNonEmptyLine());
        using(bd_storage db = new bd_storage()){
            IQueryable<Request> requestss = db.Requests
                .Include(rd => rd.Classroom)
                .Include(rd => rd.Professor)
                .Include(rd => rd.Subject)
                .Include(rd => rd.Student).Where( rd => rd.RequestId==requestID);
            IQueryable<RequestDetail> requestDetailss = db.RequestDetails
                .Include(rd => rd.Status)
                .Include(rd=> rd.Equipment)
                .Where(r => r.RequestId==requestID).Where(r=> r.ProfessorNip==0);
            var requestList = requestDetailss.ToList();
            List <Equipment> listEquipments= new List<Equipment>();
            do{
                if(requestss is not null && requestss.Any() && 
                requestDetailss is not null && requestDetailss.Any() ){
                    WriteLine("These are the fields you can update:");
                    WriteLine($"{i}. Classroom: {requestss.First().Classroom.Name}");
                    WriteLine($"{i+1}. Professor: {requestss.First().Professor.Name} {requestss.First().Professor.LastNameP}");
                    WriteLine($"{i+2}. Subject: {requestss.First().Subject.Name}");
                    WriteLine($"{i+3}. Date of the request: {requestDetailss.First().RequestedDate.Date}");
                    WriteLine($"{i+4}. Dispatch time: {requestDetailss.First().DispatchTime.TimeOfDay} and Return time: {requestDetailss.First().ReturnTime.TimeOfDay}");
                    WriteLine($"{i+5}. Equipment(s) in the request:");
                    foreach (var requestDetail in requestDetailss)
                    {
                        WriteLine($"     -{requestDetail.Equipment.EquipmentId} ({requestDetail.Equipment.Name})");
                        listEquipments.Add(requestDetail.Equipment);
                        i++;
                    }
                    WriteLine("Select an option to modify");
                    op = Convert.ToInt32(ReadNonEmptyLine());
                    validateRequest=true;
                }
                else {
                    Console.Clear();
                    WriteLine("Request not found. Try again.");
                    WriteLine("Here's a list of all the request format that has not been accepted yet. ");
                    ViewRequestFormatNotAcceptedYet(Username);
                    WriteLine();
                    WriteLine("Provide the ID of the request that you want to modify (check the list): ");
                    requestID = Convert.ToInt32(ReadNonEmptyLine());
                    requestss = db.Requests
                    .Include(rd => rd.Classroom)
                    .Include(rd => rd.Professor)
                    .Include(rd => rd.Subject)
                    .Include(rd => rd.Student).Where( rd => rd.RequestId==requestID);
                    requestDetailss = db.RequestDetails
                    .Include(rd => rd.Status)
                    .Include(rd=> rd.Equipment)
                    .Where(r => r.RequestId==requestID).Where(r=> r.ProfessorNip==0);
                        requestList = requestDetailss.ToList();
                        listEquipments= new List<Equipment>();
                }
            } while (validateRequest==false);
            switch(op)
            {
                case 1:
                {
                    int classroomId = AddClassroom();
                    requestss.First().ClassroomId=classroomId;
                    affected = db.SaveChanges();
                    if(affected>0){
                        WriteLine("Request changed");
                    }
                    else {
                        WriteLine("Request not changed");
                    }
                }break;
                case 2:
                {
                    string professorId = SearchProfessorByName("xyz", 0, 0);
                    requestss.First().ProfessorId = professorId;
                    affected = db.SaveChanges();
                    if(affected>0){
                        WriteLine("Request changed");
                    }
                    else {
                        WriteLine("Request not changed");
                    }
                }break;
                case 3:
                {
                    string subjectId = SearchSubjectsByName("xyz", 1);
                    requestss.First().SubjectId = subjectId;
                    if(affected>0){
                        WriteLine("Request changed");
                    }
                    else {
                        WriteLine("Request not changed");
                    }
                }break;
                case 4:
                {
                    DateTime newDate = AddDate(DateTime.Now.Date);
                    foreach (var requestDetail in requestDetailss)
                    {
                        requestDetail.RequestedDate = newDate;
                    }
                    affected = db.SaveChanges();
                    if(affected>0){
                        WriteLine("Request changed");
                    }
                    else {
                        WriteLine("Request not changed");
                    }
                }break;
                case 5:
                {
                    var times = AddTimes(request);
                    foreach (var requestDetail in requestDetailss)
                    {
                        requestDetail.DispatchTime = times.Item1;
                        requestDetail.ReturnTime = times.Item2;
                    }
                    affected = db.SaveChanges();
                    if(affected>0){
                        WriteLine("Request changed");
                    }
                    else {
                        WriteLine("Request not changed");
                    }
                } break;
                case 6:
                {
                    i = 1;
                    int equipId = 0;
                    foreach (var e in listEquipments)
                    {
                        WriteLine($"{i}. {e.EquipmentId}-{e.Name}");
                    }
                    WriteLine("Select the number of the equipment");
                    bool validateEq = false;
                    do{
                        try
                        {
                            equipId = Convert.ToInt32(ReadNonEmptyLine());
                            if(equipId>0 && equipId<=listEquipments.Count())
                            {
                                validateEq=true;
                            }
                        }
                        catch (FormatException)
                        {
                            WriteLine("That is not a correct option, try again.");
                        }
                        catch (OverflowException)
                        {
                            WriteLine("That is not a correct option, try again.");
                        }
                    }while (validateEq==false);

                    var selectedEquipment = listEquipments[equipId - 1];
                    List<string> equipmentsId = new List<string>();
                    List<byte?> statusIds = new List<byte?>();
                    var updatedEquipments = SearchEquipmentsRecursive(
                        equipmentsId,
                        statusIds,
                        requestDetailss.First().RequestedDate,
                        requestDetailss.First().DispatchTime,
                        requestDetailss.First().ReturnTime,
                        requestDetailss.First().RequestId,
                        1, 
                        true
                    );

                    // Obtener los valores del nuevo equipo seleccionado

                    foreach (var requestDetail in requestDetailss)
                    {
                        // Cambiar el equipo en la tabla de la base de datos
                        requestDetail.EquipmentId = updatedEquipments.equipmentsId.First();
                        // Cambiar el estado del equipo conforme al nuevo equipo seleccionado
                        requestDetail.StatusId = updatedEquipments.statusEquipments.First();
                    }

                    affected = db.SaveChanges();
                    if(affected>0){
                        WriteLine("Request changed");
                    }
                    else {
                        WriteLine("Request not changed");
                    }
                    break;
                }

                case 7:
                {
                    return;
                }
                default:{
                    WriteLine("Not a valide option. Try again.");
                }break;
            }
        }
    }
}