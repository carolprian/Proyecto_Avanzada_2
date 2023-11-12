using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

partial class Program{
    public static void DamagedLostReportInit()
    {
        WriteLine("Create a Report of Damaged or Lost Equipment");
        byte status =0;
        string? descrip ="", student="", coordi="", equipment ="";
        DateTime eventdate = DateTime.Today;

        byte opi=0;
        using(bd_storage db = new())
        {
                while(opi==0)
                {   
                    WriteLine("What was the damaged or lost equipment ID ?");
                    equipment = VerifyReadMaxLengthString(15);
                    IQueryable<Equipment> equipments = db.Equipments.Where(e=> e.EquipmentId == equipment);
                    
                    if(equipments is null || !equipments.Any())
                    { 
                        WriteLine("That equipment id doesn't exist, try again.");
                    }
                    else{ opi = 1;}
                }                
        
        WriteLine();
        WriteLine("Was the equipment 'Lost' or 'Damaged' ?");
        WriteLine("1. Lost");
        WriteLine("2. Damaged");
        status = Convert.ToByte(VerifyReadLengthStringExact(1));
        if(status == 1){ status = 3;}
        else if(status == 2){ status = 4;} // this is the status in the statusId table Statuses
        /*IQueryable<Status> statuses = db.Statuses.Where(s => s.StatusId == status);
        if(statuses is null || !statuses.Any())
        {
            
        }*/

        WriteLine("How did it happened?");
        descrip = VerifyReadMaxLengthString(200);
        WriteLine();

        opi = 0;
        while(opi==0)
        {     
            WriteLine("When did it happened? (format: yyyy-MM-dd)");
            string date = VerifyReadLengthStringExact(10);
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out eventdate)) { opi = 1; }
            else{ WriteLine("That is not a correct date, try again."); }
        }

        WriteLine();

        WriteLine("Which student was responsible for the equipment in the time of the accident?");
        string[]? students = ListStudents();
        WriteLine();
        WriteLine("Write the choosen option:");
        int studId = TryParseStringaEntero(VerifyReadLengthStringExact(1));
            if(students is not null)
            {
                student = students[studId -1];
            }
        WriteLine();
        //git cambié
        IQueryable<Coordinator>? coordinators = db.Coordinators;
        if(coordinators is not null || coordinators.Any())
        {
            coordi = coordinators?.First().CoordinatorId;
        }

        WriteLine("What is the debt of the student? What will he/she have to bring to replace the damage?");
        WriteLine("Explain, with quantities, models and especifications if it is the case.");
        string returndescription = VerifyReadMaxLengthString(100);
        
        DateTime returnDate = DateTime.Now;
        opi = 0;
        while(opi==0)
        {     
            WriteLine("When is the maximum date that the student has to return the equipment? (format: yyyy-MM-dd)");
            string date = VerifyReadLengthStringExact(10);
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out returnDate)) { opi = 1; }
            else{ WriteLine("That is not a correct date, try again."); }
        }
        

        var resultCreate = CreateReportDamagedLost( status,  equipment,  descrip,  eventdate,  student,  coordi, returndescription, returnDate );
            
            if(resultCreate.affected == 1)
            {
                WriteLine($"The Damaged Or Lost Report of the equipment of ID {resultCreate.DyLequipmentId} was created succesfully");
                int affected = UpdateEquipmentStatus(status, equipment);
                if(affected == 1)
                {
                    WriteLine("Status of the equipment was changed to lost or damaged");
                }
                else{ WriteLine("Status was not changed");}
    
            }
            else{
                WriteLine("The Report was not registered.");
            }
        
        }
                
    }

    static (int affected, int DyLequipmentId) CreateReportDamagedLost(byte statusid, string equipmentid, string description, DateTime dateofevent, string studentid, string coordinatorid, string returndescrip, DateTime returnDate)
    {
        using(bd_storage db = new())
        {
            if(db.DyLequipments is null){ return(0,0);}
            DyLequipment dl = new() 
            {
                StatusId = statusid,
                EquipmentId = equipmentid, 
                Description = description,
                DateOfEvent = dateofevent,
                StudentId = studentid,
                CoordinatorId = coordinatorid,
                DateOfReturn = returnDate,
                objectReturn = returndescrip
            };            

            EntityEntry<DyLequipment> entity = db.DyLequipments.Add(dl);
            int affected = db.SaveChanges();   
            
            
            return (affected, dl.DyLequipmentId);
        }
    }

    static int UpdateEquipmentStatus(byte newStatus, string equipmentId )
    {
        int affected = 0;
        using(bd_storage db = new())
        {
            IQueryable<Equipment> equipments = db.Equipments
            .Where(e=> e.EquipmentId == equipmentId);

            equipments.First().StatusId = newStatus;
            affected = db.SaveChanges();
        }
        return affected;
    }

    public static void StudentDebtLostDamagedEquipment()
    {
        using(bd_storage db = new())
        {
            studentsLostDamage();

            if (studentsLostDamage() == true){
                return;
            } else {

                WriteLine("Provide the ID of the student who want to discharge their debt:");
                string studentId = ReadNonEmptyLine();

                IQueryable<DyLequipment> dyLequipments = db.DyLequipments
                .Where( dl => dl.StudentId == studentId)
                .Include( e => e.Equipment)
                .Include( s => s.Student);

                if (dyLequipments == null || !dyLequipments.Any())
                {
                    WriteLine("No student found");
                    MenuStorer();
                }
                else
                {

                    foreach (var dyLequipment in dyLequipments)
                    {
                        WriteLine($"Student:{dyLequipment.StudentId}, {dyLequipment.Student?.Name} {dyLequipment.Student?.LastNameP}");
                        WriteLine($"Name: {dyLequipment.Equipment?.Name}");
                        WriteLine($"Description: {dyLequipment.Equipment?.Description}");
                        WriteLine($"Description: {dyLequipment.Description}");
                        WriteLine($"Status: {dyLequipment.StatusId}");
                        WriteLine("-----------------------------------------------------------------");

                        WriteLine("Is the information correct? (y/n)");
                        string response = ReadNonEmptyLine().ToLower();

                        if (response == "y")
                        {
                            dyLequipment.StatusId = 1;

                            Equipment equipment = dyLequipment.Equipment;
                            equipment.StatusId = 1;

                            db.Update(dyLequipment);
                            db.Update(equipment);
                            db.SaveChanges();

                            WriteLine("Equipment status updated successfully.");
                        }
                        else if (response == "n")
                        {
                            WriteLine("The student has one more week to return the equipment.");

                            dyLequipment.DateOfReturn = DateTime.Now.AddDays(7);

                            db.Update(dyLequipment);
                            db.SaveChanges();
                        }
                        else
                        {
                            WriteLine("Invalid response. Please enter 'y' or 'n'.");
                        }
                    }    
                }
            }
        }    
    }
}