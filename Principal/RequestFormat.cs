using AutoGens;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

partial class Program
{
    public static void RequestFormat(string username)
    {
        const int maxEquipment = 4;
        string[] equipmentId = new string[maxEquipment], quantity = new string[maxEquipment];
        int[] quantitys = new int[maxEquipment];
        string r = "y";
        string plantel = AddPlantel();
        DateTime currentDate = DateTime.Now;
        var student = AddStudent(username);
        string studentId = student.Item1;
        int groupId = student.Item2;
        int classroomId = AddClassroom();
        string subjectId = SearchSubjectsByName("a");
        string professorId = SearchProfessorByName("A", 0);
        string? storerId = AddStorer();
        if(storerId is null)
        {
            WriteLine("There's not a storer to do your request. Please contact the coordinator or the storer");
            WriteLine("Going back to the menu of student");
            return;
        }
        var request = AddRequest(classroomId, professorId, studentId, groupId, storerId, subjectId);
        if(request.affected > 0){
            WriteLine("Professor added");
        }
        var DatesAndTime = AddDateAndTime(currentDate);
        DateTime requestDate = DatesAndTime.date;
        DateTime initHour = DatesAndTime.init;
        DateTime endHour = DatesAndTime.end;
        int scheduleId = DatesAndTime.Id;


        //requestId
        //equipmentId
        //quantity
        //status
        //proffesorNip
        //dispatchTime
        //returnTime
        //requestDate

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

    public static string SearchSubjectsByName(string searchTerm)
    {
        int i = 0;
        WriteLine("Insert the name start of the subject WITHOUT accents");
        searchTerm = ReadNonEmptyLine();
        using (bd_storage db = new())
        {
            IQueryable<Subject>? subjects = db.Subjects.Where(s => s.Name.ToLower().StartsWith(searchTerm.ToLower()));
            if (!subjects.Any() || subjects is null)
            {
                WriteLine("No subjects found matching the search term: " + searchTerm + "Try again.");
                return SearchSubjectsByName(searchTerm);
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
                    return SearchSubjectsByName(searchTerm);
                }
                else {
                    // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                    WriteLine("Subject not found. Try again.");
                    return SearchSubjectsByName(searchTerm);
                }
            }
        }
    }

    public static string SearchProfessorByName(string searchTerm, int op){
        int i = 1;
        if (op==0){
            WriteLine("Insert the names of the professor WITHOUT accents");
            searchTerm = ReadNonEmptyLine();
            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors.Where(s => s.Name.StartsWith(searchTerm));
                if (!professors.Any())
                {
                    WriteLine($"No professors found matching the search term: {searchTerm}. Try again");
                    return SearchProfessorByName(searchTerm, 0);
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
                        WriteLine("Insert the PATERN last name of the teacher to confirm");
                        string searchTermNew = ReadNonEmptyLine();
                        return SearchProfessorByName(searchTermNew, 1);
                    }
                    else {
                        // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(searchTerm, 0);
                    }
                }
            }
        } else if(op==1) {
            WriteLine("Insert the PATERN last name of the professor WITHOUT accents");
            searchTerm = ReadNonEmptyLine();
            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors.Where(s => s.LastNameP.StartsWith(searchTerm));
                if (!professors.Any() || professors is null)
                {
                    WriteLine($"No professors found matching the search term: {searchTerm}. Try again");
                    return SearchProfessorByName(searchTerm, 0);
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
                        WriteLine("Insert the MATERN last name of the teacher to confirm WITHOUT accents");
                        string searchTermNew = ReadNonEmptyLine();
                        return SearchProfessorByName(searchTermNew, 2);
                    }
                    else {
                        // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(searchTerm, 0);
                    }
                }
            }
        } else
        {
            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors
                    .Where(s => s.LastNameM.StartsWith(searchTerm));
                db.ChangeTracker.LazyLoadingEnabled = false;
                if (!professors.Any())
                {
                    WriteLine($"No professors found matching the search term: {searchTerm}. Try again");
                    return SearchProfessorByName(searchTerm, 0);
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
                        return SearchProfessorByName(searchTerm, 0);
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

    public static (DateTime init, DateTime end, DateTime date, int Id) AddDateAndTime(DateTime currentDate)
    {
        int scheduleIdInitI = 0, scheduleIdEndI = 0;
        DateTime dateValue;
        DateTime initTimeValue;
        DateTime endTimeValue;

        while (true)
        {
            WriteLine("Insert the date of the class: yyyy/MM/dd");
            string dateInput = ReadNonEmptyLine();

            if (DateTime.TryParseExact(dateInput, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            {
                WriteLine("Insert the class start hour (HH:mm):");
                string initHourInput = ReadNonEmptyLine();

                if (TimeSpan.TryParseExact(initHourInput, "HH:mm", CultureInfo.InvariantCulture, out var initTimeSpanValue))
                {
                    WriteLine("Insert the class finish hour (HH:mm):");
                    string endHourInput = ReadNonEmptyLine();

                    if (TimeSpan.TryParseExact(endHourInput, "HH:mm", CultureInfo.InvariantCulture, out var endTimeSpanValue))
                    {
                        initTimeValue = dateValue.Date + initTimeSpanValue;
                        endTimeValue = dateValue.Date + endTimeSpanValue;

                        if (initTimeValue > currentDate && initTimeValue > currentDate.Date && endTimeValue > initTimeValue)
                        {
                            using (bd_storage db = new()){
                                DayOfWeek day = initTimeValue.DayOfWeek;
                                string dayString = day.ToString();
                                IQueryable<Schedule> schedulesInit = db.Schedules.Where(g => g.InitTime == initTimeValue && g.WeekDay == dayString);
                                IQueryable<Schedule> schedulesEnd = db.Schedules.Where(g => g.InitTime == endTimeValue && g.WeekDay == dayString);
                                scheduleIdInitI = schedulesInit.FirstOrDefault()?.ScheduleId ?? 0;
                                scheduleIdEndI = schedulesEnd.FirstOrDefault()?.ScheduleId ?? 0;
                                int difference = scheduleIdEndI - scheduleIdInitI;
                                if (difference >= 1 && difference <= 5)
                                {
                                    return (initTimeValue, endTimeValue, dateValue, scheduleIdEndI);
                                }
                                else
                                {
                                    WriteLine("The minimum loan duration is 50 minutes, and the maximum is 3 hours and 20 minutes.");
                                    WriteLine("Try again with different hours.");
                                }
                            }
                        }
                        else
                        {
                            WriteLine("The date and the registered hours are incorrect. Try again.");
                        }
                    }
                    else
                    {
                        WriteLine("The format for the class end time is incorrect. It must be HH:mm.");
                    }
                }
                else
                {
                    WriteLine("The format for the class start time is incorrect. It must be HH:mm.");
                }
            }
            else
            {
                WriteLine("The date format is incorrect. It must be yyyy/MM/dd.");
            }
        }
    }


}

/*
        que la current date sea minimo un día de diferencia de dateRequest--
        para pedir el equipo hacer una query que consulte el requestDetails con la hora de inicio en la 
        misma fecha conviertes el datetime de al fecha a string y comparas, lo mismo con la hora
        no tiene que estar aprobado 
        hacer una lista y que seleccione uno el usuario
        que no haya mas de 4 equipos en el arreglo de string de equipmentId
        que no haya hecho mas de un permiso por día para el mismo día y 
        que no sume mas de 4 el arreglo de quantitys 
        buscar equipo por name 
        que no se duplique y tampoco tenga fecha de mantenimiento programada
        //agregar hora de las 14:30
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
                equipmentId[count] = newEquipmentId;

                WriteLine("Insert how many :");
                quantity[count] = ReadNonEmptyLine();
                if (int.TryParse(quantity[count], out quantitys[count]))
                {
                    count++;
                }

                WriteLine("Do you want to add another equipment? y/n");
                r = ReadNonEmptyLine();
            }
        }
        if (count == maxEquipment)
        {
            WriteLine("You are only allowed to request up to 4 equipments");
        }  */