using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
partial class Program
{
    public static void PetitionFormat(string username)
    {
        string plantel = WritePlantel();
        DateTime currentDate = DateTime.Now;
        string professorId = EncryptPass(username);
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
        var equipments = SearchEquipmentsRecursive(equipmentsId, statusEquipments, requestDate, times.Item1, times.Item2, petition.petitionId, 4);

        if(equipments.i == 1)
        {
            return;
        } 
        else 
        {
            if(petition.affected > 0)
            {
                var petitionDetailsId = AddPetitionDetails(petition.petitionId, equipments.equipmentsId, times.Item1, times.Item2, requestDate, currentDate, equipments.statusEquipments);
                if(petitionDetailsId.affected.Count() >= 1)
                {
                    WriteLine("Petition added");
                } else
                {
                    WriteLine("The Petition was not added. Try again");
                }
            }
            else
            {
                WriteLine("The Petition couldn't be added. Try again.");
            }
        }
    }

    public static (List<int> affected, List<int> petitionDetailsId) AddPetitionDetails(int petitionId, List<string> equipmentsId, DateTime initTime, DateTime endTime, DateTime requestedDate, DateTime currentDate, List<byte?> statusEquipments)
    {
        int i=0;
        List<int>? petitionDetailsId = new List<int>();
        List<int>? affecteds = new List<int>();
        
        if (equipmentsId == null || statusEquipments == null || equipmentsId.Count != statusEquipments.Count)
        {
            // Manejar el caso donde las listas no son válidas
            WriteLine("entro el if donde valida que las listas son nulas");
            return (affecteds, petitionDetailsId);
        }

        using (bd_storage db = new())
        {
            if(db.PetitionDetails is null)
            { 
                WriteLine("Table not created");
                return(affecteds, petitionDetailsId);
            }

            foreach(var e in equipmentsId)
            {
                PetitionDetail pD = new() 
                {
                    PetitionId = petitionId,
                    EquipmentId = equipmentsId[i],
                    StatusId = statusEquipments[i],
                    DispatchTime = initTime,
                    ReturnTime = endTime,
                    RequestedDate = requestedDate,
                    CurrentDate = currentDate
                };

                EntityEntry<PetitionDetail> entity = db.PetitionDetails.Add(pD);
                affecteds.Add(db.SaveChanges());
                petitionDetailsId.Add(pD.PetitionDetailsId);
                i++;
            }
        }
        return (affecteds, petitionDetailsId);
    }

    public static (int affected, int petitionId) AddPetition(int classroomId, string professorId, string storerId, string subjectId)
    {
        using(bd_storage db = new())
        {
            if(db.Petitions is null)
            { 
                return(0, -1);            
            }

            Petition p  = new Petition()
            {
                ClassroomId = classroomId,
                ProfessorId = professorId,
                StorerId = storerId,
                SubjectId = subjectId
            };

            EntityEntry<Petition> entity = db.Petitions.Add(p);
            int affected = db.SaveChanges();
            return (affected, p.PetitionId);
        }
    }

    public static void DeletePetitionFormat(string username)
    {
        WriteLine("Here's a list of all the petition format that has not been accepted yet. ");
        ViewRequestFormatNotAcceptedYet(username);

        while (true)
        {
            WriteLine();
            WriteLine("Provide the ID of the petition that you want to delete (check the list): ");
            int petitionId = Convert.ToInt32(ReadLine());

            using (bd_storage db = new())
            {
                // Verifica si existe el petitionDetail con el ID proporcionado
                var petitionDetail = db.PetitionDetails
                    .FirstOrDefault(e => e.PetitionDetailsId == petitionId);

                if (petitionDetail == null)
                {
                    WriteLine("That petition ID doesn't exist in the database, try again");
                    continue; // Vuelve al inicio del bucle
                }

                // Elimina el registro de RequestDetails
                db.PetitionDetails.Remove(petitionDetail);
                int affectedDetails = db.SaveChanges();

                if (affectedDetails > 0)
                {
                    WriteLine("PetitionDetails successfully deleted");
                }
                else
                {
                    WriteLine("PetitionDetails couldn't be deleted");
                }

                // Obtén el RequestId asociado
                int? petitionsId = petitionDetail.PetitionId;

                // Elimina el registro de Requests
                var petition = db.Petitions
                    .FirstOrDefault(r => r.PetitionId == petitionsId);

                if (petition != null)
                {
                    db.Petitions.Remove(petition);
                    int affectedPetitions = db.SaveChanges();

                    if (affectedPetitions > 0)
                    {
                        WriteLine("Petition successfully deleted");
                    }
                    else
                    {
                        WriteLine("Petition couldn't be deleted");
                    }
                }
            }

            return;
        }
    }

    public static void UpdatePetitionFormat(string username)
    {
        int i=1, affected = 0, op=0;
        bool validateRequest=false;
        DateTime request=DateTime.Today;

        WriteLine("Here's a list of all the petition format that has not been accepted yet. ");
        ViewPetition(username);
        WriteLine();

        WriteLine("Provide the ID of the petition that you want to modify (check the list): ");
        int petitionID = Convert.ToInt32(ReadNonEmptyLine());

        using (bd_storage db = new bd_storage())
        {

            IQueryable<Petition> petitionss = db.Petitions
                .Include(rd => rd.Classroom)
                .Include(rd => rd.Professor)
                .Include(rd => rd.Subject).Where( rd => rd.PetitionId==petitionID);


            IQueryable<PetitionDetail> petitionDetailss = db.PetitionDetails
                .Include(rd => rd.Status)
                .Include(rd=> rd.Equipment)
                .Where(r => r.PetitionId==petitionID);

            var petitionList = petitionDetailss.ToList();

            List <Equipment> listEquipments= new List<Equipment>();

            do
            {
                if(petitionss is not null && petitionss.Any() && 
                petitionDetailss is not null && petitionDetailss.Any())
                {
                    WriteLine("These are the fields you can update:");
                    WriteLine($"{i}. Classroom: {petitionss.First().Classroom.Name}");
                    WriteLine($"{i+1}. Subject: {petitionss.First().Subject.Name}");
                    WriteLine($"{i+2}. Date of the request: {petitionDetailss.First().RequestedDate.Date}");
                    WriteLine($"{i+3}. Dispatch time: {petitionDetailss.First().DispatchTime.TimeOfDay} and Return time: {petitionDetailss.First().ReturnTime.TimeOfDay}");
                    WriteLine($"{i+4}. Equipment(s) in the request:");

                    foreach (var petitionDetail in petitionDetailss)
                    {
                        WriteLine($"     -{petitionDetail.Equipment.EquipmentId} ({petitionDetail.Equipment.Name})");
                        listEquipments.Add(petitionDetail.Equipment);
                        i++;
                    }

                    WriteLine("Select an option to modify");
                    op = Convert.ToInt32(ReadNonEmptyLine());
                    validateRequest=true;
                }
                else 
                {
                    Clear();
                    WriteLine("Request not found. Try again.");
                    WriteLine("Here's a list of all the petition format that has not been accepted yet. ");
                    ViewRequestFormatNotAcceptedYet(username);
                    WriteLine();

                    WriteLine("Provide the ID of the petition that you want to modify (check the list): ");
                    petitionID = Convert.ToInt32(ReadNonEmptyLine());

                    petitionss = db.Petitions
                    .Include(rd => rd.Classroom)
                    .Include(rd => rd.Professor)
                    .Include(rd => rd.Subject)
                    .Where( rd => rd.PetitionId==petitionID);

                    petitionDetailss = db.PetitionDetails
                    .Include(rd => rd.Status)
                    .Include(rd=> rd.Equipment)
                    .Where( rd => rd.PetitionId==petitionID);

                    petitionList = petitionDetailss.ToList();
                    listEquipments= new List<Equipment>();
                }
            } while (validateRequest==false);

            switch(op)
            {
                case 1:
                    int classroomId = AddClassroom();
                    petitionss.First().ClassroomId=classroomId;
                    affected = db.SaveChanges();
                    if (affected>0)
                    {
                        WriteLine("Request changed");
                    }
                    else 
                    {
                        WriteLine("Request not changed");
                    }
                break;
                case 2:
                    string subjectId = SearchSubjectsByName("xyz", 1);
                    petitionss.First().SubjectId = subjectId;
                    if (affected>0)
                    {
                        WriteLine("Request changed");
                    }
                    else 
                    {
                        WriteLine("Request not changed");
                    }
                break;
                case 3:
                    DateTime newDate = AddDate(DateTime.Now.Date);
                    foreach (var requestDetail in petitionDetailss)
                    {
                        requestDetail.RequestedDate = newDate;
                    }
                    affected = db.SaveChanges();
                    if(affected>0)
                    {
                        WriteLine("Request changed");
                    }
                    else 
                    {
                        WriteLine("Request not changed");
                    }
                break;
                case 4:
                    var times = AddTimes(request);
                    foreach (var requestDetail in petitionDetailss)
                    {
                        requestDetail.DispatchTime = times.Item1;
                        requestDetail.ReturnTime = times.Item2;
                    }
                    affected = db.SaveChanges();
                    if (affected>0)
                    {
                        WriteLine("Request changed");
                    }
                    else
                    {
                        WriteLine("Request not changed");
                    }   
                break;
                case 5:
                    i = 1;
                    int equipId = 0;
                    foreach (var e in listEquipments)
                    {
                        WriteLine($"{i}. {e.EquipmentId}-{e.Name}");
                    }
                    WriteLine("Select the number of the equipment");
                    bool validateEq = false;
                    do{
                        try
                        {
                            equipId = Convert.ToInt32(ReadNonEmptyLine());
                            if(equipId>0 && equipId<=listEquipments.Count())
                            {
                                validateEq=true;
                            }
                        }
                        catch (FormatException)
                        {
                            WriteLine("That is not a correct option, try again.");
                        }
                        catch (OverflowException)
                        {
                            WriteLine("That is not a correct option, try again.");
                        }
                    } while (validateEq==false);

                    var selectedEquipment = listEquipments[equipId - 1];

                    List<string> equipmentsId = new List<string>();
                    List<byte?> statusIds = new List<byte?>();
                    
                    var updatedEquipments = SearchEquipmentsRecursive(
                        equipmentsId,
                        statusIds,
                        petitionDetailss.First().RequestedDate,
                        petitionDetailss.First().DispatchTime,
                        petitionDetailss.First().ReturnTime,
                        petitionDetailss.First().PetitionId,
                        1
                    );

                    // Obtener los valores del nuevo equipo seleccionado
                    foreach (var requestDetail in petitionDetailss)
                    {
                        // Cambiar el equipo en la tabla de la base de datos
                        requestDetail.EquipmentId = updatedEquipments.equipmentsId.First();
                        // Cambiar el estado del equipo conforme al nuevo equipo seleccionado
                        requestDetail.StatusId = updatedEquipments.statusEquipments.First();
                    }

                    affected = db.SaveChanges();
                    if(affected>0)
                    {
                        WriteLine("Request changed");
                    }
                    else 
                    {
                        WriteLine("Request not changed");
                    }
                break;
                case 6:
                    return;
                default:
                    WriteLine("Not a valide option. Try again.");
                break;
            }
        }
    }

}
