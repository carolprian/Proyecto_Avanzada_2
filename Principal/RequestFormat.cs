using AutoGens;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;

partial class Program
{
    public static void RequestFormat(string username)
    {
        
        //string plantel = AddPlantel();
        DateTime currentDate = DateTime.Now;
        var student = AddStudent(username);
        string studentId = student.Item1;
        int groupId = student.Item2;
        int classroomId = AddClassroom();
        string subjectId = SearchSubjectsByName("a", 1);
        string professorId = SearchProfessorByName("A", 0, 0);
        string? storerId = AddStorer();
        if(storerId is null)
        {
            WriteLine("There's not a storer to do your request. Please contact the coordinator or the storer");
            WriteLine("Going back to the menu...");
            return;
        }
        var request = AddRequest(classroomId, professorId, studentId, groupId, storerId, subjectId);
        if(request.affected > 0){
            WriteLine("Professor added");
        }
        DateTime requestDate = AddDate(currentDate);
        var times = AddTimes(requestDate);
        List<string> equipmentsId = new List<string>();
        List<byte?> statusEquipments = new List<byte?>();
        var equipments = SearchEquipmentsRecursive(equipmentsId, statusEquipments, requestDate, times.Item1, times.Item2);
        string professorNip = "0";
        var requestDetailsId = AddRequestDetails(request.requestId, equipments.equipmentsId, professorNip, times.Item1, times.Item2, requestDate, currentDate, equipments.statusEquipments);
    }
    public static (List<int> affected, List<int> requestDetailsId) AddRequestDetails(int requestId, List<string> equipmentsId, string professorNip, DateTime initTime, DateTime endTime, DateTime requestedDate, DateTime currentDate, List<byte?> statusEquipments){
        int i=0;
        List<int> requestDetailsId = new List<int>();
        List<int> affecteds = new List<int>();
        if (equipmentsId == null || statusEquipments == null || equipmentsId.Count != statusEquipments.Count)
        {
            // Manejar el caso donde las listas no son válidas
            return (affecteds, requestDetailsId);
        }
        using (bd_storage db = new()){
            if (!db.Database.EnsureCreated())
            {
                // La tabla no existe, puede manejar esto según tus necesidades
                // Por ejemplo, lanzar una excepción, crear la tabla manualmente, etc.
                return (affecteds, requestDetailsId);
            }
            foreach(var e in equipmentsId){
                RequestDetail rD = new() {
                    RequestId = requestId,
                    EquipmentId = equipmentsId[i],
                    StatusId = statusEquipments[i],
                    ProfessorNip = professorNip,
                    DispatchTime = initTime,
                    ReturnTime = endTime,
                    RequestedDate = requestedDate,
                    CurrentDate = currentDate
                };
                EntityEntry<RequestDetail> entity = db.RequestDetails.Add(rD);
                affecteds[i] = db.SaveChanges();
                requestDetailsId[i] = rD.RequestDetailsId;
                i++;
            }
        }
        return (affecteds, requestDetailsId);
    }

    public static string AddPlantel(){
        bool ban=false;
        string? plantel = "nada";
        do{
            WriteLine("Insert the plantel: ");
            plantel = ReadNonEmptyLine();
            if(plantel.ToLower()=="colomos"){
                ban=true;
                return plantel;
            } else 
            {
                WriteLine("Plantel not registered. Try again");
            }
            WriteLine($"plantel: {plantel}");
        }while(ban==false);
        return plantel;
    }

    public static (string, int) AddStudent(string username){
        int groupId=0;
        using(bd_storage db = new()){
            IQueryable<Student> students = db.Students.Where(s => s.StudentId == username);
            var studentss = students.FirstOrDefault();
            if(students is not null && students.Any()){
                IQueryable<Group> groups = db.Groups.Where(g => g.GroupId == studentss.GroupId);
                var groupss = groups.FirstOrDefault();
                if(groups is not null && groups.Any())
                {
                    username = studentss.StudentId;
                    groupId = groupss.GroupId;
                }
            } 
        }
        return (username, groupId);
    }

    public static int AddClassroom(){
        int i=0, classId=0;
        bool ban=true;
        do{
            using (bd_storage db = new()){
                IQueryable<Classroom> classrooms = db.Classrooms;
                var result = classrooms; 
                i=0;
                foreach (var cl in result)
                {
                    WriteLine($"{i}. {cl.Clave}");
                    i++;
                }
                WriteLine("Select the number of the classroom: ");
                string classroom = ReadNonEmptyLine();
                classId = TryParseStringaEntero(classroom);
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

    public static string SearchSubjectsByName(string searchTerm, int op)
    {
        int i = 0;
        if(op==1){
        WriteLine("Insert the name start of the subject WITHOUT accents");
        } 
        searchTerm = ReadNonEmptyLine();
        using (bd_storage db = new())
        {
            IQueryable<Subject>? subjects = db.Subjects.Where(s => s.Name.ToLower().StartsWith(searchTerm.ToLower()));
            if (!subjects.Any() || subjects is null)
            {
                WriteLine("No subjects found matching the search term: " + searchTerm + "Try again.");
                return SearchSubjectsByName(searchTerm, 1);
            } else {
                i=1;
                if (subjects.Count() == 1)
                {
                    // Si encontramos una única materia, la retornamos
                    return subjects.FirstOrDefault().SubjectId;
                }
                else if(subjects.Count() >1){
                    foreach(var s in subjects){
                        WriteLine($"{i}. {s.Name}");
                        i++;
                    }
                    // Si hay más de una materia que coincide, pedimos el nombre completo
                    WriteLine("Insert the whole name of the subject to confirm");
                    return SearchSubjectsByName(searchTerm, 2);
                }
                else {
                    // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                    WriteLine("Subject not found. Try again.");
                    return SearchSubjectsByName(searchTerm, 1);
                }
            }
        }
    }

    public static string SearchProfessorByName(string searchTerm, int op, int recursive){
        int i = 1;
        if (op==0){
            WriteLine("Insert the names of the professor WITHOUT accents");
            searchTerm = ReadNonEmptyLine();
            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors.Where(s => s.Name.ToLower().StartsWith(searchTerm.ToLower()));
                if (!professors.Any())
                {
                    WriteLine($"No professors found matching the search term: {searchTerm}. Try again");
                    return SearchProfessorByName(searchTerm, 0, 0);
                } else {
                    if (professors.Count() == 1)
                    {
                        WriteLine("Professor found");
                        // Si encontramos una única materia, la retornamos
                        return professors.FirstOrDefault().ProfessorId;
                    }
                    else if(professors.Count() >1){
                        foreach(var s in professors){
                            WriteLine($"{i}. {s.Name} {s.LastNameP} {s.LastNameM}");
                            i++;
                        }
                        // Si hay más de una materia que coincide, pedimos el nombre completo
                        WriteLine("Insert the PATERN last name of the teacher WITHOUT accents");
                        string searchTermNew = ReadNonEmptyLine();
                        return SearchProfessorByName(searchTermNew, 1, 0);
                    }
                    else {
                        // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(searchTerm, 0, 0);
                    }
                }
            }
        } else if(op==1) {
            if(recursive==0){
                WriteLine("Insert the PATERN last name of the teacher WITHOUT accents");
            }
            searchTerm = ReadNonEmptyLine();
            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors.Where(s => s.LastNameP.StartsWith(searchTerm));
                if (!professors.Any() || professors is null)
                {
                    WriteLine($"No professors found matching the patern last name: {searchTerm}. Try again");
                    return SearchProfessorByName(searchTerm, 1, 0);
                } else {
                    if (professors.Count() == 1)
                    {
                        // Si encontramos un único profesor, lo retornamos
                        WriteLine("Professor found");
                        return professors.FirstOrDefault().ProfessorId;
                    }
                    else if(professors.Count() > 1){
                        foreach(var s in professors){
                            WriteLine($"{i}. {s.Name} {s.LastNameP} {s.LastNameM}");
                            i++;
                        }
                        // Si hay más de una materia que coincide, pedimos el apellido
                        WriteLine("Insert the MATERN last name of the teacher WITHOUT accents");
                        string searchTermNew = ReadNonEmptyLine();
                        return SearchProfessorByName(searchTermNew, 2, 0);
                    }
                    else {
                        // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(searchTerm, 0, 0);
                    }
                }
            }
        } else
        {
            if(recursive==0){
                WriteLine("Insert the MATERN last name of the teacher WITHOUT accents");
            }
            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors
                    .Where(s => s.LastNameM.StartsWith(searchTerm));
                db.ChangeTracker.LazyLoadingEnabled = false;
                if (!professors.Any())
                {
                    WriteLine($"No professors found matching the matern last name: {searchTerm}. Try again");
                    return SearchProfessorByName(searchTerm, 2, 0);
                } else {
                    if (professors.Count() == 1)
                    {
                        // Si encontramos un único profesor, lo retornamos
                        WriteLine("Professor found");
                        return professors.FirstOrDefault().ProfessorId;
                    }
                    else {
                        // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(searchTerm, 0, 0);
                    }
                }
            }
        }
    }

    public static string? AddStorer(){
        string? storerId = "";
        using(bd_storage db= new()){
            IQueryable<Storer> storers = db.Storers;
            if(storers is not null && storers.Any()){
                storerId = storers.FirstOrDefault().StorerId;
            } else {
                storerId = null;
            }
        }
        return storerId;
    }

    public static (int affected, int requestId) AddRequest(int classroomId, string professorId, string studentId, int groupId, string storerId, string subjectId){
        using(bd_storage db = new()){
            if(db.Requests is null){ return(0, -1);}
            Request r  = new Request()
            {
                ClassroomId = classroomId,
                ProfessorId = professorId,
                StudentId = studentId,
                StorerId = storerId,
                SubjectId = subjectId
            };

            EntityEntry<Request> entity = db.Requests.Add(r);
            int affected = db.SaveChanges();
            return (affected, r.RequestId);
        }
    }

    public static DateTime AddDate(DateTime currentDate)
    {
        DateTime dateValue= DateTime.MinValue;
        bool valideDate = false;
        while (valideDate==false)
        {
            WriteLine("Insert the date of the class: yyyy/MM/dd");
            string dateInput = ReadNonEmptyLine();
            if (DateTime.TryParseExact(dateInput, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            {
                if(dateValue > currentDate.Date && currentDate.AddDays(14) >= dateValue.Date){
                    if (dateValue.DayOfWeek != DayOfWeek.Saturday && dateValue.DayOfWeek != DayOfWeek.Sunday )
                    {
                        valideDate = true;
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

    public static (DateTime, DateTime) AddTimes(DateTime dateValue){
        int scheduleIdInitI = 0, scheduleIdEndI = 0, offset=0, take=9;
        DateTime initTimeValue=DateTime.MinValue, endTimeValue=DateTime.MinValue;
        DayOfWeek day = dateValue.DayOfWeek;
        string dayString = day.ToString(), scheduleIdInit, scheduleIdEnd;
        bool valideHours = false, initEnd=false; //falso para init, true para end
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
                            WriteLine($"Id + offset {scheduleIdInitI}");
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
                    IQueryable<Schedule> startHour = db.Schedules.Where(s => s.ScheduleId == scheduleIdInitI);
                    if(startHour is null || !startHour.Any()){
                        WriteLine("Wrong start hour. Try again");
                        initEnd=false;
                    } else 
                    {
                        initTimeValue = startHour.FirstOrDefault()?.InitTime ?? dateValue.Date;

                        IQueryable<Schedule> finHour = db.Schedules.Where(s => s.ScheduleId == scheduleIdEndI);
                        if(finHour is null || !finHour.Any()){
                            WriteLine("Wrong end hour. Try again");
                            initEnd=false;
                        } else {
                            endTimeValue = finHour.FirstOrDefault()?.InitTime ?? dateValue.Date;
                            valideHours=true;
                        }
                    }
                }
            }
        }
        return (initTimeValue, endTimeValue);
    }

    public static (List<string> equipmentsId, List<byte?> statusEquipments) SearchEquipmentsRecursive(List<string> selectedEquipments, List<byte?> statusEquipments, DateTime requested, DateTime init, DateTime end)
        {
            const int maxEquipment = 4;
            string response = "h";
            using (bd_storage db = new())
            {
                WriteLine("Insert the start of the name of equipment without accents: ");
                string searchTerm = ReadNonEmptyLine();

                IQueryable<Equipment>? equipments = db.Equipments
                    .Include(e => e.Status)
                    .Where(e => e.Name.StartsWith(searchTerm))
                    .Where(e => !db.RequestDetails
                        .Any(rd => rd.EquipmentId == e.EquipmentId &&
                                rd.StatusId != 3 && rd.StatusId != 4 && rd.StatusId != 5 &&
                                ((requested == rd.RequestedDate.Date) ||
                                    (init <= rd.ReturnTime) || end <= rd.ReturnTime)));

                if (!equipments.Any() || equipments.Count() < 1 || equipments is null)
                {
                    WriteLine("No equipment found matching the search term: " + searchTerm + " Try again.");
                    SearchEquipmentsRecursive(selectedEquipments, statusEquipments, requested, init, end);
                }
                else if (equipments.Count() > 1)
                {
                    do{
                    // Si hay más de una opción, seleccionar una al azar
                        var randomEquipment = equipments.OrderBy(e => Guid.NewGuid()).First();

                        WriteLine("| {0,-11} | {1,-15} | {2,-80} | {3, 17}",
                            "EquipmentId", "Name", "Description", "Status");

                        WriteLine($"| {randomEquipment.EquipmentId,-11} | {randomEquipment.Name,-15} | {randomEquipment.Description,-80} | {randomEquipment.Status?.Value,17}");

                        WriteLine("Do you want to add this equipment?");
                        WriteLine("1. Yes");
                        WriteLine("2. No. Other similar");
                        WriteLine("3. Start again with the search");
                        response = ReadNonEmptyLine();
                        if (response == "1")
                        {
                            selectedEquipments.Add(randomEquipment.EquipmentId);
                            statusEquipments.Add(randomEquipment.StatusId);
                        }
                        if (response =="3"){
                            SearchEquipmentsRecursive(selectedEquipments, statusEquipments, requested, init, end);
                        }
                    }while(response=="2");

                }
                else
                {
                    // Solo hay una opción, agregarla directamente
                    var singleEquipment = equipments.First();

                    WriteLine("| {0,-11} | {1,-15} | {2,-80} | {3, 17}",
                        "EquipmentId", "Name", "Description", "Status");
                    WriteLine($"| {singleEquipment.EquipmentId,-11} | {singleEquipment.Name,-15} | {singleEquipment.Description,-80} | {singleEquipment.Status?.Value,17}");

                    WriteLine("Do you want to add this equipment? y/n");
                    response = ReadNonEmptyLine().ToLower();
                    if (response == "y")
                    {
                        selectedEquipments.Add(singleEquipment.EquipmentId);
                        statusEquipments.Add(singleEquipment.StatusId);
                    }
                }
            }

            if (selectedEquipments.Count <= maxEquipment)
            {
                WriteLine("Do you want to add another equipment? y/n");
                response = ReadNonEmptyLine().ToLower();
                if (response == "y")
                {
                    SearchEquipmentsRecursive(selectedEquipments, statusEquipments, requested, init, end);
                }
                else
                {
                    return (selectedEquipments, statusEquipments);
                }
            }
            else
            {
                WriteLine($"You have reached the maximum limit of {maxEquipment} equipments.");
                return (selectedEquipments, statusEquipments);
            }
            return (selectedEquipments, statusEquipments);
        }

            // DELETE
    public static void DeleteRequestFormat(string username)
    {
        WriteLine("Here's a list of all the request format that has not been accepted yet. ");
        ViewRequestFormatNotAcceptedYet(username);
        do
        {
            WriteLine();
            WriteLine("Provide the ID of the request that you want to delete (check the list): ");
            int requestID = Convert.ToInt32(ReadLine());

            using(bd_storage db = new())
            {
                // checks if it exists
                IQueryable<RequestDetail> requestDetails = db.RequestDetails
                .Where(e => e.RequestDetailsId == requestID);
                                        
                if(requestDetails is null || !requestDetails.Any())
                {
                    WriteLine("That request ID doesn't exist in the database, try again");
                }
                else
                {
                    db.RequestDetails.Remove(requestDetails.First());
                    int affected = db.SaveChanges();
                    if(affected == 1)
                    {
                        WriteLine("Equipment successfully deleted");
                    }
                    else
                    {
                        WriteLine("Equipment couldn't be deleted");
                    }

                    // Obtén el RequestId asociado
                    int requestId = db.RequestDetails
                    .Where(e => e.RequestDetailsId == requestID)
                    .Select(r => r.Request.RequestId)
                    .FirstOrDefault();

                    // Elimina el registro de Requests
                    var request = db.Requests
                    .Where(r => r.RequestId == requestId)
                    .FirstOrDefault();
                    if(affected == 1)
                    {
                        WriteLine("Equipment successfully deleted");
                    }
                    else
                    {
                        WriteLine("Equipment couldn't be deleted");
                    }
                }               
            }
            return;
        } while (true);            
    }

    

    
    /*public static void SearchEquipmentsByNameStudents(string searchTerm){
        using (bd_storage db = new())
            {
                IQueryable<Equipment>? equipments = db.Equipments
                    .Include(e => e.Status)
                    .Where(e => e.Name.StartsWith(searchTerm)); // Utiliza StartsWith para buscar equipos cuyos nombres comiencen con el término de búsqueda

                db.ChangeTracker.LazyLoadingEnabled = false;

                if (!equipments.Any())
                {
                    WriteLine("No equipment found matching the search term: " + searchTerm);
                    return;
                }

                WriteLine("| {0,-11} | {1,-15} | {2,-80} | {4, 17}",
                    "EquipmentId", "Name", "Description", "Status");

                foreach (var e in equipments)
                {
                    WriteLine($"| {e.EquipmentId,-11} | {e.Name,-15} | {e.Description,-80} | {e.Status?.Value,17}");
                }
            }
        const int maxEquipment = 4;
        string[] equipmentId = new string[maxEquipment];
        string r = "y";
        int i = 0, count=0;
        bool isDuplicate = false;
        //Listado de equipos
        ViewAllEquipments(2);
        for ( i = 0; i < maxEquipment && r == "y"; i++)
        {
            WriteLine("Select the number of equipment: ");
            string newEquipmentId = ReadNonEmptyLine();
            // Verificar duplicados
            isDuplicate = false;
            for (int j = 0; j < count; j++)
            {
                if (equipmentId[j] == newEquipmentId)
                {
                    isDuplicate = true;
                    break;  // Salir del bucle si se encuentra un duplicado.
                }
            }
            if (isDuplicate)
            {
                WriteLine("Este equipmentId ya ha sido agregado anteriormente.");
            }
            else
            {
                count ++;
                WriteLine("Do you want to add another equipment? y/n");
                r = ReadNonEmptyLine();
            }
        }
        if (count == maxEquipment)
        {
            WriteLine("You are only allowed to request up to 4 equipments");
        }
    }*/

}

/*
    
        para pedir el equipo hacer una query que consulte el requestDetails con la misma hora de inicio en la 
        misma fecha conviertes el datetime de la fecha a string y lo comparas, lo mismo con la hora
        no tiene que estar aprobado 
        hacer una lista y que seleccione uno el usuario--
        que no haya mas de 4 equipos en el arreglo de string de equipmentId--
        buscar equipo por name ---
        que no se duplique y tampoco tenga fecha de mantenimiento programada---
        //que no haya hecho mas de un permiso por día para el mismo día y 

          */