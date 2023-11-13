using AutoGens;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

partial class Program{
    public static void StudentsPrincipal(string username){
       MenuStudents(username);
    } 

    public static void MenuStudents(string username){ 
        bool validate = true, continued=true;
        while(continued)
        {
            WriteLine();
            WriteLine("Welcome Student!");
            WriteLine("********Menu********");
            WriteLine("1. Fill a request format"); //SI
            WriteLine("2. View equipments"); //SI
            WriteLine("3. View request formats"); //sam SI
            WriteLine("4. Edit request formats that aren't signed yet"); //CARO
            WriteLine("5. Delete request formats that aren't signed yet"); //SAM SI
            WriteLine("6. See a list of equipments that are late for returning"); //SAM ver sus request tardios Si
            WriteLine("7. Sign out");
            string opString = ReadNonEmptyLine();
            int op = TryParseStringaEntero(opString);
            Console.Clear();
            switch(op){
                case 1:
                    validate = ValidateAddRequest(username);
                    if(validate == true){
                        RequestFormat(username);
                    }
                    else {
                        WriteLine("You are only allow to fill a request format per day.");
                        continued=true;
                    }
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                break;
                case 2:
                    ViewAllEquipments(2);
                    continued=true;
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                break;
                case 3:
                    ListEquipmentsRequestsStudent(username);
                    continued=true;
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                break;
                case 4:

                break;
                case 5:
                    DeleteRequestFormat(username);
                    continued=true;
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                break;
                case 6:
                    LateReturningStudent(username);
                    continued=true;
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                break;
                case 7:
                    Console.Clear();
                    continued=false;
                break;

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