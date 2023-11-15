using AutoGens;
using Microsoft.EntityFrameworkCore;

partial class Program
{
    public static void StudentDebtLostDamagedEquipment()
    {
        using(bd_storage db = new())
        {

            if (studentsLostDamage() == true){
                return;
            } else {

                WriteLine("Provide the ID of the damage and lost report to discharge their debt:");
                string reportid = ReadNonEmptyLine();

                IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Where( dl => dl.DyLequipmentId.Equals(TryParseStringaEntero(reportid)))
                .Include( e => e.Equipment)
                .Include( s => s.Student);

                if (dyLequipments == null || !dyLequipments.Any())
                {
                    WriteLine("No reports found");
                    MenuStorer();
                }
                else
                {

                    foreach (var dyLequipment in dyLequipments)
                    {
                        WriteLine($"Report ID: {dyLequipment.DyLequipmentId}");
                        WriteLine($"Student:{dyLequipment.StudentId}, {dyLequipment.Student?.Name} {dyLequipment.Student?.LastNameP}");
                        WriteLine($"Name: {dyLequipment.Equipment?.Name}");
                        WriteLine($"Equipment id: {dyLequipment.EquipmentId}");
                        WriteLine($"Description of what happened to the Equipment: {dyLequipment.Equipment?.Description}");
                        WriteLine($"Description on what to return: {dyLequipment.Description}");
                        WriteLine($"Status: {dyLequipment.StatusId}");
                        WriteLine("-----------------------------------------------------------------");

                        WriteLine("Is the information correct? (y/n)(e to exit)");
                        string response = ReadNonEmptyLine().ToLower();

                        if (response == "y")
                        {
                            
                            dyLequipments.First().StatusId = 1;

                            IQueryable<Equipment> equipment = db.Equipments
                            .Where(e=>e.EquipmentId.Equals(dyLequipment.EquipmentId));
                            equipment.First().StatusId = 1;

                            int affected = db.SaveChanges();
    
                            if(affected==2)
                            {
                            WriteLine("Equipment status updated successfully.");
                            }
                        }
                        else if (response == "n")
                        {
                            WriteLine("The student has one more week to return the equipment.");

                            dyLequipment.DateOfReturn = DateTime.Now.AddDays(7);

                            db.Update(dyLequipment);
                            db.SaveChanges();
                        }
                        else if (response == "e")
                        {
                            return;
                        }
                        else
                        {
                            WriteLine("Invalid response. Please enter 'y', 'n' or 'e'.");
                        }
                    }    
                }
            }
        }    
    }
}