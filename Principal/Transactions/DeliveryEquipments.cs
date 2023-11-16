 using Microsoft.EntityFrameworkCore;
using AutoGens;
using ConsoleTables;
using System.Runtime.Intrinsics.Arm;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
partial class Program
{
    public static void DeliveryEquipmentsStudents()
    {
        using(bd_storage db = new()) // creating a new conection to the database
        {
            WriteLine("Search for a students request for today");
            WriteLine("Provide the student's register:");
            string register = VerifyReadLengthStringExact(8); // verify the register provided is of 8 characters
            //query that look in the database in the table students where the StudentId equals the register
            IQueryable<Student> students = db.Students.Where(s => s.StudentId.Equals(register));
            if(students is null || !students.Any()) // if the sequence doesn't contains any elements
            {
                WriteLine($"A student with the register {register} do not exist.");
                return;
            }
            else
            {
                WriteLine($"This are the due to deliver request(s) for today of the student: {students.First().StudentId} - {students.First().Name} {students.First().LastNameP} {students.First().LastNameM} ");
         
                var result = TodaysEquipmentRequestsByStudentId(register); // show all students requests for today that haven't been delivered to the student yet
                string requestid = "0";
                if(result.count >= 1)
                {
                    bool opp = false;
                    while(opp==false) // verify the storer provides a valid request id
                    {
                        WriteLine("Provide the RequestId of the request you will deliver:");
                        requestid = VerifyReadMaxLengthString(2); 
                        foreach(var rid in result.requestId)
                        {
                            if(requestid == rid.ToString())
                            {
                                opp = true;
                                break;
                            }
                        }    

                    }
                
                    WriteLine("This are the equipments you will have to deliver:");
                    IQueryable<RequestDetail> requestDetailss = db.RequestDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.RequestId.Equals(TryParseStringaEntero(requestid))).OrderBy(e=>e.Equipment.StatusId);
                    if(requestDetailss is not null || requestDetailss.Any())
                    {
                        foreach(var r in requestDetailss) // show every request 
                        {
                            WriteLine();
                            if(r.Equipment?.StatusId == 1)
                            {
                                WriteLine("{0,-15} | {1,-80} | {2,7} | {3} ","EquipmentId", "Equipment Name", "Year", "Description");
                                WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");        
                                WriteLine("{0,-15} | {1,-80} | {2,7} | {3}",
                                r.Equipment.EquipmentId, r.Equipment.Name, r.Equipment.Year, r.Equipment.Description);  
                            }
                            else
                            {
                                WriteLine($"I'm sorry to inform you that the equipment {r.EquipmentId} you were going to use is not available anymore");
                                WriteLine("Would you like to choose another one instead? (y/n)");
                                string option = VerifyReadLengthStringExact(1);
                                if(option=="y" || option == "Y")
                                {
                                    WriteLine(r.Equipment.AreaId);
                                    WriteLine("This are the equipments in the same area yours was:");
                                    IQueryable<Equipment> equipmentsAvailableNow = db.Equipments
                                    .Where(e=>e.AreaId.Equals(r.Equipment.AreaId))
                                    .Where(e=>e.StatusId == 1);

                                    if(equipmentsAvailableNow is null || !equipmentsAvailableNow.Any()){}
                                    else
                                    {

                                        WriteLine("{0,-15} | {1,-80} | {2,7} | {3} ",
                                        "EquipmentId", "Equipment Name", "Year", "Description");
                                        WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                                        List<string> eqAvailableId = new List<string>();
                                        foreach(var eq in equipmentsAvailableNow)
                                        {
                                            eqAvailableId.Add(eq.EquipmentId);
                                            WriteLine("{0,-15} | {1,-80} | {2,7} | {3}",
                                            eq.EquipmentId, eq.Name, eq.Year, eq.Description);                                      
                                        }   
                                        WriteLine("Write the equipment id you would like to use:");
                                        string equipmnew = VerifyReadMaxLengthString(15);
                                        if(eqAvailableId.Contains(equipmnew))
                                        {
                                            // modificar en request details donde estaba el primer id equip por el nuevo
                                            IQueryable<RequestDetail> requestDetailsnew = db.RequestDetails
                                            .Where(r=>r.RequestId.Equals(TryParseStringaEntero(requestid)))
                                            .Where(r=>r.EquipmentId.Equals(r.EquipmentId));
                                            requestDetailsnew.First().EquipmentId= equipmnew;
                                            int affected = db.SaveChanges();
                                            if(affected == 1){WriteLine($"Equipment changed to {equipmnew}");}
                                            else{WriteLine("Equipment not changed");}
                                            //modificar status equipo en equipment del nuevo
                                        }
                                    }    
                                }
                            }
                        }
                    }
                    
                }
                else if(result.count == 0)
                {
                    return;
                }

                WriteLine($"Is the deliver of the equipments of the request {requestid} made succesfully?");
                WriteLine("Remember, if you put 'n', that means that the process of delivery has been terminated, and you will have to start again");
                WriteLine("Write the selected option y/n");
                    string opi = "";
                bool op = false;
                while(op==false)
                {
                     opi = VerifyReadLengthStringExact(1);
                     if(opi =="y" || opi =="n"){op=true;}
                     else{WriteLine("That is not a valide option, try again.");}
                }
                if(opi=="y")
                {
                    // update request details format status to In Use
                    int Affected = UpdateRequestFormatStatus(requestid);
                    
                    if(Affected > 0)
                    {
                        WriteLine("Status of the requests have been changed!");
                    }
                    else{
                        WriteLine("The status of the request was not changed");
                    }
                    
                    // update equipments status that are in the request details delivered to In Use
                    // returns how many updates were made, and the list of equipments id that should have been updated
                    var EquipmentAffected = UpdateRequestEquipmentsStatus(requestid); 
                    if(EquipmentAffected.Affected == EquipmentAffected.ListEquipmentsId.Count())
                    {
                        WriteLine("Status of all equipments was changed.");
                    }
                    else if(EquipmentAffected.Affected == 0)
                    {
                        WriteLine("Sorry, the status of the equipments wasn't changed"); 
                    }
                    else 
                    {
                        WriteLine($"The status of {EquipmentAffected.Affected} was succesfully changed!");
                    }
                }
            }
        }
            WriteLine("Press the enter key to return to the menu:"); // return menu storer
            if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
            {
                Console.Clear();
                return;
            }
    }

    public static int UpdateRequestFormatStatus(string RequestId)
    {
        int affected = 0;
        using(bd_storage db = new())
        {
            byte status = 2;
            //update request details status where RequestId == requestid (variable)
            IQueryable<RequestDetail> requestDetails = db.RequestDetails.Where(r=> r.RequestId.Equals(TryParseStringaEntero(RequestId)));
            
            if(requestDetails is not null || requestDetails.Any())
            {
                affected = requestDetails.ExecuteUpdate(u => u.SetProperty(
                    p => p.StatusId, // Property Selctor
                    p => status // Value to edit
                ));
                db.SaveChanges();
            }
        }
        return affected;
    }

    public static(int Affected, List<string> ListEquipmentsId) UpdateRequestEquipmentsStatus( string RequestId)
    {
        int Affected = 0;
        List<string> EquipmentsIds = new List<string>();
        using(bd_storage db = new())
        {
            IQueryable<RequestDetail> reqs = db.RequestDetails.Where(r=>r.RequestId == TryParseStringaEntero(RequestId));
            if(reqs is  null || !reqs.Any() ){}
            else
            {
                foreach (var r in reqs)
                {
                    EquipmentsIds.Add(r.EquipmentId);
                }
                foreach(var eq in EquipmentsIds)
                {
                    IQueryable<Equipment>? equipments = db.Equipments?.Where(e => e.EquipmentId.Equals(eq));
                            
                    if (equipments is not null || equipments.Any())
                    {
                        equipments.First().StatusId = 2;
                        Affected += db.SaveChanges();
                    }
                } 
            }
        }
        return (Affected, EquipmentsIds);
    }
  
    public static void DeliveryEquipmentsProfessors()
    {
        using(bd_storage db = new()) // creating a new conection to the database
        {
            WriteLine("Search for a professor's request for today");
            WriteLine("Provide the professor's register:");
            string register = VerifyReadLengthStringExact(8); // verify the register provided is of 8 characters
            //query that look in the database in the table professors where the ProfessorId equals the register
            IQueryable<Professor> Professors = db.Professors.Where(s => s.ProfessorId.Equals(EncryptPass(register)));
            if(Professors is null || !Professors.Any()) // if the sequence doesn't contains any elements
            {
                WriteLine($"A student with the register {register} do not exist.");
                return;
            }
            else
            {
                WriteLine($"This are the due to deliver request(s) for today of the student:  {Professors.First().Name} {Professors.First().LastNameP} {Professors.First().LastNameM} ");
         
                var result = TodaysEquipmentRequestsByProfessorId(register); // show all professor's requests for today that haven't been delivered to the student yet
                string requestid = "0";
                if(result.count >= 1) //if there are equipments request for today from one professor in especific
                {
                    bool opp = false;
                    while(opp==false) // verify the storer provides a valid request id
                    {
                        WriteLine("Provide the RequestId of the request you will deliver:");
                        requestid = VerifyReadMaxLengthString(2); 
                        foreach(var rid in result.requestId)
                        {
                            if(requestid == rid.ToString())
                            {
                                opp = true;
                                break;
                            }
                        }    

                    }
                
                    WriteLine("This are the equipments you will have to deliver:");
                    IQueryable<PetitionDetail> requestDetailss = db.PetitionDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.PetitionId.Equals(TryParseStringaEntero(requestid))).OrderBy(e=>e.Equipment.StatusId);
                    if(requestDetailss is not null || requestDetailss.Any())
                    {
                        foreach(var r in requestDetailss) // show every request 
                        {
                            WriteLine();
                            if(r.Equipment?.StatusId == 1) // show just the equipment of the request that is available at the moment
                            {
                                WriteLine("{0,-15} | {1,-80} | {2,7} | {3} ","EquipmentId", "Equipment Name", "Year", "Description");
                                WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");        
                                WriteLine("{0,-15} | {1,-80} | {2,7} | {3}",
                                r.Equipment.EquipmentId, r.Equipment.Name, r.Equipment.Year, r.Equipment.Description);  
                            }
                            else // if the equipment doesn't have the status available
                            {
                                WriteLine($"I'm sorry to inform you that the equipment {r.EquipmentId} you were going to use is not available anymore");
                                WriteLine("Would you like to choose another one instead? (y/n)");
                                string option = VerifyReadLengthStringExact(1);
                                if(option=="y" || option == "Y") // if the user wants to replace the equipment for another
                                {
                                    // query to show the equipments that are available and have the same area as the past one
                                    WriteLine("This are the equipments in the same area yours was:");
                                    IQueryable<Equipment> equipmentsAvailableNow = db.Equipments
                                    .Where(e=>e.AreaId.Equals(r.Equipment.AreaId))
                                    .Where(e=>e.StatusId == 1); 

                                    if(equipmentsAvailableNow is null || !equipmentsAvailableNow.Any()){}
                                    else // if the query does have results
                                    {
                                        // show the equipments that area available and are from the same area
                                        WriteLine("{0,-15} | {1,-80} | {2,7} | {3} ",
                                        "EquipmentId", "Equipment Name", "Year", "Description");
                                        WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                                        List<string> eqAvailableId = new List<string>(); // declaration of list of all equipments id that can be the replacement
                                        foreach(var eq in equipmentsAvailableNow)
                                        {
                                            eqAvailableId.Add(eq.EquipmentId); // storage the equipments id that can be the replacement
                                            WriteLine("{0,-15} | {1,-80} | {2,7} | {3}", // shows the equipments
                                            eq.EquipmentId, eq.Name, eq.Year, eq.Description);                                      
                                        }   
                                        WriteLine("Write the equipment id you would like to use:");
                                        string equipmnew = VerifyReadMaxLengthString(15); // user chooses the equipment by EquipmentID
                                        if(eqAvailableId.Contains(equipmnew))
                                        {
                                            // modificar en request details donde estaba el primer id equip por el nuevo
                                            IQueryable<PetitionDetail> requestDetailsnew = db.PetitionDetails
                                            .Where(r=>r.PetitionId.Equals(TryParseStringaEntero(requestid)))
                                            .Where(r=>r.EquipmentId.Equals(r.EquipmentId)); // query finds the petition details where the past equipment was
                                            requestDetailsnew.First().EquipmentId= equipmnew; // update the petition, put the new equipment id instead of the old one
                                            int affected = db.SaveChanges(); // save changes in the bd
                                            if(affected == 1){WriteLine($"Equipment changed to {equipmnew}");}
                                            else{WriteLine("Equipment not changed");}
                                        }
                                    }    
                                }
                            }
                        }
                    }
                    
                }
                else if(result.count == 0) // if there wasn't any requests of the professor specified for today
                {
                    return;
                }

                WriteLine($"Is the deliver of the equipments of the request {requestid} made succesfully?");
                WriteLine("Remember, if you put 'n', that means that the process of delivery has been terminated, and you will have to start again");
                WriteLine("Write the selected option y/n");
                string opi = "";
                bool op = false;
                while(op==false)
                {
                     opi = VerifyReadLengthStringExact(1);
                     if(opi =="y" || opi =="n"){op=true;}
                     else{WriteLine("That is not a valide option, try again.");}
                }
                if(opi=="y")
                {
                    // update petition details format status to In Use (2)
                    int Affected = UpdatePetitionFormatStatus(requestid);
                    // verify the status of petition details was changed
                    if(Affected > 0)
                    {
                        WriteLine("Status of the requests have been changed!");
                    }
                    else{
                        WriteLine("The status of the request was not changed");
                    }
                    
                    // update equipments status that are in the request details delivered to In Use
                    // returns how many updates were made, and the list of equipments id that should have been updated
                    var EquipmentAffected = UpdatePetitionEquipmentsStatus(requestid); 
                    if(EquipmentAffected.Affected == EquipmentAffected.ListEquipmentsId.Count())
                    {
                        WriteLine("Status of all equipments was changed.");
                    }
                    else if(EquipmentAffected.Affected == 0)
                    {
                        WriteLine("Sorry, the status of the equipments wasn't changed"); 
                    }
                    else 
                    {
                        WriteLine($"The status of {EquipmentAffected.Affected} was succesfully changed!");
                    }
                }
            }
        }
            WriteLine("Press the enter key to return to the menu:"); // return menu storer
            if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
            {
                Console.Clear();
                return;
            }
    }

    public static int UpdatePetitionFormatStatus(string RequestId)
    {
        int affected = 0;
        using(bd_storage db = new()) // creating new connection to the bd
        {
            byte status = 2;
            //update request details status where RequestId == requestid (variable)
            IQueryable<PetitionDetail> requestDetails = db.PetitionDetails.Where(r=> r.PetitionId.Equals(TryParseStringaEntero(RequestId)));
            
            if(requestDetails is not null || requestDetails.Any())
            {
                affected = requestDetails.ExecuteUpdate(u => u.SetProperty(
                    p => p.StatusId, // Property Selctor
                    p => status // Value to edit
                ));
                db.SaveChanges(); // save changes
            }
        }
        return affected; // return how many registers were changed
    }

    public static(int Affected, List<string> ListEquipmentsId) UpdatePetitionEquipmentsStatus( string RequestId)
    {
        int Affected = 0;
        List<string> EquipmentsIds = new List<string>(); // equipment id List declaration
        using(bd_storage db = new()) // creating connection to the database
        { // query that finds the the register of the petition details is the one the user specified
            IQueryable<PetitionDetail> reqs = db.PetitionDetails.Where(r=>r.PetitionId == TryParseStringaEntero(RequestId));
            if(reqs is  null || !reqs.Any() ){}
            else // if there are elements found by the query
            {
                foreach (var r in reqs)
                {
                    EquipmentsIds.Add(r.EquipmentId); // add the equipment id to the list
                }
                foreach(var eq in EquipmentsIds) // for each one of the equipments id found in the petition
                {
                    IQueryable<Equipment>? equipments = db.Equipments?.Where(e => e.EquipmentId.Equals(eq)); // find the equipment id in the table Equipments
                            
                    if (equipments is not null || equipments.Any())
                    {
                        equipments.First().StatusId = 2; // change the status to In Use
                        Affected += db.SaveChanges(); // save changes on the bd, every time a equipment status is changed, so it keeps track of how many registers of the petition were changed at the end
                    }
                } 
            }
        }
        return (Affected, EquipmentsIds);
    }

    public static (int count, List<int?> requestId) TodaysEquipmentRequestsByStudentId(string Register) // returns the 
    {
        using(bd_storage db = new()) // creating a new connection to the database
        {
            DateTime today = DateTime.Now.Date;  // save today's date, it will be used later
            // query : select * from RequestDetails as rd JOIN Equipments as e ON rd.EquipmentId = e.EquipmentId
            // JOIN Status as s ON rd.StatusId = s.StatusId WHERE el ProfessorNip = 1 and StudentId of Request = register introduced
            IQueryable<RequestDetail> requestDetailsToday = db.RequestDetails
            .Include( e => e.Equipment).Include(e=> e.Status)
            .Where( r => r.ProfessorNip == 1)
            .Where(r => r.DispatchTime != DateTime.MinValue && r.RequestedDate.Date == today)
            .Where(r=>r.StatusId != 2)
            .Where(r=>r.Request.StudentId == Register).OrderBy(r=>r.RequestId);

            List<int?> requestsid = new List<int?>(); // declaring list of int to storage the request, is empty
            if ((requestDetailsToday is null) || !requestDetailsToday.Any())
            {
                WriteLine($"There are no requests for today of the student {Register}.");
                return (0,requestsid); // returning the count of 0, and an empty list
            }
            else
            {
                var table = new ConsoleTable("Request Details", ""); // creates a console table with the following collumns
                int? AnteriorRequestId = 0;
                foreach(var r in requestDetailsToday) // showing the info of every single request of today
                {
                    if(r.RequestId != AnteriorRequestId)
                    {
                        requestsid.Add(r.RequestId);
                        table.AddRow("RequestId", r.RequestId); //add a row to the table
                        table.AddRow("StudentId", Register);
                        table.AddRow("Dispatch Time", $"{r.DispatchTime.Hour}:{r.DispatchTime.Minute}");
                        table.AddRow("Return Time", $"{r.ReturnTime.Hour}:{r.ReturnTime.Minute}");
                        table.AddRow("Requested Date:", r.RequestedDate.ToString("dd/MM/yyyy"));

                        IQueryable<RequestDetail> requestDetailss = db.RequestDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.RequestId.Equals(r.RequestId));
                        var Equips = requestDetailss.Select(e=>e.EquipmentId);
                        if(requestDetailss is not null && requestDetailss.Any())
                        {
                            foreach(var rd in requestDetailss) // show every request 
                            {
                                table.AddRow("Equipment", $"{rd.Equipment?.Name}");
                            }
                        }
                        table.AddRow("", "");
                        AnteriorRequestId = r.RequestId; // checks that all the information of the requestId won't be repeated, only the equipments will be shown
                    }                                       
                }
                table.Write(); // show the table in the console
                return (requestDetailsToday.Count(), requestsid);
            }
        }
    }

    public static (int count, List<int> requestId) TodaysEquipmentRequestsByProfessorId(string Register)
    {
        using(bd_storage db = new()) // creating a new connection with the database
        {
            DateTime today = DateTime.Now.Date;  // today's date
            // save today's date, it will be used later
            // query : select * from RequestDetails as rd JOIN Equipments as e ON rd.EquipmentId = e.EquipmentId
            // JOIN Status as s ON rd.StatusId = s.StatusId WHERE el ProfessorId of Request = register introduced and status is not in use
            IQueryable<PetitionDetail> RequestDetailsToday = db.PetitionDetails
            .Include( e => e.Equipment).Include(e=> e.Status)
            .Where(r => r.DispatchTime != DateTime.MinValue && r.RequestedDate.Date == today)
            .Where(r=>r.Petition.ProfessorId == EncryptPass(Register)) // the ProfessorId in the database is encrypted
            .Where(r=>r.StatusId != 2)
            .OrderBy(r=>r.PetitionId);
           
            List<int> RequestsId = new List<int>(); // declaring list of int to storage the request, is empty
            if ((RequestDetailsToday is null) || !RequestDetailsToday.Any())
            {
                WriteLine($"There are no requests for today found.");
                return (0,RequestsId);
            }
            
            else
            {
                var table = new ConsoleTable("Request Details", ""); // creates a console table with the following collumns
                int? AnteriorRequestId = 0;
                foreach(var r in RequestDetailsToday) // showing the info of every single request of today
                {
                    if(r.PetitionId != AnteriorRequestId)
                    {
                        RequestsId.Add(r.PetitionId);
                        table.AddRow("PetitionId", r.PetitionId); //add a row to the table
                        table.AddRow("Professor", $"{r.Petition.Professor.Name} {r.Petition.Professor.LastNameP} {r.Petition.Professor.LastNameM}");
                        table.AddRow("Dispatch Time", $"{r.DispatchTime.Hour}:{r.DispatchTime.Minute}");
                        table.AddRow("Return Time", $"{r.ReturnTime.Hour}:{r.ReturnTime.Minute}");
                        table.AddRow("Requested Date:", r.RequestedDate.Date);

                        IQueryable<RequestDetail> RequestDetailss = db.RequestDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.RequestId.Equals(r.PetitionId));
                        var Equips = RequestDetailss.Select(e=>e.EquipmentId);
                        if(RequestDetailss is not null && RequestDetailss.Any())
                        {
                            foreach(var rd in RequestDetailss) // show every request 
                            {
                                table.AddRow("Equipment", $"{rd.Equipment?.Name}");
                            }
                        }
                        table.AddRow("", "");
                        AnteriorRequestId = r.PetitionId; // checks that all the information of the requestId won't be repeated, only the equipments will be shown
                    }                                       
                }
                table.Write(); // show the table in the console
                return (RequestDetailsToday.Count(), RequestsId);
            }
        }
    }

/*
    public static void DeliveryEquipmentsProfessors()
    {
        using(bd_storage db = new())
        {
            WriteLine("Search for a professor request for today");
            WriteLine("Provide the professor's id:");
            string profid = VerifyReadLengthStringExact(10); // verifying that the ProfessorId has 10 characters
            //query: select * from Professors where ProfessorId = Profid
            IQueryable<Professor> professors = db.Professors.Where(p => p.ProfessorId.Equals(profid)); // encrypt pass
            if(professors is null || !professors.Any()) // checking if the query result has not any elements
            {
                WriteLine($"A professor with the register {profid} do not exist.");
                return;
            }
            else // if the query resulted element(s)
            {
                WriteLine($"This are the due to delivery request(s) for today of the professor: {professors.First().ProfessorId} - {professors.First().Name} {professors.First().LastNameP} {professors.First().LastNameM} ");
                
                var result = TodaysEquipmentRequestsByProfessorId(profid);
                string requestid = "0";
                if(result.count == 0)
                {
                    return;
                }
                else if(result.count >= 1)
                {
                    bool opp = false;
                    while(opp==false)
                    {
                        WriteLine("Provide the RequestId of the request you will deliver:");
                        requestid = VerifyReadMaxLengthString(2); 
                        foreach(var rid in result.requestId)
                        {
                            if(requestid == rid.ToString())
                            {
                                opp = true;
                                break;
                            }
                        }    

                    }
                
                    WriteLine("Equipments:");
                    IQueryable<PetitionDetail> requestDetailss = db.PetitionDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.PetitionId.Equals(TryParseStringaEntero(requestid))).OrderBy(e=>e.Equipment.StatusId);
                    
                    foreach(var r in requestDetailss) // show every request 
                    {
                        var table = new ConsoleTable("EquipmentId", "Name", "Status", "Description");
                        if(r.StatusId == 1)
                        {
                            table.AddRow(r.EquipmentId, r.Equipment?.Name, r.Status?.Value, r.Equipment?.Description);
                        }
                        else
                        {
                            WriteLine($"I'm sorry to inform you that the equipment {r.EquipmentId} you were going to use is not available anymore");
                            WriteLine("Would you like to choose another one instead? (y/n)");
                            string option = VerifyReadLengthStringExact(1);
                            if(option=="y" || option == "Y")
                            {
                                WriteLine("This are the equipments in the same area yours was:");
                                IQueryable<Equipment> equipmentsAvailableNow = db.Equipments
                                .Include(e=>e.RequestDetails)
                                .Where(e=>e.StatusId.Equals(1))
                                .Where(e=>e.AreaId.Equals(r.Equipment.AreaId));

                                WriteLine("{0,-15} | {1,-80} | {2,7} | {3} |",
                                "EquipmentId", "Equipment Name", "Year", "Description");
                                WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
                                List<string> eqAvailableId = new List<string>();
                                foreach(var eq in equipmentsAvailableNow)
                                {
                                    eqAvailableId.Add(eq.EquipmentId);
                                    WriteLine("{0,-15} | {1,-80} | {2,7} | {3}",
                                    eq.EquipmentId, eq.Name, eq.Year, eq.Description);                                      
                                }   
                                WriteLine("Write the equipment id you would like to use:");
                                string equipmnew = VerifyReadMaxLengthString(15);
                                if(eqAvailableId.Contains(equipmnew))
                                {
                                    // modificar en request details donde estaba el primer id equip por el nuevo
                                    IQueryable<PetitionDetail> requestDetailsnew = db.PetitionDetails
                                    .Where(r=>r.PetitionId.Equals(TryParseStringaEntero(requestid)))
                                    .Where(r=>r.EquipmentId.Equals(r.EquipmentId));
                                    requestDetailsnew.First().EquipmentId= equipmnew;
                                    int affected = db.SaveChanges();
                                    //modificar status equipo en equipment del nuevo
                                    IQueryable<Equipment> equipmentnew = db.Equipments
                                    .Where(e=>e.EquipmentId.Equals(equipmnew));
                                    equipmentnew.First().StatusId = 2;
                                    affected= db.SaveChanges();
                                }
                            }    
                        }
                        
                        table.Write();
                    }

                    WriteLine($"Is the deliver of the equipments of the request {requestid} made succesfully?");
                    WriteLine("Remember, if you put 'n', that means that the process of delivery has been terminated, and you will have to start again");
                    WriteLine("Write the selected option y/n");
                        string opi = "";
                    bool op = false;
                    while(op==false)
                    {
                        opi = VerifyReadLengthStringExact(1);
                        if(opi =="y" || opi =="n"){op=true;}
                        else{WriteLine("That is not a valide option, try again.");}
                    }
                    if(opi=="y")
                    {
                        // update request details format status to In Use
                        int Affected = UpdatePetitionDetailsStatus(requestid);
                        
                        if(Affected > 0)
                        {
                            WriteLine("Status of the requests have been changed!");
                        }
                        else{
                            WriteLine("The status of the request was not changed");
                        }
                        
                        // update equipments status that are in the request details delivered to In Use
                        // returns how many updates were made, and the list of equipments id that should have been updated
                        var EquipmentAffected = UpdatePetitionEquipmentsStatus(requestid); 
                        if(EquipmentAffected.Affected == EquipmentAffected.ListEquipmentsId.Count())
                        {
                            WriteLine("Status of all equipments was changed.");
                        }
                        else if(EquipmentAffected.Affected == 0)
                        {
                            WriteLine("Sorry, the status of the equipments wasn't changed"); 
                        }
                        else 
                        {
                            WriteLine($"The status of {EquipmentAffected.Affected} was succesfully changed!");
                        }
                    }
                }
            }
        }
    }
  */

}