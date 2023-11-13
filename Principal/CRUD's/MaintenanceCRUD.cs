using System.Threading.Tasks.Dataflow;
using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
            WriteLine(
                "3. All programmed maintenances that haven't been made yet (just programmed)"
            );
            WriteLine("4. exit");

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

                WriteLine(
                    "|{0,-2}|{1,-10}|{2,-45}|{3,-11}|{4,-20}|{5, -20}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
                    "ID",
                    "ID Equipment",
                    "Equipment",
                    "Maintenance",
                    "Instructions for maintenance",
                    "Description of the maintenance",
                    "Started",
                    "Returned",
                    "Storer",
                    "",
                    "Used Materials"
                );
                WriteLine(
                    "-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------"
                );
                foreach (var m in maintain)
                {
                    if (
                        m.Maintenance.ExitDate.Date == dateTime
                        || m.Maintenance.MaintenanceMaterialsDescription == "0"
                        || m.Maintenance.MaintenanceDescription == "0"
                    )
                    {
                        WriteLine(
                            "|{0,-2}|{1,-10}|{2,-50}|{3,-11}|{4,-20}|{5, -20}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
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
                            "----"
                        );
                    }
                    else
                    {
                        WriteLine(
                            "|{0,-2}|{1,-10}|{2,-50}|{3,-11}|{4,-20}|{5, -20}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
                            m.MaintenanceId,
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
                }

                WriteLine();
                WriteLine($"Total:   {countTotal} ");
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
        Console.Clear();
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
                "|{0,-3}|{1,-12}|{2,-55}|{3,-11}|{4,-50}|{5, -15}|{6, -15}|",
                "ID",
                "ID Equipment",
                "Equipment",
                "Maintenance",
                "Instructions for maintenance",
                "Started",
                "Storer ID"
            );
            WriteLine(
                "-------------------------------------------------------------------------------------------------------------------------------------------------------"
            );
            foreach (var m in maintain)
            {
                WriteLine(
                "|{0,-3}|{1,-12}|{2,-55}|{3,-11}|{4,-50}|{5, -15}|{6, -15}|",
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
                foreach (var m in maintain)
                {
                    if (
                        m.Maintenance.ExitDate.Date == dateTime
                        || m.Maintenance.MaintenanceMaterialsDescription == "0"
                        || m.Maintenance.MaintenanceDescription == "0"
                    )
                    {
                        WriteLine(
                            "|{0,-2}|{1,-10}|{2,-35}|{3,-11}|{4,-20}|{5, -20}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
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
                            "----"
                        );
                    }
                    else
                    {
                        WriteLine(
                            "|{0,-2}|{1,-10}|{2,-35}|{3,-11}|{4,-20}|{5, -20}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
                            m.MaintenanceId,
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
                }
            }
        }
    }

    public static int RegisterNewMaintenance(string username)
    {
        WriteLine(
            "Here's a list of all the available equipment for maintenance (Only Available or Damaged Equipment)"
        );
        ViewAllEquipmentsForMaintenance();
        List<string> equipmentIdList = new();
        List<byte?> statusList = new();
        bool valid = false;
        string equipmentId = "";
        do
        {
            WriteLine("Please select the ID of the Equipment you wish to add");
            equipmentId = ReadNonEmptyLine();
            using (bd_storage db = new())
            {
                IQueryable<Equipment>? availableEquipment = db.Equipments.Where(
                    e => e.EquipmentId == equipmentId
                ); // checks if the user selected a valid id from the table
                if (availableEquipment is null || !availableEquipment.Any())
                {
                    WriteLine($"Equipment ID : {equipmentId} is not-existent");
                }
                else
                {
                    if (
                        availableEquipment.First().StatusId != 2
                        && availableEquipment.First().StatusId != 3
                        && availableEquipment.First().StatusId != 5
                    )
                    {
                        bool repeated = false;
                        foreach (var eq in equipmentIdList)
                        {
                            if (equipmentId == eq)
                            {
                                WriteLine($"{equipmentId} was already added");
                                repeated = true;
                            }
                        }
                        if (!repeated)
                        {
                            equipmentIdList.Add(equipmentId);
                            WriteLine(
                                "Is the equipment under maintenance or should it be kept avalaible until the programmed date?"
                            );
                            WriteLine("1- Under Maintenance");
                            WriteLine("2- Keep available (if it isn't damaged)");
                            bool optValid = false;
                            do
                            {
                                Write("Option : ");
                                string opt = ReadNonEmptyLine();

                                IQueryable<Equipment>? statusEquipment = db.Equipments.Where(
                                    e => e.EquipmentId == equipmentId
                                );
                                if (opt != "1" && opt != "2")
                                {
                                    WriteLine("Please select a valid option, try again");
                                }
                                else if (opt == "1" || statusEquipment.First().StatusId == 4)
                                {
                                    statusList.Add(5);
                                    optValid = true;
                                }
                                else
                                {
                                    statusList.Add(statusEquipment.First().StatusId);
                                    optValid = true;
                                }
                            } while (!optValid);
                            WriteLine(
                                "Do you want to add another equipment to the Maintenance Register? y/n"
                            );
                            string moreEquipment = "";
                            bool validAns = false;
                            do
                            {
                                Write("Option: ");
                                moreEquipment = ReadNonEmptyLine();
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
                                case "y":
                                case "Y":
                                    WriteLine(
                                        $"You have added {equipmentIdList.Count()} equipments until now"
                                    );
                                    break;

                                case "n":
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
        ListMaintenanceTypes();
        valid = false;
        string mTypeID = "";
        do
        {
            WriteLine("Please select the ID of the Maintenance Type you wish to create");
            mTypeID = ReadNonEmptyLine();
            if (mTypeID != "1" && mTypeID != "2" && mTypeID != "3")
            {
                WriteLine("Please select a valid option");
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        WriteLine();
        WriteLine("Instructions for Maintenance: ");
        string instruct = VerifyReadMaxLengthString(255);

        //! A CHAMBEAR
        // TODO: Valid date protections (not a date in the past or before programmed date, a day between 1 and 31, a month between 1 and 12, and if the day exists on that month)
        //! A CHAMBEAR

        // normal

        WriteLine();
        WriteLine("Programmed date for Maintenance: ");
        DateTime initialDate = ProgrammedMaintenanceDate();
        List<DateTime> dateList = new();
        dateList.Add(initialDate);
        DateTime date;
        valid = false;
        if (mTypeID != "2")
        {
            WriteLine();
            WriteLine("Do you want to periodically repeat this maintenance? y/n");
            string repeatMaintenance = "";
            do
            {
                Write("Option: ");
                repeatMaintenance = ReadNonEmptyLine();
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
                case "y":
                case "Y":
                    int maintenanceFrequency = 0;
                    int maintenanceQuantity = 0;
                    WriteLine();
                    WriteLine(
                        "Frequency of Maintenance in Months (ex. 1 - Every Month, 2 - Every 2 months, ...) : "
                    );
                    maintenanceFrequency = TryParseStringaEntero(ReadNonEmptyLine());

                    WriteLine("How many times do you want to repeat the maintenance?");
                    maintenanceQuantity = TryParseStringaEntero(ReadNonEmptyLine());

                    date = initialDate;
                    for (int i = 0; i < maintenanceQuantity; i++)
                    {
                        date = date.Date.AddMonths(maintenanceFrequency);
                        dateList.Add(date);
                    }
                    break;

                case "n":
                case "N":
                    WriteLine("Maintenance will only happen once");
                    break;

                default:
                    WriteLine("Option is not valid");
                    break;
            }
        }

        int affected = 0;
        // Add values to new register
        using (bd_storage db = new())
        {
            if (db.MaintenanceRegisters is null)
                return 0;
            foreach (var dateValue in dateList)
            {
                MaintenanceRegister m =
                    new()
                    {
                        MaintenanceTypeId = Convert.ToByte(mTypeID),
                        MaintenanceInstructions = instruct,
                        ProgrammedDate = dateValue,
                        ExitDate = new(year: 2001, month: 01, day: 01),
                        MaintenanceDescription = "0",
                        StorerId = EncryptPass(username),
                        MaintenanceMaterialsDescription = "0"
                    };
                EntityEntry<MaintenanceRegister> entity = db.MaintenanceRegisters.Add(m);
                affected += db.SaveChanges();
                int i = 0;
                foreach (var EID in equipmentIdList)
                {
                    Maintain maintainValue =
                        new() { MaintenanceId = m.MaintenanceId, EquipmentId = EID };
                    EntityEntry<Maintain> entity2 = db.Maintain.Add(maintainValue);
                    affected += db.SaveChanges();

                    IQueryable<Equipment>? equipments = db.Equipments.Where(
                        e => e.EquipmentId == EID
                    );
                    byte? status = statusList[i];
                    if (equipments is not null && equipments.Any())
                    {
                        equipments.ExecuteUpdate(
                            u =>
                                u.SetProperty(
                                    p => p.StatusId, // Property Selctor
                                    p => status // Value to edit
                                )
                        );
                        affected += db.SaveChanges();
                    }
                    i++;
                }
            }
            return affected;
        }
    }

    // TODO: finish execute update and add protections for reading data from the user
    public static int FinishMaintenanceReport(string username)
    {
        using (bd_storage db = new())
        {
            DateTime? endDate = new();
            DateTime startDate = new();
            string maintenanceDesc = "";
            string maintenanceMatsDesc = "";

            WriteLine(
                "Here's a list of all Maintenances Registers that haven't been completed yet"
            );
            WriteLine();
            ViewMaintenanceNotMade();
            WriteLine();

            bool valid = false;
            string maintenanceRegId = "";
            do
            {
                WriteLine(
                    "Please select the ID of the Maintenance and the date in which the maintenance you wish to complete was started"
                );
                Write("Maintenance ID : ");
                maintenanceRegId = ReadNonEmptyLine();

                WriteLine();
                WriteLine("Date : ");
                startDate = ProgrammedMaintenanceDate();

                IQueryable<Maintain>? mRegisters = db.Maintain
                    .Include(m => m.Maintenance)
                    .Include(m => m.Equipment)
                    .Where(
                        m =>
                            m.Maintenance.MaintenanceId == Convert.ToInt32(maintenanceRegId)
                            && m.Maintenance.ProgrammedDate == startDate
                    ); // checks if the user selected a valid id from the table
                if (mRegisters is null || !mRegisters.Any())
                {
                    WriteLine($"ID '{maintenanceRegId}' is not-existent or date entered is wrong");
                }
                else
                {
                    foreach (var mRegister in mRegisters)
                    {
                        if (
                            mRegister.Maintenance?.ExitDate.Date
                            < mRegister.Maintenance?.ProgrammedDate.Date
                        )
                        {
                            valid = true;

                            WriteLine();
                            WriteLine(
                                "Enter the date when the equipment(s) was(were) returned after maintenance"
                            );
                            endDate = ReturnMaintenanceDate(mRegisters.First().Maintenance.ProgrammedDate);

                            WriteLine();
                            WriteLine("Please describe what was done to the equipment");
                            maintenanceDesc = VerifyReadMaxLengthString(255);

                            WriteLine();
                            WriteLine(
                                "Please describe which materials were used for the maintenance"
                            );
                            maintenanceMatsDesc = VerifyReadMaxLengthString(100);
                        }
                    }
                    if (!valid)
                    {
                        WriteLine("Please select an ID of the ones showed above");
                    }
                }
            } while (!valid);

            int affected = 0;
            byte status = 1;

            IQueryable<MaintenanceRegister>? updateRegisters = db.MaintenanceRegisters.Where(
                m =>
                    m.MaintenanceId == Convert.ToInt32(maintenanceRegId)
                    && m.ProgrammedDate == startDate
            ); 

            if (updateRegisters is not null && updateRegisters.Any())
            {
                updateRegisters.ExecuteUpdate(
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
                affected += db.SaveChanges();
            }

            IQueryable<string>? maintainedEquipmentsId = db.Maintain
                .Include(m => m.Maintenance)
                .Include(m => m.Equipment)
                .Where(
                    m =>
                        m.Maintenance.MaintenanceId == Convert.ToInt32(maintenanceRegId)
                        && m.Maintenance.ProgrammedDate == startDate
                )
                .Select(m => m.EquipmentId);

            if (maintainedEquipmentsId is not null && maintainedEquipmentsId.Any())
            {
                foreach (var EID in maintainedEquipmentsId)
                {
                    IQueryable<Equipment>? maintainedEquipments = db.Equipments.Where(
                        m => m.EquipmentId == EID
                    );

                    if (maintainedEquipments is not null && maintainedEquipments.Any())
                    {
                        maintainedEquipments.ExecuteUpdate(
                            u =>
                                u.SetProperty(
                                    w => w.StatusId, // Property Selctor
                                    w => status // Value to edit
                                )
                        );
                        affected += db.SaveChanges();
                    }
                }
            }
            return affected;
        }
    }
}