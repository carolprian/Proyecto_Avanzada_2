using Microsoft.EntityFrameworkCore;
using AutoGens;
using ConsoleTables;
partial class Program
{
    
    public static string? ListEquipmentsRequests()
    {
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail>? requestDetails = db.RequestDetails
            .Include( e => e.Equipment)
            .Include( s => s.Status)
            .Where( r => r.ProfessorNip == 1)
            .OrderByDescending( f  => f.RequestedDate);

            if ((requestDetails is null) || !requestDetails.Any())
            {
                WriteLine("There are no request found");
                return null;
            }

            var groupedRequests = requestDetails.GroupBy(r => r.RequestId);

            int i = 0;

            foreach (var group in groupedRequests)
            {
                i++;
                string count = i + "";
                string nip = "";
                var firstRequest = group.First();
                if(firstRequest.ProfessorNip == 1)
                {
                    nip = "aceptado";
                }
                else if (firstRequest.ProfessorNip == 0 )
                {
                    nip = "Pendiente";
                }

                var table = new ConsoleTable("NO. Request", count);

                table.AddRow("RequestId", firstRequest.RequestId);
                table.AddRow("StatusId", $"{firstRequest.Status.Value}");
                table.AddRow("ProfessorNip", nip);
                table.AddRow("DispatchTime", $"{firstRequest.DispatchTime.TimeOfDay}");
                table.AddRow("Return Time", $"{firstRequest.ReturnTime.TimeOfDay}");
                table.AddRow("RequestedDate", $"{firstRequest.RequestedDate.Day}/{firstRequest.RequestedDate.Month}/{firstRequest.RequestedDate.Year}");
                table.AddRow("", "");
                foreach (var r in group)
                {
                    // Adding an empty string as the first column to match the table structure
                    table.AddRow("Equipment Name", r.Equipment.Name);
                }

                table.Write();
                WriteLine();
            }
            return "";
        }
    }

    public static string? TomorrowsEquipmentRequests()
    {
        using (bd_storage db = new())
        {
            DateTime today = DateTime.Now;

            DateTime tomorrow;

            if (today.DayOfWeek == DayOfWeek.Friday)
            {
                tomorrow = DateTime.Now.Date.AddDays(3);
            }
            else
            {
                tomorrow = DateTime.Now.Date.AddDays(1);
            }

            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Include( e => e.Equipment)
            .Include( s => s.Status)
            .Where( r => r.ProfessorNip == 1)
            .Where(r => r.DispatchTime.Date == tomorrow);
            
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((requestDetails is null) || !requestDetails.Any())
            {
                WriteLine("There are no request found");
                return null;
            }

            var groupedRequests = requestDetails.GroupBy(r => r.RequestId);

            int i = 0;

            foreach (var group in groupedRequests)
            {
                i++;
                string count = i + "";
                string nip = "";
                var firstRequest = group.First();
                if(firstRequest.ProfessorNip == 1)
                {
                    nip = "aceptado";
                }
                else if (firstRequest.ProfessorNip == 0 )
                {
                    nip = "Pendiente";
                }

                var table = new ConsoleTable("Request Details", count);

                table.AddRow("RequestId", firstRequest.RequestId);
                table.AddRow("StatusId", $"{firstRequest.Status.Value}");
                table.AddRow("ProfessorNip", nip);
                table.AddRow("DispatchTime", $"{firstRequest.DispatchTime.TimeOfDay}");
                table.AddRow("Return Time", $"{firstRequest.ReturnTime.TimeOfDay}");
                table.AddRow("RequestedDate", $"{firstRequest.RequestedDate.Day}/{firstRequest.RequestedDate.Month}/{firstRequest.RequestedDate.Year}");
                table.AddRow("", "");
                foreach (var r in group)
                {
                    // Adding an empty string as the first column to match the table structure
                    table.AddRow("Equipment Name", r.Equipment.Name);
                }

                table.Write();
                WriteLine();
            }

            return "";
        }
    }

    public static bool studentsLostDamage()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment>? dyLequipments = db.DyLequipments
            .Include( s => s.Student.Group)
            .Include( e => e.Equipment)
            .Include( t => t.Status)
            .Where(d=>d.StatusId != 1);

            if (!dyLequipments.Any() || dyLequipments is null)
            {
                WriteLine("No students found.");
                aux = true;
            }

            foreach (var use in dyLequipments)
            {
                var table = new ConsoleTable("Student Information", " ");

                table.AddRow("ID of Damage or Lost Report", use.DyLequipmentId);
                table.AddRow("Name", use.Student.Name);
                table.AddRow("Last Name", use.Student.LastNameP);
                table.AddRow("Group", use.Student.Group.Name);
                table.AddRow("Equipment Information", "" );
                table.AddRow("Status", use.Status.Value);
                table.AddRow("Equipment Name", use.Equipment.Name);
                table.AddRow("Description", use.Description);
                table.AddRow("Date of event", $"{use.DateOfEvent.Day}/{use.DateOfEvent.Month}/{use.DateOfEvent.Year}");
                table.AddRow("Debt", use.objectReturn);

                WriteLine();
                table.Write();   
                WriteLine();
            } 
        }
        return aux;
    }

    public static bool StudentsUsingEquipments()
    {
        bool aux = false;
        using (bd_storage db = new())
        {

            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Include( r => r.Request.Student.Group)
            .Include( r => r.Equipment)
            .Where( s => s.StatusId == 2)
            .Where( r => r.ProfessorNip == 1);

            if (!requestDetails.Any() || requestDetails is null)
            {
                WriteLine("No students found.");
                SubMenuStudentsUsingEquipment();
                aux = true;
            }

            int i = 0;
            foreach (var use in requestDetails)
            {

                i++;
                string count = i + "";
                var table = new ConsoleTable("Students Using Equipments", count);

                table.AddRow("Student Information", $"{use.Request.Student.Name} {use.Request.Student.LastNameP} {use.Request.Student.Group.Name}");
                table.AddRow("Equipment Name", use.Equipment.Name);
                table.AddRow("Return Time", $"{use.ReturnTime.Hour}:{use.ReturnTime.Minute}");
                table.AddRow("Date:", use.RequestedDate.Date);

                table.Write();
                WriteLine();

            } 
        }
        return aux;
    }

    public static bool StudentsLateReturn()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            var currentDate = DateTime.Now;

            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Include(r => r.Request.Student.Group)
            .Include(r => r.Equipment)
            .Where(s => s.StatusId == 2 && s.ProfessorNip == 1 && s.RequestedDate < currentDate);

            if (!requestDetails.Any())
            {
                WriteLine("No students found with overdue equipment.");
                SubMenuStudentsUsingEquipment();
                aux = true;
                return aux;
            }

            var table = new ConsoleTable("Id", "Name", "LastName P", "Group", "Equipment", "Return Time", "Return Date");
            int i = 0;
            foreach (var use in requestDetails)
            {
                    i++;

                    table.AddRow(use.Request.Student.StudentId, use.Request.Student.Name, use.Request.Student.LastNameP, 
                    use.Request.Student.Group.Name, use.Equipment.Name, $"{use.ReturnTime.Hour}:{use.ReturnTime.Minute}", use.RequestedDate);
            }

            table.Write();
            WriteLine();
        }
        return aux;
    }

    public static int ListAreas()
    {
        using( bd_storage db = new())
        {
        IQueryable<Area> areas = db.Areas;
            if ((areas is null) || !areas.Any())
            {
                WriteLine("There are no areas found");
                return 0;
            }
            // Use the data
            foreach (var area in areas)
            {
                WriteLine($"{area.AreaId} . {area.Name} ");
            }
            return areas.Count();
        }
    }

    public static int ListStatus()
    {
        using( bd_storage db = new())
        {
        IQueryable<Status> status = db.Statuses;
            if ((status is null) || !status.Any())
            {
                WriteLine("There are no status found");
                return 0;
            }
            // Use the data
            foreach (var stat in status)
            {
                WriteLine($"{stat.StatusId} . {stat.Value} ");
            }
            return status.Count();
        }

    }

    public static string[]? ListCoordinators()
    {
        using (bd_storage db = new())
        {
            IQueryable<Coordinator> coordinators = db.Coordinators;
            if ((coordinators is null) || !coordinators.Any())
            {
                WriteLine("There are no registered coordinators found");
                return null;
            }

            int i = 0;
            string[] coordinatorsid = new string[coordinators.Count()]; // Declarar el arreglo con el tamaño adecuado

            foreach (var coordinator in coordinators)
            {
                coordinatorsid[i] = Decrypt(coordinator.CoordinatorId);
                i++;
                WriteLine($"{i}. {Decrypt(coordinator.CoordinatorId)} . {coordinator.Name} {coordinator.LastNameP}");
            }

            return coordinatorsid;
        }
    }


    public static string[]? ListStudents()
    {
        using (bd_storage db = new())
        {
            IQueryable<Student> students = db.Students;
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((students is null) || !students.Any())
            {
                WriteLine("There are no registered students found");
                return null;
            }

            int i = 0;
            string[] studentsid = new string[students.Count()]; // Declarar el arreglo con el tamaño adecuado

            foreach (var s in students)
            {
                studentsid[i] = s.StudentId;
                i++;
                WriteLine($"{i}. {s.StudentId} . {s.Name} {s.LastNameP}");
            }

            return studentsid;
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
                // Muestra toda la lista de classrooms con un indice y la clave de este
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
    
    private static void ShowScheduleOptions(IQueryable<Schedule> schedules, int offset, int startHourId)
    {
        bool aux = false;
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

    public static bool ViewAllEquipmentsCoord()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator);

            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((equipments is null) || !equipments.Any())
            {
                WriteLine("There are no equipments found");
                aux = true;
                return aux;
            }
            else
            {
                
                int countTotal = equipments.Count();
                bool continueListing = true;
                int offset = 0, batchS = 20;
                int pages = countTotal / batchS;
                if(countTotal/batchS != 0){pages+=1;}
                int pp=1;
                int i=0;
                while (continueListing)
                {
                    var equips = equipments.Skip(offset).Take(batchS);
                        
                    WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                        "EquipmentId", "Equipment Name", "Year", "Status");
                    WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
                        
                    foreach( var e in equips)
                    {
                        WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                            e.EquipmentId, e.Name, e.Year, e.Status?.Value);
                            
                    }
                    
                    WriteLine();
                    WriteLine($"Total:   {countTotal} ");
                    WriteLine($"{pp} / {pages}");
                    WriteLine("");

                    WriteLine("Do you want to see more information about any of the equipments?(y/n)");
                    string read = VerifyReadLengthStringExact(1);

                    if(read == "y" || read =="Y")
                    {
                        WriteLine("Provide the equipment ID you want to see more info:");
                        read = VerifyReadMaxLengthString(15);
                        int found = ShowEquipmentBylookigForEquipmentId(read);   

                        if(found == 0)
                        {
                            WriteLine($"There are no equipments that match the id:  {read}" );
                        }
                            
                    }

                    WriteLine("Press the left or right arrow key to see more results (press 'q' to exit)...");

                    if(ReadKey(intercept: true).Key == ConsoleKey.LeftArrow)
                    {
                        offset = offset - batchS;

                        if(pp>1)
                        {
                            pp--;
                        }

                        Console.Clear();

                    }

                    if(ReadKey(intercept: true).Key == ConsoleKey.RightArrow)
                    {
                        offset = offset + batchS;

                        if(pp < pages)
                        {
                            pp ++;
                        }

                        Console.Clear();
                    }

                    if(ReadKey(intercept: true).Key == ConsoleKey.Q)
                    {
                        continueListing = false;
                        Console.Clear();
                    }
                }
            }
        }
        return aux;
    }

    public static bool ListStudentsforCoord()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<Student> students = db.Students
                .Include(s => s.Group);
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((students is null) || !students.Any())
            {
                WriteLine("There are no registered students found");
                aux = true;
                return aux;
            }

            WriteLine("| {0,-9} | {1,-30} | {2,-30} | {3,-30} | {4,50} | {5,7}",
                "StudentId", "Name", "LastNameP", "LastNameM", "Password", "GroupId");

            // Use the data
            foreach (var s in students)
            {
                WriteLine($"| {s.StudentId,-9} | {s.Name,-30} | {s.LastNameP,-30} | {s.LastNameM,-30} | {Decrypt(s.Password),50} | {s.Group?.Name,7}");
            }
        }
        return aux;
    }

    public static bool ListDandLequipment()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .OrderBy( dal => dal.DateOfEvent);

            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((dyLequipments is null) || !dyLequipments.Any())
            {
                WriteLine("There are no registered damaged or lost equipments found");
                aux = true;
                return aux;
            }

            WriteLine("|{0,12} | {1,9} | {2,40} | {3,35} | {4,11} | {5,10} | {6,7} |",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordi");

            // Use the data
            foreach (var dal in dyLequipments)
            {
               WriteLine($"|{dal.DyLequipmentId,12} | {dal.Status?.Value,9} | {dal.Equipment?.Name,40} | {dal.Description,35} | {dal.DateOfEvent.ToString("dd/MM/yyyy"),11} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,7} |");
            }
        }
        return aux;
    }

     public static bool FindDandLequipmentById(string? EquipmentIdToFind)
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment>? dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.DyLequipmentId.ToString().StartsWith(EquipmentIdToFind));

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with ID: " + EquipmentIdToFind);
                aux = true;
                return aux;
            }

            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }
        return aux;
    }

    public static bool FindDandLequipmentByName(string? EquipmentNameToFind)
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment>? dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.Equipment.Name.StartsWith(EquipmentNameToFind)); // Utiliza StartsWith para buscar coincidencias parciales en el nombre del equipo

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found matching the equipment name: " + EquipmentNameToFind);
                aux = true;
                return aux;
            }

            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }
        return aux;
    }

    public static bool FindDandLequipmentByDate(string? DateToFind)
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.DateOfEvent.ToString().StartsWith(DateToFind));

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with Date of Event: " + DateToFind);
                aux = true;
                return aux;
            }

            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }
        return aux;
    }

    public static bool FindDandLequipmentByStudentName(string? StudentNameToFind)
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.Student.Name.StartsWith(StudentNameToFind)); // Utiliza StartsWith para buscar coincidencias parciales en el nombre del alumno

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with Student Name: " + StudentNameToFind);
                aux = true;
                return aux;
            }

            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }
        return aux;
    }
    
    public static bool ListMaintenanceTypes()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<MaintenanceType> mTypes = db.MaintenanceTypes; 

            if (!mTypes.Any() || mTypes is null)
            {
                WriteLine("No Maintenance Types were found");
                aux = true;
                return aux;
            }

            WriteLine("| {0,-8} | {1,-15} |",
                "ID", "Name");

            foreach (var type in mTypes)
            {
                WriteLine($"| {type.MaintenanceTypeId,-8} | {type.Name,-15} |");
            }
        }
        return aux;
    }

    public static bool ListEquipmentsRequestsStudent(string Username)
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail>? requestDetails = db.RequestDetails
            .Include( r => r.Equipment)
            .Include( r => r.Request).
            Include ( r => r.Status)
            .Where( s => s.Request.StudentId == Username);

            if (!requestDetails.Any() || requestDetails is null)
            {
                WriteLine("No results found.");
                aux = true;
                return aux;
            }

            var groupedRequests = requestDetails.GroupBy(r => r.RequestId);

            int i = 0;

            foreach (var group in groupedRequests)
            {
                i++;
                var firstRequest = group.First();
                WriteLine();
                WriteLine($"{i}. RequestId: {firstRequest.RequestId}");
                WriteLine($"Status: {firstRequest.Status.Value}");
                WriteLine($"DispatchTime: {firstRequest.DispatchTime}");
                WriteLine($"ReturnTime: {firstRequest.ReturnTime}");
                WriteLine($"RequestedDate: {firstRequest.RequestedDate}");
                WriteLine();

                WriteLine("Equipment:");
                foreach (var r in group)
                {
                    WriteLine($"  - Equipment Name: {r.Equipment.Name}");
                }
            }
        }
        return aux;
    }

    public static bool ViewPetition(string Username)
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<PetitionDetail> petitionDetails = db.PetitionDetails
            .Include( r => r.Equipment)
            .Include( r => r.Petition)
            .Include( s => s.Status)
            .Where( s => s.Petition.ProfessorId.Equals(EncryptPass(Username)));

            if (!petitionDetails.Any() || petitionDetails is null)
            {
                WriteLine("No results found.");
                aux = true;
                return aux;
            }

            var groupedPetitions = petitionDetails.GroupBy(r => r.PetitionId);

            int i = 0;

            foreach (var group in groupedPetitions)
            {
                i++;
                string count = i + "";
                var firstRequest = group.First();
                var table1 = new ConsoleTable("NO. Petition", count);

                table1.AddRow("Petition Id", firstRequest.PetitionDetailsId);
                table1.AddRow("StatusId", $"{firstRequest.Status.Value}");
                table1.AddRow("DispatchTime", $"{firstRequest.DispatchTime.TimeOfDay}");
                table1.AddRow("Return Time", $"{firstRequest.ReturnTime.TimeOfDay}");
                table1.AddRow("RequestedDate", $"{firstRequest.RequestedDate.Day}/{firstRequest.RequestedDate.Month}/{firstRequest.RequestedDate.Year}");
                table1.AddRow("", "");
                foreach (var r in group)
                {
                    // Adding an empty string as the first column to match the table structure
                    table1.AddRow("Equipment Name", r.Equipment.Name);
                }

                table1.Write();
                WriteLine();
            }
        } 
        return aux;
    }

    public static bool ViewRequestFormatNotAcceptedYet(string Username)
    {
        bool aux = false;
        int i =0;
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Where( s => s.ProfessorNip == 0)
            .Include( r => r.Equipment)
            .Include( r => r.Request)
            .Where( s => s.Request.StudentId == Username);

            IQueryable<Request> requests = db.Requests
            .Include( r => r.Classroom)
            .Include (r => r.Professor)
            .Where( s => s.RequestId == requestDetails.First().RequestId);

            if (!requestDetails.Any() || requestDetails is null)
            {
                WriteLine("No results found.");
                MenuProfessors();
                aux = true;
                return aux;
            }

            var groupedRequests = requestDetails.GroupBy(r => r.RequestId);

            foreach (var group in groupedRequests)
            {
                 i++;
                string count = i + "";
                string nip = "";
                var firstRequest = group.First();
                if(firstRequest.ProfessorNip == 0)
                {
                    nip = "Pendiente";
                }

                var table = new ConsoleTable("NO. Request", count);

                table.AddRow("RequestId", firstRequest.RequestId);
                table.AddRow("StatusId", $"{firstRequest.Status.Value}");
                table.AddRow("ProfessorNip", nip);
                table.AddRow("DispatchTime", $"{firstRequest.DispatchTime.TimeOfDay}");
                table.AddRow("Return Time", $"{firstRequest.ReturnTime.TimeOfDay}");
                table.AddRow("RequestedDate", $"{firstRequest.RequestedDate.Day}/{firstRequest.RequestedDate.Month}/{firstRequest.RequestedDate.Year}");
                table.AddRow("", "");
                foreach (var r in group)
                {
                    // Adding an empty string as the first column to match the table structure
                    table.AddRow("Equipment Name", r.Equipment.Name);
                }

                table.Write();
                WriteLine();
            }
        } 
        return aux;
    }

    public static bool LateReturningStudent(string Username)
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            var currentDate = DateTime.Now;

            IQueryable<RequestDetail>? requestDetails = db.RequestDetails
            .Where( s => s.Request.StudentId == Username)
            .Include(r => r.Request.Student.Group)
            .Include(r => r.Equipment)
            .Where(s => s.StatusId == 2 && s.ProfessorNip == 1 && s.RequestedDate < currentDate);

            if (!requestDetails.Any())
            {
                WriteLine("You dont have overdue equipment.");
                aux = true;
                return aux;
            }

            int i = 0;
            foreach (var use in requestDetails)
            {
                if (use.ReturnTime < currentDate)
                {
                    i++;
                    WriteLine($"{i}. Equipment Name: {use.Equipment?.Name} ");
                    WriteLine($" Equipment ID: {use.Equipment?.EquipmentId} ");
                    WriteLine($"Return Time: {use.ReturnTime.TimeOfDay}");
                    WriteLine($"Date: {use.RequestedDate}");
                }
            }
        }
        return aux;
    }

    public static bool ListGroups()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<Group> groups = db.Groups;

            if ((groups is null) || !groups.Any())
            {
                WriteLine("There are no groups");
                aux = true;
                return aux;
            }

            WriteLine("| {0,-3} | {1,-10} |", "Id", "Group Name");

            foreach (var group in groups)
            {
                WriteLine("| {0:000} | {1,-10} |", group.GroupId, group.Name);
            }
        }
        return aux;
    }

    public static bool ListProfessors()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<Professor> professors = db.Professors;

            if ((professors is null) || !professors.Any())
            {
                WriteLine("There are no professors");
                aux = true;
                return aux;
            }

            WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-4} | {5,-50} |", "Id", "Name", "Last Name P", "Last Name M", "NIP", "Password");

            foreach (var professor in professors)
            {
                WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-4} | {5,-50} |", professor.ProfessorId, professor.Name, professor.LastNameP, professor.LastNameM, Decrypt(professor.Nip), Decrypt(professor.Password));
            }
        }
        return aux;
    }

    public static bool ListStorers()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<Storer> storers = db.Storers;

            if ((storers is null) || !storers.Any())
            {
                WriteLine("There are no storers");
                aux = true;
                return aux;
            }

            WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", "Id", "Name", "Last Name P", "Last Name M", "Password");

            foreach (var storer in storers)
            {
                WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", storer.StorerId, storer.Name, storer.LastNameP, storer.LastNameM, Decrypt(storer.Password));
            }
        }
        return aux;
    }

    public static bool ListSubjects()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<Subject> subjects = db.Subjects
                .Include(su => su.Academy);
            if ((subjects is null) || !subjects.Any())
            {
                WriteLine("There are no subjects");
                aux = true;
                return aux;
            }

            WriteLine("| {0,-13} | {1,-55} | {2,-7} |", "Id", "Name", "Academy");

            foreach (var subject in subjects)
            {
                WriteLine("| {0:0000000000000} | {1,-55} | {2,-7} |", subject.SubjectId, subject.Name, subject.Academy?.Name);
            }
        }
        return aux;
    }

    public static bool WatchPermissions(string User)
    {
        bool aux = false;
        int i = 1;

        using (bd_storage db = new bd_storage())
        {
            IQueryable<RequestDetail> requests = db.RequestDetails
                .Include(r => r.Request).ThenInclude(s=>s.Student).ThenInclude(g=>g.Group)
                .Include(e=>e.Equipment).Where(d =>d.Request.ProfessorId == EncryptPass(User));
                
            if (requests == null || !requests.Any())
                {
                    WriteLine("There are no permissions");
                    WriteLine();
                    aux = true;
                    return aux;
                }
                var table = new ConsoleTable("NO. ", "Student Name", 
                "Last Name P","Last Name M", "Group", 
                "Equipment Name","Current Date", "Dispatch Time", "Return Time");

                foreach (var element in requests)
                {
                    table.AddRow(i, element.Request.Student.Name, element.Request.Student.LastNameP, 
                        element.Request.Student.LastNameM, element.Request.Student.Group.Name, element.Equipment.Name, $"{element.RequestedDate.Day}/{element.RequestedDate.Month}/{element.RequestedDate.Year}",
                        element.DispatchTime.TimeOfDay, element.ReturnTime.TimeOfDay);

                    i++;
                }
                Clear();
                table.Write();
                WriteLine();
        }
        return aux;
    }
}