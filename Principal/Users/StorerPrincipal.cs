using AutoGens;
//este es el bueno

partial class Program{
     public static void StorersPrincipal(string username){  

        WriteLine();
        WriteLine("Welcome Storer !");

        while (true)
        {
            int op = MenuStorer();

            WriteLine();
            switch(op)
            {
                case 1: //  Añadir un equipo usando la funcion dentro de EquipmentsCRUD
                    //Aux porque es un auxiliar para hacer el formulario y validacion y ya despues llamar al CRUD
                    AuxAddEquipmentStorer();
                break;

                case 2: // Actualizar informacion de algun equipo
                    UpdateEquipment();
                break;

                case 3: // Ver una lista de todos los equipos
                    ViewAllEquipments();
                    BackToMenu();
                break;

                case 4: // Eliminar un equipo
                    DeleteEquipment();
                break;

                case 5:  // Buscar un equipo ya sea por su nombre o por su ID
                    //Aux porque es un auxiliar para hacer el formulario y validacion y ya despues llamar al CRUD
                    AuxSearchEquipment();
                    BackToMenu();
                break;

                case 6: // Lista de todos los Request que YA hayan sido aceptados por un maestro
                    ListEquipmentsRequests();
                    BackToMenu();
                break;

                case 7: // Lista de los equipos de los Request de mañana
                    TomorrowsEquipmentRequests();
                    BackToMenu();
                break;

                case 8: // Submenu para poder ver a todos los estudiantes, buscar uno en especifico o ver los que han roto o dañado
                    StudentsHistory();   
                    BackToMenu();       
                break;

                case 9: // Submenu para poder ver a todos los estudiantes que esten usando un material, buscar uno en especifico o los que esten tarde de entregar
                    StudentsUsingEquipment();
                    BackToMenu();
                break;

                case 10: // Entrega del material de parte de la almacenista
                    DeliveryEquipment(); 
                    BackToMenu();
                break;

                case 11: //Regreso del material por parte del alumno
                    ReturnEquipment();
                    BackToMenu();
                break;

                case 12: //Hacer un reporte de material dañado o perdido
                    DamagedLostReportInit();
                    BackToMenu();
                break;

                case 13: // El estudiante vino a pagar du "deuda" por haber dañado o perdido un equipo
                    StudentDebtLostDamagedEquipment();
                    BackToMenu();
                break;

                case 14: //Reporte para programar mantenimiento de un equipo
                    MaintenanceRegister(username);
                    BackToMenu();
                break;

                case 15: // Ver el historial de mantenimiento
                    ViewMaintenanceHistory();
                    BackToMenu();
                break;
                case 16:// Cambiar la contraseña
                    var resultChangeStorerPsw = ChangeStorerPsw(username);
                    if(resultChangeStorerPsw.affected == 1)
                    {
                        WriteLine($"Password was successfully changed for {Decrypt(resultChangeStorerPsw.storerId)}");
                    }
                    BackToMenu();
                break;

                case 17: // Salir de la cuenta
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
        string op = SubMenuDeliveryAndReturn();
        switch(op)
        {
            case "a":
                ReturnEquipmentByStudent();
            break;
            case "b":
                ReturnEquipmentByProfessor();
            break;
            case "c":
                return;
            default:
                WriteLine("That option doesnt exist. ");
            break;
        }
    }

    public static void StudentsHistory()
    {
        string op = SubMenuStudentsHistory();
        switch(op)
        {
            case "a":
                ListStudents();
            break;
            case "b":
                SearchStudentGeneral();
            break;
            case "c":
                studentsLostDamage();
            break;
            case "d":
                return;
            default:
                WriteLine("That option doesnt exist. ");
            break;
        }   
    }

    public static void StudentsUsingEquipment()
    {   
        string op = SubMenuStudentsUsingEquipment();
        switch(op)
        {
            case "a":
                StudentsUsingEquipments();
            break;
            case "b":
                SearchStudentUsingEquipment();
            break;
            case "c":
                StudentsLateReturn();
            break;
            case "d":
                return;
            default:
                WriteLine("That option doesnt exist. ");
            break;
        } 
    }

    public static void DeliveryEquipment()
    {        
        string op = SubMenuDeliveryAndReturn();
        switch(op)
        {
            case "a":
                DeliveryEquipmentsStudents();
            break;
            case "b":
                DeliveryEquipmentsProfessors();
            break;
            case "c":
                return;
            default:
                WriteLine("That option doesnt exist. ");
            break;
        }
    }

    public static void MaintenanceRegister(string username)
    {   
        int affected = 0;
        WriteLine("Program Maintenance for a Equipment!");

        string op = SubMenuMantenance();
        WriteLine();
        switch (op)
        {
            case "a":
                affected = RegisterNewMaintenance(username);
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

            case "b":
                affected = FinishMaintenanceReport(username);
                if(affected > 1)
                {
                    WriteLine($"{affected} rows were succesfully added");
                    WriteLine("Here's a list of all still pending Maintenance Registers");
                    WriteLine();
                    ViewMaintenanceNotMade();
                }
            break;

            case "c":
            return;

            default:
                WriteLine("Option is not valid");
            break;
        }
        return;        
    }

    public static void AuxSearchEquipment()
    {
        string equipment = "";
        string op = SubMenuSearchEquipment();
        switch(op)
        {
            case "a":
                WriteLine("Provide the equipment ID: (it can be the first part of it)");
                equipment = VerifyReadMaxLengthString(15);
                SearchEquipmentsById(equipment);
            break;
            case "b":
                WriteLine("Provide the equipment name: (it can be the first part of it)");
                equipment = VerifyReadMaxLengthString(15);
                SearchEquipmentsByName(equipment);
            break;
            case "c":
            return;
            default:
                WriteLine("Option is not valid");
            break;

        }
        return;
    }

    public static void AuxAddEquipmentStorer()
    {
        WriteLine("Adding a new equipment:");
        string equipmentid = "";
        string controlnumber = "";
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

                IQueryable<Equipment> equipments = db.Equipments
                .Where(e=> e.ControlNumber == controlnumber);
                                
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

        if(equipmentid!="" && name !="" && areaid !=-1 && description !="" && year !=0 && statusid != 0 && controlnumber !="" && coordinatorid !="")
        {
            var resultAdd = AddEquipment(equipmentid, name, areaid, description, year, statusid, controlnumber, coordinatorid);
            if (resultAdd.affected == 1)
            {
                WriteLine($"The equipment {resultAdd.EquipmentId} was created succesfully");
            }
            else
            {
                WriteLine("The equipment was not registered.");
            }
        }
        else
        {
            WriteLine("The was en error introducing the values for the equipment, the equipment was not created!");
        }
    }
}