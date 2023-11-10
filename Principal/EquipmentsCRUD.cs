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

    // UPDATE
    public static void UpdateEquipment()
    {
        WriteLine("Here's a list of all registered equipment, please enter the ID of the equipment you wish to change");
        ViewAllEquipments(1);
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

    // DELETE
    public static void DeleteEquipment()
    {
        WriteLine("Here's a list of all registered equipment, please enter the ID of the equipment you wish to change");
        ViewAllEquipments(1);
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

    // READ
    public static void ViewAllEquipments(int op)
    {
        if (op==1){
            using( bd_storage db = new())
            {
                IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Area).Include(e => e.Status).Include(e => e.Coordinator);

                db.ChangeTracker.LazyLoadingEnabled = false;
                if((equipments is null) || !equipments.Any())
                {
                    WriteLine("There are no status found");
                }
                int i=1;
                WriteLine("| {0,-5} | {1,-15} | {2,-27} | {3,-22} | {4,-58} | {5,7} | {6,-13} | {7,15}",
                    "Index", "EquipmentId", "Equipment Name", "Area", "Description", "Year", "Status", "Control Number", "Coordinator ID");
                Write("-------------------------------------------------------------------------------------------------------------------");
                WriteLine("-------------------------------------------------------------------------------");

                foreach (var e in equipments)
                {
                    WriteLine("| {0,-5} | {1,-15} | {2,-27} | {3,-22} | {4,-58} | {5,7} | {6,-13} | {7,15}",
                        i, e.EquipmentId, e.Name, e.Area?.Name, e.Description, e.Year, e.Status?.Value, e.ControlNumber, e.Coordinator?.CoordinatorId);
                    i++;
                }
            }
        } else 
        {
            using( bd_storage db = new())
            {
                IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Status).Where(s => s.Status.StatusId==1 || s.Status.StatusId==2);

                db.ChangeTracker.LazyLoadingEnabled = false;
                if((equipments is null) || !equipments.Any())
                {
                    WriteLine("There are no status found");
                }
                int i=1;
                WriteLine("| {0,-5} | {1,-15} | {2,-27} | {3,-22}", "Index", "EquipmentId", "Equipment Name", "Description");
                WriteLine("-------------------------------------------------------------------------------");

                foreach (var e in equipments)
                {
                    WriteLine("| {0,-5} | {1,-15} | {2,-27} | {3,-22}",
                        i, e.EquipmentId, e.Name, e.Description);
                    i++;
                }
            }
        }
    }

    public static void SearchEquipmentsByName(string searchTerm)
    {
        using (bd_storage db = new())
        {
            IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator)
                .Where(e => e.Name.StartsWith(searchTerm)); // Utiliza StartsWith para buscar equipos cuyos nombres comiencen con el término de búsqueda

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!equipments.Any())
            {
                WriteLine("No equipment found matching the search term: " + searchTerm);
                return;
            }

            WriteLine("| {0,-11} | {1,-15} | {2,-26} | {3,-80} | {4,4} | {5,17} | {6,20} | {7,6}",
                "EquipmentId", "Name", "Area", "Description", "Year", "Status", "ControlNumber", "Coordinator");

            foreach (var e in equipments)
            {
                WriteLine($"| {e.EquipmentId,-11} | {e.Name,-15} | {e.Area?.Name,-26} | {e.Description,-80} | {e.Year,4} | {e.Status?.Value,17} | {e.ControlNumber,20} | {e.Coordinator?.Name,6}");
            }
        }
        
    }

    public static void ViewAllEquipmentsForMaintenance()
    {
        using( bd_storage db = new())
        {
            IQueryable<Equipment>? equipments = db.Equipments
            .Include(e => e.Area).Include(e => e.Status).Include(e => e.Coordinator);

            if((equipments is null) || !equipments.Any())
            {
                WriteLine("No equipment was found");
            }
            WriteLine("| {1,-15} | {2,-27} | {3,-22} | {4,-58} | {5,7} | {6,-13} | {7,15}",
                "EquipmentId", "Equipment Name", "Area", "Description", "Year", "Status", "Control Number", "Coordinator ID");
            Write("-------------------------------------------------------------------------------------------------------------------");
            WriteLine("-------------------------------------------------------------------------------");

            foreach (var e in equipments)
            {
                if(e.StatusId != 2 && e.StatusId != 3 && e.StatusId != 5)
                {
                     WriteLine("| {1,-15} | {2,-27} | {3,-22} | {4,-58} | {5,7} | {6,-13} | {7,15}",
                    e.EquipmentId, e.Name, e.Area?.Name, e.Description, e.Year, e.Status?.Value, e.ControlNumber, e.Coordinator?.CoordinatorId);
                }               
            }
        }
    }
    
/*
    public static void ViewAEquipments()
{
        using (bd_storage db = new())
        {
        IQueryable<Equipment>? equipments = (IQueryable<Equipment>?)db.Equipments
    .Join(
        db.Areas,
        equipment => equipment.AreaId,
        area => area.AreaId,
        (equipment, area) => new { Equipment = equipment, Area = area }
    )
    .Join(
        db.Statuses,
        combined => combined.Equipment.StatusId,
        status => status.StatusId,
        (combined, status) => new 
        {
            EquipmentId = combined.Equipment.EquipmentId,
            Name = combined.Equipment.Name,
            AreaName = combined.Area.Name,
            Description = combined.Equipment.Description,
            Year = combined.Equipment.Year,
            StatusName = status.Value,
            ControlNumber = combined.Equipment.ControlNumber
        }   
    );


                if (equipments == null || !equipments.Any())
                {
                    WriteLine("There are no equipments found");
                }
                else
                {
                    foreach (var equipment in equipments)
                    {
                        string? areaName = equipment.Area != null ? equipment.Area.Name : "N/A";
                        string? statusValue = equipment.Status != null ? equipment.Status.Value : "N/A";
                        WriteLine($"{equipment.EquipmentId} . {areaName} . {statusValue}");
                    }
                }

        }
    }

    public static void ViewAlEquipments()
{
    using (bd_storage db = new())
    {
        var query = db.Equipments
            .Join(
                db.Areas,
                equipment => equipment.AreaId,
                area => area.AreaId,
                (equipment, area) => new { Equipment = equipment, AreaName = area.Name }
            )
            .Join(
                db.Statuses,
                combined => combined.Equipment.StatusId,
                status => status.StatusId,
                (combined, status) => new
                {
                    combined.Equipment.EquipmentId,
                    combined.Equipment.Name,
                    combined.AreaName,
                    combined.Equipment.Description,
                    combined.Equipment.Year,
                    StatusValue = status.Value,
                    combined.Equipment.ControlNumber,
                    combined.Equipment.CoordinatorId
                }
            )
            .Take(20);

        WriteLine($"ToQueryString: {query.ToQueryString()}");

        if (query is null || !query.Any())
        {
            WriteLine("No hay resultados.");
        }
        else
        {
            foreach (var equipment in query)
            {
                WriteLine($"Equipment ID: {equipment.EquipmentId}");
                WriteLine($"Name: {equipment.Name}");
                WriteLine($"Area: {equipment.AreaName}");
                WriteLine($"Description: {equipment.Description}");
                WriteLine($"Year: {equipment.Year}");
                WriteLine($"Status ID: {equipment.StatusValue}");
                WriteLine($"Control Number: {equipment.ControlNumber}");
                WriteLine($"Coordinator ID: {equipment.CoordinatorId}");
                WriteLine();
            }

            WriteLine("Presiona una tecla para cargar más resultados...");
            ReadKey();
        }
    }
}
*/    
}
