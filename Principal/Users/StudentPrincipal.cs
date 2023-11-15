using AutoGens;
using Microsoft.EntityFrameworkCore;

partial class Program{
    public static void StudentsPrincipal(string UserName)
    {
        // Menu del estudiante
        // Bandera para que se repita el menú hasta que el usuario elija salir
        bool ExitRequested = false;
        // Bandera para decir si el usuario puede o no llenar un Request Format
        bool Validate = false;
        WriteLine("Welcome Student!");
        // Ciclo para que se repita el menu
        while (!ExitRequested)
        {
            // Lee la opcion que selecciona el usuario en el menu del estudiante
            int op = MenuStudents();
            WriteLine();
            switch(op){
                case 1:
                    Validate = ValidateAddRequest(UserName);
                    // Si retorna un valor true si es valido que inserte un RequestFormat
                    //Si retorna false no puede llenar más permisos por el día
                    if(Validate == true)
                    {
                        RequestFormat(UserName);
                    }
                    else 
                    {
                        WriteLine("You are only allow to fill a request format per day.");
                    }
                    BackToMenu();
                break;
                case 2:
                // Opcion para ver todos los equipos del almacen
                    ViewAllEquipments();
                    BackToMenu();
                break;
                case 3:
                // Opción para ver todos los Request Formats que el usuario ha llenado
                    ListEquipmentsRequestsStudent(UserName);
                    BackToMenu();
                break;
                case 4:
                // Opción para modificar campos de los Request Formats
                    UpdateRequestFormat(UserName);
                break;
                case 5:
                // Opción para borrar permisos no aprobados
                    DeleteRequestFormat(UserName);
                    BackToMenu();
                break;
                case 6:
                // Opción para ver los equipos de almacén que no han sido regresados
                    LateReturningStudent(UserName);
                    BackToMenu();
                break;
                case 7:
                // Opción para ver los equipos que el estudiante ha dañado o perdido
                    FindDandLequipmentByStudentId(UserName);
                    BackToMenu();
                break;
                case 8:
                // Opción para salir del menu de estudiantes
                    return;
                default:
                    WriteLine("Invalid option. Please try again.");
                break;
            }
        }
    } 

    public static bool ValidateAddRequest(string UserName)
    {
        DateTime currentDate = DateTime.Now.Date;  // Solo obtenemos la parte de la fecha con la hora 12:00 am

        using (bd_storage db = new())
        {
            // Verificar si existe algún RequestDetail asociado al usuario y con la misma fecha
            bool requestExists = db.RequestDetails.Include(r => r.Request).Include(r => r.Request.Student)
                .Any(rd => rd.Request.StudentId.Equals(UserName) && rd.CurrentDate.Date == currentDate);

            return !requestExists;  // Devolvemos true si no existe la solicitud, false si existe
        }
    }

}