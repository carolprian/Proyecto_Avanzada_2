using Microsoft.EntityFrameworkCore;
using AutoGens;
partial class Program
{
    
    public static string? ListEquipmentsRequests()
    {
        using (bd_storage db = new())
        {
            IQueryable<RequestDetail> requestDetails = db.RequestDetails;
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((requestDetails is null) || !requestDetails.Any())
            {
                WriteLine("There are no request found");
                return null;
            }

            int i = 0;
            string[] requestDetailsid = new string[requestDetails.Count()]; // Declarar el arreglo con el tamaño adecuado

            foreach (var r in requestDetails)
            {
                i++;
                WriteLine($"{i}. RequestId: {r.RequestId}, Quantity: {r.Quantity}, StatusId: {r.StatusId}, ProfessorNip: {r.ProfessorNip}, DispatchTime: {r.DispatchTime}, ReturnTime: {r.ReturnTime}, RequestedDate: {r.RequestedDate}");
            }

            return "hola";
        }
    }

    public static void studentsLostDamage()
    {

    }

    public static void StudentsUsingEquipments()
    {

    }

    public static void StudentsLateReturn()
    {

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
}