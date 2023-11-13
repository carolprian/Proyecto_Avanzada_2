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

                var table = new ConsoleTable("Request Details", count);

                table.AddRow("RequestId", firstRequest.RequestId);
                table.AddRow("StatusId", $"{firstRequest.Status.Value}");
                table.AddRow("ProfessorNip", nip);
                table.AddRow("DispatchTime", $"{firstRequest.DispatchTime.Hour}:{firstRequest.DispatchTime.Minute}");
                table.AddRow("Return Time", $"{firstRequest.ReturnTime.Hour}:{firstRequest.ReturnTime.Minute}");
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
                table.AddRow("DispatchTime", $"{firstRequest.DispatchTime.Hour}:{firstRequest.DispatchTime.Minute}");
                table.AddRow("Return Time", $"{firstRequest.ReturnTime.Hour}:{firstRequest.ReturnTime.Minute}");
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

    public static void StudentsUsingEquipments()
    {
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
                SubMenuStudentsusingEquipment();
                return;
            }

            int i = 0;
            foreach (var use in requestDetails)
            {

                i++;
                WriteLine();
                WriteLine($"Student {i} Information: ");
                WriteLine("");
                WriteLine($"Name: {use.Request.Student.Name}, Last Name: {use.Request.Student.LastNameP}, Group: {use.Request.Student.Group.Name}");
                WriteLine($"Equipment Name: {use.Equipment.Name} ");
                WriteLine($"Return Time: {use.ReturnTime.Hour}:{use.ReturnTime.Minute}");
                WriteLine($"Date: {use.RequestedDate.Date}");
                WriteLine();

            } 
        }
    }

    public static void StudentsLateReturn()
    {
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
                SubMenuStudentsusingEquipment();
                return;
            }

            int i = 0;
            foreach (var use in requestDetails)
            {
                    i++;
                    WriteLine();
                    WriteLine($"Student {i} Information: ");
                    WriteLine("");
                    WriteLine($"Name: {use.Request.Student.Name}, Last Name: {use.Request.Student.LastNameP}, Group: {use.Request.Student.Group.Name}");
                    WriteLine($"Equipment Name: {use.Equipment.Name}");
                    WriteLine($"Return Time: {use.ReturnTime.Hour}:{use.ReturnTime.Minute}");
                    WriteLine($"Date: {use.RequestedDate}");
            }
        }
    }

    public static int ListAreas()
    {
        using( bd_storage db = new())
        {
        IQueryable<Area> areas = db.Areas;
            db.ChangeTracker.LazyLoadingEnabled = false;
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
            db.ChangeTracker.LazyLoadingEnabled = false;
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
            db.ChangeTracker.LazyLoadingEnabled = false;
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
    
    public static void ViewAllEquipmentsCoord()
    {
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
                return;
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
    }

    public static void ListStudentsforCoord()
    {
        using (bd_storage db = new())
        {
            IQueryable<Student> students = db.Students
                .Include(s => s.Group);
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((students is null) || !students.Any())
            {
                WriteLine("There are no registered students found");
                return;
            }

            WriteLine("| {0,-9} | {1,-30} | {2,-30} | {3,-30} | {4,50} | {5,7}",
                "StudentId", "Name", "LastNameP", "LastNameM", "Password", "GroupId");

            // Use the data
            foreach (var s in students)
            {
                WriteLine($"| {s.StudentId,-9} | {s.Name,-30} | {s.LastNameP,-30} | {s.LastNameM,-30} | {Decrypt(s.Password),50} | {s.Group?.Name,7}");
            }
        }
    }

    public static void ListDandLequipment()
    {
        
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator);

            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((dyLequipments is null) || !dyLequipments.Any())
            {
                WriteLine("There are no registered damaged or lost equipments found");
                return;
            }

            WriteLine("| {0,12} | {1,13} | {2,45} | {3,25} | {4,21} | {5,10} | {6,7} |",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            // Use the data
            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,12} | {dal.Status?.Value,13} | {dal.Equipment?.Name,45} | {dal.Description,25} | {dal.DateOfEvent,21} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,7}");
            }
        }
    }

     public static void FindDandLequipmentById(string? equipmentIdToFind)
    {
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment>? dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.DyLequipmentId.ToString().StartsWith(equipmentIdToFind));

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with ID: " + equipmentIdToFind);
                return;
            }

            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }
    }

    public static void FindDandLequipmentByName(string? equipmentNameToFind)
    {
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment>? dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.Equipment.Name.StartsWith(equipmentNameToFind)); // Utiliza StartsWith para buscar coincidencias parciales en el nombre del equipo

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found matching the equipment name: " + equipmentNameToFind);
                return;
            }

            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }
    }

    public static void FindDandLequipmentByDate(string? dateToFind)
    {
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.DateOfEvent.ToString().StartsWith(dateToFind));

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with Date of Event: " + dateToFind);
                return;
            }

            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }
    }

    public static void FindDandLequipmentByStudentName(string? studentNameToFind)
    {
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.Student.Name.StartsWith(studentNameToFind)); // Utiliza StartsWith para buscar coincidencias parciales en el nombre del alumno

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with Student Name: " + studentNameToFind);
                return;
            }

            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }
    }
    
    public static void ListMaintenanceTypes()
    {
        using (bd_storage db = new())
        {
            IQueryable<MaintenanceType> mTypes = db.MaintenanceTypes; 

            if (!mTypes.Any() || mTypes is null)
            {
                WriteLine("No Maintenance Types were found");
                return;
            }

            WriteLine("| {0,-8} | {1,-15} |",
                "ID", "Name");

            foreach (var type in mTypes)
            {
                WriteLine($"| {type.MaintenanceTypeId,-8} | {type.Name,-15} |");
            }
        }
    }

    public static void ListEquipmentsRequestsStudent(string username)
    {
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail>? requestDetails = db.RequestDetails
            .Include( r => r.Equipment)
            .Include( r => r.Request)
            .Where( s => s.Request.StudentId == username);

            if (!requestDetails.Any() || requestDetails is null)
            {
                WriteLine("No results found.");
                return;
            }

            var groupedRequests = requestDetails.GroupBy(r => r.RequestId);

            int i = 0;

            foreach (var group in groupedRequests)
            {
                i++;
                var firstRequest = group.First();
                WriteLine();
                WriteLine($"{i}. RequestId: {firstRequest.RequestId}");
                WriteLine($"StatusId: {firstRequest.StatusId}");
                WriteLine($"ProfessorNip: {firstRequest.ProfessorNip}");
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
            return; 
        }
    }

    public static void ViewPetition(string username)
    {
        using (bd_storage db = new())
        {
            IQueryable<PetitionDetail> petitionDetails = db.PetitionDetails
            .Include( r => r.Equipment)
            .Include( r => r.Petition)
            .Where( s => s.Petition.ProfessorId.Equals(EncryptPass(username)));

            if (!petitionDetails.Any() || petitionDetails is null)
            {
                WriteLine("No results found.");
                return;
            }

            var groupedPetitions = petitionDetails.GroupBy(r => r.PetitionId);

            int i = 0;

            foreach (var group in groupedPetitions)
            {
                i++;
                var firstRequest = group.First();

                WriteLine($"PetitionId: {firstRequest.PetitionId}, PetitionDId: {firstRequest.PetitionDetailsId}, StatusId: {firstRequest.StatusId}, DispatchTime: {firstRequest.DispatchTime.TimeOfDay}, ReturnTime: {firstRequest.ReturnTime.TimeOfDay}, RequestedDate: {firstRequest.RequestedDate}");

                WriteLine("Equipment:");
                foreach (var r in group)
                {
                    WriteLine($"Equipment Name: {r.Equipment.Name}");
                }
            }
            return;
        } 
    }

    public static void ViewRequestFormatNotAcceptedYet(string username)
    {
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Where( s => s.ProfessorNip == 0)
            .Include( r => r.Equipment)
            .Include( r => r.Request)
            .Where( s => s.Request.StudentId == username);

            if (!requestDetails.Any() || requestDetails is null)
            {
                WriteLine("No results found.");
                return;
            }

            var groupedRequests = requestDetails.GroupBy(r => r.RequestId);

            int i = 0;

            foreach (var group in groupedRequests)
            {
                i++;
                var firstRequest = group.First();

                WriteLine($"{i}. RequestId: {firstRequest.RequestId}, RequestDId: {firstRequest.RequestDetailsId}, StatusId: {firstRequest.StatusId}, ProfessorNip: {firstRequest.ProfessorNip}, DispatchTime: {firstRequest.DispatchTime.TimeOfDay}, ReturnTime: {firstRequest.ReturnTime.TimeOfDay}, RequestedDate: {firstRequest.RequestedDate}");

                WriteLine("Equipment:");
                foreach (var r in group)
                {
                    WriteLine($"Equipment Name: {r.Equipment.Name}");
                }
            }
            return;
        } 
    }


    public static void LateReturningStudent(string username)
    {
        using (bd_storage db = new())
        {
            var currentDate = DateTime.Now;

            IQueryable<RequestDetail>? requestDetails = db.RequestDetails
            .Where( s => s.Request.StudentId == username)
            .Include(r => r.Request.Student.Group)
            .Include(r => r.Equipment)
            .Where(s => s.StatusId == 2 && s.ProfessorNip == 1 && s.RequestedDate < currentDate);

            if (!requestDetails.Any())
            {
                WriteLine("You dont have overdue equipment.");
                return;
            }

            int i = 0;
            foreach (var use in requestDetails)
            {
                if (use.ReturnTime < currentDate)
                {
                    i++;
                    WriteLine($"{i}. Equipment Name: {use.Equipment?.Name} ");
                    WriteLine($"Return Time: {use.ReturnTime.Hour}:{use.ReturnTime.Minute}");
                    WriteLine($"Date: {use.RequestedDate}");
                }
            }
        }
    }

    public static void ListGroups()
    {
        using (bd_storage db = new())
        {
            IQueryable<Group> groups = db.Groups;

            if ((groups is null) || !groups.Any())
            {
                WriteLine("There are no groups");
                return;
            }

            WriteLine("| {0,-3} | {1,-10} |", "Id", "Group Name");

            foreach (var group in groups)
            {
                WriteLine("| {0:000} | {1,-10} |", group.GroupId, group.Name);
            }
        }
    }

    public static void ListProfessors()
    {
        using (bd_storage db = new())
        {
            IQueryable<Professor> professors = db.Professors;

            if ((professors is null) || !professors.Any())
            {
                WriteLine("There are no professors");
                return;
            }

            WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-4} | {5,-50} |", "Id", "Name", "Last Name P", "Last Name M", "NIP", "Password");

            foreach (var professor in professors)
            {
                WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-4} | {5,-50} |", professor.ProfessorId, professor.Name, professor.LastNameP, professor.LastNameM, Decrypt(professor.Nip), Decrypt(professor.Password));
            }
        }
    }

    public static void ListStorers()
    {
        using (bd_storage db = new())
        {
            IQueryable<Storer> storers = db.Storers;

            if ((storers is null) || !storers.Any())
            {
                WriteLine("There are no storers");
                return;
            }

            WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", "Id", "Name", "Last Name P", "Last Name M", "Password");

            foreach (var storer in storers)
            {
                WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", storer.StorerId, storer.Name, storer.LastNameP, storer.LastNameM, Decrypt(storer.Password));
            }
        }
    }

    public static void ListSubjects()
    {
        using (bd_storage db = new())
        {
            IQueryable<Subject> subjects = db.Subjects
                .Include(su => su.Academy);
            if ((subjects is null) || !subjects.Any())
            {
                WriteLine("There are no subjects");
                return;
            }

            WriteLine("| {0,-13} | {1,-55} | {2,-7} |", "Id", "Name", "Academy");

            foreach (var subject in subjects)
            {
                WriteLine("| {0:0000000000000} | {1,-55} | {2,-7} |", subject.SubjectId, subject.Name, subject.Academy?.Name);
            }
        }
    }

    public static void WatchPermissions(string user)
    {
        int i = 1;
        using (bd_storage db = new bd_storage())
        {
            IQueryable<RequestDetail> requests = db.RequestDetails
                .Include(r => r.Request).ThenInclude(s=>s.Student).ThenInclude(g=>g.Group)
                .Include(e=>e.Equipment).Where(d =>d.Request.ProfessorId == EncryptPass(user));
                WriteLine("| {0,-1} | {1,-15} | {2,-26} | {3,-10} | {4,-3} | {5,-22} | {6,-22} | {7, -15}",
                        "Number of permission | "," Students Name | ","Students Paternal Last Name | ",
                         "Students Maternal Last Name | ", "Students Group | ","Equipments Name | ", "Dispatch Time | ", "Return Time");

                foreach (var element in requests)
                {
                    WriteLine("| {0,-1} | {1,-23} | {2,-12} | {3,-10} | {4,-3} | {5,-41} | {6,-22} | {7, -22}",
                        i, element.Request.Student.Name,element.Request.Student.LastNameP, 
                        element.Request.Student.LastNameM,element.Request.Student.Group.Name, element.Equipment.Name,
                        element.DispatchTime,element.ReturnTime);
                        i++;
                }
        }
    }

}