using Microsoft.EntityFrameworkCore;
using AutoGens;
using System.Linq;
partial class Program
{
    public static void SearchStudentGeneral()
    {
        using (bd_storage db = new())
        {
            WriteLine("Provide the ID of the student you want to search:");
            string studentId = ReadNonEmptyLine();

            var student = db.Students
            .Include( g => g.Group)
            .SingleOrDefault(s => s.StudentId == studentId);

            if (student == null)
            {
                WriteLine("No student found");
                SubMenuStudentsHistory();
            }

            WriteLine("Student Information: ");
            
            WriteLine($"Name: {student.Name}, Paternal Last Name: {student.LastNameP}, Maternal Last Name: {student.LastNameM}, Group: {student.Group.Name}");

            var requests = db.Requests.Where(r => r.StudentId == student.StudentId).ToList();

            if (requests.Count == 0)
            {
                WriteLine("No history found for the student.");
                SubMenuStudentsUsingEquipment();
            }

            List<int> requestIds = requests.Select(r => r.RequestId).ToList();
            
            IQueryable<RequestDetail> RequestDetails = db.RequestDetails
            .Where(rd => requestIds.Contains((int)rd.RequestId))
            .Include(rd => rd.Equipment);

            var groupedRequests = RequestDetails.GroupBy(r => r.RequestId);

            int i = 0;
            WriteLine("");
            WriteLine("Student History: ");

            foreach (var group in groupedRequests)
            {
                i++;
                var firstRequest = group.First();

                WriteLine($"Request {i}: ");
                WriteLine($"Request Detail: {firstRequest.RequestId}");
                WriteLine($"Dispatch Time: {firstRequest.DispatchTime}");
                WriteLine($"Return Time: {firstRequest.ReturnTime}");
                WriteLine($"Requested Date: {firstRequest.RequestedDate}");
                WriteLine($"Current Date: {firstRequest.CurrentDate}");

                WriteLine("Equipment:");
                foreach (var r in group)
                {
                    WriteLine($"Equipment Name: {r.Equipment.Name}");
                }
            }  
        }
    }

    public static void SearchStudentUsingEquipment()
    {
        using (bd_storage db = new())
        {
            WriteLine("Provide the ID of the student you want to search:");
            string studentId = ReadNonEmptyLine();

            var student = db.Students
            .Include( g => g.Group)
            .SingleOrDefault(s => s.StudentId == studentId);

            if (student == null)
            {
                WriteLine("No student found");
                SubMenuStudentsUsingEquipment();
            }

            var requests = db.Requests.Where(r => r.StudentId == student.StudentId).ToList();

            if (requests.Count == 0)
            {
                WriteLine("No history found for the student.");
                SubMenuStudentsUsingEquipment();
            }

            List<int> requestIds = requests.Select(r => r.RequestId).ToList();
            
            IQueryable<RequestDetail>? RequestDetails = db.RequestDetails
            .Where(rd => requestIds.Contains((int)rd.RequestId) && rd.StatusId == 2)
            .Include(rd => rd.Equipment);

            var groupedRequests = RequestDetails.GroupBy(r => r.RequestId);

            int i = 0;
            WriteLine("");
            WriteLine("Students with Equipments in use: ");

            foreach (var group in groupedRequests)
            {
                i++;
                var firstRequest = group.First();

                WriteLine($"Student {i} Information: ");
                WriteLine("");
                WriteLine($"Name: {student.Name}, Last Name: {student.LastNameP}, Group: {student.Group.Name}");
                WriteLine("Equipment(s):");
                foreach (var r in group)
                {
                    WriteLine($" Equipment Name: {r.Equipment.Name}");
                }

                WriteLine($"Return Time: {firstRequest.ReturnTime.Hour}:{firstRequest.ReturnTime.Minute}");
                WriteLine($"Date: {firstRequest.RequestedDate.Date}");

            } 
        }
    }

     public static string SearchProfessorByName(string SearchTerm, int Op, int Recursive){
        int i = 1;

        if (Op==0)
        {
            WriteLine("Insert the names of the professor WITHOUT accents");
            SearchTerm = ReadNonEmptyLine();

            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors.Where(s => s.Name.ToLower().StartsWith(SearchTerm.ToLower()));
                
                if (!professors.Any())
                {
                    WriteLine($"No professors found matching the search term: {SearchTerm}. Try again");
                    return SearchProfessorByName(SearchTerm, 0, 0);
                } 
                else 
                {
                    if (professors.Count() == 1)
                    {
                        // Si encontramos una única materia, la retornamos
                        return professors.FirstOrDefault().ProfessorId;
                    }
                    else if(professors.Count() >1)
                    {
                        foreach(var s in professors)
                        {
                            WriteLine($"{i}. {s.Name} {s.LastNameP} {s.LastNameM}");
                            i++;
                        }

                        // Si hay más de una materia que coincide, pedimos el nombre completo
                        WriteLine("Insert the PATERN last name of the teacher WITHOUT accents");
                        string searchTermNew = ReadNonEmptyLine();
                        return SearchProfessorByName(searchTermNew, 1, 0);
                    }
                    else 
                    {
                        // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(SearchTerm, 0, 0);
                    }
                }
            }
        } else if(Op==1) 
        {
            if(Recursive==0)
            {
                WriteLine("Insert the PATERN last name of the teacher WITHOUT accents");
            }

            SearchTerm = ReadNonEmptyLine();

            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors.Where(s => s.LastNameP.StartsWith(SearchTerm));
                if (!professors.Any() || professors is null)
                {
                    WriteLine($"No professors found matching the patern last name: {SearchTerm}. Try again");
                    return SearchProfessorByName(SearchTerm, 1, 0);
                } else
                {
                    if (professors.Count() == 1)
                    {
                        // Si encontramos un único profesor, lo retornamos
                        return professors.FirstOrDefault().ProfessorId;
                    }
                    else if(professors.Count() > 1)
                    {
                        foreach(var s in professors)
                        {
                            WriteLine($"{i}. {s.Name} {s.LastNameP} {s.LastNameM}");
                            i++;
                        }

                        // Si hay más de una materia que coincide, pedimos el apellido
                        WriteLine("Insert the MATERN last name of the teacher WITHOUT accents");
                        string searchTermNew = ReadNonEmptyLine();
                        return SearchProfessorByName(searchTermNew, 2, 0);
                    }
                    else 
                    {
                        // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(SearchTerm, 0, 0);
                    }
                }
            }
        } 
        else
        {
            if(Recursive==0)
            {
                WriteLine("Insert the MATERN last name of the teacher WITHOUT accents");
            }

            using (bd_storage db = new())
            {
                IQueryable<Professor>? professors = db.Professors
                    .Where(s => s.LastNameM.StartsWith(SearchTerm));
                
                if (!professors.Any())
                {
                    WriteLine($"No professors found matching the matern last name: {SearchTerm}. Try again");
                    return SearchProfessorByName(SearchTerm, 2, 0);
                } 
                else 
                {
                    if (professors.Count() == 1)
                    {
                        // Si encontramos un único profesor, lo retornamos
                        return professors.FirstOrDefault().ProfessorId;
                    }
                    else 
                    {
                        // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(SearchTerm, 0, 0);
                    }
                }
            }
        }
    }

    public static string SearchSubjectsByName(string SearchTerm, int Op)
    {
        int i = 0;

        if(Op==1)
        {
            WriteLine("Insert the name start of the subject WITHOUT accents");
            SearchTerm = VerifyAlphabeticInput();
        } 
    
        using (bd_storage db = new())
        {
            IQueryable<Subject>? subjects = db.Subjects.Where(s => s.Name.ToLower().StartsWith(SearchTerm.ToLower()));
            
            if (!subjects.Any() || subjects is null)
            {
                WriteLine("No subjects found matching the search term: " + SearchTerm + "Try again.");
                return SearchSubjectsByName(SearchTerm, 1);
            } 
            else 
            {
                i = 1;

                if (subjects.Count() == 1)
                {
                    // Si encontramos una única materia, la retornamos
                    return subjects.FirstOrDefault().SubjectId;
                }
                else if(subjects.Count() >1)
                {
                    foreach(var s in subjects){
                        WriteLine($"{i}. {s.Name}");
                        i++;
                    }
                    // Si hay más de una materia que coincide, pedimos el nombre completo
                    WriteLine("Insert the whole name of the subject to confirm");
                    return SearchSubjectsByName(SearchTerm, 2);
                }
                else 
                {
                    // Si no encontramos ninguna materia, solicitamos que se ingrese el nombre nuevamente
                    WriteLine("Subject not found. Try again.");
                    return SearchSubjectsByName(SearchTerm, 1);
                }
            }
        }
    }

    public static void SearchEquipmentsById(string? SearchTerm)
    {
        if (string.IsNullOrEmpty(SearchTerm))
        {
            throw new InvalidOperationException();
        }
        using (bd_storage db = new())
        {
            IQueryable<Equipment>? equipments = db.Equipments
                .Include(e => e.Area)
                .Include(e => e.Status)
                .Include(e => e.Coordinator)
                .Where(e => e.EquipmentId.StartsWith(SearchTerm)); // Utiliza StartsWith para buscar equipos cuyos nombres comiencen con el término de búsqueda

            db.ChangeTracker.LazyLoadingEnabled = false;

            if (!equipments.Any())
            {
                WriteLine("No equipment found matching the search term: " + SearchTerm);
                return;
            }
            else
            {
                WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                        "EquipmentId", "Equipment Name", "Year", "Status");
                WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
                    
                foreach( var e in equipments)
                {
                    WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                        e.EquipmentId, e.Name, e.Year, e.Status?.Value); 
                }
                    
                WriteLine("Do you want to see more information about any of the equipments?(y/n)");
                string read = VerifyReadLengthStringExact(1);
                if(read == "y" || read =="Y")
                {
                    WriteLine("Provide the equipment ID you want to see more info:");
                    read = VerifyReadMaxLengthString(15);
                    int found = ShowEquipmentBylookigForEquipmentId(read);   
                    if(found == 0){ WriteLine($"There are no equipments that match the id:  {read}" );}
                        
                }
            }
        }
    }

    
    public static (List<string>? equipmentsId, List<byte?>? statusEquipments, int i) SearchEquipmentsRecursive(List<string>? SelectedEquipments, List<byte?>? StatusEquipments, DateTime Requested, DateTime Init, DateTime End, int? RequestId, int MaxEquipment, bool IsStudent)
    {
        string response = "h", response2 = "hi";
        using (bd_storage db = new())
        {
            WriteLine("Insert the start of the name of equipment without accents: ");
            string searchTerm = ReadNonEmptyLine().ToLower();

            var equipmentIdsInUseStud = db.RequestDetails
            .Where(rd => rd.RequestedDate.Date == Requested && rd.DispatchTime < End && rd.ReturnTime > Init)
            .Select(rd => rd.EquipmentId)
            .ToList();

            var equipmentIdsInUseProf = db.PetitionDetails
            .Where(rd => rd.RequestedDate.Date == Requested && rd.DispatchTime < End && rd.ReturnTime > Init)
            .Select(rd => rd.EquipmentId)
            .ToList();

            IQueryable<Equipment>? equipments = db.Equipments
            .Include(s => s.Status)
            .Where(e => e.Name.ToLower().StartsWith(searchTerm) &&
                            !(equipmentIdsInUseStud.Contains(e.EquipmentId) ||
                            equipmentIdsInUseProf.Contains(e.EquipmentId) ||
                            e.StatusId == 3 || e.StatusId == 4 || e.StatusId == 5))
            .AsEnumerable()
            .OrderBy(e => Guid.NewGuid())
            .AsQueryable();

            if (!equipments.Any() || equipments.Count() < 1 || equipments is null)
            {
                WriteLine("No equipment found matching the search term: " + searchTerm + " Try again.");
                SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
            }
            else if (equipments.Count() > 1)
            {
                do
                {
                    // Si hay más de una opción, seleccionar una al azar sin OrderBy(Guid.NewGuid())
                    var random = new Random();
                    var randomEquipment = equipments.OrderBy(e => random.Next()).First();

                    WriteLine("| {0,-11} | {1,-30} | {2,-55} | {3, 17}",
                        "EquipmentId", "Name", "Description", "Status");

                    WriteLine($"| {randomEquipment.EquipmentId,-11} | {randomEquipment.Name,-30} | {randomEquipment.Description,-55} | {randomEquipment.Status?.Value,17}");

                    WriteLine("Do you want to add this equipment?");
                    WriteLine("1. Yes");
                    WriteLine("2. No. Other similar");
                    WriteLine("3. Start again with the search");
                    WriteLine("4. No. End the search");
                    response = ReadNonEmptyLine();
                    if (response == "1")
                    {
                        SelectedEquipments.Add(randomEquipment.EquipmentId);
                        StatusEquipments.Add(randomEquipment.StatusId);
                    }
                    if (response == "3")
                    {
                        SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
                    }
                    if (response == "4")
                    {
                        if (SelectedEquipments.Count < 1)
                        {
                            WriteLine("There's not an equipment registered. Select an option:");
                            WriteLine("1. Delete the whole request");
                            WriteLine("2. Start again adding the equipments");
                            response2 = ReadNonEmptyLine();
                            if (response2 == "1")
                            {
                                if(IsStudent == true){
                                    DeleteRequest(RequestId);
                                    return (SelectedEquipments, StatusEquipments, 1);
                                } else {
                                    DeletePetition(RequestId);
                                    return (SelectedEquipments, StatusEquipments, 1);
                                }
                            }
                            if(response2== "2")
                            {
                                SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
                            }
                        }
                    }
                } while (response == "2");
            }
            else
            {
                // Solo hay una opción, agregarla directamente
                var singleEquipment = equipments.First();
                WriteLine("| {0,-11} | {1,-30} | {2,-55} | {3, 17}",
                    "EquipmentId", "Name", "Description", "Status");
                WriteLine($"| {singleEquipment.EquipmentId,-11} | {singleEquipment.Name,-30} | {singleEquipment.Description,-55} | {singleEquipment.Status?.Value,17}");

                WriteLine("Do you want to add this equipment? y/n");
                response = ReadNonEmptyLine().ToLower();
                if (response == "y")
                {
                    SelectedEquipments.Add(singleEquipment.EquipmentId);
                    StatusEquipments.Add(singleEquipment.StatusId);
                }
                else if(response == "n")
                {
                   if (SelectedEquipments.Count < 1)
                    {
                        WriteLine("There's not an equipment registered. Select an option:");
                        WriteLine("1. Delete the whole request");
                        WriteLine("2. Start again adding the equipments");
                        response2 = ReadNonEmptyLine();
                        if (response2 == "1")
                        {
                            if(IsStudent == true){
                                DeleteRequest(RequestId);
                                return (SelectedEquipments, StatusEquipments, 1);
                            } else {
                                DeletePetition(RequestId);
                                return (SelectedEquipments, StatusEquipments, 1);
                            }
                        }
                        if(response2== "2")
                        {
                            SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
                        }
                    }
                    else {
                        WriteLine("Select an option");
                        WriteLine("1. Continued the request");
                        WriteLine("2. End the request");
                        response2 = ReadNonEmptyLine();
                        if(response2== "1")
                        {
                            SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
                        }
                        if (response2 == "2")
                        {
                            return (SelectedEquipments, StatusEquipments, 1);
                        }
                    }
                }
            }
        }
        if (SelectedEquipments.Count <= MaxEquipment)
        {
            WriteLine("Do you want to add another equipment? y/n");
            response = ReadNonEmptyLine().ToLower();
            if (response == "y")
            {
                SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
            }
            else if(response == "n")
            {
                return (SelectedEquipments, StatusEquipments, 0);
            }
        }
        else
        {
            WriteLine($"You have reached the maximum limit of {MaxEquipment} equipments.");
            return (SelectedEquipments, StatusEquipments, 0);
        }
        return (SelectedEquipments, StatusEquipments, 0);
    }
}

