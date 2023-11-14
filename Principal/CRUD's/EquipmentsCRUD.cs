using System.Linq;
using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


partial class Program{
    
    // CREATE
    static (int affected, string EquipmentId) AddEquipment(string equipmentid, string name, short areaid, string description, int year, byte statusid, string controlnumber, string coordinatorid )
    {
        using(bd_storage db = new())
        {
            if(db.Equipments is null){ return(0,"0");}
            Equipment e = new() 
            {
                EquipmentId = equipmentid, 
                Name = name,
                AreaId = areaid, 
                Description = description, 
                Year = year, 
                StatusId = statusid, 
                ControlNumber = controlnumber,
                CoordinatorId = EncryptPass(coordinatorid)
            };

            EntityEntry<Equipment> entity = db.Equipments.Add(e);
            int affected = db.SaveChanges();
            return (affected, e.EquipmentId);
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

            using(bd_storage db = new())
            {
                // checks if it exists
                IQueryable<Equipment> equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator)
                .Where(e => e.EquipmentId == equipmentID);
                                        
                if(equipments is null || !equipments.Any())
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
                        // WriteLine($"7 - Coordinator ID : {equipments.First().Coordinator.CoordinatorId} - Name : {equipments.First().Coordinator.Name} {equipments.First().Coordinator.LastNameP} {equipments.First().Coordinator.LastNameM}");
                        WriteLine();
                        WriteLine("Please choose a number between 1 and 7 to change the respective field");
                        WriteLine("Choose 8 if you are done editing the information");
                        bool valid = false;
                        do
                        {
                            Write("Option : ");
                            option = VerifyReadLengthStringExact(1);
                            if(option != "1" && option != "2" && option != "3" && option != "4" && option != "5" && option != "6" && option != "7" && option != "8")
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
                                string newEquipmentName = VerifyReadMaxLengthString(40);
                                equipments.First().Name = newEquipmentName;
                                affected = db.SaveChanges();
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
                                    controlnumber = VerifyReadMaxLengthString(20);
                                    IQueryable<Equipment> repeatedEquipment = db.Equipments.Where(e => e.ControlNumber == controlnumber);
                                    
                                    if(repeatedEquipment is null || !repeatedEquipment.Any())
                                    {
                                        repeated = false;
                                    }
                                    else
                                    {
                                        WriteLine("That control number is already in use, try again.");
                                    }                                    
                                } while (repeated);
                                equipments.First().ControlNumber = controlnumber;
                                affected = db.SaveChanges();
                                if(affected == 1)
                                {
                                    WriteLine($"Control Number was successfully changed for ID : {equipments.First().EquipmentId}");
                                }                                
                                break;

                            case "3": // change area id
                                WriteLine("Here's a list of the available areas : ");
                                short areaid=-1;                       
                                int areasCount = ListAreas();
                                WriteLine();
                                WriteLine("Please choose the area of the equipment:");
                                while(areaid <= 0 || areaid > areasCount )
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
                                equipments.First().AreaId = areaid;
                                affected = db.SaveChanges();
                                if(affected == 1)
                                {
                                    WriteLine($"Area was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "4": // change description
                                WriteLine($"Please enter the new description for the Equipment {equipments.First().EquipmentId}");
                                string newEquipmentDesc = VerifyReadMaxLengthString(200);
                                equipments.First().Description = newEquipmentDesc;
                                affected = db.SaveChanges();
                                if(affected == 1)
                                {
                                    WriteLine($"Description was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "5": //change year of fabrication
                                WriteLine($"Please enter the new Year of Fabrication of the Equipment {equipments.First().EquipmentId}");
                                int year = TryParseStringaEntero(ReadNonEmptyLine());
                                equipments.First().Year = year;
                                affected = db.SaveChanges();
                                if(affected == 1)
                                {
                                    WriteLine($"Year of Fabrication was successfully changed for ID : {equipments.First().EquipmentId}");
                                }                                
                                break;

                            case "6": // change status
                                WriteLine("Here's a list of the possible Equipment Statuses");
                                int statusCount = ListStatus();
                                byte statusid = 0;
                                WriteLine();
                                WriteLine("Please choose the current status of the equipment:");
                                while(statusid == 0 || statusid > statusCount)
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
                                equipments.First().StatusId = statusid;
                                affected = db.SaveChanges();
                                if(affected == 1)
                                {
                                    WriteLine($"Status was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "7": // change coordinator
                                WriteLine("Here's a list of all coordinators:");
                                string[]? coordinators = ListCoordinators();
                                WriteLine();
                                WriteLine("Please choose the coordinator in charge of the equipment:");
                                int coordid = TryParseStringaEntero(VerifyReadLengthStringExact(1));
                                string coordinatorid = "";
                                if(coordinators is not null)
                                {
                                    coordinatorid = coordinators[coordid - 1];
                                }
                                equipments.First().CoordinatorId = coordinatorid;
                                affected = db.SaveChanges();
                                if(affected == 1)
                                {
                                    WriteLine($"Coordinator in charge was successfully changed for ID : {equipments.First().EquipmentId}");
                                }
                                break;

                            case "8": // exit equipment info editor
                            Console.Clear();
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
                else
                {
                    db.Equipments.Remove(equipments.First());
                    int affected = db.SaveChanges();
                    if(affected == 1)
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

    public static void ViewAllEquipments(int op)
    {
        if (op==1){
            using( bd_storage db = new()) // connection to the database bd_storage.db
            {
                // query to select all the equipments ordered by area
                IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Area).Include(e => e.Status).Include(e => e.Coordinator).OrderBy(e=>e.AreaId);
                
                if((equipments is null) || !equipments.Any()) // check if the list is empty
                {
                    WriteLine("There are no equipments found");
                }
                else 
                { // if the list is not empty

                    int countTotal = equipments.Count(); // save the total quantity of equipments in the inventory
                    bool continueListing = true;
                    int offset = 0, batchS = 20; // that starts showing from the first equipments, until the first 20
                    int pages = countTotal / batchS; // quantity of pages available
                    if(countTotal/batchS != 0){pages+=1;} 
                    int pp=1; // initial page
                    int i=0;
                    while (continueListing)
                    {
                        var equips = equipments.Skip(offset).Take(batchS); // new list of a range of 20 equipments, each time the list of equipments change                      
                        // header of the format table
                        WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                            "EquipmentId", "Equipment Name", "Year", "Status");
                        WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
                        
                        foreach( var e in equips)
                            { // printing each equipments register in general
                                WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                                e.EquipmentId, e.Name, e.Year, e.Status?.Value);
                            
                            }
                        //printing the page number and the total count of equipments in the inventory at the end of the table
                        WriteLine();
                        WriteLine($"Total:   {countTotal} ");
                        WriteLine($"{pp} / {pages}");
                        WriteLine("");
                        // start the asking to see all the information about one equipment ID in specific
                        WriteLine("Do you want to see more information about any of the equipments?(y/n)");
                        string read = VerifyReadLengthStringExact(1);
                        if(read == "y" || read =="Y") // if user wants to see the info
                        {
                            WriteLine("Provide the equipment ID you want to see more info:");
                            read = VerifyReadMaxLengthString(15); // insert and verify maximum 15 characters of equipment id to look for
                            int found = ShowEquipmentBylookigForEquipmentId(read); // function that looks for the equipment ID and shows it
                            if(found == 0){ WriteLine($"There are no equipments that match the id:  {read}" );} // if the id doesn't exist
                            
                        }
                        WriteLine("Press the left or right arrow key to see more results (press 'Enter' to exit)...");
                        if(ReadKey(intercept: true).Key == ConsoleKey.LeftArrow) // go to the last page
                        {
                            offset = offset - batchS; // show the last 20 equipments
                            if(pp>1){
                                pp--; // return one page count
                            }
                            Clear();

                        }
                        if(ReadKey(intercept: true).Key == ConsoleKey.RightArrow) // go to the next page
                        {
                            offset = offset + batchS; // show the next 20 equipments
                            if(pp < pages) // check that the pages count shown is not bigger than the actual total pages
                            {
                                pp ++; // go to the next page count
                            }
                            Clear();
                        }
                        if(ReadKey(intercept: true).Key == ConsoleKey.Enter) // Exit the View ALL Equipments
                        {
                            continueListing = false;
                            Clear();
                        }
                    }
                    }
            }
        } else 
        {
            using( bd_storage db = new())
            {
                IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Status).Where(s => s.Status.StatusId==1 || s.Status.StatusId==2);

                if((equipments is null) || !equipments.Any())
                {
                    WriteLine("There are no status found");
                }
                int i=1;
                WriteLine("| {0,-5} | {1,-15} | {2,-27} | {3}", "Index", "EquipmentId", "Equipment Name", "Description");
                WriteLine("-------------------------------------------------------------------------------");

                foreach (var e in equipments)
                {
                    WriteLine("| {0,-5} | {1,-15} | {2,-27} | {3}",
                        i, e.EquipmentId, e.Name, e.Description);
                    i++;
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
            // query that has the result of all registers in the table Equipments that has an EquipmentID that matches the parameter of the function
            IQueryable<Equipment>  Equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator)
                .Where(e=>e.EquipmentId.Equals(Id));
                //checking that the results of the query are not null
                if(Equipments is not null)
                {
                    //print all the information of each equipment
                    foreach(var Equipment in Equipments)
                    {
                        WriteLine();
                        WriteLine($"Equipment ID:  {Equipment.EquipmentId}");
                        WriteLine($"Equipment Name:  {Equipment.Name}");
                        WriteLine($"Equipment Area:  {Equipment.Area?.Name}");
                        WriteLine($"Equipment Description:  {Equipment.Description}");
                        WriteLine($"Equipment Year of Fabrication:  {Equipment.Year}");
                        WriteLine($"Equipment Status:  {Equipment.Status?.Value}");
                        WriteLine($"Equipment Control Number: {Equipment.ControlNumber}");
                        WriteLine();
                    }
                    return Equipments.Count();
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
