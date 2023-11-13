using AutoGens;
using Microsoft.EntityFrameworkCore;

partial class Program{
    public static void StudentsPrincipal(string username)
    {
        // menu del estudiante
        bool exitRequested = false;
        bool validate = false;
        WriteLine("Welcome Student!");

        while (!exitRequested)
        {
            string op = MenuStudents();
            WriteLine();
            switch(op){
                case "1":
                    validate = ValidateAddRequest(username);
                    if(validate == false)
                    {
                        RequestFormat(username);
                    }
                    else 
                    {
                        WriteLine("You are only allow to fill a request format per day.");
                        return;
                    }
                    BackToMenu();
                break;
                case "2":
                    ViewAllEquipments(2);
                    BackToMenu();
                break;
                case "3":
                    ListEquipmentsRequestsStudent(username);
                    BackToMenu();
                break;
                case "4":
                    UpdateRequestFormat(username);
                break;
                case "5":
                    DeleteRequestFormat(username);
                    BackToMenu();
                break;
                case "6":
                    LateReturningStudent(username);
                    BackToMenu();
                break;
                case "7":
                    return;

                default:
                break;
            }
        }
    } 

    public static bool ValidateAddRequest(string username)
    {
        DateTime currentDate = DateTime.Now.Date;  // Solo obtenemos la parte de la fecha sin la hora

        using (bd_storage db = new())
        {
            // Verificar si existe algún RequestDetail asociado al usuario y con la misma fecha
            bool requestExists = db.RequestDetails.Include(r => r.Request).Include(r => r.Request.Student)
                .Any(rd => rd.Request.StudentId.Equals(username) && rd.CurrentDate.Date == currentDate);

            return !requestExists;  // Devolvemos true si no existe la solicitud, false si existe
        }
    }

}