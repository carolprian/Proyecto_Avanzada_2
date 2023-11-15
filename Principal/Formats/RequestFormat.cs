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
        string Plantel="";
        bool Ban = false;
        do{
            WriteLine("Insert the plantel: ");
            Plantel = ReadNonEmptyLine();
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

    public static int ListClassrooms()
    {
        // Indice de la lista
        int i = 1;

        using (bd_storage db = new())
        {
            // verifica que exista la tabla de Classroom
            if( db.Classrooms is null)
            {
                throw new InvalidOperationException("The table does not exist.");
            } 
            else
            {
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
            using (bd_storage db = new())
            {
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

    public static (DateTime, DateTime) AddTimes(DateTime dateValue)
    {
        // Variables para almacenar los identificadores de las horas de inicio y fin,
        // así como las horas de inicio y fin reales.
        int scheduleIdInit = 0, scheduleIdEnd = 0, take = 9;
        DateTime initTimeValue = DateTime.MinValue, endTimeValue = DateTime.MinValue;

        // Obtener el día de la semana de la fecha proporcionada.
        DayOfWeek day = dateValue.DayOfWeek;

        // Indicador para saber si estamos seleccionando la hora de inicio o fin.
        bool validHours = false;
        bool selectingInit = true;

        // Bucle principal para la selección de horas.
        while (!validHours)
        {
            // Se utiliza el contexto de base de datos con una declaración 'using' para
            // asegurar que se libere correctamente después de su uso.
            using (bd_storage db = new())
            {
                IQueryable<Schedule> schedules = db.Schedules;

                // Obtener todas las horas de inicio o fin disponibles para el día y la fase de selección.
                int offset = GetOffset(day, selectingInit);
                var selectedSchedules = schedules.Skip(offset).Take(take);

                // Mostrar las opciones de horas de inicio/fin disponibles.
                // Le paso el horario del inicio de la clase para que no le muestre horas menores para elegir el final de la clase
                ShowScheduleOptions(selectedSchedules, offset, scheduleIdInit);

                // Leer la entrada del usuario para seleccionar una opción.
                string scheduleIdInput = ReadNonEmptyLine();
                int selectedScheduleId = TryParseStringaEntero(scheduleIdInput) + offset;

                // Validar la opción seleccionada.
                if (IsValidScheduleId(selectedScheduleId, offset, offset + take) && 
                    (selectingInit || selectedScheduleId > scheduleIdInit))
                {
                    // Procesar la opción seleccionada.
                    if (selectingInit)
                    {
                        WriteLine("Class start hour added");
                        scheduleIdInit = selectedScheduleId;
                    }
                    else
                    {
                        WriteLine("Class end hour added");
                        scheduleIdEnd = selectedScheduleId;
                    }

                    // Cambiar a la fase de selección opuesta.
                    selectingInit = !selectingInit;

                    // Si se han seleccionado ambas horas, verificar y calcular las horas de inicio y fin.
                    if (scheduleIdInit > 0 && scheduleIdEnd > 0)
                    {
                        if (scheduleIdInit < scheduleIdEnd)
                        {
                            validHours = true;
                            InitAndEndTimesFromIds(scheduleIdInit, scheduleIdEnd, dateValue, out initTimeValue, out endTimeValue);
                        }
                        else
                        {
                            WriteLine("It can't be first the end of the class. Try again.");
                        }
                    }
                }
                else
                {
                    WriteLine("Invalid option. Try again.");
                }
            }
        }

        // Devolver las horas de inicio y fin seleccionadas.
        return (initTimeValue, endTimeValue);
    }



    private static int GetOffset(DayOfWeek day, bool selectingInit)
    {
        // Método para determinar el desplazamiento (offset) en función del día de la semana y
        // de si estamos seleccionando la hora de inicio o fin.
        int offset = 0;

        switch (day)
        {
            // Los números específicos son asignados para evitar superposiciones y garantizar la coherencia
            // "condición ? expresión_si_verdadero : expresión_si_falso"
            case DayOfWeek.Monday:
                offset = selectingInit ? 0 : 1; 
            break;

            case DayOfWeek.Tuesday:
                offset = selectingInit ? 10 : 11; 
            break;
            
            case DayOfWeek.Wednesday:
                offset = selectingInit ? 20 : 21; 
            break;

            case DayOfWeek.Thursday: 
                offset = selectingInit ? 30 : 31;
            break;

            case DayOfWeek.Friday: 
                offset = selectingInit ? 40 : 41; 
            break;
        }

        return offset;
    }

    private static void ShowScheduleOptions(IQueryable<Schedule> schedules, int offset, int startHourId)
    {
        // Filtrar las opciones de horario para mostrar solo aquellas que sean mayores que la hora de inicio seleccionada.
        var filteredSchedules = schedules.Where(s => s.ScheduleId > startHourId);

        foreach (var sch in filteredSchedules)
        {
            int hour = sch.InitTime.Hour;
            int minute = sch.InitTime.Minute;
            WriteLine($"{sch.ScheduleId - offset}. {hour:D2}:{minute:D2}");
        }

        string prompt = "Select the number to choose the class end hour";
        WriteLine(prompt);
    }


    private static bool IsValidScheduleId(int selectedScheduleId, int min, int max)
    {
        // Método para validar si el identificador de la hora seleccionada es válido.
        return selectedScheduleId >= min && selectedScheduleId <= max;
    }

    private static void InitAndEndTimesFromIds(int initId, int endId, DateTime dateValue, out DateTime initTime, out DateTime endTime)
    {
        // Método para obtener las horas de inicio y fin reales a partir de los identificadores seleccionados.
        using (bd_storage db = new())
        {
            var initSchedule = db.Schedules.FirstOrDefault(s => s.ScheduleId == initId);
            var endSchedule = db.Schedules.FirstOrDefault(s => s.ScheduleId == endId);

            // Calcular las horas de inicio y fin basadas en las horas de inicio de la base de datos.
            initTime = dateValue.Date + (initSchedule?.InitTime ?? DateTime.MinValue).TimeOfDay;
            endTime = dateValue.Date + (endSchedule?.InitTime ?? DateTime.MinValue).TimeOfDay;

        }
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