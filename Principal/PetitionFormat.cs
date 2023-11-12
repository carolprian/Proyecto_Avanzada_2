using AutoGens;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.Arm;

partial class Program
{
    public static void PetitionFormat(string username)
    {
        string plantel = AddPlantel();
        DateTime currentDate = DateTime.Now;
        string professorId = AddProfessor(username);
        int classroomId = AddClassroom();
        string subjectId = SearchSubjectsByName("a", 1);
        string? storerId = AddStorer();
        if(storerId is null)
        {
            WriteLine("There's not a storer to do your request. Please contact the coordinator or the storer");
            WriteLine("Going back to the menu...");
            return;
        }
        DateTime requestDate = AddDate(currentDate);
        var times = AddTimes(requestDate);
        List<string> equipmentsId = new List<string>();
        List<byte?> statusEquipments = new List<byte?>();
        var petition = AddPetition(classroomId, professorId, storerId, subjectId);
        var equipments = SearchEquipmentsRecursive(equipmentsId, statusEquipments, requestDate, times.Item1, times.Item2, request.requestId);
        if(equipments.i == 1){
            return;
        } else {
            if(petition.affected > 0){
                var petitionDetailsId = AddPetitionDetails(petition.petitionId, equipments.equipmentsId, times.Item1, times.Item2, requestDate, currentDate, equipments.statusEquipments);
                if(petitionDetailsId.affected.Count() >= 1){
                    WriteLine("Petition added");
                } else
                {
                    WriteLine("The petition was not added. Try again");
                }
            }
            else {
                WriteLine("The petition couldnt be added. Try again.");
            }
        }
    }
    public static (List<int> affected, List<int> requestDetailsId) AddPetitionDetails(int requestId, List<string> equipmentsId, int professorNip, DateTime initTime, DateTime endTime, DateTime requestedDate, DateTime currentDate, List<byte?> statusEquipments){
        int i=0;
        List<int>? requestDetailsId = new List<int>();
        List<int>? affecteds = new List<int>();
        if (equipmentsId == null || statusEquipments == null || equipmentsId.Count != statusEquipments.Count)
        {
            // Manejar el caso donde las listas no son válidas
            WriteLine("entro el if donde valida que las listas son nulas");
            return (affecteds, requestDetailsId);
        }
        using (bd_storage db = new()){
            if(db.RequestDetails is null){ 
                WriteLine("Table not created");
                return(affecteds, requestDetailsId);}
            foreach(var e in equipmentsId){
                RequestDetail rD = new() {
                    RequestId = requestId,
                    EquipmentId = equipmentsId[i],
                    StatusId = statusEquipments[i],
                    DispatchTime = initTime,
                    ReturnTime = endTime,
                    RequestedDate = requestedDate,
                    CurrentDate = currentDate
                };
                EntityEntry<RequestDetail> entity = db.RequestDetails.Add(rD);
                affecteds.Add(db.SaveChanges());
                requestDetailsId.Add(rD.RequestDetailsId);
                i++;
            }
        }
        return (affecteds, requestDetailsId);
    }


    public static (string, int) AddProfessor(string username){
        int groupId=0;
        using(bd_storage db = new()){
            IQueryable<Professor> professors = db.Professors.Where(s => s.ProfessorId.Equals(EncryptPass(username)));
            var professorss = professors.FirstOrDefault();
            if(professors is not null && professors.Any()){
                username = professors.First().ProfessorId;
            } 
        }
        return (username, groupId);
    }

    public static (int affected, int requestId) AddPetition(int classroomId, string professorId, string studentId, string storerId, string subjectId){
        using(bd_storage db = new()){
            if(db.Petitions is null){ return(0, -1);}
            Petition p  = new Petition()
            {
                ClassroomId = classroomId,
                ProfessorId = professorId,
                StorerId = storerId,
                SubjectId = subjectId
            };

            EntityEntry<Request> entity = db.Petition.Add(p);
            int affected = db.SaveChanges();
            return (affected, p.PetitionId);
        }
    }

    public static void DeletePetition(int requestId)
    {
        using(bd_storage db = new()){
            var request = db.Requests
                    .Where(r => r.RequestId == requestId)
                    .FirstOrDefault();
             WriteLine();
             db.Requests.Remove(request);
                    int affected = db.SaveChanges();
        }
    }

    public static void DeletePetitionFormat (string username)
    {
        WriteLine("Here's a list of all the request format that has not been accepted yet. ");
        ViewRequestFormatNotAcceptedYet(username);
        do
        {
            WriteLine();
            WriteLine("Provide the ID of the request that you want to delete (check the list): ");
            int detailsId = Convert.ToInt32(ReadLine());

            using(bd_storage db = new())
            {
                // checks if it exists
                IQueryable<RequestDetail> requestDetails = db.RequestDetails
                .Where(e => e.RequestDetailsId == detailsId);

                // Obtén el RequestId asociado
                int requestId = db.RequestDetails
                    .Where(e => e.RequestDetailsId == detailsId)
                    .Select(r => r.Request.RequestId)
                    .FirstOrDefault();

                var request = db.Requests
                    .Where(r => r.RequestId == requestId)
                    .FirstOrDefault();
                    
                                        
                if(requestDetails is null || !requestDetails.Any())
                {
                    WriteLine("That request ID doesn't exist in the database, try again");
                }
                else
                {
                    db.RequestDetails.Remove(requestDetails.First());
                    int affected = db.SaveChanges();
                    if(affected > 0)
                    {
                        WriteLine("Equipment successfully deleted");
                    }
                    else
                    {
                        WriteLine("Equipment couldn't be deleted");
                    }

                    // Obtén el RequestId asociado
                    int requestsId = db.RequestDetails
                    .Where(e => e.RequestDetailsId == requestId)
                    .Select(r => r.Request.RequestId)
                    .FirstOrDefault();

                    // Elimina el registro de Requests
                    var requests = db.Requests
                    .Where(r => r.RequestId == requestId)
                    .FirstOrDefault();
                    db.Requests.Remove(requests);
                    affected = db.SaveChanges();
                    if(affected == 1)
                    {
                        WriteLine("Equipment successfully deleted");
                    }
                    else
                    {
                        WriteLine("Equipment couldn't be deleted");
                    }

                }               
            }
            return;
        } while (true);            
    }

    public static void UpdatePetitionFormat(string username){
        int i=1;
        DateTime request=DateTime.Today;
        WriteLine("Here's a list of all the request format that has not been accepted yet. ");
        ViewRequestFormatNotAcceptedYet(username);
        WriteLine();
        WriteLine("Provide the ID of the request that you want to modify (check the list): ");
        int requestID = Convert.ToInt32(ReadNonEmptyLine());
        using(bd_storage db = new bd_storage()){
            IQueryable<Request> requestss = db.Requests
            .Include(rd => rd.Classroom)
            .Include(rd => rd.Professor)
            .Include(rd => rd.Subject)
            .Include(rd => rd.Student);
            IQueryable<RequestDetail> requestDetailss = db.RequestDetails
            .Include(rd => rd.Status)
            .Include(rd=> rd.Equipment)
            .Where(r => r.RequestId==requestID).Where(r=> r.ProfessorNip==0);
            WriteLine("These are the fields you can update:");
            WriteLine($"{i}. Classroom: {requestss.First().Classroom.Name}");
            WriteLine($"{i+1}. Professor: {requestss.First().Professor.Name} {requestss.First().Professor.LastNameP}");
            WriteLine($"{i+2}. Subject: {requestss.First().Subject.Name}");
            WriteLine($"{i+3}. Date of the request: {requestDetailss.First().RequestedDate}");
            WriteLine($"{i+4}. Dispatch time: {requestDetailss.First().DispatchTime} and Return time: {requestDetailss.First().ReturnTime}");
            WriteLine($"{i+5}. Equipment(s) in the request:");
            foreach (var requestDetail in requestDetailss)
            {
                WriteLine($"     -{requestDetail.Equipment.EquipmentId} ({requestDetail.Equipment.Name})");
                i++;
            }
            int op = Convert.ToInt32(ReadNonEmptyLine());
            switch(op)
            {
                case 1:
                {
                    int classroomId = AddClassroom();
                }break;
                case 2:
                {
                    string professorId = SearchProfessorByName("xyz", 0, 0);
                }break;
                case 3:
                {
                    string subjectId = SearchSubjectsByName("xyz", 1);
                }break;
                case 4:
                {
                    request = AddDate(DateTime.Now.Date);
                }break;
                case 5:
                {
                    var times = AddTimes(request);
                }break;
                case 6:
                {

                }break;
                case 7:
                {
                    return;
                }
                default:{
                    WriteLine("Not a valide option. Try again.");
                }break;
            }
        }
    }
}
