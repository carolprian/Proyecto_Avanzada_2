using AutoGens;
partial class Program{
    
    static (int affected, int productId) AddEquipment()
    {
        using(bd_storage db = new())
        {
            if(db.Equipments is null){ return(0,0);}
            Equipment e = new() 
            {
                // EquipmentId = equipmentid, 
                // Name = name,
                // AreaId = areaid, 
                // Description = description, 
                // Year = year, 
                // StatusId = statusid, 
                // ControlNumber = controlnumber
            };
        }
        return (0,0);
    }
    public static void UpdateEquipment()
    {

    }
    public static void DeleteEquipment()
    {
    
    }
    public static void ViewAllEquipments()
    {
        static void ListProducts(int[]? productsToHiglight = null)
        {
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
        }
    }
    
}