using System.Linq;
using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


partial class Program{
    
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
                ControlNumber = controlnumber
            };
            

            EntityEntry<Equipment> entity = db.Equipments.Add(e);
            int affected = db.SaveChanges();
            return (affected, e.EquipmentId);
        }
    }
    public static void UpdateEquipment()
    {

    }
    public static void DeleteEquipment()
    {
    
    }
/*
    public static void ViewAEquipments()
{
        using (bd_storage db = new())
        {
<<<<<<< HEAD
        // using (bd_storage db = new())
        // {
        //     if((db.Products is null) || (!db.Products.Any()))
        //     {
        //         Fail("There are no products");
        //         return;
        //     }
        //     WriteLine("| {0,-3} | {1,-35} | {2,8} | {3,5} | {4}",
        //     "Id", "Product Name", "Cost", "Stock", "Disc");
        //     foreach (var product in db.Products)
        //     {
        //         ConsoleColor backgroundColor = ForegroundColor;
        //         if((productsToHiglight is not null) && productsToHiglight.Contains(product.ProductId))
        //         {
        //             ForegroundColor = ConsoleColor.Red;
        //         }
        //         WriteLine("| {0:000} | {1,-35} | {2,8:$#,##0.00} | {3,5} | {4}",
        //         product.ProductId, product.ProductName, product.Cost, product.Stock, product.Discontinued);
        //         ForegroundColor = backgroundColor;
        //     }
        // }
=======
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

>>>>>>> 405b74ce6784aaa8a54e86b24c017990daedf7ff
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

    public static void ViewAllEquipments()
    {
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
            WriteLine("| {0,-5} | {1,-15} | {2,-27} | {3,-22} | {4,-58} | {5,7} | {6,-13} | {7,-15}",
                "Index", "EquipmentId", "Equipment Name", "Area", "Description", "Year", "Status", "Control Number", "Coordinator ID");

            foreach (var e in equipments)
            {
                WriteLine("| {0,-5} | {1,-15} | {2,-27} | {3,-22} | {4,-58} | {5,7} | {6,-13} | {7,-15}",
                    i, e.EquipmentId, e.Name, e.Area?.Name, e.Description, e.Year, e.Status?.Value, e.ControlNumber, e.Coordinator?.CoordinatorId);
                i++;
            }
        }
    }

}
