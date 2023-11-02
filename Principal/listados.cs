using LogInPrincipal;
using LogInPrincipal.AutoGens;
using Microsoft.EntityFrameworkCore;

public partial class Login
{
    public void ListadosAlmacenista()
    {
        while (true)
        {
            WriteLine("Seleccione una opción:");
            WriteLine("1. Listado de vales");
            WriteLine("2. Listado de alumnos");
            WriteLine("0. Salir");
            string option = ReadLine();

            switch (option)
            {
                case "1":
                    ListadoVales();
                    break;
                case "2":
                    ListadoAlumnos();
                    break;
                case "0":
                    return;
                default:
                    WriteLine("Seleccione una opción válida.");
                    break;
            }
        }
    }

    public void ListadoVales()
    {
        using (var dbContext = new bd_storage())
        {
            int skip = 0;
            int take = 20;

            do{
                IQueryable<RequestDetail> solicitudesAprobadasQuery = dbContext.RequestDetails
                    .Where(rd => rd.ProfessorNIP != null) // solo solicitudes aprobadas
                    .GroupBy(rd => rd.requestId) // Agrupar por requestId
                    .Skip(skip)
                    .Take(take)
                    .Select(group => new
                    {
                        RequestId = group.Key,
                        Detalles = group.Select(rd => new
                        {
                            EquipmentId = rd.equipmentId,
                            Cantidad = rd.quantity,
                            DispatchTime = rd.dispatchTime,
                            ReturnTime = rd.returnTime
                        })
                    });

                var solicitudesAprobadas = solicitudesAprobadasQuery.ToList();

                foreach (var solicitud in solicitudesAprobadas)
                {
                    WriteLine($"Solicitud ID: {solicitud.RequestId}");
                    WriteLine("Detalles:");

                    foreach (var detalle in solicitud.Detalles)
                    {
                        WriteLine($"Material ID: {detalle.EquipmentId}, Cantidad: {detalle.Cantidad}");
                        WriteLine($"Hora de dispatch: {detalle.DispatchTime}, Hora de return: {detalle.ReturnTime}");
                    }

                    WriteLine();
                }    

                    if (dbContext.RequestDetails.Count(rd => rd.ProfessorNIP != null) > skip + take)
                    {
                        WriteLine("Presione cualquier tecla para cargar más solicitudes...");
                        ReadKey();
                        skip += take;
                    }
                    else
                    {
                        break;
                    }

            } while (true);
        }    
    }

    public void ListadoAlumnos()
    {
        using (var dbContext = new bd_storage())
        {
            
        }    
    }
}
