using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

partial class Program{
    public static void DamagedLostReportInit()
    {
            WriteLine("Create a Report of Damaged or Lost Equipment");
                byte status =0;
                string descrip ="", student="", coordi="", equipment ="";
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

                WriteLine("Choose the Coordinator in charge:");
                    string[]? coordinators = ListCoordinators();
                    WriteLine();
                    WriteLine("Write the choosen option:");
                    int coordid = TryParseStringaEntero(VerifyReadLengthStringExact(1));
                        if(coordinators is not null)
                        {
                            coordi = coordinators[coordid -1];
                        }

                var resultCreate = CreateReportDamagedLost( status,  equipment,  descrip,  eventdate,  student,  coordi );
                   
                   if(resultCreate.affected == 1)
                    {
                        WriteLine($"The Damaged Or Lost Report of the equipment of ID {resultCreate.DyLequipmentId} was created succesfully");
                    }
                    else{
                        WriteLine("The Report was not registered.");
                    }
                
                }
                
    }

    static (int affected, int DyLequipmentId) CreateReportDamagedLost(byte statusid, string equipmentid, string description, DateTime dateofevent, string studentid, string coordinatorid )
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
                CoordinatorId = coordinatorid
            };            

            EntityEntry<DyLequipment> entity = db.DyLequipments.Add(dl);
            int affected = db.SaveChanges();    
            return (affected, dl.DyLequipmentId);
        }
    }
}