using Microsoft.EntityFrameworkCore;
using AutoGens;
partial class Program
{
    
    public static string? ListEquipmentsRequests()
    {
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Include( e => e.Equipment)
            .Where( r => r.ProfessorNip == "1");

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
                var firstRequest = group.First();

                WriteLine($"{i}. RequestId: {firstRequest.RequestId}, StatusId: {firstRequest.StatusId}, ProfessorNip: {firstRequest.ProfessorNip}, DispatchTime: {firstRequest.DispatchTime}, ReturnTime: {firstRequest.ReturnTime}, RequestedDate: {firstRequest.RequestedDate}");

                WriteLine("Equipment:");
                foreach (var r in group)
                {
                    WriteLine($"  - Equipment Name: {r.Equipment.Name}");
                }
            }

            return "";

        }
    }

    public static string? TomorrowsEquipmentRequests()
    {
        using (bd_storage db = new())
        {
            DateTime tomorrow = DateTime.Now.Date.AddDays(1);
            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Include( e => e.Equipment)
            .Where( r => r.ProfessorNip == "1")
            .Where(r => r.DispatchTime != null && r.DispatchTime.Date == tomorrow);
            
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
                var firstRequest = group.First();

                WriteLine($"{i}. RequestId: {firstRequest.RequestId}, StatusId: {firstRequest.StatusId}, ProfessorNip: {firstRequest.ProfessorNip}, DispatchTime: {firstRequest.DispatchTime}, ReturnTime: {firstRequest.ReturnTime}, RequestedDate: {firstRequest.RequestedDate}");

                WriteLine("Equipments:");
                foreach (var r in group)
                {
                    WriteLine($" Equipment Name: {r.Equipment.Name}");
                }
            }

            return "";
        }
    }

    public static void studentsLostDamage()
    {
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
            .Include( s => s.Student.Group)
            .Include( e => e.Equipment)
            .Include( t => t.Status);

            if (!dyLequipments.Any() || dyLequipments is null)
            {
                WriteLine("No students found.");
                SubMenuStudentsusingEquipment();
                return;
            }

            int i = 0;
            foreach (var use in dyLequipments)
            {
                i++;
                WriteLine($"Student {i} Information: ");
                WriteLine($"Name: {use.Student.Name}, Last Name: {use.Student.LastNameP}, Group: {use.Student.Group.Name}");
                WriteLine("Equipment Information");
                WriteLine($"status: {use.Status.Value}");
                WriteLine($"Equipment Name: {use.Equipment.Name} ");
                WriteLine($"Description: {use.Description}");
                WriteLine($"Date of event: {use.DateOfEvent}");
            } 
        }
    }

    public static void StudentsUsingEquipments()
    {
        using (bd_storage db = new())
        {

            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Include( r => r.Request.Student.Group)
            .Include( r => r.Equipment)
            .Where( s => s.StatusId == 2)
            .Where( r => r.ProfessorNip == "1");

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
                WriteLine($"Student {i} Information: ");
                WriteLine("");
                WriteLine($"Name: {use.Request.Student.Name}, Last Name: {use.Request.Student.LastNameP}, Group: {use.Request.Student.Group.Name}");
                WriteLine($"Equipment Name: {use.Equipment.Name} ");
                WriteLine($"Return Time: {use.ReturnTime}");
                WriteLine($"Date: {use.RequestedDate}");

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
            .Where(s => s.StatusId == 2 && s.ProfessorNip == "1" && s.RequestedDate < currentDate);
            WriteLine($"Query: {requestDetails.ToQueryString()}");
            if (!requestDetails.Any())
            {
                WriteLine("No students found with overdue equipment.");
                SubMenuStudentsusingEquipment();
                return;
            }

            int i = 0;
            foreach (var use in requestDetails)
            {
                if (use.ReturnTime < currentDate)
                {
                    i++;
                    WriteLine($"Student {i} Information: ");
                    WriteLine("");
                    WriteLine($"Name: {use.Request.Student.Name}, Last Name: {use.Request.Student.LastNameP}, Group: {use.Request.Student.Group.Name}");
                    WriteLine($"Equipment Name: {use.Equipment.Name} ");
                    WriteLine($"Return Time: {use.ReturnTime}");
                    WriteLine($"Date: {use.RequestedDate}");
                }
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
                WriteLine("There are no status found");
            }
            WriteLine("| {0,-11} | {1,-15} | {2,-26} | {3,-80} | {4,4} | {5,17} | {6,20} | {7,6}",
            "EquipmentId", "Name", "Area", "Description", "Year", "Status", "ControlNumber", "Coordinator");
            foreach (var e in equipments)
            {
                WriteLine($"| {0,-11} | {1,-15} | {2,-26} | {3,-80} | {4,4} | {5,17} | {6,20} | {7,6}",
                e.EquipmentId, e.Name, e.Area?.Name, e.Description, e.Year, e.Status?.Value, e.ControlNumber, e.Coordinator?.Name);
            }
        }
    }

    public static void SearchEquipmentsById(string searchTerm)
    {
        using (bd_storage db = new())
        {
            IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator)
                .Where(e => e.EquipmentId.StartsWith(searchTerm)); // Utiliza StartsWith para buscar equipos cuyos nombres comiencen con el término de búsqueda

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!equipments.Any())
            {
                WriteLine("No equipment found matching the search term: " + searchTerm);
                return;
            }

            WriteLine("| {0,-11} | {1,-15} | {2,-26} | {3,-80} | {4,4} | {5,17} | {6,20} | {7,6}",
                "EquipmentId", "Name", "Area", "Description", "Year", "Status", "ControlNumber", "Coordinator");

            foreach (var e in equipments)
            {
                WriteLine($"| {e.EquipmentId,-11} | {e.Name,-15} | {e.Area?.Name,-26} | {e.Description,-80} | {e.Year,4} | {e.Status?.Value,17} | {e.ControlNumber,20} | {e.Coordinator?.Name,6}");
            }
        }
    }

    public static void ListStudentsforCoord()
    {
        //string[] studentss = {};
        using (bd_storage db = new())
        {
            IQueryable<Student> students = db.Students
            .Include(s => s.Group);
            //.Include(category => category.Products);
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((students is null) || !students.Any())
            {
                WriteLine("There are no registered students found");
                return;
            }
            WriteLine("| {0,-9} | {1,-10} | {2,-15} | {3,-15} | {4,15} | {5,7}",
            "StudentId", "Name", "LastNameP", "LastNameM", "Password", "GroupId");
            //int i=1;
            // Use the data
            foreach (var s in students)
            {

                //studentss[i] = Decrypt(student.Password);
                WriteLine($"| {0,-9} | {1,-10} | {2,-15} | {3,-15} | {4,15} | {5,7}",
                s.StudentId, s.Name, s.LastNameP, s.LastNameM, Decrypt(s.Password), s.Group?.Name);
                //i++;
            }
        }
    }

    public static void ListDandLequipment()
    {
        //string[] dyLequipmentss = {};
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
            .Include(dal => dal.Status)
            .Include(dal => dal.Equipment)
            .Include(dal => dal.Student)
            .Include(dal => dal.Coordinator);
            //.Include(category => category.Products);
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((dyLequipments is null) || !dyLequipments.Any())
            {
                WriteLine("There are no registered damaged or lost equipments found");
                return;
            }
            //aqui
            WriteLine("| {0,-11} | {1,-17} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
            "EquipmentId", "Status", "Name", "Description", "Date", "Student", "Coordinator");
            //int i=1;
            // Use the data
            foreach (var dal in dyLequipments)
            {
                //dyLequipmentss[i] = Decrypt(dyLequipments.Password);
                WriteLine($"| {0,-9} | {1,-10} | {2,-15} | {3,-80} | {4,4} | {5,10} | {6,6}",
                dal.DyLequipmentId, dal.Status?.Value, dal.Equipment?.Name, dal.Description, dal.DateOfEvent, dal.Student?.Name, dal.Coordinator?.Name);
                //i++;
            }
        }
    }

     public static void FindDandLequipmentById(string equipmentIdToFind)
    {
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
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

    public static void FindDandLequipmentByName(string equipmentNameToFind)
    {
        using (bd_storage db = new())
        {
            IQueryable<DyLequipment> dyLequipments = db.DyLequipments
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

    public static void FindDandLequipmentByDate(string dateToFind)
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

    public static void FindDandLequipmentByStudentName(string studentNameToFind)
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
}