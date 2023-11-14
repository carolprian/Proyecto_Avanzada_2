using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

partial class Program{
    // Crear un reporte de un material dañado o perdido (esta funcion es para llenar el formulario)
    public static void DamagedLostReportInit()
    {
        // Inicialización de variables
        WriteLine("Create a Report of Damaged or Lost Equipment");
        byte status =0;
        string? student="", coordi="", equipment ="";
        DateTime eventdate = DateTime.Today;
        byte opi=0;

        using(bd_storage db = new())
        {
            // Validación del ID del equipo
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
        
            // Fue dañado o perdido?
            WriteLine();
            WriteLine("Was the equipment 'Lost' or 'Damaged'?");
            WriteLine("1. Lost");
            WriteLine("2. Damaged");

            status = Convert.ToByte(VerifyReadLengthStringExact(1));

            if (status == 1)
            { 
                status = 3; // Estado de 'Perdido' en la tabla de estados
            }
            else if (status == 2)
            { 
                status = 4; // Estado de 'Dañado' en la tabla de estados
            }

            // Descripción del evento
            WriteLine("How did it happened?");

            string? descrip = VerifyReadMaxLengthString(200);

            WriteLine();

            opi = 0;

            // Validación de la fecha del evento
            while(opi==0)
            {     
                // Cuando sucedio 
                WriteLine("When did it happened? (format: yyyy-MM-dd)");
                //verificacion de el formato de la fecha
                string date = VerifyReadLengthStringExact(10);

                if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out eventdate)) 
                { 
                    opi = 1; 
                }
                else
                {
                    WriteLine("That is not a correct date, try again."); 
                }
                WriteLine();
            }

            // Obtención del estudiante responsable y del coordinador
            WriteLine("Which student was responsible for the equipment in the time of the accident?");
            // Ver listas de estudiantes para poder buscar su registro sabiendo el nombre.
            string[]? students = ListStudents();
            WriteLine();

            // Ingresa el ID del estudiante
            WriteLine("Write the choosen option:");
            int studId = TryParseStringaEntero(VerifyReadLengthStringExact(1));

            if(students is not null)
            {
                student = students[studId -1];
            }
            WriteLine();

            // Escoger automaticamente al coordinador ya que solo pueda existir uno en la division
            IQueryable<Coordinator>? coordinators = db.Coordinators;

            if(coordinators is not null || coordinators.Any())
            {
                coordi = coordinators?.First().CoordinatorId;
            }

            // El alumno va a tener una "deuda" hasta que no sea traido el material que se le pidio para reponer el daño.
            WriteLine("What is the debt of the student? What will he/she have to bring to replace the damage?");
            WriteLine("Explain, with quantities, models and especifications if it is the case.");
            string returndescription = VerifyReadMaxLengthString(100);
            
            DateTime returnDate = DateTime.Now;
            opi = 0;
            while(opi==0)
            {     
                //Le asignaremos un dia maximo para poder traer y pagar su "deuda", normalmente se le asigna 1 semana
                WriteLine("When is the maximum date that the student has to return the equipment? (format: yyyy-MM-dd)");
                string date = VerifyReadLengthStringExact(10);
                if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out returnDate)) 
                { 
                    opi = 1; 
                }
                else
                { 
                    // Tienes que seguir el formato de ingreso de la fecha
                    WriteLine("That is not a correct date, try again."); 
                }
            }
            
            // Llama a la funcion para crear el reporte y se le asignan todos los valores llenados del formulario
            var resultCreate = CreateReportDamagedLost( status,  equipment,  descrip,  eventdate,  student,  coordi, returndescription, returnDate );
                
            if (resultCreate.Affected == 1)
            {
                WriteLine($"The Damaged Or Lost Report of the equipment, of Report ID {resultCreate.DyLequipmentId} was created succesfully");
                int affected = UpdateEquipmentStatus(status, equipment);
                if(affected == 1)
                {
                    WriteLine("Status of the equipment was changed to lost or damaged");
                }
                else
                { 
                    WriteLine("Status was not changed");
                }
        
            }
            else
            {
                WriteLine("The Report was not registered.");
            }
        
        }
                
    }

    // Aqui se guarda el formulario en la base de datos
    static (int Affected, int DyLequipmentId) CreateReportDamagedLost(byte StatusId, string EquipmentId, string Description, DateTime DateOfEvent, string StudentId, string? CoordinatorId, string ReturnDescrip, DateTime ReturnDate)
    {
        using(bd_storage db = new())
        {
            if(db.DyLequipments is null){ return(0,0);}
            DyLequipment dl = new() 
            {
                StatusId = StatusId,
                EquipmentId = EquipmentId, 
                Description = Description,
                DateOfEvent = DateOfEvent,
                StudentId = StudentId,
                CoordinatorId = CoordinatorId,
                DateOfReturn = ReturnDate,
                objectReturn = ReturnDescrip
            };            

            EntityEntry<DyLequipment> entity = db.DyLequipments.Add(dl);
            int affected = db.SaveChanges();   
            
            
            return (affected, dl.DyLequipmentId);
        }
    }

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