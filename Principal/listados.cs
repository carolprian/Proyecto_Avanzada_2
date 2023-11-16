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
            .Include(e => e.Equipment)
            .Include(s => s.Status)
            .Include(e => e.Request.Student)
            .Where(r => r.ProfessorNip == 1)
            .OrderByDescending(f => f.RequestedDate);

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
                if (firstRequest.ProfessorNip == 1)
                {
                    nip = "aceptado";
                }
                else if (firstRequest.ProfessorNip == 0)
                {
                    nip = "Pendiente";
                }

                var table = new ConsoleTable("NO. Request", count);

                table.AddRow("RequestId", firstRequest.RequestId);
                table.AddRow("Student", $"{firstRequest.Request?.Student?.Name} {firstRequest.Request?.Student?.LastNameP} {firstRequest.Request?.Student?.LastNameM}");
                table.AddRow("StatusId", $"{firstRequest.Status?.Value}");
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
            .Include(e => e.Equipment)
            .Include(s => s.Status)
            .Where(r => r.ProfessorNip == 1)
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
                if (firstRequest.ProfessorNip == 1)
                {
                    nip = "aceptado";
                }
                else if (firstRequest.ProfessorNip == 0)
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
            .Include(s => s.Student.Group)
            .Include(e => e.Equipment)
            .Include(t => t.Status)
            .Where(d => d.StatusId != 1);

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
                table.AddRow("Equipment Information", "");
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
            .Include(r => r.Request.Student.Group)
            .Include(r => r.Equipment)
            .Where(s => s.StatusId == 2)
            .Where(r => r.ProfessorNip == 1);

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
        using (bd_storage db = new())
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
        using (bd_storage db = new())
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
            if (db.Classrooms is null)
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

    // Views details of all equipments with pagination and additional options
    public static bool ViewAllEquipmentsCoord()
    {
        // Variable to indicate if there are no equipments found
        bool aux = false;

        // Use Entity Framework to query the database for equipments, including related entities
        using (bd_storage db = new())
        {
            IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator);

            // Disable lazy loading to ensure related entities are loaded
            db.ChangeTracker.LazyLoadingEnabled = false;

            // Check if there are no equipments found
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

                // Calculate the number of pages, considering the batch size
                if (countTotal / batchS != 0)
                {
                    pages += 1;
                }

                int pp = 1; // Current page number
                int i = 0;

                // Loop for paginated display of equipments
                while (continueListing)
                {
                    // Retrieve a batch of equipments based on the offset and batch size
                    var equips = equipments.Skip(offset).Take(batchS);

                    // Display table header
                    WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                        "EquipmentId", "Equipment Name", "Year", "Status");
                    WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");

                    // Display each equipment in the batch
                    foreach (var e in equips)
                    {
                        WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                            e.EquipmentId, e.Name, e.Year, e.Status?.Value);
                    }

                    WriteLine();
                    WriteLine($"Total:   {countTotal} ");
                    WriteLine($"{pp} / {pages}");
                    WriteLine("");

                    // Prompt the user if they want to see more information about any equipment
                    WriteLine("Do you want to see more information about any of the equipments?(y/n)");
                    string read = VerifyReadLengthStringExact(1);

                    // If the user wants to see more information, prompt for equipment ID
                    if (read == "y" || read == "Y")
                    {
                        WriteLine("Provide the equipment ID you want to see more info:");
                        read = VerifyReadMaxLengthString(15);
                        int found = ShowEquipmentBylookigForEquipmentId(read);

                        if (found == 0)
                        {
                            WriteLine($"There are no equipments that match the id:  {read}");
                        }
                    }

                    // Prompt the user to navigate through pages or exit
                    WriteLine("Press the left or right arrow key to see more results (press 'q' to exit)...");

                    // Handle user input for navigation or exit
                    if (ReadKey(intercept: true).Key == ConsoleKey.LeftArrow)
                    {
                        offset = offset - batchS;

                        if (pp > 1)
                        {
                            pp--;
                        }

                        Console.Clear();
                    }

                    if (ReadKey(intercept: true).Key == ConsoleKey.RightArrow)
                    {
                        offset = offset + batchS;

                        if (pp < pages)
                        {
                            pp++;
                        }

                        Console.Clear();
                    }

                    if (ReadKey(intercept: true).Key == ConsoleKey.Q)
                    {
                        continueListing = false;
                        Console.Clear();
                    }
                }
            }
        }

        // Return the flag indicating if there are no equipments found
        return aux;
    }

    // This method lists and displays information about students and their groups.
    public static bool ListStudentsforCoord()
    {
        // Flag to indicate whether there are registered students.
        bool aux = false;

        // Set up a connection to the database.
        using (bd_storage db = new())
        {
            // Retrieve students and their associated groups from the database.
            IQueryable<Student> students = db.Students
                .Include(s => s.Group);

            // Disable lazy loading for better performance.
            db.ChangeTracker.LazyLoadingEnabled = false;

            // Check if there are no students in the database.
            if (!students.Any())
            {
                // Print a message and set the flag to true.
                WriteLine("There are no registered students found");
                aux = true;
                return aux;
            }

            // Print a formatted table header with column names.
            WriteLine("| {0,-9} | {1,-30} | {2,-30} | {3,-30} | {4,50} | {5,7}",
                "StudentId", "Name", "LastNameP", "LastNameM", "Password", "GroupId");

            // Iterate through students and display their details.
            foreach (var s in students)
            {
                // Print student details in a formatted table, including decrypted password and group name.
                WriteLine($"| {s.StudentId,-9} | {s.Name,-30} | {s.LastNameP,-30} | {s.LastNameM,-30} | {Decrypt(s.Password),50} | {s.Group?.Name,7}");
            }
        }

        // Return the flag indicating whether there are registered students.
        return aux;
    }

    // Lists details of damaged or lost equipments
    public static bool ListDandLequipment()
    {
        // Variable to indicate if there are no damaged or lost equipments found
        bool aux = false;

        // Use Entity Framework to query the database for damaged or lost equipments, including related entities
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .OrderBy(dal => dal.DateOfEvent);

            // Disable lazy loading to ensure related entities are loaded
            db.ChangeTracker.LazyLoadingEnabled = false;

            // Check if there are no damaged or lost equipments found
            if ((dyLequipments is null) || !dyLequipments.Any())
            {
                WriteLine("There are no registered damaged or lost equipments found");
                aux = true;
                return aux;
            }

            // Display table header
            WriteLine("|{0,12} | {1,9} | {2,40} | {3,35} | {4,11} | {5,10} | {6,7} |",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordi");

            // Display details of each damaged or lost equipment
            foreach (var dal in dyLequipments)
            {
                WriteLine($"|{dal.DyLequipmentId,12} | {dal.Status?.Value,9} | {dal.Equipment?.Name,40} | {dal.Description,35} | {dal.DateOfEvent.ToString("dd/MM/yyyy"),11} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,7} |");
            }
        }

        // Return the flag indicating if there are no damaged or lost equipments found
        return aux;
    }

    // Method to find damaged or lost equipment by ID.
    public static bool FindDandLequipmentById(string? EquipmentIdToFind)
    {
        // Variable to indicate if the damaged or lost equipment is found.
        bool aux = false;

        // Use Entity Framework to query the database for damaged or lost equipment by ID.
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment>? dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.DyLequipmentId.ToString().StartsWith(EquipmentIdToFind));

            // Disable lazy loading to ensure related entities are loaded.
            db.ChangeTracker.LazyLoadingEnabled = false;

            // Check if no damaged or lost equipment is found with the given ID.
            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with ID: " + EquipmentIdToFind);
                aux = true;
                return aux;
            }

            // Display table header.
            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            // Display details of the damaged or lost equipment with the given ID.
            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }

        // Return the flag indicating if the damaged or lost equipment is found.
        return aux;
    }

    // Method to find damaged or lost equipment by name.
    public static bool FindDandLequipmentByName(string? EquipmentNameToFind)
    {
        // Variable to indicate if the damaged or lost equipment is found.
        bool aux = false;

        // Use Entity Framework to query the database for damaged or lost equipment by name.
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment>? dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.Equipment.Name.StartsWith(EquipmentNameToFind)); // Use StartsWith for partial name matches.

            // Disable lazy loading to ensure related entities are loaded.
            db.ChangeTracker.LazyLoadingEnabled = false;

            // Check if no damaged or lost equipment is found with the given name.
            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found matching the equipment name: " + EquipmentNameToFind);
                aux = true;
                return aux;
            }

            // Display table header.
            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            // Display details of the damaged or lost equipment with the given name.
            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }

        // Return the flag indicating if the damaged or lost equipment is found.
        return aux;
    }

    // Method to find damaged or lost equipment by date
    public static bool FindDandLequipmentByDate(string? DateToFind)
    {
        // Variable to indicate if the damaged or lost equipment is found
        bool aux = false;

        // Use Entity Framework to query the database for damaged or lost equipment by date
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.DateOfEvent.ToString().StartsWith(DateToFind));

            // Disable lazy loading to ensure related entities are loaded
            db.ChangeTracker.LazyLoadingEnabled = false;

            // Check if no damaged or lost equipment is found with the given date
            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with Date of Event: " + DateToFind);
                aux = true;
                return aux;
            }

            // Display table header
            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

            // Display details of the damaged or lost equipment with the given date
            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
            }
        }

        // Return the flag indicating if the damaged or lost equipment is found
        return aux;
    }

    // Method to find damaged or lost equipment by student name
    public static bool FindDandLequipmentByStudentName(string? StudentNameToFind)
    {
        // Variable to indicate if the damaged or lost equipment is found
        bool aux = false;

        // Use Entity Framework to query the database for damaged or lost equipment by student name
        using (bd_storage db = new())
        {
            // Check if the DyLequipments table is null (additional check for safety)
            if (db.DyLequipments == null)
            {
                aux = false;
                return aux;
            }

            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.Student.Name.StartsWith(StudentNameToFind));

            // Disable lazy loading to ensure related entities are loaded
            db.ChangeTracker.LazyLoadingEnabled = false;

            // Check if no damaged or lost equipment is found with the given student name
            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with Student Name: " + StudentNameToFind);
                aux = true;
                return aux;
            }
            else
            {
                // Display table header
                WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                    "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");

                // Display details of the damaged or lost equipment with the given student name
                foreach (var dal in dyLequipments)
                {
                    WriteLine($"| {dal.DyLequipmentId,-11} | {dal.Status?.Value,-17} | {dal.Equipment?.Name,-15} | {dal.Description,-80} | {dal.DateOfEvent,4} | {dal.Student?.Name,10} | {dal.Coordinator?.Name,6}");
                }
                aux = true;
                return aux;
            }
        }
    }

    public static void FindDandLequipmentByStudentId(string? StudentIdToFind)
    {
        using (bd_storage db = new())
        {
            //busca equipos registrados como dañados o perdidos
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Include(dal => dal.Status)
                .Include(dal => dal.Equipment)
                .Include(dal => dal.Student)
                .Include(dal => dal.Coordinator)
                .Where(dal => dal.Student.StudentId == StudentIdToFind);

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!dyLequipments.Any())
            {
                WriteLine("No damaged or lost equipment found with Student Id: " + StudentIdToFind);
                return;
            }

            WriteLine("| {0,-11} | {1,-8} | {2,-35} | {3,-30} | {4}",
                "EquipmentId", "Status", "Name", "To Return", "Date of Return");

            foreach (var dal in dyLequipments)
            {
                WriteLine($"| {dal.Equipment?.EquipmentId,-11} | {dal.Status?.Value,-8} | {dal.Equipment?.Name,-35} | {dal.objectReturn,-30} | {dal.DateOfReturn}");
            }
        }
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
        int i = 0;
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail>? requestDetails = db.RequestDetails
           .Include(r => r.Equipment)
           .Include(r => r.Request).
           Include(r => r.Status)
           .Where(s => s.Request.StudentId == Username).OrderBy(r => r.RequestedDate);
            if (requestDetails == null || !requestDetails.Any())
            {
                WriteLine("No results found.");
                aux = true;
                return aux;
            }

            var groupedRequests = requestDetails.GroupBy(r => r.RequestId);

            foreach (var group in groupedRequests)
            {
                i++;
                var firstRequest = group.First();
                WriteLine();
                WriteLine($"RequestId: {firstRequest.RequestId}");
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
            foreach (var group in groupedRequests)
            {
                i++;
                string count = i + "";
                string nip = "";
                var firstRequest = group.First();
                var fistStatus = requestDetails.First();
                if (firstRequest.ProfessorNip == 0)
                {
                    nip = "Pendiente";
                }

                var table = new ConsoleTable("NO. Request", count);

                table.AddRow("RequestId", firstRequest.RequestId);
                table.AddRow("StatusId", $"{fistStatus.Status?.Value ?? "Not delivered"}");
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
            return aux;
        }
    }

    public static bool ViewPetition(string Username)
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<PetitionDetail> petitionDetails = db.PetitionDetails
            .Include(r => r.Equipment)
            .Include(r => r.Petition)
            .Include(s => s.Status)
            .Where(s => s.Petition.ProfessorId.Equals(EncryptPass(Username))).Where(p => p.RequestedDate > DateTime.Today);

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
        int i = 0;
        bool aux = false;
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail> requestDetails = db.RequestDetails
                .Where(s => s.ProfessorNip == 0)
                .Include(r => r.Equipment)
                .Include(r => r.Request)
                .Where(s => s.Request != null && s.Request.StudentId == Username);
            if (requestDetails == null || !requestDetails.Any())
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
                var fistStatus = requestDetails.First();
                if (firstRequest.ProfessorNip == 0)
                {
                    nip = "Pendiente";
                }

                var table = new ConsoleTable("NO. Request", count);

                table.AddRow("RequestId", firstRequest.RequestId);
                table.AddRow("StatusId", $"{fistStatus.Status?.Value ?? "Not delivered"}");
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
            .Where(s => s.Request.StudentId == Username)
            .Include(r => r.Request.Student.Group)
            .Include(r => r.Equipment)
            .Where(s => s.StatusId == 2 && s.ProfessorNip == 1 && s.RequestedDate < currentDate);

            if (!requestDetails.Any())
            {
                WriteLine("You dont have overdue equipment.");
                aux = true;
                return aux;
            }

            foreach (var use in requestDetails)
            {
                if (use.ReturnTime < currentDate)
                {
                    WriteLine();
                    WriteLine($"Equipment Name: {use.Equipment?.Name} ");
                    WriteLine($" Equipment ID: {use.Equipment?.EquipmentId} ");
                    WriteLine($"Return Time: {use.ReturnTime.TimeOfDay}");
                    WriteLine($"Date: {use.RequestedDate.Day}/{use.RequestedDate.Month}/{use.RequestedDate.Year}");
                    WriteLine();
                }
            }
        }
        return aux;
    }

    // This method lists and displays information about groups.
    public static bool ListGroups()
    {
        // Flag to indicate whether there are any groups.
        bool aux = false;

        // Set up a connection to the database.
        using (bd_storage db = new())
        {
            // Retrieve groups from the database.
            IQueryable<Group> groups = db.Groups;

            // Check if there are no groups in the database.
            if ((groups is null) || !groups.Any())
            {
                // Print a message and set the flag to true.
                WriteLine("There are no groups");
                aux = true;
                return aux;
            }

            // Print a formatted table header with column names.
            WriteLine("| {0,-3} | {1,-10} |", "Id", "Group Name");

            // Iterate through groups and display their details.
            foreach (var group in groups)
            {
                // Print group details in a formatted table.
                WriteLine("| {0:000} | {1,-10} |", group.GroupId, group.Name);
            }
        }

        // Return the flag indicating whether there are groups.
        return aux;
    }


    // This method lists and displays information about professors.
    public static bool ListProfessors()
    {
        // Flag to indicate whether there are any professors.
        bool aux = false;

        // Set up a connection to the database.
        using (bd_storage db = new())
        {
            // Retrieve professors from the database.
            IQueryable<Professor> professors = db.Professors;

            // Check if there are no professors in the database.
            if ((professors is null) || !professors.Any())
            {
                // Print a message and set the flag to true.
                WriteLine("There are no professors");
                aux = true;
                return aux;
            }

            // Print a formatted table header with column names.
            WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-4} | {5,-50} |", "Id", "Name", "Last Name P", "Last Name M", "NIP", "Password");

            // Iterate through professors and display their details.
            foreach (var professor in professors)
            {
                // Print professor details in a formatted table, including decrypted NIP and password.
                WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-4} | {5,-50} |", professor.ProfessorId, professor.Name, professor.LastNameP, professor.LastNameM, Decrypt(professor.Nip), Decrypt(professor.Password));
            }
        }

        // Return the flag indicating whether there are professors.
        return aux;
    }


    // This method lists and displays information about storers.
    public static bool ListStorers()
    {
        // Flag to indicate whether there are any storers.
        bool aux = false;

        // Set up a connection to the database.
        using (bd_storage db = new())
        {
            // Retrieve storers from the database.
            IQueryable<Storer> storers = db.Storers;

            // Check if there are no storers in the database.
            if ((storers is null) || !storers.Any())
            {
                // Print a message and set the flag to true.
                WriteLine("There are no storers");
                aux = true;
                return aux;
            }

            // Print a formatted table header with column names.
            WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", "Id", "Name", "Last Name P", "Last Name M", "Password");

            // Iterate through storers and display their details.
            foreach (var storer in storers)
            {
                // Print storer details in a formatted table, including decrypted password.
                WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", storer.StorerId, storer.Name, storer.LastNameP, storer.LastNameM, Decrypt(storer.Password));
            }
        }

        // Return the flag indicating whether there are storers.
        return aux;
    }


    // This method lists and displays information about subjects.
    public static bool ListSubjects()
    {
        // Flag to indicate whether there are any subjects.
        bool aux = false;

        // Set up a connection to the database.
        using (bd_storage db = new())
        {
            // Retrieve subjects and their associated academies from the database.
            IQueryable<Subject> subjects = db.Subjects
                .Include(su => su.Academy);

            // Check if there are no subjects in the database.
            if ((subjects is null) || !subjects.Any())
            {
                // Print a message and set the flag to true.
                WriteLine("There are no subjects");
                aux = true;
                return aux;
            }

            // Print a formatted table header with column names.
            WriteLine("| {0,-13} | {1,-55} | {2,-7} |", "Id", "Name", "Academy");

            // Iterate through subjects and display their details.
            foreach (var subject in subjects)
            {
                // Print subject details in a formatted table, including associated academy name.
                WriteLine("| {0:0000000000000} | {1,-55} | {2,-7} |", subject.SubjectId, subject.Name, subject.Academy?.Name);
            }
        }

        // Return the flag indicating whether there are subjects.
        return aux;
    }

    public static bool WatchPermissions(string User)
    {
        bool aux = true;
        int i = 1;

        using (bd_storage db = new bd_storage())
        {
            IQueryable<RequestDetail> requests = db.RequestDetails
                .Include(r => r.Request).ThenInclude(s => s.Student).ThenInclude(g => g.Group)
                .Include(e => e.Equipment).Where(d => d.Request.ProfessorId == EncryptPass(User));
            //Query to include the info about equipments, groups and students after includind request table where
            //the professor Id equals to the Professor's Id who logged in
            if (requests == null || !requests.Any())
            {
                WriteLine("There are no permissions");
                WriteLine();
                aux = false; //Returns false if there are no permissions
                return aux;
            }
            var table = new ConsoleTable("NO. ", "Student Name",
            "Last Name P", "Last Name M", "Group",
            "Equipment Name", "Current Date", "Dispatch Time", "Return Time");
            //Shows the information as a table without using formatting

            foreach (var element in requests)
            {
                table.AddRow(i, element.Request.Student.Name, element.Request.Student.LastNameP,
                    element.Request.Student.LastNameM, element.Request.Student.Group.Name, element.Equipment.Name, $"{element.RequestedDate.Day}/{element.RequestedDate.Month}/{element.RequestedDate.Year}",
                    element.DispatchTime.TimeOfDay, element.ReturnTime.TimeOfDay);

                i++;
                //We use i to show an index for each permission
            }
            Clear();
            table.Write();
            WriteLine();
        }
        return aux;//Returns true if there are permissions
    }
}