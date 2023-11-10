using Microsoft.EntityFrameworkCore;
using AutoGens;
partial class Program{
   public static void ReturnEquipmentByStudent()
   {
        using(bd_storage db = new())
        {
            WriteLine("Return the equipments of a request.");
            WriteLine("Provide the student's register:");
                string register = VerifyReadLengthStringExact(8);
                IQueryable<Student> students = db.Students.Where(s => s.StudentId.Equals(register));
                if(students is null || !students.Any())
                {
                    WriteLine($"A student with the register {register} do not exist.");
                    return;
                }
                else
                {
                    WriteLine("This are the due to return request(s) by the student:");
                    var result = DueToReturnRequestsByStudent(register);
                    string requestid = "0";
                    if(result.count >= 1)
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
                        IQueryable<RequestDetail> requestDetailss = db.RequestDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.RequestId.Equals(TryParseStringaEntero(requestid)));
                        if(requestDetailss is not null || requestDetailss.Any())
                        {
                            foreach(var r in requestDetailss)
                            {
                                WriteLine($"Id: {r.EquipmentId} . Name: {r.Equipment?.Name} . Status: {r.Status?.Value}");
                            }
                        }
                    } // fin del if  
                    else if(result.count == 0)
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
                            ChangeStatusRequestToReturned(requestid);
                            ChangeStatusEquipment(requestid);
                        }
                    }
                }
        }
   }

   public static void ReturnEquipmentByProfessor()
   {
        using(bd_storage db = new())
        {
            WriteLine("Search for a professor request for today");
            WriteLine("Provide the professor's id:");
            string profid = VerifyReadLengthStringExact(10);
            IQueryable<Professor> professors = db.Professors.Where(p => p.ProfessorId.Equals(profid)); // encrypt pass
            if(professors is null || !professors.Any())
            {
                WriteLine($"A professor with the id {profid} do not exist.");
                return;
            }
            else
            {// decrpt professors.First().ProfessorId
                WriteLine($"This are the due to return request(s) for today by the teacher: {professors.First().ProfessorId} - {professors.First().Name} {professors.First().LastNameP} {professors.First().LastNameM} ");
                //IQueryable<RequestDetail> requestDetails = db.RequestDetails;
                var result = DueToReturnRequestsByStudent(profid);
                string requestid = "0";
                if(result.count >= 1)
                {
                    bool opp = false;
                    while(opp==false)
                    {
                        WriteLine("Provide the RequestId of the request is being returned:");
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
                    IQueryable<RequestDetail> requestDetailss = db.RequestDetails.Include(e=>e.Status).Include(e=>e.Equipment).Where(e=>e.RequestId.Equals(TryParseStringaEntero(requestid)));
                    if(requestDetailss is not null || requestDetailss.Any())
                    {
                        foreach(var r in requestDetailss)
                        {
                            WriteLine($"Id: {r.EquipmentId} . Name: {r.Equipment?.Name} . Status: {r.Status?.Value}");
                        }
                    }
                    
                }
                else if(result.count == 0)
                {
                    return;
                }

                WriteLine($"Is the returning of the equipments of the request {requestid} made succesfully?");
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
                            ChangeStatusRequestToReturned(requestid);
                            ChangeStatusEquipment(requestid);
                }
            }
        }
   }

    public static (int count, int[] requestId) DueToReturnRequestsByStudent(string register)
    {
        DateTime now = DateTime.Now.Date;
        using(bd_storage db = new())
        {            
            IQueryable<RequestDetail> requestDetailsToReturn = db.RequestDetails
            .Include( e => e.Equipment).Include(e=> e.Status)
            .Where( r => r.ProfessorNip == "1")
            .Where(r => r.DispatchTime != null)
            .Where(r=>r.StatusId == 2)
            .Where(r=>r.Request.StudentId.Equals(register));
           
            WriteLine($"Query: {requestDetailsToReturn.ToQueryString()}");
            List<int> requestsid = new List<int>();
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((requestDetailsToReturn is null) || !requestDetailsToReturn.Any())
            {
                WriteLine($"There are no requests due to return by the student {register}.");
                return (0,requestsid.ToArray());
            }
            else
            {
                int i = 0;
                foreach(var r in requestDetailsToReturn)
                {
                    requestsid.Add((int)r.RequestId);
                    if(r.ReturnTime > now)
                    {               
                        ConsoleColor backGroundColor = ForegroundColor;
                        ForegroundColor = ConsoleColor.Red;  
                        WriteLine($"RequestId: {r.RequestId} . StudentId: {register} , ProfessorNip: {r.ProfessorNip}, DispatchTime: {r.DispatchTime}, ReturnTime: {r.ReturnTime}, RequestedDate: {r.RequestedDate.Date}"); 
                        ForegroundColor = backGroundColor;
                    }
                    else
                    {
                    //WriteLine($"StudentId: {register} RequestId: {r.RequestId}, Quantity: {r.Quantity}, StatusId: {r.Status?.Value}, ProfessorNip: {r.ProfessorNip}, DispatchTime: {r.DispatchTime.Hour}, ReturnTime: {r.ReturnTime.Hour}, RequestedDate: {r.RequestedDate}, EquipmentNames: {r.Equipment?.Name}");
                        WriteLine($"RequestId: {r.RequestId} . StudentId: {register} , ProfessorNip: {r.ProfessorNip}, DispatchTime: {r.DispatchTime}, ReturnTime: {r.ReturnTime}, RequestedDate: {r.RequestedDate.Date}");
                    }       
                }
                return (requestDetailsToReturn.Count(), requestsid.ToArray());
            }
        }
    }

    public static (int count, int[] requestId) DueToReturnRequestsByProfessor(string register)
    {
        DateTime now = DateTime.Now.Date;
        using(bd_storage db = new())
        {            
            IQueryable<RequestDetail>? requestDetailsToReturn = db.RequestDetails?
            .Include( e => e.Equipment).Include(e=> e.Status)
            .Where( r => r.ProfessorNip == "1")
            .Where(r => r.DispatchTime != null)
            .Where(r=>r.StatusId == 2)
            .Where(r=>r.Request.StudentId.Equals("11111111"));
           
            WriteLine($"Query: {requestDetailsToReturn?.ToQueryString()}");
            List<int> requestsid = new List<int>();
            db.ChangeTracker.LazyLoadingEnabled = false;
            if ((requestDetailsToReturn is null) || !requestDetailsToReturn.Any())
            {
                WriteLine($"There are no requests due to return by the student {register}.");
                return (0,requestsid.ToArray());
            }
            else
            {
                int i = 0;
                foreach(var r in requestDetailsToReturn)
                {
                    requestsid.Add((int)r?.RequestId);
                    if(r.ReturnTime > now)
                    {               
                        ConsoleColor backGroundColor = ForegroundColor;
                        ForegroundColor = ConsoleColor.Red;  
                        WriteLine($"RequestId: {r.RequestId} . StudentId: {register} , DispatchTime: {r.DispatchTime}, ReturnTime: {r.ReturnTime}, RequestedDate: {r.RequestedDate.Date}"); 
                        ForegroundColor = backGroundColor;
                    }
                    else
                    {
                    //WriteLine($"StudentId: {register} RequestId: {r.RequestId}, Quantity: {r.Quantity}, StatusId: {r.Status?.Value}, ProfessorNip: {r.ProfessorNip}, DispatchTime: {r.DispatchTime.Hour}, ReturnTime: {r.ReturnTime.Hour}, RequestedDate: {r.RequestedDate}, EquipmentNames: {r.Equipment?.Name}");
                        WriteLine($"RequestId: {r.RequestId} . ProfessorId: {register} , DispatchTime: {r.DispatchTime}, ReturnTime: {r.ReturnTime}, RequestedDate: {r.RequestedDate.Date}");
                    }       
                }
                return (requestDetailsToReturn.Count(), requestsid.ToArray());
            }
        }
    }

    public static void ChangeStatusRequestToReturned(string requestid)
    {
        using(bd_storage db = new()) 
        {
            byte status = 6;
                    //update request details status where RequestId == requestid (variable)
                    IQueryable<RequestDetail> requestDetails = db.RequestDetails.Where(r=> r.RequestId.Equals(TryParseStringaEntero(requestid)));
                    int affected = 0;
                    if(requestDetails is not null || requestDetails.Any())
                    {
                        affected = requestDetails.ExecuteUpdate(u => u.SetProperty(
                            p => p.StatusId, // Property Selctor
                            p => status // Value to edit
                        ));
                        db.SaveChanges();
                    }
                    if(affected > 0)
                    {
                        WriteLine("Status of the requests have been changed!");
                    }
                    else{
                        WriteLine("The status of the request was not changed");
                    }
        }
    }

    public static void ChangeStatusEquipment(string requestid)
    {
        using(bd_storage db = new())
        {
            List<string> equipmentsid = new List<string>();
                    IQueryable<RequestDetail> reqs = db.RequestDetails.Where(r=>r.RequestId == TryParseStringaEntero(requestid));
                    if(reqs is not null || reqs.Any() )
                    {
                        foreach (var r in reqs)
                        {
                            equipmentsid.Add(r.EquipmentId);
                        }
                        foreach(var eq in equipmentsid)
                        {
                            IQueryable<Equipment>? equipments = db.Equipments?.Where(e => e.EquipmentId.Equals(eq));
                            
                            if (equipments is not null || equipments.Any())
                            {
                                equipments.First().StatusId = 1;
                                db.SaveChanges();
                            }
                        } 
                        if(reqs.Count() == equipmentsid.Count())
                        {
                            WriteLine("Status of all equipments was changed.");
                        }
                        else if(0 < equipmentsid.Count() && equipmentsid.Count() < reqs.Count())
                        {
                            WriteLine($"The status of {equipmentsid.Count()} was succesfully changed!");
                        }
                        else 
                        {
                            WriteLine("Sorry, the status of the equipments wasn't changed"); 
                        }
                    }
        }
    }
}