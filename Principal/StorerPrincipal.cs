partial class Program{
    public static void StorersPrincipal(){
        string op = MenuStorer();
        WriteLine();
        switch(op)
        {
            case "1": //  Add equipment
                var resultAdd = AddEquipment();
                if(resultAdd.affected == 1)
                {
                    
                }
            break;
            case "2": // Update equipment info
                UpdateEquipment();
            break;
            case "3": // Equipment List
                ViewAllEquipments();
            break;
            case "4": // Delete equipment
                DeleteEquipment();
            break;
            case "5":  // List Equipment Requests
                //ListEquipmentsRequests();
            break;
            case "6": // LIst Equipment Requests only for tomorrow
            break;
            case "7":
            break;
            case "8":
            break;
            case "9":
            break;
            case "10":
            break;
            case "11":
            break;
            case "12":
            break;
            default:
            break;
        }
        
    } 

    public static string MenuStorer()
    {
        string op = "";
        WriteLine();
        WriteLine("Welcome Storer !");
        WriteLine("Please choose an option, a number between 1 and 12");
        WriteLine(" 1. Add new equipment"); // volley
        WriteLine(" 2. Update equipment information");  // furry
        WriteLine(" 3. View equipments");  // volley
        WriteLine(" 4. Delete equipment");  // furry
        WriteLine(" 5. View Equipment Requests"); // sam
        WriteLine(" 6. View Tomorrows Equipment Requests"); // sam
        WriteLine(" 7. View and Search for a Students History"); // volley
        WriteLine(" 8. Create report of damaged or lost equipment");  // volley
        WriteLine(" 9. Program maintenance for a equipment ");  // furry
        WriteLine(" 10. View Maintenance History");  // volley
        WriteLine(" 11. Change password");  // furry
        WriteLine(" 12. Sign out"); // ni pa las muelas chaparro
        bool valid = false;
        do{
            op = ReadNonEmptyLine();
            if (op != "1"  &&  op != "2" &&  op != "3" &&  op != "4" &&  op != "5" &&  op != "6" &&  op != "7" &&  op != "8" &&  op != "9" &&  op != "10" &&  op != "11" &&  op != "12")
            {
                WriteLine("Please choose a valid option (1 - 12)");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
 }

    public void SubMenuRequestsForStorers()
    {
        WriteLine("a. View a request info");

    }

    
}