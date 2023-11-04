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

    public static IQueryable<Area>? ListAreas()
    {
        using( bd_storage db = new())
        {
        IQueryable<Area> areas = db.Areas;
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((areas is null) || !areas.Any())
            {
                WriteLine("There are no areas found");
                return null;
            }
            // Use the data
            foreach (var area in areas)
            {
                WriteLine($"{area.AreaId} . {area.Name} ");
            }
            return areas;
        }
    }

    public static IQueryable<Status>? ListStatus()
    {
        using( bd_storage db = new())
        {
        IQueryable<Status> status = db.Statuses;
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((status is null) || !status.Any())
            {
                WriteLine("There are no status found");
                return null;
            }
            // Use the data
            foreach (var stat in status)
            {
                WriteLine($"{stat.StatusId} . {stat.Value} ");
            }
            return status;
        }

    }

    public static string[]? ListCoordinators() // en esta la i al mostrar la lista empieza en 1, pero al guardar el CoordinatorId empieza en 1 en el arreglo, asi que si lo usan para buscar el coordinatorId escogido, buscar en el arreglo coordinatorsid[i-1];
    {
        string[] coordinatorsid = {};
        using( bd_storage db = new())
        {
        IQueryable<Coordinator> coordinators = db.Coordinators;
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((coordinators is null) || !coordinators.Any())
            {
                WriteLine("There are no registered coordinators found");
                return null;
            }
            int i=0;
            // Use the data
            foreach (var coordinator in coordinators)
            {
                coordinatorsid[i] = Decrypt(coordinator.CoordinatorId);
                i++;
                WriteLine($"{i}. {Decrypt(coordinator.CoordinatorId)} . {coordinator.Name} {coordinator.LastNameP}");
            }
            return coordinatorsid;
        }
    }
}