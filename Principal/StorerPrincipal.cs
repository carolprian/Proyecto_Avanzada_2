using System.Formats.Asn1;
using AutoGens;
using Microsoft.VisualBasic;

partial class Program{
     public static void StorersPrincipal(){
        string op = MenuStorer();
        WriteLine();
        switch(op)
        {
            case "1": //  Add equipment
                WriteLine("Adding a new equipment:");
                WriteLine("Provide the equipment ID for the inventory:");
                string equipmentid = VerifyReadMaxLengthString(15);
                WriteLine();
                WriteLine("Provide the equipment name:");
                string name = VerifyReadMaxLengthString(40);
                WriteLine();
                IQueryable<Area>? areas = ListAreas();
                WriteLine("Choose the area of the equipment:");
                short areaid=-1;
                while(areaid <= 0 || areaid > areas?.Count() )
                {
                    try
                    {
                        areaid = Convert.ToInt16(VerifyReadLengthStringExact(1));
                    }
                    catch (FormatException)
                    {
                        WriteLine("That is not a correct option, try again.");
                        areaid = -1;
                    }
                    catch (OverflowException)
                    {
                        WriteLine("That is not a correct option, try again.");
                        areaid = -1;
                    }
                }

                WriteLine();
                WriteLine("Provide the description of the equipment:");
                string description = VerifyReadMaxLengthString(200);
                WriteLine();
                WriteLine("Insert the year of fabrication of the equipment:");
                int year = TryParseStringaEntero(ReadNonEmptyLine());
                WriteLine();
                IQueryable<Status>? status = ListStatus();
                WriteLine("Choose and write the option of the current status of the equipment:");

                byte statusid = 0;
                while(statusid == 0 || statusid > status?.Count())
                {
                    try
                    {
                        statusid = Convert.ToByte(VerifyReadLengthStringExact(1));
                    }
                    catch (FormatException)
                    {
                        WriteLine("That is not a correct option, try again.");
                        statusid = 0;
                    }
                    catch (OverflowException)
                    {
                        WriteLine("That is not a correct option, try again.");
                        statusid = 0;
                    }
                }
                WriteLine();

                WriteLine("Insert the control Number provided by the equipments's manufacturer:");
                string controlnumber = VerifyReadMaxLengthString(20);
                WriteLine();
                WriteLine("Choose the coordinator in charge:");
                string[]? coordinators = ListCoordinators();
                WriteLine();
                WriteLine("Write the choosen option:");
                int coordid = TryParseStringaEntero(VerifyReadLengthStringExact(1));
                string coordinatorid = "";
                    if(coordinators is not null)
                    {
                        coordinatorid = coordinators[coordid -1];
                    }
                    var resultAdd = AddEquipment(equipmentid, name, areaid, description, year, statusid, controlnumber, coordinatorid );
                    if(resultAdd.affected == 1)
                    {
                        WriteLine($"The equipment {resultAdd.EquipmentId} was created succesfully");
                    }
                    else{
                        WriteLine("The equipment was not registered.");
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
                ListEquipmentsRequests();
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