using AutoGens;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;

partial class Program{
    public static void StudentsPrincipal(string username){
        //menu de acciones
        //1.Llenar permisos__Caro //agregar validacion de un permiso por día
        //2.Reportes de material dañado
        //3.Listado de materiales
        //4.Listado de permisos
       MenuStudents(username);
    } 

    public static void MenuStudents(string username){
        WriteLine();
        WriteLine("Welcome Student!");
        WriteLine("********Menu********");
        WriteLine("1. Fill a request format");
        WriteLine("2. Report damage and lost equipment");
        WriteLine("3. View equipments");
        WriteLine("4. View request formats");
        WriteLine("5. Edit request formats that aren't signed yet");
        WriteLine("6. Delete request formats that aren't signed yet");
        WriteLine("7. Exit");
        string opString = ReadNonEmptyLine();
        int op = TryParseStringaEntero(opString);
        bool validate = false, continued=true;
        while(continued)
        {
            switch(op){
                case 1:
                    validate = ValidateAddRequest(username);
                    if(validate == true){
                        RequestFormat(username);
                    }
                    else {
                        WriteLine("You are only allow to fill a request format per day.");
                    }
                break;
                case 2:

                break;
                case 3:
                    ViewAllEquipments(2);
                break;
                case 4:

                break;
                case 5:

                break;
                case 6:

                break;
                case 7:

                break;
                default:

                break;
            }
        }
    }

    public static bool ValidateAddRequest(string username){
        DateTime dateTime = DateTime.Now;
        using (bd_storage db = new())
        {
            //hacer una query de request que currentDate del request details sea igual a dateTime
            IQueryable<RequestDetail> requestedDetails = db.RequestDetails
            .Include(rd => rd.Request)
            .Where(rd => rd.Request.StudentId.Equals(username) 
            && rd.CurrentDate.Date == dateTime.Date);
            if(requestedDetails is not null && requestedDetails.Any()){
                return false;
            } else {
                return true;
            }
        }
    }
}