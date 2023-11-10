using System.Formats.Asn1;
using AutoGens;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualBasic;
//este es el bueno

partial class Program{
     public static void StorersPrincipal(string username){  

        WriteLine();
        WriteLine("Welcome Storer !");

        while (true)
        {
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
                        coordinatorid = coordinators[coordid - 1];
                    }
                    var resultAdd = AddEquipment(equipmentid, name, areaid, description, year, statusid, controlnumber, coordinatorid);
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
                    ViewAllEquipments(1);
                break;

                case "4": // Delete equipment
                    DeleteEquipment();
                break;

                case "5":  // Search for a equipment by equipment ID
                    WriteLine();
                    WriteLine("1. Search for a equipment info by the equipment ID");
                    WriteLine("2. Search for a equipments info by the equipment name");
                    WriteLine("Choose the option:");
                    string opic = VerifyReadLengthStringExact(1);
                    if(opic == "1")
                    {
                        WriteLine("Provide the equipment ID: (it can be the first part of it)");
                        string equipment = VerifyReadMaxLengthString(15);
                        SearchEquipmentsById(equipment);
                    }
                    else if(opic == "2")
                    {
                        WriteLine("Provide the equipment name: (it can be the first part of it)");
                        string equipment = VerifyReadMaxLengthString(15);
                        SearchEquipmentsByName(equipment);
                    }
                    else
                    {
                        WriteLine("Sorry, that is not an option.");
                    }
                break;

                case "6": // LIst Equipment Requests
                    ListEquipmentsRequests();
                break;

                case "7": // LIst Equipment Requests only for tomorrow
                    TomorrowsEquipmentRequests();
                break;

                case "8":    
                    SubMenuStudentsHistory();            
                break;

                case "9":
                    SubMenuStudentsusingEquipment();
                break;

                case "10":
                    DeliveryEquipment(); 
                break;

                case "11":
                    ReturnEquipment();
                break;

                case "12":
                    DamagedLostReportInit();
                break;

                case "13": // Student debt of lost or damaged
                    StudentDebtLostDamagedEquipment();
                break;

                case "14":// Program maintenance for a equipment
                    MaintenanceRegisterSubMenu(username);
                break;

                case "15": 
                    ViewMaintenanceHistory();
                break;

                case "16":// change storer password
                    var resultChangeStorerPsw = ChangeStorerPsw(username);
                    if(resultChangeStorerPsw.affected == 1)
                    {
                        WriteLine($"Password was successfully changed for {Decrypt(resultChangeStorerPsw.storerId)}");
                    }
                break;

                case "17":
                return;

                default:
                break;
            } 
        }               
    }

    static (int affected, string storerId) ChangeStorerPsw(string username)
    {
        using(bd_storage db = new())
        {
            int incorrect = 0;
            // checks if it exists
            IQueryable<Storer> storers = db.Storers
            .Where(s => s.StorerId == EncryptPass(username));
                                    
            if(storers is null || !storers.Any())
            {
                WriteLine("Storer not found");
            }
            else
            {
                string oldPassword = "";
                do
                {
                    WriteLine();
                    WriteLine("Please enter your old password"); 
                    oldPassword = ReadNonEmptyLine();
                    if(EncryptPass(oldPassword) != storers.First().Password)
                    {
                        incorrect++;
                        if(incorrect >= 3) return (0,"0");
                        WriteLine($"Incorrect Password, please try again ({incorrect})");
                    }
                    else
                    {
                        string newPassword = "";
                        WriteLine("Please enter your new password");
                        newPassword = EncryptPass(VerifyReadLengthString(8));
                        storers.First().Password = newPassword;
                        int affected = db.SaveChanges();
                        return(affected, storers.First().StorerId);;
                    }
                } while (true);                
            }
        }
        return (0,"0");
    }


    public static void ReturnEquipment()
    {        
        string op = "";
        WriteLine("Choose an option: ");
        WriteLine("a. By student");
        WriteLine("b. By teacher");
        WriteLine("c. Exit");
        bool valid = false;
        do{
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c")
            {
                WriteLine("Please choose a valid option (a, b or c )");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);
        switch(op)
        {
            case "a": // students
                ReturnEquipmentByStudent();
            break;
            case "b": // professors
                ReturnEquipmentByProfessor();
            break;
            case "c":
                return;
            default:
                WriteLine("That option doesnt exist. ");
            break;
        }
    }
    
    DateTime date = new(year: 2005, month: 05, day: 14);
    public static string MenuStorer()
    {
        //hay que hacer cambios en el switch case arriba
        string op = "";
        WriteLine();
        WriteLine("**********************************MENU**********************************");
        WriteLine("Please choose an option, a number between 1 and 14");
        WriteLine(" 1. Add new equipment"); // SI volley SI
        WriteLine(" 2. Update equipment information");  // SI furry
        WriteLine(" 3. View equipments");  // SI volley SI y axel hace el de buscar un equipo en especifico
        WriteLine(" 4. Delete equipment");  // SI furry
        WriteLine(" 5. Search for a equipment by equipment ID or equipment Name"); // SI cambia para ID y aparte nombre
        WriteLine(" 6. View Equipment Requests"); // SI sam SI
        WriteLine(" 7. View Tomorrows Equipment Requests"); // SI sam SI
        WriteLine(" 8. View and Search for a Students History"); //SI sam
        WriteLine("         a. See all students"); // SI
        WriteLine("         b. Search for a student in specific"); // sam SI (con historial) si
        WriteLine("         c. See students that have lost or damaged an equipment (and haven't made up for it)"); // SI sam si
        WriteLine(" 9. View and Search for Students using Equipment at this moment"); // SI sam
        WriteLine("         a. See all students using equipments "); // SI sam si
        WriteLine("         b. Search for a specific student in this list"); //SI sam si
        WriteLine("         c. See the list of students that are late for returning equipments"); // sam si
        WriteLine(" 10. Delivery equipment."); // SI vali (cambiar status)
        WriteLine("         a. To students "); // SI vali
        WriteLine("         b. To professor "); // SI vali
        WriteLine(" 11. Register the Return of Equipment(s) of a request"); // SI
        //volley WriteLine(" 9. Return a equipment"); // busca por registro de estudiante, y verifica que todo sea igual a su request, al final pregunta si llegó dañado o en malas condiciones y lo manda a create
        WriteLine(" 12. Create report of damaged or lost equipment");  // SI volley SI
        WriteLine(" 13. Student debt Of LostDamagedEquipment"); //sam 
        WriteLine(" 14. Program maintenance for a equipment");  // furry  ( cambiar sttatus) (se debe poder programar frecuencia mantenimiento)
        WriteLine("         a. Program a new maintenance");
        WriteLine("         b. Report Finished Maintenance");
        WriteLine(" 15. View Maintenance History");  // SI volley 
        WriteLine(" 16. Change password");  // SI furry
        WriteLine(" 17. Sign out");
        // View equipments actualmente prestados en general 
        // hacer 8, 9, 10, 11, 12,    
        bool valid = false;
        do
        {
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "1"  &&  op != "2" &&  op != "3" &&  op != "4" &&  op != "5" &&  op != "6" &&  op != "7" &&  op != "8" &&  op != "9" &&  op != "10" &&  op != "11" &&  op != "12" &&  op != "13" &&  op != "14" && op!="15" && op!="16" && op!="17")
            {
                WriteLine("Please choose a valid option (1 - 17)");
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

    public static void SubMenuStudentsHistory()
    {
        string op = "";
        WriteLine("Choose an option: ");
        WriteLine("a. See all students");
        WriteLine("b. Search for a student in specific");
        WriteLine("c. See students that have lost or damaged an equipment (and haven't made up for it)");
        WriteLine("d. Exit");
        bool valid = false;
        do{
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c" &&  op != "d")
            {
                WriteLine("Please choose a valid option (a, b, c or d)");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);
        switch(op)
        {
            case "a": // all students
                ListStudents();
            break;
            case "b": // search
                SearchStudentGeneral();
            break;
            case "c": // students with lost or damaged
                studentsLostDamage();
            break;
            case "d":
                return;
            break;
            default:
                WriteLine("That option doesnt exist. ");
            break;

        }   
    }

    public static void SubMenuStudentsusingEquipment()
    {        
        string op = "";
        WriteLine("Choose an option: ");
        WriteLine("a. See all students using equipments ");
        WriteLine("b. Search for a specific student in this list");
        WriteLine("c. See the list of students that are late for returning equipments");
        WriteLine("d. Exit");
        bool valid = false;
        do{
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c" &&  op != "d")
            {
                WriteLine("Please choose a valid option (a, b, c or d)");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);
        switch(op)
        {
            case "a": // student that r using equipments
                StudentsUsingEquipments();
            break;
            case "b": // search
                SearchStudentUsingEquipment();
            break;
            case "c": // late for returning
                StudentsLateReturn();
            break;
            case "d":
                return;
            break;
            default:
                WriteLine("That option doesnt exist. ");
            break;
        } 
    }

    public static void DeliveryEquipment()
    {        
        string op = "";
        WriteLine("Choose an option: ");
        WriteLine("a. To student");
        WriteLine("b. To teacher");
        WriteLine("d. Exit");
        bool valid = false;
        do{
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c")
            {
                WriteLine("Please choose a valid option (a, b or c )");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);
        switch(op)
        {
            case "a": // students
                DeliveryEquipmentsStudents();
            break;
            case "b": // professors
                DeliveryEquipmentsProfessors();
            break;
            case "c":
                return;
            break;
            default:
                WriteLine("That option doesnt exist. ");
            break;
        }
    }

    public static void MaintenanceRegisterSubMenu(string username)
    {   
        WriteLine("Program Maintenance for Equipment:");
        WriteLine("1. Program a new maintenance");
        WriteLine("2. Report Finished Maintenance");
        WriteLine("3. Cancel");
        string ans = "";
        bool valid = false;
        do
        {
            WriteLine();
            Write("Option : ");
            ans = ReadNonEmptyLine();
            if(ans != "1" && ans != "2" && ans != "3") WriteLine("Please select a valid option 1 / 2 / 3");
            else valid = true;
        } while (!valid);
        switch (ans)
        {
            case "1":
                int affected = RegisterNewMaintenance(username);
                if(affected > 1)
                {
                    WriteLine($"{affected} rows were succesfully added");
                    WriteLine("Here's a list of all Maintenances Registers that haven't been made yet");
                    WriteLine();
                    ViewMaintenanceNotMade();
                }
                else
                {
                    WriteLine("Maintenances Register couldn't be created");
                }
            break;

            case "2":
                FinishMaintenanceReport(username);
            break;

            case "3":
            return;

            default:
                WriteLine("Option is not valid");
            break;
        }
        return;        
    }
}