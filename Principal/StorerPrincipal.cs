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
                    string equipmentid="", controlnumber="";
                    int opi=0;        
                    using(bd_storage db = new())
                    {
                        while(opi==0)
                        {   
                            WriteLine("Provide the equipment ID for the inventory:");
                            equipmentid = VerifyReadMaxLengthString(15);
                            IQueryable<Equipment> equipments = db.Equipments.Where(e=> e.EquipmentId == equipmentid);
                            
                            if(equipments is null || !equipments.Any())
                            {
                                opi = 1;
                            }
                            else
                            {
                                WriteLine("That equipment id is already in use, try again.");
                            }
                        }                
                    }

                WriteLine();
                WriteLine("Provide the equipment name:");
                string name = VerifyReadMaxLengthString(40);
                WriteLine();
                short areaid=-1;
                    
                    int areasCount = ListAreas();
                    WriteLine("Choose the area of the equipment:");
                    while(areaid <= 0 || areaid > areasCount )
                    {
                        try
                        {
                            areaid = Convert.ToInt16(VerifyReadMaxLengthString(2));
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
                WriteLine("Choose and write the option of the current status of the equipment:");
                int statusCount = ListStatus();

                byte statusid = 0;
                while(statusid == 0 || statusid > statusCount)
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

                opi=0;
                    using(bd_storage db = new())
                    {
                        while(opi==0)
                        {   
                            WriteLine("Insert the control Number provided by the equipments's manufacturer:");
                            controlnumber = VerifyReadMaxLengthString(20);
                            IQueryable<Equipment> equipments = db.Equipments.Where(e=> e.ControlNumber == controlnumber);
                            
                            if(equipments is null || !equipments.Any())
                            {
                                opi = 1;
                            }
                            else
                            {
                                WriteLine("That control number is already in use, try again.");
                            }
                        }                
                    }

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
                DamagedLostReportInit();                
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
        WriteLine(" 1. Add new equipment"); // volley SI
        WriteLine(" 2. Update equipment information");  // furry
        WriteLine(" 3. View equipments");  // volley SI
        WriteLine(" 4. Delete equipment");  // furry
        WriteLine(" 5. View Equipment Requests"); // sam SI
        WriteLine(" 6. View Tomorrows Equipment Requests"); // sam SI
        /*
        WriteLine(" 7. View and Search for a Students History"); // volley 
        WriteLine("         a. See all students");
        WriteLine("         b. Search for a student in specific");
        WriteLine("         c. See students that have lost or damaged an equipment (and haven't made up for it)");
        WriteLine(" 8. View and Search for Students using Equipment at this moment");
        WriteLine("         a. See all students using equipments ");
        WriteLine("         b. Search for a specific student in this list");
        WriteLine("         c. See the list of students that are late for returning equipments");
        WriteLine(" 9. Return a equipment"); // busca por registro de estudiante, y verifica que todo sea igual a su request, al final pregunta si llegó dañado o en malas condiciones y lo manda a create
        */  
        WriteLine(" 10. Create report of damaged or lost equipment");  // volley MASO
        WriteLine(" 11. Program maintenance for a equipment ");  // furry
        WriteLine(" 12. View Maintenance History");  // volley 
        WriteLine(" 13. Change password");  // furry
        WriteLine(" 14. Sign out"); // ni pa las muelas chaparro 
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