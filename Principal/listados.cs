using Microsoft.EntityFrameworkCore;
using AutoGens;
partial class Program
{
    public void ListEquipmentsRequests()
    {
        using (bd_storage db = new())
        {
            int skip = 0;
            int take = 20;

            IQueryable<RequestDetail> requestDetails = db.RequestDetails.Where( r => r.ProfessorNip != null);
        }
    }

    public static void ListAreas()
    {
        using( bd_storage db = new())
        {
        IQueryable<Area> areas = db.Areas;
            //.Include(category => category.Products);
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((areas is null) || !areas.Any())
            {
                WriteLine("There are no areas found");
                return;
            }
            // Use the data
            foreach (var area in areas)
            {
                WriteLine($"{area.AreaId} . {area.Name} ");
            }
        }
    }

    public static void ListStatus()
    {
        using( bd_storage db = new())
        {
        IQueryable<Status> status = db.Statuses;
            //.Include(category => category.Products);
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((status is null) || !status.Any())
            {
                WriteLine("There are no status found");
                return;
            }
            // Use the data
            foreach (var stat in status)
            {
                WriteLine($"{stat.StatusId} . {stat.Value} ");
            }
        }

    }

    public static void ListCoordinators()
    {
        string[] coordinatorss = {};
        using( bd_storage db = new())
        {
        IQueryable<Coordinator> coordinators = db.Coordinators;
            //.Include(category => category.Products);
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((coordinators is null) || !coordinators.Any())
            {
                WriteLine("There are no registered coordinators found");
                return;
            }
            int i=1;
            // Use the data
            foreach (var coordinator in coordinators)
            {
                coordinatorss[i] = Decrypt(coordinator.CoordinatorId);
                WriteLine($"{Decrypt(coordinator.CoordinatorId)} . {coordinator.Name} {coordinator.LastNameP}");
                i++;
            }
        }
    }
}


//                 IQueryable<RequestDetail> solicitudesAprobadasQuery = dbContext.RequestDetails
//                     .Where(rd => rd.ProfessorNip!= null) // solo solicitudes aprobadas
//                     .GroupBy(rd => rd.requestId) // Agrupar por requestId
//                     .Skip(skip)
//                     .Take(take)
//                     .Select(group => new
//                     {
//                         RequestId = group.RequestDetailsId, // Estaba como group.Key pero no existe en BD, RequestDetailsId????????
//                         EquipmentId = rd.equipmentId,
//                         Cantidad = rd.quantity,
//                         DispatchTime = rd.dispatchTime,
//                         ReturnTime = rd.returnTime
//                         /*
//                         Detalles = group.Select(rd => new
//                         {
//                             EquipmentId = rd.equipmentId,
//                             Cantidad = rd.quantity,
//                             DispatchTime = rd.dispatchTime,
//                             ReturnTime = rd.returnTime
//                         })
//                         */
//                     });

//                 var solicitudesAprobadas = solicitudesAprobadasQuery.ToList();

//                 foreach (var solicitud in solicitudesAprobadas)
//                 {
//                     WriteLine($"Solicitud ID: {solicitud.RequestId}");
//                     WriteLine("Detalles:");
//                     WriteLine($"Material ID: {solicitud.EquipmentId}, Cantidad: {solicitud.Cantidad}");
//                     WriteLine($"Hora de dispatch: {solicitud.DispatchTime}, Hora de return: {solicitud.ReturnTime}");

//                     /*
//                     foreach (var detalle in solicitud.Detalles)
//                     {
//                         WriteLine($"Material ID: {detalle.EquipmentId}, Cantidad: {detalle.Cantidad}");
//                         WriteLine($"Hora de dispatch: {detalle.DispatchTime}, Hora de return: {detalle.ReturnTime}");
//                     }
//                     */

//                     WriteLine();
//                 }    

//                     if (dbContext.RequestDetails.Count(rd => rd.ProfessorNIP != null) > skip + take)
//                     {
//                         WriteLine("Presione cualquier tecla para cargar más solicitudes...");
//                         ReadKey();
//                         skip += take;
//                     }
//                     else
//                     {
//                         break;
//                     }

//             } while (true);
//         }    
//     }

//     public void ListadoAlumnos()
//     {
//         using (var dbContext = new bd_storage())
//         {
            
//         }    
//     }
// }
