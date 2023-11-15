using Microsoft.EntityFrameworkCore;
using AutoGens;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using ConsoleTables;
partial class Program{
   public static void ReturnEquipmentByStudent() // function to when the storer registers the return of a equipment that was loaned to a student
   {
        using(bd_storage db = new()) // connecting to the database
        {
            
            string requestid = "0";
            WriteLine("Return the equipments of a request.");
            WriteLine("Provide the student's register:");
            string register = VerifyReadLengthStringExact(8); // verify the register is of 8 digits
            //query that consults the table to student to se if the register provided exists
            IQueryable<Student> students = db.Students.Where(p => p.StudentId.Equals(register)); 
                if(students is null || !students.Any())
                {
                    WriteLine($"A student with the register {register} do not exist.");
                    return;
                }
                else // if the register exists
                {
                    WriteLine("This are the due to return request(s) by the student:");
                    var result = DueToReturnRequestsByStudent(register); // shows the requests that the student has to return
                    if(result.Count >= 1) // if there are requests
                    {
                        bool opp = false;
                        while(opp==false)
                        {
                            WriteLine("Provide the RequestId of the request you will deliver:");
                            requestid = VerifyReadMaxLengthString(4); 
                            foreach(var rid in result.RequestId)
                            {
                                if(requestid == rid.ToString()) // verify that the request ID provided exists
                                {
                                    opp = true;
                                    break;
                                }
                            }    
                        }
                        
                        WriteLine("Equipments:"); // query to get all the request details that have the request ID provided
                        IQueryable<RequestDetail> requestDetailss = db.RequestDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.RequestId.Equals(TryParseStringaEntero(requestid)));
                        if(requestDetailss is not null || requestDetailss.Any())
                        {
                            foreach(var r in requestDetailss) // list every single one of the equipments of the request 
                            {
                                WriteLine();
                                var table = new ConsoleTable("Equipments", ""); // a table to each one of the equipments that have to be returned
                                table.AddRow("Id", r.EquipmentId);
                                table.AddRow("Name", r.Equipment?.Name);
                                table.AddRow("Status", r.Equipment?.Status?.Value);
                                table.AddRow("Decription", r.Equipment?.Description);
                                table.Write();
                                WriteLine();
                            }
                            WriteLine();
                        }
                    } // fin del if  
                    else if(result.Count == 0) // if there aren't any requests to return
                    {
                        WriteLine("There are no due to return equipments.");
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
                        else{WriteLine("That is not a valide option, try again."); }
                        if(opi=="y")
                        {
                            // change status of the requests to delivered
                            if(ChangeStatusRequestToReturned(requestid) > 0)
                            {
                                WriteLine("Status of the requests have been changed!");
                            }
                            else{
                                WriteLine("The status of the request was not changed");
                            }

                            // change the status of all equipents of the request to Available
                            var Result = ChangeStatusEquipment(requestid); // returns the amount of Affected Equipments, and how many were supposed to be affected
                            if(Result.Affected == Result.CountEquipmentsIds)
                            {
                                WriteLine("Status of all equipments was changed.");
                            }
                            else if(Result.Affected == 0)
                            {
                                WriteLine("Sorry, the status of the equipments wasn't changed"); 
                            }
                            else 
                            {
                                WriteLine($"The status of {Result.Affected} equipments was succesfully changed!");
                            }
                            
                        }
                    }
                }
        }
   }

   public static void ReturnEquipmentByProfessor()
   {
        using(bd_storage db = new()) // creating connection to the database
        {
            WriteLine("Search for a professor request for today");
            WriteLine("Provide the professor's id:");
            string profid = VerifyReadLengthStringExact(10);
            IQueryable<Professor> professors = db.Professors.Where(p => p.ProfessorId.Equals(EncryptPass(profid))); // encrypt pass
            if(professors is null || !professors.Any())
            {
                WriteLine($"A professor with the id {profid} do not exist.");
                return;
            }
            else
            {// decrpt professors.First().ProfessorId
                WriteLine($"This are the due to return request(s) for today by the teacher:  {professors.First().Name} {professors.First().LastNameP} {professors.First().LastNameM} ");
                //IQueryable<RequestDetail> requestDetails = db.RequestDetails;
                var result = DueToReturnRequestsByProfessor(profid);
                string requestid = "0";
                if(result.Count >= 1)
                {
                    bool opp = false;
                    while(opp==false) 
                    {
                        WriteLine("Provide the RequestId of the request is being returned:");
                        requestid = VerifyReadMaxLengthString(2);
                        foreach(var rid in result.RequestId)
                        {
                            if(requestid == rid.ToString()) // verify that the request provided exists
                            {
                                opp = true;
                                break;
                            }
                        }    

                    }
                
                    WriteLine("Equipments:"); // query that finds the petition details that have the same PetitionId 
                    IQueryable<PetitionDetail> requestDetailss = db.PetitionDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.PetitionId.Equals(TryParseStringaEntero(requestid)));
                    if(requestDetailss is null || !requestDetailss.Any()){}
                    else
                    {
                        foreach(var r in requestDetailss) // list every single one of the equipments of the Petition ID
                            {
                                WriteLine();
                                var table = new ConsoleTable("Equipments", ""); // a table to each one of the equipments that have to be returned
                                table.AddRow("Id", r.EquipmentId);
                                table.AddRow("Name", r.Equipment?.Name);
                                table.AddRow("Status", r.Equipment?.Status?.Value);
                                table.AddRow("Decription", r.Equipment?.Description);
                                table.Write();
                                WriteLine();
                            }
                        WriteLine();
                    }
                    
                }
                else if(result.Count == 0) // if there are no pet
                {
                    return;
                }

                WriteLine($"Is the returning of the equipments of the petition {requestid} made succesfully?");
                WriteLine("Remember, if you put 'n', that means that the process of return equipment has been terminated, and you will have to start again");
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
                    // change status of the requests to delivered
                            if(ChangeStatusRequestToReturnedProf(requestid) > 0)
                            {
                                WriteLine("Status of the requests have been changed!");
                            }
                            else{
                                WriteLine("The status of the request was not changed");
                            }

                            // change the status of all equipents of the request to Available
                            var Result = ChangeStatusEquipmentProf(requestid); // returns the amount of Affected Equipments, and how many were supposed to be affected
                            if(Result.Affected == Result.CountEquipmentsIds)
                            {
                                WriteLine("Status of all equipments was changed.");
                            }
                            else if(Result.Affected == 0)
                            {
                                WriteLine("Sorry, the status of the equipments wasn't changed"); 
                            }
                            else 
                            {
                                WriteLine($"The status of {Result.Affected} equipments was succesfully changed!");
                            }
                }
            }
        }
   }

    public static (int Count, int[] RequestId) DueToReturnRequestsByStudent(string Register)
    {
        DateTime now = DateTime.Now.Date;
        using(bd_storage db = new())
        {            
            IQueryable<RequestDetail> requestDetailsToReturn = db.RequestDetails
            .Include( e => e.Equipment).Include(e=> e.Status)
            .Where( r => r.ProfessorNip == 1)
            .Where(r=>r.StatusId == 2)
            .Where(r=>r.Request.StudentId.Equals(Register));
           
            List<int> requestsid = new List<int>();
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((requestDetailsToReturn is null) || !requestDetailsToReturn.Any())
            {
                WriteLine($"There are no requests due to return by the student {Register}.");
                return (0,requestsid.ToArray());
            }
            else
            {
                foreach(var r in requestDetailsToReturn)
                {
                    requestsid.Add((int)r.RequestId);
                    
                    if(r.ReturnTime > now )
                    {               
                        ConsoleColor backGroundColor = ForegroundColor;
                        ForegroundColor = ConsoleColor.Red;  
                        WriteLine($"RequestId: {r.RequestId} ");
                        WriteLine($"StudentId: {Register} ");
                        WriteLine($"DispatchTime: {r.DispatchTime} ");
                        WriteLine($"ReturnTime: {r.ReturnTime}");
                        ForegroundColor = backGroundColor;
                    }
                    else
                    {
                        WriteLine($"RequestId: {r.RequestId} ");
                        WriteLine($"StudentId: {Register} "); 
                        WriteLine($"DispatchTime: {r.DispatchTime} ");
                        WriteLine($"ReturnTime: {r.ReturnTime}");
                    }   
                    WriteLine();    
                }
                return (requestDetailsToReturn.Count(), requestsid.ToArray());
            }
        }
    }

    public static (int Count, int[] RequestId) DueToReturnRequestsByProfessor(string Register)
    {
        DateTime now = DateTime.Now.Date;
        using(bd_storage db = new())
        {            
            IQueryable<PetitionDetail>? requestDetailsToReturn = db.PetitionDetails?
            .Include( e => e.Equipment).Include(e=> e.Status).Include(e=>e.Petition.Professor)
            .Where(r=>r.StatusId == 2)
            .Where(r=>r.Petition.ProfessorId.Equals(EncryptPass(Register)));
           
            List<int> requestsid = new List<int>();
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((requestDetailsToReturn is null) || !requestDetailsToReturn.Any())
            {
                WriteLine($"There are no requests due to return by the professor.");
                return (0,requestsid.ToArray());
            }
            else
            {
                foreach(var r in requestDetailsToReturn)
                {
                    requestsid.Add((int)r?.PetitionId);
                    if(r.ReturnTime > now )
                    {               
                        ConsoleColor backGroundColor = ForegroundColor;
                        ForegroundColor = ConsoleColor.Red;  
                        WriteLine($"RequestId: {r.PetitionId} ");
                        WriteLine($"ProfessorId: {r.Petition.Professor.Name} {r.Petition.Professor.LastNameP} {r.Petition.Professor.LastNameM} ");
                        WriteLine($"RequestedDate: {r.RequestedDate.Date}"); 
                        WriteLine($"DispatchTime: {r.DispatchTime} ");
                        WriteLine($"ReturnTime: {r.ReturnTime}");
                        ForegroundColor = backGroundColor;
                    }
                    else
                    {
                    //WriteLine($"StudentId: {register} RequestId: {r.RequestId}, Quantity: {r.Quantity}, StatusId: {r.Status?.Value}, ProfessorNip: {r.ProfessorNip}, DispatchTime: {r.DispatchTime.Hour}, ReturnTime: {r.ReturnTime.Hour}, RequestedDate: {r.RequestedDate}, EquipmentNames: {r.Equipment?.Name}");
                        WriteLine($"RequestId: {r.PetitionId} ");
                        WriteLine($"ProfessorId: {r.Petition.Professor.Name} {r.Petition.Professor.LastNameP} {r.Petition.Professor.LastNameM} ");
                        WriteLine($"RequestedDate: {r.RequestedDate.Date}"); 
                        WriteLine($"DispatchTime: {r.DispatchTime} ");
                        WriteLine($"ReturnTime: {r.ReturnTime}");
                    }      
                    WriteLine();
                }
                return (requestDetailsToReturn.Count(), requestsid.ToArray());
            }
        }
    }

    public static int ChangeStatusRequestToReturned(string RequestId)
    {
        using(bd_storage db = new())  // creating connection to the database
        {
            byte status = 6; // status of Delivered
                    //update request details status where RequestId == requestid (variable)
                    IQueryable<RequestDetail> requestDetails = db.RequestDetails.Where(r=> r.RequestId.Equals(TryParseStringaEntero(RequestId)));
                    int Affected = 0;
                    if(requestDetails is null || !requestDetails.Any()){}
                    else
                    {
                        Affected = requestDetails.ExecuteUpdate(u => u.SetProperty(
                            p => p.StatusId, // Property Selctor
                            p => status // Value to edit
                        ));
                        db.SaveChanges(); // save changes made to the database
                    }
                    return Affected;
        }
    }
    
    public static int ChangeStatusRequestToReturnedProf(string RequestId)
    {
        using(bd_storage db = new()) // create connection to the database
        {
            byte status = 6;
                    //update request details status where RequestId == requestid (variable)
                    IQueryable<PetitionDetail> requestDetails = db.PetitionDetails.Where(r=> r.PetitionId.Equals(TryParseStringaEntero(RequestId)));
                    int Affected = 0;
                    if(requestDetails is null || !requestDetails.Any()){}
                    else // if the query returns elements found
                    {
                        Affected = requestDetails.ExecuteUpdate(u => u.SetProperty(
                            p => p.StatusId, // Property Selctor
                            p => status // Value to edit
                        ));
                        db.SaveChanges(); //save changes
                    }
                    
            return Affected;
        }
    }

    public static (int Affected, int CountEquipmentsIds) ChangeStatusEquipment(string RequestId)
    {
        int Affected = 0; 
        using(bd_storage db = new()) // creating connection to the database
        {
            List<string> EquipmentsIds = new List<string>(); // declaring list of all equipments IDs of the petition
            //query that finds the petition details that are from one same petition
            IQueryable<RequestDetail> RequestDetails = db.RequestDetails.Where(r=>r.RequestId == TryParseStringaEntero(RequestId));
            if(RequestDetails is null || !RequestDetails.Any() ){}
            else // if the query result found elements
            {
                foreach (var r in RequestDetails)
                {
                    EquipmentsIds.Add(r.EquipmentId);  // add to the list the equipments IDs
                }
                foreach(var eq in EquipmentsIds)
                {   // query that finds the equipments where the equipment id is equivalent to each one of the equipments in the petition
                    IQueryable<Equipment>? Equipments = db.Equipments?.Where(e => e.EquipmentId.Equals(eq));
                        
                    if (Equipments is null || !Equipments.Any()){} // if there aren't any equipments
                    else
                    {
                        Equipments.First().StatusId = 1; // change the status to available of each equipment
                        Affected += db.SaveChanges(); // keep track of all the changed registers in the table equipments
                    }
                } 
            }
            return (Affected, EquipmentsIds.Count()); // return how many equipments status were modified, and how many were supposed to be modified
        }
    }

    
    public static (int Affected, int CountEquipmentsIds) ChangeStatusEquipmentProf(string RequestId)
    {
        int Affected = 0; 
        using(bd_storage db = new()) // creating connection to the database
        {
            List<string> EquipmentsIds = new List<string>(); // declaring list of all equipments IDs of the petition
            //query that finds the petition details that are from one same petition
                IQueryable<PetitionDetail> reqs = db.PetitionDetails.Where(r=>r.PetitionId == TryParseStringaEntero(RequestId));
                if(reqs is null || reqs.Any()){}
                else // if the query result found elements
                {
                    foreach (var r in reqs)
                    {
                        EquipmentsIds.Add(r.EquipmentId); // add to the list the equipments IDs
                    }
                    foreach(var eq in EquipmentsIds)
                    {   // query that finds the equipments where the equipment id is equivalent to each one of the equipments in the petition
                        IQueryable<Equipment>? Equipments = db.Equipments?.Where(e => e.EquipmentId.Equals(eq));
                        
                        if (Equipments is null || !Equipments.Any()){} // if there aren't any equipments
                        else
                        {
                            Equipments.First().StatusId = 1; // change the status to available of each equipment
                            Affected += db.SaveChanges(); // keep track of all the changed registers in the table equipments
                        }
                    }
                }
                    
            return (Affected, EquipmentsIds.Count()); // return how many equipments status were modified, and how many were supposed to be modified
        }
    }
}