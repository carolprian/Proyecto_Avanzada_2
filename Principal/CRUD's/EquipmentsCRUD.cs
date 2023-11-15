using System.Linq;
using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


partial class Program{
    
    // CREATE
    static (int affected, string EquipmentId) AddEquipment(string equipmentid, string name, short areaid, string description, int year, byte statusid, string controlnumber, string coordinatorid )
    {
        using(bd_storage db = new()) // creates connects connection with the database
        {
            if(db.Equipments is null){ return(0,"0");} // checks if the table exists inside the database
            Equipment e = new()  // creates a new object of Equipment type with all the previosuly entered values
            {
                EquipmentId = equipmentid, 
                Name = name,
                AreaId = areaid, 
                Description = description, 
                Year = year, 
                StatusId = statusid, 
                ControlNumber = controlnumber,
                CoordinatorId = EncryptPass(coordinatorid) // encrypts Coordinator ID
            };

            EntityEntry<Equipment> entity = db.Equipments.Add(e); // adds the object that was just created to Equipments table inside the database
            int affected = db.SaveChanges(); // saves chenages that were made inside the database and stores the number of rows that were affected
            return (affected, e.EquipmentId); // returns number of rows that were affected and the new Equipment ID
        }
    }
    public static void UpdateEquipment()
    {
        //WriteLine("Here's a list of all registered equipment");
        WriteLine("Please enter the ID of the equipment you wish to change");
        //ViewAllEquipments(1);
        do
        {
            WriteLine();
            Write("Equipment ID : ");
            string equipmentID = VerifyReadMaxLengthString(15); // reads the equipment ID from the user

            using(bd_storage db = new()) // creates connectio with the database
            {
                // checks if it exists inside the Equipment table, if it does, it gets all values from other tables with any of the Euqipments
                IQueryable<Equipment> equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator)
                .Where(e => e.EquipmentId == equipmentID);
                                        
                if(equipments is null || !equipments.Any()) // checks if the query returned anything
                {
                    WriteLine("That equipment ID doesn't exist in the database");
                }
                else
                {
                    string option;
                    do
                    {
                        WriteLine($"Here's the full information of {equipments.First().EquipmentId} - {equipments.First().Name} : ");
                        WriteLine($"1 - Name : {equipments.First().Name}");
                        WriteLine($"2 - Control Number : {equipments.First().ControlNumber}");
                        WriteLine($"3 - Area ID : {equipments.First().Area?.Name}");
                        WriteLine($"4 - Description : {equipments.First().Description}");
                        WriteLine($"5 - Year of Fabrication : {equipments.First().Year}");
                        WriteLine($"6 - Status ID : {equipments.First().Status.Value}");                   
                        WriteLine($"7 - Coordinator ID : {Decrypt(equipments.First().Coordinator.CoordinatorId)} - Name : {equipments.First().Coordinator.Name} {equipments.First().Coordinator.LastNameP} {equipments.First().Coordinator.LastNameM}");
                        WriteLine();
                        WriteLine("Please choose a number between 1 and 7 to change the respective field");
                        WriteLine("Choose 8 if you are done editing the information");
                        bool valid = false;
                        do
                        {
                            Write("Option : ");
                            option = VerifyReadLengthStringExact(1); // reads option from user
                            if(option != "1" && option != "2" && option != "3" && option != "4" && option != "5" && option != "6" && option != "7" && option != "8") // verifies it is one of the valid options
                            {
                                WriteLine("Please choose a number between 1 and 8");
                            }
                            else
                            {
                                valid = true;
                            }
                        } while (!valid);  

                        int affected = 0;
                        switch (option)
                        {
                            case "1": // change name
                                WriteLine($"Please enter the new name for the Equipment {equipments.First().EquipmentId}");
                                string newEquipmentName = VerifyReadMaxLengthString(40); // reads new name from the user and verifies it does not exceed 40 characters
                                equipments.First().Name = newEquipmentName; // changes the name of the desired equipment previously chosen
                                affected = db.SaveChanges(); // saves changes on the database and stores number of rows affected
                                if(affected == 1)
                                {
                                    WriteLine($"Name was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "2": // change control number
                                bool repeated = true;
                                string controlnumber = "";
                                do
                                {
                                    WriteLine($"Please enter the new Control Number for the Equipment {equipments.First().EquipmentId}");
                                    controlnumber = VerifyReadMaxLengthString(20); // reads new control number from the user and verifies it does not exceed 20 characters
                                    IQueryable<Equipment> repeatedEquipment = db.Equipments.Where(e => e.ControlNumber == controlnumber); // checks the controll numbe does not already exist on any of the other equipments
                                    
                                    if(repeatedEquipment is null || !repeatedEquipment.Any()) //  checks if the query returned anything, if it did, user must enter another control number that is not on the database
                                    {
                                        repeated = false;
                                    }
                                    else
                                    {
                                        WriteLine("That control number is already in use, try again.");
                                    }                                    
                                } while (repeated);
                                equipments.First().ControlNumber = controlnumber; // changes the control numbber of the desired equipment previously chosen
                                affected = db.SaveChanges(); // saves changes on the database and stores number of rows affected
                                if(affected == 1)
                                {
                                    WriteLine($"Control Number was successfully changed for ID : {equipments.First().EquipmentId}");
                                }                                
                                break;

                            case "3": // change area id
                                WriteLine("Here's a list of the available areas : ");
                                short areaid=-1;                       
                                int areasCount = ListAreas(); // lists all existing equipment areas
                                WriteLine();
                                WriteLine("Please choose the area of the equipment:");
                                while(areaid <= 0 || areaid > areasCount ) // reads option of the user and asks him again if it isnt in the desired format or if it isnt an option from the list
                                {
                                    try
                                    {   
                                        Write("Option : ");
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
                                equipments.First().AreaId = areaid; // changes the area of the desired equipment previously chosen
                                affected = db.SaveChanges(); // saves changes on the database and stores number of rows affected
                                if(affected == 1)
                                {
                                    WriteLine($"Area was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "4": // change description
                                WriteLine($"Please enter the new description for the Equipment {equipments.First().EquipmentId}");
                                string newEquipmentDesc = VerifyReadMaxLengthString(200); // reads the new description from the user, verifies it does not exceedd 200 characters
                                equipments.First().Description = newEquipmentDesc; // changes the new description of the previously chosen equipment
                                affected = db.SaveChanges(); // save changes on the database and stores number of rows affected
                                if(affected == 1)
                                {
                                    WriteLine($"Description was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "5": //change year of fabrication
                                WriteLine($"Please enter the new Year of Fabrication of the Equipment {equipments.First().EquipmentId}");
                                int year = TryParseStringaEntero(ReadNonEmptyLine()); // reads an integer number from the user
                                equipments.First().Year = year; // changes the year of fabrication of the previously chosen equipment
                                affected = db.SaveChanges(); // sabes changes on the database and stores number of rows affected
                                if(affected == 1)
                                {
                                    WriteLine($"Year of Fabrication was successfully changed for ID : {equipments.First().EquipmentId}");
                                }                                
                                break;

                            case "6": // change status
                                WriteLine("Here's a list of the possible Equipment Statuses");
                                int statusCount = ListStatus(); // lists all existing statuses
                                byte statusid = 0;
                                WriteLine();
                                WriteLine("Please choose the current status of the equipment:");
                                while(statusid == 0 || statusid > statusCount) // reads a valid status if from the user
                                {
                                    try
                                    {
                                        Write("Option : ");
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
                                equipments.First().StatusId = statusid; // changes the status of the chosen equipment
                                affected = db.SaveChanges(); // saves changes in the database and stores number of rows affected
                                if(affected == 1)
                                {
                                    WriteLine($"Status was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "7": // change coordinator
                                WriteLine("Here's a list of all coordinators:");
                                string[]? coordinators = ListCoordinators(); // lists all existing coordinators
                                WriteLine();
                                WriteLine("Please choose the coordinator in charge of the equipment:");
                                int coordid = TryParseStringaEntero(VerifyReadLengthStringExact(1)); // reads the a valid number from the corrdinators list
                                string coordinatorid = "";
                                if(coordinators is not null)
                                {
                                    coordinatorid = coordinators[coordid - 1];
                                }
                                equipments.First().CoordinatorId = coordinatorid; // changes the coordinator id of the desired equipment
                                affected = db.SaveChanges(); // saves changes made on the database
                                if(affected == 1)
                                {
                                    WriteLine($"Coordinator in charge was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "8": // exit equipment info editor
                            Console.Clear(); // clears everyything on console
                            return;

                            default:
                                WriteLine("Not a valid option");
                                break;
                        }                      

                    } while (true);                                      
                }
            }           
        } while (true);
    }

    public static void DeleteEquipment()
    {
        //WriteLine("Here's a list of all registered equipment");
        WriteLine("Please enter the ID of the equipment you wish to change");
        //ViewAllEquipments(1);
        do
        {
            WriteLine();
            Write("Equipment ID : ");
            string equipmentID = VerifyReadMaxLengthString(15); // reads the equipment ID from the user

            using(bd_storage db = new())
            {
                // checks if it exists
                IQueryable<Equipment> equipments = db.Equipments
                .Where(e => e.EquipmentId == equipmentID);
                                        
                if(equipments is null || !equipments.Any())
                {
                    WriteLine("That equipment ID doesn't exist in the database, try again");
                }
                else // if it does exist, it removes it from Equipments table
                {
                    db.Equipments.Remove(equipments.First());
                    int affected = db.SaveChanges(); // saves changes on the database
                    if(affected == 1) // verifies if it was successfully deleted
                    {
                        WriteLine("Equipment successfully deleted");
                    }
                    else
                    {
                        WriteLine("Equipment couldn't be deleted");
                    }
                }               
            }
            return;
        } while (true);            
    }

    public static void ViewAllEquipments()
    {
        using( bd_storage db = new())
        {
            IQueryable<Equipment>? equipments = db.Equipments
            .Include(e => e.Area).Include(e => e.Status).Include(e => e.Coordinator).OrderBy(e=>e.AreaId);

            if((equipments is null) || !equipments.Any())
            {
                WriteLine("There are no status found");
            } else {
                int countTotal = equipments.Count();
                bool continueListing = true;
                int offset = 0, batchS = 20;
                int pages = countTotal / batchS;
                if(countTotal/batchS != 0){pages+=1;}
                int pp=1;
                int i=0;
                while (continueListing)
                    {
                var equips = equipments.Skip(offset).Take(batchS);
                    WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                        "EquipmentId", "Equipment Name", "Year", "Status");
                    WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
                    
                    foreach( var e in equips)
                        {
                            WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                             e.EquipmentId, e.Name, e.Year, e.Status?.Value);
                        
                        }
                
                    WriteLine();
                    WriteLine($"Total:   {countTotal} ");
                    WriteLine($"{pp} / {pages}");
                    WriteLine("");

                    WriteLine("Do you want to see more information about any of the equipments?(y/n)");
                    string read = VerifyReadLengthStringExact(1);
                    if(read == "y" || read =="Y")
                    {
                        WriteLine("Provide the equipment ID you want to see more info:");
                        read = VerifyReadMaxLengthString(15);
                        int found = ShowEquipmentBylookigForEquipmentId(read);   
                        if(found == 0){ WriteLine($"There are no equipments that match the id:  {read}" );}
                        
                    }
                    WriteLine("Press the left or right arrow key to see more results (press 'q' to exit)...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.LeftArrow)
                    {
                        offset = offset - batchS;
                        if(pp>1){
                            pp--;
                        }
                        Console.Clear();

                    }
                    if(ReadKey(intercept: true).Key == ConsoleKey.RightArrow)
                    {
                        offset = offset + batchS;
                        if(pp < pages)
                        {
                        pp ++;
                        }
                        Console.Clear();
                    }
                    if(ReadKey(intercept: true).Key == ConsoleKey.Q)
                    {
                        continueListing = false;
                        Console.Clear();
                    }
                }
            }
        }
    }

    public static void SearchEquipmentsByName(string SearchTerm)
    {
        using (bd_storage db = new())
        {
            // query encuentra los equipos de la tabla Equipments donde el nombre del equipo empiece con el SearchItem
            IQueryable<Equipment>? Equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator)
                .Where(e => e.Name.StartsWith(SearchTerm)); // Utiliza StartsWith para buscar equipos cuyos nombres comiencen con el término de búsqueda


            if (!Equipments.Any()) // si no existen ningun equipo en la lista
            {
                WriteLine("No equipment found matching the search term:  " + SearchTerm);
                return;
            }
            else // si existen equipos en la lista
            {
                // encabezado de la tabla de equipos existentes
                WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                        "EquipmentId", "Equipment Name", "Year", "Status");
                    WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
                    
                    foreach( var Equipment in Equipments) // en cada equipo mostrar la información más importante 
                    {
                        WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                         Equipment.EquipmentId, Equipment.Name, Equipment.Year, Equipment.Status?.Value); 
                    }
                    
                    WriteLine("Do you want to see more information about any of the equipments?(y/n)");
                    string Read = VerifyReadLengthStringExact(1);
                    if(Read == "y" || Read =="Y") // mostrar la información completa del equipo especificado
                    {
                        WriteLine("Provide the equipment ID you want to see more info:");
                        Read = VerifyReadMaxLengthString(15);
                        int Found = ShowEquipmentBylookigForEquipmentId(Read);   
                        if(Found == 0){ WriteLine($"There are no equipments that match the id:  {Read}" );}
                        
                    }
            }
        }
        
    }

    public static int ShowEquipmentBylookigForEquipmentId(string Id) // returns a 1 if the equipment exists or a 0 if it doesn't
    {
        using(bd_storage db = new())  // connection to the database
        {
        IQueryable<Equipment>  equipms = db.Equipments
            .Include(e => e.Area)
            .Include(e => e.Status)
            .Include(e => e.Coordinator)
            .Where(e=>e.EquipmentId.Equals(Id));

            if(equipms is not null)
            {
                foreach(var equip in equipms)
                {
                    WriteLine($"Equipment ID:  {equip.EquipmentId}");
                    WriteLine($"Equipment Name:  {equip.Name}");
                    WriteLine($"Equipment Area:  {equip.Area?.Name}");
                    WriteLine($"Equipment Description:  {equip.Description}");
                    WriteLine($"Equipment Year of Fabrication:  {equip.Year}");
                    WriteLine($"Equipment Status:  {equip.Status?.Value}");
                    WriteLine($"Equipment Control Number: {equip.ControlNumber}");
                    WriteLine($"Equipment Coordinator:  {equip.Coordinator?.Name} {equip.Coordinator?.LastNameP} {equip.Coordinator?.LastNameM}");
                }
                return equipms.Count();
            }
            else
            {
                return 0;
            }
        }
    }

    public static void ViewAllEquipmentsForMaintenance()
    {
        using(bd_storage db = new()) // creating a connection to the database
        {
            //query that returns all the equipments   
            IQueryable<Equipment>? Equipments = db.Equipments
            .Include(e => e.Area).Include(e => e.Status).Include(e => e.Coordinator)
            .Where(e=>e.StatusId == 1 || e.StatusId == 4);

            if((Equipments is null) || !Equipments.Any())
            {
                WriteLine("No equipment was found");
            }
            else 
            {
                // encabezado de la tabla de equipos existentes
                WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                        "EquipmentId", "Equipment Name", "Year", "Status");
                    WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
                    
                    foreach( var Equipment in Equipments) // en cada equipo mostrar la información más importante 
                    {
                        WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                         Equipment.EquipmentId, Equipment.Name, Equipment.Year, Equipment.Status?.Value); 
                    }
                    
                    WriteLine("Do you want to see more information about any of the equipments?(y/n)");
                    string Read = VerifyReadLengthStringExact(1);
                    if(Read == "y" || Read =="Y") // mostrar la información completa del equipo especificado
                    {
                        WriteLine("Provide the equipment ID you want to see more info:");
                        Read = VerifyReadMaxLengthString(15);
                        int Found = ShowEquipmentBylookigForEquipmentId(Read);   
                        if(Found == 0){ WriteLine($"There are no equipments that match the id:  {Read}" );}
                        
                    }
            }
        }
    }
    
    public static int UpdateEquipmentStatus(byte NewStatus, string EquipmentIdNew ) // returns 1 if the register data was changed, 0 if not
    {
        int Affected = 0;
        using(bd_storage db = new()) // create new connection with the database
        {
            //query that finds equipment from the table Equipment where the EquipmentId matches the sent equipmentID
            IQueryable<Equipment> Equipments = db.Equipments
            .Where(e=> e.EquipmentId == EquipmentIdNew);

            Equipments.First().StatusId = NewStatus; //change the status of one equipment to the new status
            Affected = db.SaveChanges(); //save the changes in the database
        }
        return Affected;
    }
    
}
