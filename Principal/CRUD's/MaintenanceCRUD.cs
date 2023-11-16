using System.Threading.Tasks.Dataflow;
using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ConsoleTables;
partial class Program
{
    public static void ViewMaintenanceHistory() //order by equipments
    {
        //ver el historia de mantenimiento
        int op = 0;
        while (op != 1 || op != 2)
        {
            WriteLine("See the Maintenance History.");
            WriteLine("How would you like to see the information?");
            WriteLine("1. All history of maintenances ordered by programmed date");
            WriteLine("2. Search for one equipment maintenance history specifically");
            WriteLine("3. All programmed maintenances that haven't been made yet (just programmed)");
            WriteLine("4. Exit");

            op = TryParseStringaEntero(VerifyReadLengthStringExact(1));
            switch (op)
            {
                case 1:
                    ViewMaintenanceHistoryByEquipment();
                    break;
                case 2:
                    SearchMaintenanceOfEquipment();
                    break;
                case 3:
                    ViewMaintenanceNotMade();
                    break;
                case 4:
                return;
                default:
                    WriteLine("Sorry, that is not an option.");
                    break;
            }
            //Console.Clear();
        }
    }

    /*
        IQueryable<MaintenanceRegister>? maintainHistory = db.MaintenanceRegisters?
        .Include(m=>m.MaintenanceType).Include(m=>m.Maintains.OrderBy(m=>m.EquipmentId));
    */
    public static void ViewMaintenanceHistoryByEquipment()
    {
        //ver historial de mantienimiento de solo un equipo
        Console.Clear();
        using (bd_storage db = new())
        {
            DateTime dateTime = new(year: 2001, month: 01, day: 01);
            IQueryable<Maintain>? maintain = db.Maintain
                .Include(m => m.Maintenance)
                .Include(m => m.Maintenance.MaintenanceType)
                .Include(m => m.Equipment)
                .Include(m => m.Maintenance.Storer)
                .OrderBy(m => m.MaintenanceId)
                .OrderByDescending(m => m.Maintenance.ProgrammedDate);
            //    var maintainGroup = maintain.GroupBy(ma=>ma.Maintenance.MaintenanceId);

            if (maintain is null || !maintain.Any())
            {
                WriteLine("There are no materials registered in maintenance");
                return;
            }

            int countTotal = maintain.Count();
            bool continueListing = true;
            int offset = 0,
                batchS = 20;
            int pages = (countTotal / batchS) + 1;
            int pp = 1;

            while (continueListing)
            {
                var maintainn = maintain.Skip(offset).Take(batchS);

                //                Console.Clear();
                var table = new ConsoleTable("ID", "Equipment ID", "Equipment", "Maintenance", "Instruct. for Maintenance", "Made Maintenance", "Started", "Returned", "Storer", "", "Used Materials");
                
                foreach (var m in maintain)
                {
                    if (
                        m.Maintenance.ExitDate.Date == dateTime
                        || m.Maintenance.MaintenanceMaterialsDescription == "0"
                        || m.Maintenance.MaintenanceDescription == "0"
                    )
                    {
                        table.AddRow(
                            m.MaintenanceId,
                            m.Equipment?.EquipmentId,
                            m.Equipment?.Name,
                            m.Maintenance?.MaintenanceType?.Name,
                            m.Maintenance?.MaintenanceInstructions,
                            "---",
                            m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"),
                            "--/--/----",
                            m.Maintenance?.Storer?.Name,
                            m.Maintenance?.Storer?.LastNameP,
                            "----");
                    }
                    else
                    {
                        table.AddRow(m.MaintenanceId,
                            m.Equipment?.EquipmentId,
                            m.Equipment?.Name,
                            m.Maintenance?.MaintenanceType?.Name,
                            m.Maintenance?.MaintenanceInstructions,
                            m.Maintenance?.MaintenanceDescription,
                            m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"),
                            m.Maintenance?.ExitDate.ToString("dd-MM-yyyy"),
                            m.Maintenance?.Storer?.Name,
                            m.Maintenance?.Storer?.LastNameP,
                            m.Maintenance?.MaintenanceMaterialsDescription
                        );
                    }
                } table.Write();

                WriteLine();
                //WriteLine($"Total:   {countTotal} ");
                WriteLine($"{pp} / {pages}");
                WriteLine("");

                WriteLine(
                    "Presiona una tecla para cargar más resultados (o presiona 'q' para salir)..."
                );
                if (ReadKey(intercept: true).Key == ConsoleKey.LeftArrow)
                {
                    offset = offset - batchS;
                    if (pp > 1)
                    {
                        pp--;
                    }
                    Console.Clear();
                }
                if (ReadKey(intercept: true).Key == ConsoleKey.RightArrow)
                {
                    offset = offset + batchS;
                    if (pp < pages)
                    {
                        pp++;
                    }
                    Console.Clear();
                }
                if (ReadKey(intercept: true).Key == ConsoleKey.Q)
                {
                    continueListing = false;
                    Console.Clear();
                }
            }
        }
    }

    public static void ViewMaintenanceNotMade()
    {
        using (bd_storage db = new())
        {
            DateTime dateTime = new(year: 2001, month: 01, day: 01);
            IQueryable<Maintain>? maintain = db.Maintain
                .Include(m => m.Maintenance)
                .Include(m => m.Maintenance.MaintenanceType)
                .Include(m => m.Equipment)
                .Include(m => m.Maintenance.Storer)
                .Where(m => m.Maintenance.MaintenanceDescription == "0")
                .Where(m => m.Maintenance.ExitDate.Date == dateTime.Date)
                .Where(m => m.Maintenance.MaintenanceMaterialsDescription == "0")
                .OrderBy(m => m.MaintainId);
            // var maintainGroup = maintain.GroupBy(ma=>ma.Maintenance.MaintenanceId);


            if (maintain is null || !maintain.Any())
            {
                WriteLine("There are no pending maintenance registers right now");
                return;
            }

            WriteLine(
                "|{0,-3}|{1,-12}|{2,-55}|{3,-11}|{4,-75}|{5, -15}|{6, -15}|",
                "ID",
                "ID Equipment",
                "Equipment",
                "Maintenance",
                "Instructions for maintenance",
                "Started",
                "Storer ID"
            );
            WriteLine(
                "-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------"
            );
            foreach (var m in maintain)
            {
                WriteLine(
                "|{0,-3}|{1,-12}|{2,-55}|{3,-11}|{4,-75}|{5, -15}|{6, -15}|",
                    m.Maintenance?.MaintenanceId,
                    m.Equipment?.EquipmentId,
                    m.Equipment?.Name,
                    m.Maintenance?.MaintenanceType?.Name,
                    m.Maintenance?.MaintenanceInstructions,
                    m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"),
                    Decrypt(m.Maintenance?.Storer?.StorerId)
                );
            }
        }
    }

    public static void SearchMaintenanceOfEquipment()
    {
        Console.Clear();
        DateTime dateTime = new(year: 2001, month: 01, day: 01);
        using (bd_storage db = new())
        {
            WriteLine();
            WriteLine("Search the Equipment ID:");
            string equipment = VerifyReadMaxLengthString(15);

            IQueryable<Maintain>? maintain = db.Maintain
                .Include(m => m.Maintenance)
                .Include(m => m.Maintenance.MaintenanceType)
                .Include(m => m.Equipment)
                .Include(m => m.Maintenance.Storer)
                .OrderByDescending(m => m.Maintenance.ProgrammedDate)
                .Where(m => m.EquipmentId.StartsWith(equipment));
            //    var maintainGroup = maintain.GroupBy(ma=>ma.Maintenance.MaintenanceId);


            if (maintain is null || !maintain.Any())
            {
                WriteLine("There are no equipments registered with that equipmentID");
                return;
            }
            else
            {
                Console.Clear();

/*
                WriteLine(
                    "|{0,-2}|{1,-10}|{2,-35}|{3,-11}|{4,-20}|{5, -20}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
                    "ID",
                    "ID Equipment",
                    "Equipment",
                    "Maintenance",
                    "Instructions for maintenance",
                    "Description of the made maintenance",
                    "Started",
                    "Returned",
                    "Storer",
                    "",
                    "Used Materials"
                );
                WriteLine(
                    "-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------"
                );
                */
                var table = new ConsoleTable("ID", "Equipment ID", "Equipment", "Maintenance", "Instruct. for Maintenance", "Made Maintenance", "Started", "Returned", "Storer", "", "Used Materials");
                
                foreach (var m in maintain)
                {
                    if (
                        m.Maintenance.ExitDate.Date == dateTime
                        || m.Maintenance.MaintenanceMaterialsDescription == "0"
                        || m.Maintenance.MaintenanceDescription == "0"
                    )
                    {
                        table.AddRow(
                            m.MaintenanceId,
                            m.Equipment?.EquipmentId,
                            m.Equipment?.Name,
                            m.Maintenance?.MaintenanceType?.Name,
                            m.Maintenance?.MaintenanceInstructions,
                            "---",
                            m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"),
                            "--/--/----",
                            m.Maintenance?.Storer?.Name,
                            m.Maintenance?.Storer?.LastNameP,
                            "----");
                    }
                    else
                    {
                        table.AddRow(m.MaintenanceId,
                            m.Equipment?.EquipmentId,
                            m.Equipment?.Name,
                            m.Maintenance?.MaintenanceType?.Name,
                            m.Maintenance?.MaintenanceInstructions,
                            m.Maintenance?.MaintenanceDescription,
                            m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"),
                            m.Maintenance?.ExitDate.ToString("dd-MM-yyyy"),
                            m.Maintenance?.Storer?.Name,
                            m.Maintenance?.Storer?.LastNameP,
                            m.Maintenance?.MaintenanceMaterialsDescription
                        );
                    }
                } table.Write();

            }
        }
    }

    public static int RegisterNewMaintenance(string username) // asks for all info required to create a new maintenance register, creates one, and connects it with all equipments related to it through a maintains table
    {
        WriteLine(
            "Here's a list of all the available equipment for maintenance (Only Available or Damaged Equipment)"
        );
        ViewAllEquipmentsForMaintenance(); // lists all equipments available for maintenance (available or damaged equipment)
        List<string> equipmentIdList = new(); // creates a list that will store every equipment id added to the maintenance register
        List<byte?> statusList = new(); // creates a list that will store a status for each of the equipments added to the maintenance register
        bool valid = false;
        string equipmentId = "";
        do
        {
            WriteLine("Please select the ID of the Equipment you wish to add");
            equipmentId = ReadNonEmptyLine(); // reads a string from the user which is not empty, a whitespace or null
            using (bd_storage db = new()) // creates connection to the database
            {
                IQueryable<Equipment>? availableEquipment = db.Equipments.Where( // searches for the id inside the equipment table
                    e => e.EquipmentId == equipmentId
                ); // checks if the user selected a valid id from the table
                if (availableEquipment is null || !availableEquipment.Any()) // checks if the query returner anything
                {
                    WriteLine($"Equipment ID : {equipmentId} is not-existent");
                }
                else
                {
                    if (
                        availableEquipment.First().StatusId != 2
                        && availableEquipment.First().StatusId != 3
                        && availableEquipment.First().StatusId != 5
                    ) // verifies the equipment chosen has a valid status in order to add it to the report
                    {
                        bool repeated = false;
                        foreach (var eq in equipmentIdList) // verifies that the id chosen wasn´t previously added to the equipment list of the maintenance report
                        {
                            if (equipmentId == eq)
                            {
                                WriteLine($"{equipmentId} was already added");
                                repeated = true;
                            }
                        }
                        if (!repeated) // if it isn´t added yet, it asks for the equipment status
                        {
                            equipmentIdList.Add(equipmentId); // adds the equipment id to the list
                            WriteLine(
                                "Is the equipment under maintenance or should it be kept avalaible until the programmed date?"
                            );
                            WriteLine("1- Under Maintenance");
                            WriteLine("2- Keep available (if it isn't damaged)");
                            bool optValid = false;
                            do // reads an option from the user until them enter a valid option
                            {
                                Write("Option : ");
                                string opt = ReadNonEmptyLine(); // reads a string from the user which is not empty, a whitespace or null

                                IQueryable<Equipment>? statusEquipment = db.Equipments.Where( // finds the equipment with its equipment id
                                    e => e.EquipmentId == equipmentId
                                );
                                if (opt != "1" && opt != "2")
                                {
                                    WriteLine("Please select a valid option, try again");
                                }
                                else if (opt == "1" || statusEquipment.First().StatusId == 4) // changes status to "under maintenance" if the user chooses this option or if the old status was "damaged"
                                {
                                    statusList.Add(5); // adds the status id to the status list
                                    optValid = true;
                                }
                                else // keeps old status
                                {
                                    statusList.Add(statusEquipment.First().StatusId); // adds old status to the status list
                                    optValid = true;
                                }
                            } while (!optValid);
                            WriteLine(
                                "Do you want to add another equipment to the Maintenance Register? y/n"
                            );
                            string moreEquipment = "";
                            bool validAns = false;
                            do // reads from the user until them enter a valid answer
                            {
                                Write("Option: ");
                                moreEquipment = ReadNonEmptyLine(); // reads a string from the user which is not empty, a whitespace or null
                                if (
                                    moreEquipment != "y"
                                    && moreEquipment != "n"
                                    && moreEquipment != "Y"
                                    && moreEquipment != "N"
                                )
                                {
                                    WriteLine("Please select a valid option");
                                }
                                else
                                {
                                    validAns = true;
                                }
                            } while (!validAns);
                            switch (moreEquipment)
                            {
                                case "y": // tells the user how many equipments have they added to the list and starts reading the information necessary to add the next one
                                case "Y":
                                    WriteLine(
                                        $"You have added {equipmentIdList.Count()} equipments until now"
                                    );
                                    break;

                                case "n": // breaks the cycle and continues reading the other info to create the maintenance report
                                case "N":
                                    WriteLine("Continuing...");
                                    valid = true;
                                    break;

                                default:
                                    WriteLine("Option is not valid");
                                    break;
                            }
                        }
                        repeated = false;
                    }
                    else
                    {
                        WriteLine("Please select an equipment from the table shown above");
                    }
                }
            }
        } while (!valid);

        WriteLine();
        WriteLine("Here's a list of all the maintenance types");
        ListMaintenanceTypes(); // lists all three of the maintenance types
        valid = false;
        string mTypeID = "";
        do // reads from the user until them enter a valid option
        {
            WriteLine("Please select the ID of the Maintenance Type you wish to create");
            mTypeID = ReadNonEmptyLine(); // reads a string from the user which is not empty, a whitespace or null
            if (mTypeID != "1" && mTypeID != "2" && mTypeID != "3") // checks if the user entered a valid option
            {
                WriteLine("Please select a valid option");
            }
            else
            {
                valid = true; // continues if the option is valid
            }
        } while (!valid);

        WriteLine();
        WriteLine("Instructions for Maintenance: ");
        string instruct = VerifyReadMaxLengthString(255); // reads maintenance instructions from the user, verifying it does not exceed 255 characters

        WriteLine();
        WriteLine("Programmed date for Maintenance: ");
        DateTime initialDate = ProgrammedMaintenanceDate(); // reads a valid date from the user
        List<DateTime> dateList = new(); // if the user chooses to periodically repeat the maintenance, this list will increase
        dateList.Add(initialDate); // adds the first date to the date list
        DateTime date;
        valid = false;
        if (mTypeID != "2")
        {
            WriteLine();
            WriteLine("Do you want to periodically repeat this maintenance? y/n");
            string repeatMaintenance = "";
            do // reads from the user until them enter a valid option
            {
                Write("Option: ");
                repeatMaintenance = ReadNonEmptyLine(); // reads a string from the user which is not empty, a whitespace or null
                if (
                    repeatMaintenance != "y"
                    && repeatMaintenance != "n"
                    && repeatMaintenance != "Y"
                    && repeatMaintenance != "N"
                )
                {
                    WriteLine("Please select a valid option");
                }
                else
                {
                    valid = true;
                }
            } while (!valid);
            switch (repeatMaintenance)
            {
                case "y": // if the maintenance will be repeates periodically
                case "Y":
                    int maintenanceFrequency = 0; // will tell how many months will pass between each maintenance  
                    int maintenanceQuantity = 0; // will tell how many times the maintenance is repeated
                    WriteLine();
                    WriteLine(
                        "Frequency of Maintenance in Months (ex. 1 - Every Month, 2 - Every 2 months, ...) : "
                    );
                    maintenanceFrequency = TryParseStringaEntero(ReadNonEmptyLine()); // reads a number from the user 

                    WriteLine("How many times do you want to repeat the maintenance?");
                    maintenanceQuantity = TryParseStringaEntero(ReadNonEmptyLine()); // reads a number from the user 

                    date = initialDate;
                    for (int i = 0; i < maintenanceQuantity; i++) // moves x amount of months x amount of times and adds each date to the date list
                    {
                        date = date.Date.AddMonths(maintenanceFrequency); // moves the specified amount of months in the future to create a new date
                        dateList.Add(date); // adds created date to the date list
                    }
                    break;

                case "n": // maintenance won't be repeated
                case "N":
                    WriteLine("Maintenance will only happen once");
                    break;

                default:
                    WriteLine("Option is not valid");
                    break;
            }
        }

        int affected = 0;
        // Add values to new registers from maintenance registers and maintains table and  updates statuses from equipment table on maintenance-related equipments
        using (bd_storage db = new()) // creates connection to database
        {
            if (db.MaintenanceRegisters is null) // checks if maintenance register table exists
                return 0;
            foreach (var dateValue in dateList) // creates a maintenance register for each date on the date lists previosuly filled
            {
                MaintenanceRegister m =
                    new() // creates a new maintenance register object and stores all information related to it
                    {
                        MaintenanceTypeId = Convert.ToByte(mTypeID),
                        MaintenanceInstructions = instruct,
                        ProgrammedDate = dateValue,
                        ExitDate = new(year: 2001, month: 01, day: 01),
                        MaintenanceDescription = "0",
                        StorerId = EncryptPass(username),
                        MaintenanceMaterialsDescription = "0"
                    };
                EntityEntry<MaintenanceRegister> entity = db.MaintenanceRegisters.Add(m); // adds the created object to the maintenance register table
                affected += db.SaveChanges(); // saves changes on the database and adds the amount of rows affected to the total
                int i = 0;
                foreach (var EID in equipmentIdList) // creates a maintains row for each equipment related to the equipment register, and connects it to the maintenance register through maintenance register id 
                {
                    Maintain maintainValue = // creates a new maintain object with the maintenance register id and each of the equipments id related to it
                        new() { MaintenanceId = m.MaintenanceId, EquipmentId = EID };
                    EntityEntry<Maintain> entity2 = db.Maintain.Add(maintainValue); // saves each of the maintain objects on the database
                    affected += db.SaveChanges(); // saves changes on the database and adds the amount of rows affected to the total

                    IQueryable<Equipment>? equipments = db.Equipments.Where(
                        e => e.EquipmentId == EID // searches for the equipment id on the equipment table
                    );
                    byte? status = statusList[i]; // creates a variable status to save each of the statuses previously saved on the status list
                    if (equipments is not null && equipments.Any())
                    {
                        equipments.ExecuteUpdate( // updates status on each of the equipments whick were added to the maintenance report
                            u =>
                                u.SetProperty(
                                    p => p.StatusId, // Property Selctor
                                    p => status // Value to edit
                                )
                        );
                        affected += db.SaveChanges(); // saves changes on the database and adds the amount of rows affected to the total
                    }
                    i++; // increments index for status list
                }
            }
            return affected;
        }
    }

    public static int FinishMaintenanceReport(string username) // updates returnedDate, maintenanceDescription and maintenanceMatsDescription with values entered by user for a pending maintenance register chosen by the user
    {
        using (bd_storage db = new()) // creates connection with the database
        {
            DateTime? endDate = new();
            DateTime startDate = new();
            string maintenanceDesc = "";
            string maintenanceMatsDesc = "";

            WriteLine(
                "Here's a list of all Maintenances Registers that haven't been completed yet"
            );
            WriteLine();
            ViewMaintenanceNotMade(); // lists all pending maintenance registers
            WriteLine();

            bool valid = false;
            string maintenanceRegId = "";
            do
            {
                WriteLine(
                    "Please select the ID of the Maintenance and the date in which the maintenance you wish to complete was started"
                );
                Write("Maintenance ID : ");
                maintenanceRegId = ReadNonEmptyLine(); // reads a string from the user which is not empty, a whitespace or null

                WriteLine();
                WriteLine("Date : ");
                startDate = ProgrammedMaintenanceDate(); // reads a valid date from the user

                IQueryable<Maintain>? mRegisters = db.Maintain // searches for maintains register that have the maintenance register id and that inside them, have the programmed date entered by the user                    .Include(m => m.Maintenance)
                    .Include(m => m.Equipment)
                    .Include(m => m.Maintenance)
                    .Where(
                        m =>
                            m.Maintenance.MaintenanceId == Convert.ToInt32(maintenanceRegId)
                            && m.Maintenance.ProgrammedDate == startDate
                    ); // checks if the user selected a valid maintenance register from the table
                if (mRegisters is null || !mRegisters.Any()) // checks if the query returned anything
                {
                    WriteLine($"ID '{maintenanceRegId}' is not-existent or date entered is wrong");
                }
                else
                {
                    valid = true;

                    WriteLine();
                    WriteLine(
                        "Enter the date when the equipment(s) was(were) returned after maintenance"
                    );
                    endDate = ReturnMaintenanceDate(mRegisters.First().Maintenance.ProgrammedDate); // reads a valid date from the user and verifies it isn´t before the programmed date

                    WriteLine();
                    WriteLine("Please describe what was done to the equipment");
                    maintenanceDesc = VerifyReadMaxLengthString(255); // reads a description from the user and verifies it doesn't exceed 255 characters

                    WriteLine();
                    WriteLine(
                        "Please describe which materials were used for the maintenance"
                    );
                    maintenanceMatsDesc = VerifyReadMaxLengthString(100); // reads a materials description from the user and verifies it doesn't exceed 100 characters
                    if (!valid)
                    {
                        WriteLine("Please select an ID of the ones showed above");
                    }
                }
            } while (!valid);

            int affected = 0;
            byte status = 1;

            IQueryable<MaintenanceRegister>? updateRegisters = db.MaintenanceRegisters.Where( // searches for the chosen maintenance register id and the programmed date inside it
                m =>
                    m.MaintenanceId == Convert.ToInt32(maintenanceRegId)
                    && m.ProgrammedDate == startDate
            ); 

            if (updateRegisters is not null && updateRegisters.Any()) // verifies the query returned something
            {
                updateRegisters.ExecuteUpdate( // updates fields with the info entered by the user
                    u =>
                        u.SetProperty(
                                m => m.ExitDate, // Property Selctor
                                m => endDate // Value to edit
                            )
                            .SetProperty(
                                m => m.MaintenanceDescription, // Property Selctor
                                m => maintenanceDesc // Value to edit
                            )
                            .SetProperty(
                                m => m.MaintenanceMaterialsDescription, // Property Selctor
                                m => maintenanceMatsDesc // Value to edit
                            )
                );
                affected += db.SaveChanges(); // saves changes on the database and adds the amount of rows affected to the total
            }

            IQueryable<string>? maintainedEquipmentsId = db.Maintain // returns all equipments related to the maintenance register 
                .Include(m => m.Maintenance)
                .Include(m => m.Equipment)
                .Where(
                    m =>
                        m.Maintenance.MaintenanceId == Convert.ToInt32(maintenanceRegId)
                        && m.Maintenance.ProgrammedDate == startDate
                )
                .Select(m => m.EquipmentId);

            if (maintainedEquipmentsId is not null && maintainedEquipmentsId.Any()) //checks if the query returned anything
            {
                foreach (var EID in maintainedEquipmentsId) // goes through each one of ther equipments related to the maintenance register
                {
                    IQueryable<Equipment>? maintainedEquipments = db.Equipments.Where( // searches for the equipment inside the equipment table
                        m => m.EquipmentId == EID
                    );

                    if (maintainedEquipments is not null && maintainedEquipments.Any()) // checks if the query returned anything
                    {
                        maintainedEquipments.ExecuteUpdate( // updates the status of each of the equipments to "Available"
                            u =>
                                u.SetProperty(
                                    w => w.StatusId, // Property Selctor
                                    w => status // Value to edit
                                )
                        );
                        affected += db.SaveChanges(); // saves changes on the database and adds the amount of rows affected to the total
                    }
                }
            }
            return affected;
        }
    }
}