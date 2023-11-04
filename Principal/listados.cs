using Microsoft.EntityFrameworkCore;
using AutoGens;
partial class Program
{
    public static void ListEquipmentsRequests()
    {
        using (bd_storage db = new())
        {

            IQueryable<RequestDetail> requestDetails = db.RequestDetails.Where( r => r.ProfessorNip != null)
            .GroupBy( r => new
            {
                r.RequestId,
                r.Quantity,
                r.StatusId,
                r.ProfessorNip,
                r.DispatchTime,
                r.ReturnTime,
                r.RequestedDate
            })
            .Select( group => new RequestDetail
            {
                RequestId = group.Key.RequestId,
                Quantity = group.Key.Quantity,
                StatusId = group.Key.StatusId,
                ProfessorNip = group.Key.ProfessorNip,
                DispatchTime = group.Key.DispatchTime,
                ReturnTime = group.Key.ReturnTime,
                RequestedDate = group.Key.RequestedDate,
                EquipmentId = string.Join(",", group.Select(r => r.EquipmentId))
            })
            .Join(
                db.Equipments,
                detail => detail.EquipmentId,
                equipment => equipment.EquipmentId,
                (detail, equipment) => new RequestDetail
                {
                    RequestId = detail.RequestId,
                    Quantity = detail.Quantity,
                    StatusId = detail.StatusId,
                    ProfessorNip = detail.ProfessorNip,
                    DispatchTime = detail.DispatchTime,
                    ReturnTime = detail.ReturnTime,
                    RequestedDate = detail.RequestedDate,
                    EquipmentName = equipment.Name
                }
            )
            .Take(20);

            WriteLine($"ToQueryString: {requestDetails.ToQueryString()}");

            if (requestDetails is null || !requestDetails.Any())
            {
                WriteLine("No hay resultados.");
            }
            else
            {
                foreach (var details in requestDetails)
                {
                    WriteLine($"RequestId: {details.RequestId}, Quantity: {details.Quantity}, StatusId: {details.StatusId}, ProfessorNip: {details.ProfessorNip}, DispatchTime: {details.DispatchTime}, ReturnTime: {details.ReturnTime}, RequestedDate: {details.RequestedDate}, EquipmentNames: {details.EquipmentName}");
                }

                WriteLine("Presiona una tecla para cargar más resultados...");
                ReadKey();
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


}