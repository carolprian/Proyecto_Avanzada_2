using System.Threading.Tasks.Dataflow;
using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

partial class Program{
    public static void ViewMaintenanceHistory() //order by equipments
    {  
        int op=0;
        while(op!=1 ||op!=2)
        {
            WriteLine("See the Maintenance History.");
            WriteLine("How would you like to see the information?");
            WriteLine("1. All history of maintenances ordered by programmed date");
            WriteLine("2. Search for one equipment maintenance history specifically");
            WriteLine("3. All programmed maintenances that haven't been made yet (just programmed)");
            WriteLine("3. exit");
  
            op = TryParseStringaEntero(VerifyReadLengthStringExact(1));
            switch(op){
                case 1:
                    ViewMaintenanceHistoryByEquipment();
                break;
                case 2:
                    SearchMaintenanceOfEquipment();
                break;
                case 3:
                    ViewMaintenanceNotMade();
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
    public static void ViewMaintenanceHistoryByEquipment(){
        Console.Clear();
        using(bd_storage db = new())
        {
            DateTime dateTime = new(year:2001, month: 01, day: 01);
            IQueryable<Maintain>? maintain = db.Maintain
            .Include(m=>m.Maintenance)
            .Include(m=>m.Maintenance.MaintenanceType)
            .Include(m=>m.Equipment)
            .Include(m=>m.Maintenance.Storer)
            .OrderBy(m=>m.MaintenanceId)
            .OrderByDescending(m=>m.Maintenance.ProgrammedDate);
        //    var maintainGroup = maintain.GroupBy(ma=>ma.Maintenance.MaintenanceId);
        
            if(maintain is null || !maintain.Any())
            {
                WriteLine("There are no materials registered in maintenance");
                return;
            } 
            
            int countTotal = maintain.Count();
            bool continueListing = true;
            int offset = 0, batchS = 20;
            int pages = (countTotal / batchS) +1;
            int pp=1;

            while (continueListing)
                {
               var maintainn = maintain.Skip(offset).Take(batchS);

//                Console.Clear();
                
                    WriteLine("|{0,-2}|{1,-15}|{2,-20}|{3,-11}|{4,-40}|{5, -40}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
                "ID", "ID Equipment", "Equipment", "Maintenance", "Instructions for maintenance", "Description of the made maintenance", "Started", "Returned", "Storer", "",  "Used Materials" );
            WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    foreach( var m in maintain)
                    {
                         if(m.Maintenance.ExitDate.Date == dateTime || m.Maintenance.MaintenanceMaterialsDescription == "0" || m.Maintenance.MaintenanceDescription == "0")
                         {
                                WriteLine("|{0,-2}|{1,-15}|{2,-20}|{3,-11}|{4,-40}|{5, -40}|{6, -10}|{7,-10}|{8, 10} {9,-10}|{10}",
                            m.MaintenanceId, m.Equipment?.EquipmentId, m.Equipment?.Name, m.Maintenance?.MaintenanceType?.Name, m.Maintenance?.MaintenanceInstructions, "---", m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"), "--/--/----", m.Maintenance?.Storer?.Name, m.Maintenance?.Storer?.LastNameP, "----"); 
                         }
                         else{
                                WriteLine("|{0,-2}|{1,-15}|{2,-20}|{3,-11}|{4,-40}|{5, -40}|{6, -10}|{7,-10}|{8, 10} {9,-10}|{10}",
                            m.MaintenanceId, m.Equipment?.EquipmentId, m.Equipment?.Name, m.Maintenance?.MaintenanceType?.Name, m.Maintenance?.MaintenanceInstructions, m.Maintenance?.MaintenanceDescription, m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"), m.Maintenance?.ExitDate.ToString("dd-MM-yyyy"), m.Maintenance?.Storer?.Name, m.Maintenance?.Storer?.LastNameP, m.Maintenance?.MaintenanceMaterialsDescription);
                    
                         }
                    }
            
                WriteLine();
                WriteLine($"Total:   {countTotal} ");
                WriteLine($"{pp} / {pages}");
                WriteLine("");

                WriteLine("Presiona una tecla para cargar más resultados (o presiona 'q' para salir)...");
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

   public static void ViewMaintenanceNotMade(){
        Console.Clear();
        using(bd_storage db = new())
        {
            DateTime dateTime = new(year:2001, month: 01, day: 01);
            IQueryable<Maintain>? maintain = db.Maintain
            .Include(m=>m.Maintenance)
            .Include(m=>m.Maintenance.MaintenanceType)
            .Include(m=>m.Equipment)
            .Include(m=>m.Maintenance.Storer)
            .Where(m=>m.Maintenance.MaintenanceDescription == "0")
            .Where(m=>m.Maintenance.ExitDate.Date == dateTime.Date)
            .Where(m=>m.Maintenance.MaintenanceMaterialsDescription == "0")
            .OrderBy(m=>m.MaintenanceId)
            .OrderByDescending(m=>m.Maintenance.ProgrammedDate);
        //    var maintainGroup = maintain.GroupBy(ma=>ma.Maintenance.MaintenanceId);
        

            if(maintain is null || !maintain.Any())
            {
                WriteLine("There are no materials registered in maintenance");
                return;
            } 
            
            int countTotal = maintain.Count();
            bool continueListing = true;
            int offset = 0, batchS = 20;
            int pages = (countTotal / batchS) +1;
            int pp=1;

            while (continueListing)
                {
               var maintainn = maintain.Skip(offset).Take(batchS);

//                Console.Clear();
                
                        WriteLine("|{0,-2}|{1,-15}|{2,-20}|{3,-11}|{4,-40}|{5, -40}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
                "ID", "ID Equipment", "Equipment", "Maintenance", "Instructions for maintenance", "Description of the made maintenance", "Started", "Returned", "Storer", "",  "Used Materials" );
            WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    foreach( var m in maintainn)
                    {
                        WriteLine("|{0,-2}|{1,-15}|{2,-20}|{3,-11}|{4,-40}|{5, -40}|{6, -10}|{7,-10}|{8, 10} {9,-10}|{10}",
                    m.MaintenanceId, m.Equipment?.EquipmentId, m.Equipment?.Name, m.Maintenance?.MaintenanceType?.Name, m.Maintenance?.MaintenanceInstructions, m.Maintenance?.MaintenanceDescription, m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"), m.Maintenance?.ExitDate.ToString("dd-MM-yyyy"), m.Maintenance?.Storer?.Name, m.Maintenance?.Storer?.LastNameP, m.Maintenance?.MaintenanceMaterialsDescription);
                    
                    }
            
                WriteLine();
                WriteLine($"Total:   {countTotal} ");
                WriteLine($"{pp} / {pages}");
                WriteLine("");

                WriteLine("Presiona una tecla para cargar más resultados (o presiona 'q' para salir)...");
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

    public static void SearchMaintenanceOfEquipment()
    {
        Console.Clear();
            DateTime dateTime = new(year:2001, month: 01, day: 01);
        using(bd_storage db = new())
        {
            WriteLine();
            WriteLine("Search the Equipment ID:");
            string equipment = VerifyReadMaxLengthString(15);
            
            IQueryable<Maintain>? maintain = db.Maintain
            .Include(m=>m.Maintenance).Include(m=>m.Maintenance.MaintenanceType).Include(m=>m.Equipment).Include(m=>m.Maintenance.Storer).OrderByDescending(m=>m.Maintenance.ProgrammedDate).Where(m=>m.EquipmentId.StartsWith(equipment));
        //    var maintainGroup = maintain.GroupBy(ma=>ma.Maintenance.MaintenanceId);
        

            if(maintain is null || !maintain.Any())
            {
                WriteLine("There are no equipments registered with that equipmentID");
                return;
            } 
            else
            {
                Console.Clear();
                
                        WriteLine("|{0,-2}|{1,-15}|{2,-20}|{3,-11}|{4,-40}|{5, -40}|{6, 10}|{7,-10}|{8, 10} {9,-10}|{10}",
                "ID", "ID Equipment", "Equipment", "Maintenance", "Instructions for maintenance", "Description of the made maintenance", "Started", "Returned", "Storer", "",  "Used Materials" );
            WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    foreach( var m in maintain)
                    {
                        if(m.Maintenance.ExitDate.Date == dateTime || m.Maintenance.MaintenanceMaterialsDescription == "0" || m.Maintenance.MaintenanceDescription == "0")
                         {
                                WriteLine("|{0,-2}|{1,-15}|{2,-20}|{3,-11}|{4,-40}|{5, -40}|{6, -10}|{7,-10}|{8, 10} {9,-10}|{10}",
                            m.MaintenanceId, m.Equipment?.EquipmentId, m.Equipment?.Name, m.Maintenance?.MaintenanceType?.Name, m.Maintenance?.MaintenanceInstructions, "---", m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"), "--/--/----", m.Maintenance?.Storer?.Name, m.Maintenance?.Storer?.LastNameP, "----"); 
                         }
                         else
                         {
                                WriteLine("|{0,-2}|{1,-15}|{2,-20}|{3,-11}|{4,-40}|{5, -40}|{6, -10}|{7,-10}|{8, 10} {9,-10}|{10}",
                            m.MaintenanceId, m.Equipment?.EquipmentId, m.Equipment?.Name, m.Maintenance?.MaintenanceType?.Name, m.Maintenance?.MaintenanceInstructions, m.Maintenance?.MaintenanceDescription, m.Maintenance?.ProgrammedDate.ToString("dd-MM-yyyy"), m.Maintenance?.ExitDate.ToString("dd-MM-yyyy"), m.Maintenance?.Storer?.Name, m.Maintenance?.Storer?.LastNameP, m.Maintenance?.MaintenanceMaterialsDescription); 
                         }
                    }
            }
            
    }
    }
}