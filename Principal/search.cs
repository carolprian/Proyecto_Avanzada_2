using Microsoft.EntityFrameworkCore;
using AutoGens;
using System.Linq;
using ConsoleTables;
partial class Program
{
    public static bool SearchStudentGeneral()
    {
        bool aux = false;
        using (bd_storage db = new())
        {
            // Se le pide el registro del usuario
            WriteLine("Provide the ID of the student you want to search:");
            string studentId = ReadNonEmptyLine();
            // Se selecciona uno solo ya que el registro es un numero irrepetible 
            var student = db.Students
            .Include( g => g.Group)
            .SingleOrDefault(s => s.StudentId == studentId);
        
            if (student == null)
            {
                // No se encontro ningun estudiante
                WriteLine("No student found");
                // Se vuelve a mandar al menu
                aux = true;
                return aux;
            } else
            {
                // Se muestra la información encontrada del estudiante y los permisos que ha solicitado

                WriteLine("Student Information: ");
                var table = new ConsoleTable("Id", "Name", "LastName P", "LasName M", "Group");
                
                table.AddRow(student.StudentId, student.Name, student.LastNameP, student.LastNameM, student.Group.Name);

                table.Write();
                WriteLine();
                // Se realiza una lista de los permisos que ha solicitado el estudiante
                var requests = db.Requests.Where(r => r.StudentId == student.StudentId).ToList();
                // Si no hay se manda de nuevo al menu y se muestra el mensaje explicativo
                if (requests.Count == 0)
                {
                    WriteLine("No history found for the student.");
                    SubMenuStudentsUsingEquipment();
                }
                // Realiza una lista de enteros de los Id de los request del estudiante
                List<int> requestIds = requests.Select(r => r.RequestId).ToList();
                
                IQueryable<RequestDetail> RequestDetails = db.RequestDetails
                .Include(s=>s.Status)
                .Where(rd => requestIds.Contains((int)rd.RequestId))
                .Include(rd => rd.Equipment);
                // Agrupa los Request Details por su Request Id
                var groupedRequests = RequestDetails.GroupBy(r => r.RequestId);

                int i = 0;
                foreach (var group in groupedRequests)
                {
                    i++;
                    var firstRequest = group.First();

                    var table1 = new ConsoleTable("No.", "Request Detail Id", "Dispatch Time", "Return Time", "Request Date", "Current Date");
                
                    table1.AddRow(i, firstRequest.RequestId, firstRequest.DispatchTime.TimeOfDay, firstRequest.ReturnTime.TimeOfDay, 
                    $"{firstRequest.RequestedDate.Day}/{firstRequest.RequestedDate.Month}/{firstRequest.RequestedDate.Year}", $"{firstRequest.CurrentDate.Day}/{firstRequest.CurrentDate.Month}/{firstRequest.CurrentDate.Year}");

                    foreach (var r in group)
                    {
                        // Adding an empty string as the first column to match the table structure
                        table1.AddRow("Equipment Name", r.Equipment.Name, "", "", "", "");
                    }

                    table1.Write();
                    WriteLine();
                } 
            }
        }
        return aux;
    }

    public static bool SearchStudentUsingEquipment()
    {
        bool aux = false;
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
                aux = true;
                return aux;
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
        return aux;
    }

     public static string SearchProfessorByName(string SearchTerm, int Op, int Recursive){
        // Indice de busqueda
        int i = 1;
        // Si Op == 0 se busca empezando por los nombres
        if (Op==0)
        {
            // Se piden los nombres
            WriteLine("Insert the names of the professor WITHOUT accents");
            SearchTerm = ReadNonEmptyLine();

            using (bd_storage db = new())
            {
                // Se buscan los profesores cuyos nombres inicien por el termino escrito por el usuario
                IQueryable<Professor>? Professors = db.Professors
                .Where(s => s.Name.ToLower().StartsWith(SearchTerm.ToLower()));
                // Si no encuentra ningun profesor vuelve a llamar a la funcion en el apartado de nombres
                if (!Professors.Any())
                {
                    WriteLine($"No professors found matching the search term: {SearchTerm}. Try again");
                    return SearchProfessorByName(SearchTerm, 0, 0);
                } 
                else 
                {
                    if (Professors.Count() == 1)
                    {
                        // Si encontramos un único profesor, lo retornamos
                        return Professors.First().ProfessorId;
                    }
                    else if(Professors.Count() >1)
                    {
                        // Si encontramos 2 o más profesores se muestra el nombre completo de todos los profesores
                        foreach(var s in Professors)
                        {
                            WriteLine($"{i}. {s.Name} {s.LastNameP} {s.LastNameM}");
                            i++;
                        }

                        // Pedir el apellido paterno para volver a buscar pero ahora con ese dato
                        WriteLine("Insert the PATERN last name of the teacher WITHOUT accents");
                        return SearchProfessorByName(SearchTerm, 1, 1);
                    }
                    else 
                    {
                        // Si no encontramos ningun profesor, solicitamos que se ingrese el nombre nuevamente
                        WriteLine("Professor not found. Try again");
                        return SearchProfessorByName(SearchTerm, 0, 0);
                    }
                }
            }
        } else if(Op==1) 
        {
            // Esto es para que solo se muestre este writeLine cuando no se ha preguntado anteriormente
            if(Recursive==0)
            {
                WriteLine("Insert the PATERN last name of the teacher WITHOUT accents");
            }
            // Se escribe el apellido paterno
            SearchTerm = ReadNonEmptyLine();
            using (bd_storage db = new())
            {
                // busca a los profesores cuyo apellido paterno inicie con el apellido que nos dio el usuario
                IQueryable<Professor>? Professors = db.Professors.Where(s => s.LastNameP.ToLower().StartsWith(SearchTerm.ToLower()));
                if (!Professors.Any() || Professors is null)
                {
                    // Si no se encuentra ningun profesor se busca de nuevo desde el nombre
                    WriteLine($"No professors found matching the patern last name: {SearchTerm}. Try again");
                    return SearchProfessorByName(SearchTerm, 1, 0);
                } else
                {
                    if (Professors.Count() == 1)
                    {
                        // Si encontramos un único profesor, lo retornamos
                        return Professors.FirstOrDefault().ProfessorId;
                    }
                    else if(Professors.Count() > 1)
                    {
                        // Si encontramos 2 o más profesores se muestra el nombre completo de todos los profesores
                        foreach(var s in Professors)
                        {
                            WriteLine($"{i}. {s.Name} {s.LastNameP} {s.LastNameM}");
                            i++;
                        }

                        // Si hay más de un profesor que coincide, pedimos el apellido materno
                        WriteLine("Insert the MATERN last name of the teacher WITHOUT accents");
                        return SearchProfessorByName(SearchTerm, 2, 1);
                    }
                    else 
                    {
                        // Si no encontramos ninguna profesor, solicitamos que se ingrese el nombre nuevamente
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
            // Se ingresa el apellido paterno del profesor
            SearchTerm = ReadNonEmptyLine();
            using (bd_storage db = new())
            {
                // Se busca un profesor cuyo apellido materno comience como lo indica el usuario
                IQueryable<Professor>? Professors = db.Professors
                    .Where(s => s.LastNameM.ToLower().StartsWith(SearchTerm.ToLower()));
                
                if (!Professors.Any() || Professors is null)
                {
                    // Si no se encuentra se pide que vuelva a ingresar el nombre
                    WriteLine($"No professors found matching the matern last name: {SearchTerm}. Try again");
                    return SearchProfessorByName(SearchTerm, 2, 0);
                } 
                else 
                {
                    if (Professors.Count() == 1)
                    {
                        // Si encontramos un único profesor, lo retornamos
                        return Professors.First().ProfessorId;
                    }
                    else 
                    {
                        // Si no encontramos ninguna profesor, solicitamos que se ingrese el nombre nuevamente
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
        //Para que no se impriman los mismos WriteLine mas de lo necesario
        if(Op==1)
        {
            WriteLine("Insert the name start of the subject WITHOUT accents");
        } 
        SearchTerm = VerifyAlphabeticInput();
        using (bd_storage db = new())
        {
            if( db.Subjects is null){
                throw new InvalidOperationException("Not table found");
            } else {
                // Se buscan materias que su nombre empiece con el que el usuario puso
                IQueryable<Subject>? subjects = db.Subjects.Where(s => s.Name.ToLower().StartsWith(SearchTerm.ToLower()));
                // Si no se encuentra ninguna se manda el mensaje y se manda al inicio de la funcion
                if (!subjects.Any() || subjects is null)
                {
                    WriteLine("No subjects found matching the search term: " + SearchTerm + "Try again.");
                    return SearchSubjectsByName(SearchTerm, 1);
                } 
                else 
                {
                    //Indice
                    i = 1;
                    if (subjects.Count() == 1)
                    {
                        // Si encontramos una única materia, la retornamos
                        return subjects.First().SubjectId;
                    }
                    else if(subjects.Count() >1)
                    {
                        // Se imprime la lista de las materias con el mismo inicio de nombre
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
    }

    public static bool SearchEquipmentsById(string? SearchTerm)
    {
        bool aux = false;
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
                aux = true;
                return aux;
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
        return aux;
    }

    public static (List<string>? EquipmentsId, List<byte?>? StatusEquipments, int i) SearchEquipmentsRecursive(List<string>? SelectedEquipments, List<byte?>? StatusEquipments, DateTime Requested, DateTime Init, DateTime End, int RequestId, int MaxEquipment, bool IsStudent)
    {
        //Inicializacion de variables
        string Response = " ", Response2 = " ";
        using (bd_storage db = new())
        {
            //Inserta como comienza el nombre del equipo
            WriteLine("Insert the start of the name of equipment without accents: ");
            string SearchTerm = ReadNonEmptyLine().ToLower();
            // Hace un var donde se guardan los equipos que estan en uno en la fecha y hora que el usuario registro el nuevo permiso y lo convierte a una lista
            var EquipmentIdsInUseStud = db.RequestDetails
            .Where(rd => rd.RequestedDate.Date == Requested && rd.DispatchTime < End && rd.ReturnTime > Init)
            .Select(rd => rd.EquipmentId)
            .ToList();
            // Hace un var donde se guardan los equipos que estan en uno en la fecha y hora que el usuario registro el nuevo permiso y lo convierte a una lista
            // Pero ahora con los equipos solicitados por el profesor
            var EquipmentIdsInUseProf = db.PetitionDetails
            .Where(rd => rd.RequestedDate.Date == Requested && rd.DispatchTime < End && rd.ReturnTime > Init)
            .Select(rd => rd.EquipmentId)
            .ToList();

            // Busca equipos que no esten en mantenimiento, dañados o perdidos que su nombre empiece con el termino que especifico el usuario
            // Y de los equipos que aparezcan se quitan los que estan en las listas de equipmentsInUse
            IQueryable<Equipment>? Equipments = db.Equipments
            .Include(s => s.Status)
            .Where(e => e.Name.ToLower().StartsWith(SearchTerm) &&
                            !(EquipmentIdsInUseStud.Contains(e.EquipmentId) ||
                            EquipmentIdsInUseProf.Contains(e.EquipmentId) ||
                            e.StatusId == 3 || e.StatusId == 4 || e.StatusId == 5))
            .AsEnumerable().OrderBy(e => Guid.NewGuid())
            // Se convierte a IEnumerable para poder acormodarse en un orden aleatorio gracias al identificador GUID
            // Que produce numeros de manera aleatoria para realizar combinaciones, por eso la conversion y poder tratar "la lista"
            // como "numeros"
            .AsQueryable();
            // Y se vuelve a convertir a IQueryable para manipularlo pero con el orden diferente
            if (!Equipments.Any() || Equipments.Count() < 1 || Equipments is null)
            {
                // Si no se encuentra algun valor se vuelve a llamar la funcion
                WriteLine("No equipment found matching the search term: " + SearchTerm + " Try again.");
                SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
            }
            else if (Equipments.Count() > 1)
            {
                do
                {
                    // Si hay más de una opción, seleccionar una al azar con random
                    var Random = new Random();
                    // Se ordena por un número random que sea positivo que se especifica con el .Next()
                    // Y se selecciona el primero
                    var RandomEquipment = Equipments.OrderBy(e => Random.Next()).First();
                    // Imprimir los valores del equipo
                    WriteLine("| {0,-11} | {1,-60} | {2,-65} | {3, -17}",
                        "EquipmentId", "Name", "Description", "Status");

                    WriteLine($"| {RandomEquipment.EquipmentId,-11} | {RandomEquipment.Name,-60} | {RandomEquipment.Description,-65} | {RandomEquipment.Status?.Value,-17}");
                    // Selecciona que hacer con el equipo que se muestra
                    WriteLine("Do you want to add this equipment?");
                    WriteLine("1. Yes");
                    WriteLine("2. No. Other similar");
                    WriteLine("3. Start again with the search");
                    WriteLine("4. No. End the search");
                    Response = VerifyAllNumbers();
                    if (Response == "1")
                    {
                        // Lo agrega a la lista de equipos con su respectivo status que se guarda en la lista de status
                        SelectedEquipments.Add(RandomEquipment.EquipmentId);
                        StatusEquipments.Add(RandomEquipment.StatusId);
                    }
                    if (Response == "3")
                    {
                        // Empieza a buscar otra vez el equipo, pidiendole un nombre inicial
                        SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
                    }
                    if (Response == "4")
                    {
                        // Si no hay equipos registrados en la lista
                        if (SelectedEquipments.Count < 1)
                        {
                            WriteLine("There's not an equipment registered. Select an option:");
                            WriteLine("1. Delete the whole request");
                            WriteLine("2. Start again adding the equipments");
                            Response2 = VerifyAllNumbers();
                            // Borrará el registro de la linking table que se creo
                            if (Response2 == "1")
                            {
                                // Si el permiso es para el estudiante borra en la tabla request
                                if(IsStudent == true)
                                {
                                    DeleteRequest(RequestId);
                                    return (SelectedEquipments, StatusEquipments, 1);
                                } else // Si el permiso es para un profesor se borra de la tabla petition
                                {
                                    DeletePetition((int)RequestId);
                                    return (SelectedEquipments, StatusEquipments, 1);
                                }
                            }
                            if(Response2== "2")
                            {
                                // vuelve a llamar a la funcion para que se inicie la busqueda otra vez
                                SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
                            }
                        }
                    } else if(Response2!="1" && Response2!="2" && Response2!="3" && Response2!="4" ){
                        WriteLine("Option not valid. Try again.");
                        Response = "2";
                    }
                    // El proceso de escoger uno random y todas las opciones que se mencionaron se realiza
                    // si el usuario sigue presionando 2 para generar equipos similares
                    // Esto con el proposito de que sea el equipo que el usuario quiere
                } while (Response == "2");
            }
            else
            {
                // Solo hay una opción, agregarla directamente
                var singleEquipment = Equipments.First();
                WriteLine("| {0,-11} | {1,-30} | {2,-55} | {3, -17}",
                    "EquipmentId", "Name", "Description", "Status");
                WriteLine($"| {singleEquipment.EquipmentId,-11} | {singleEquipment.Name,-30} | {singleEquipment.Description,-55} | {singleEquipment.Status?.Value,-17}");
                // Preguntar para confirmacion
                WriteLine("Do you want to add this equipment? y/n");
                Response = ReadNonEmptyLine().ToLower();
                if (Response == "y")
                {
                    // Si su respuesta es y o si se agrega el equipo a la lista
                    SelectedEquipments.Add(singleEquipment.EquipmentId);
                    StatusEquipments.Add(singleEquipment.StatusId);
                }
                else if(Response == "n") // Si no quiere agregar ese equipo se despliegan otras opciones
                {
                    // si su respuesta es no se muestra otro menu
                   if (SelectedEquipments.Count < 1)
                    {
                        WriteLine("There's not an equipment registered. Select an option:");
                        WriteLine("1. Delete the whole request");
                        WriteLine("2. Start again adding the equipments");
                        Response2 = VerifyAllNumbers();
                        if (Response2 == "1")
                        {
                            // Borrará el registro de la linking table que se creo
                            if(IsStudent == true){
                                DeleteRequest(RequestId);
                                return (SelectedEquipments, StatusEquipments, 1);
                            } else {
                                DeletePetition(RequestId);
                                return (SelectedEquipments, StatusEquipments, 1);
                            }
                        }
                        if(Response2== "2")
                        {
                            // Inicia otra vez la busqueda
                            SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
                        }
                    }
                    else { // Si hay equipos guardados en la lista muestra lo siguiente

                        WriteLine("Select an option");
                        WriteLine("1. Continued the request");
                        WriteLine("2. End the request");
                        Response2 = VerifyAllNumbers();
                        if(Response2== "1")
                        {
                            //Inicia de nuevo la busqueda
                            SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
                        }
                        if (Response2 == "2") //termina la busqueda con los equipos ya registrados
                        {
                            return (SelectedEquipments, StatusEquipments, 1);
                        }
                    }
                }
            }
        }
        // Verifica si el maximo de equipos ya se alcanzo
        if (SelectedEquipments.Count < MaxEquipment)
        {
            // Si no se ha superado se puede agregar otro equipo
            WriteLine("Do you want to add another equipment? y/n");
            Response = ReadNonEmptyLine().ToLower();
            if (Response == "y")
            {
                // Si la respuesta es que si se inicia la busqueda otra vez
                SearchEquipmentsRecursive(SelectedEquipments, StatusEquipments, Requested, Init, End, RequestId, 4, IsStudent);
            }
            else if(Response == "n")
            {
                // Si la respuesta es no se termina y se agregan los equipos seleccionados al permiso
                return (SelectedEquipments, StatusEquipments, 0);
            }
        }
        else
        {
            // Mostrar mensaje de limite de equipos alcanzados
            WriteLine($"You have reached the maximum limit of {MaxEquipment} equipments.");
            return (SelectedEquipments, StatusEquipments, 0);
        }

        return (SelectedEquipments, StatusEquipments, 0);
    }
}

