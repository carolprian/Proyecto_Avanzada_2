using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
partial class Program
{
    public static void PetitionFormat(string UserName)
    {
        // Escribe y verifica el plantel de donde se esta pidiendo el permiso
        string Plantel = WritePlantel();
        DateTime CurrentDate = DateTime.Now;
        // Encripta la nomina
        string ProfessorId = EncryptPass(UserName);
        // Selecciona el salon y retorna el ID
        int ClassroomId = AddClassroom();
        // Buscar la materia y regresa el ID cuando ya solo haya uno
        string SubjectId = SearchSubjectsByName(" ", 1);
        // Selecciona el primer storer regustrado en la lista
        string? StorerId = AddStorer();
        if(StorerId is null)
        {
            WriteLine("There's not a storer to do your request. Please contact the coordinator or the storer");
            WriteLine("Going back to the menu...");
            return;
        }
        // Ingesa la fecha y la regresa después de las verificaciones
        DateTime RequestDate = AddDate(CurrentDate);
        // Selecciona los horarios de la clase
        var Times = AddTimes(RequestDate);
        // Manda listas vacias para la busqueda de equipos
        List<string> EquipmentsId = new List<string>();
        List<byte?> StatusEquipments = new List<byte?>();
        // Se crea primero el registro en la linking table ya que se necesita para buscar los equipos
        var Petition = AddPetition(ClassroomId, ProfessorId, StorerId, SubjectId);
        // Busqueda recursiva de equipos para agregarlos a la peticion de material
        var Equipments = SearchEquipmentsRecursive(EquipmentsId, StatusEquipments, RequestDate, Times.Item1, Times.Item2, Petition.PetitionId, 4, false);
        // Si se selecciono que se termine la peticion de materiales
        if(Equipments.i == 1)
        {
            return;
        } 
        else 
        {
            // Si se agrego el registro de petition se agrega los datos en petitionDetails
            if(Petition.Affected > 0)
            {
                //Si hay equipos registrados en la lista
                if(Equipments.EquipmentsId is not null && Equipments.EquipmentsId.Count()>0 
                && Equipments.StatusEquipments is not null && Equipments.StatusEquipments.Count() > 0)
                {
                    // Si estan todos los datos correctos se agregan los registros necesarios en PetitionDetails
                    var PetitionDetailsId = AddPetitionDetails(Petition.PetitionId, Equipments.EquipmentsId, Times.Item1, Times.Item2, RequestDate, CurrentDate, Equipments.StatusEquipments);
                    // Muestra los mensajes respectivos de acuerdo a las filas afectadas
                    if(PetitionDetailsId.Affected.Count() >= 1)
                    {
                        WriteLine("Petition added");
                    } else
                    {
                        WriteLine("The Petition was not added. Try again");
                        BackToMenu();
                    }
                }
                else { 
                    // Si la lista de equipos esta vacia
                    WriteLine("There were not equipments registered. Try again");
                    DeletePetition(Petition.PetitionId);
                    BackToMenu();
                }
            }
            else
            {
                WriteLine("The Petition couldn't be added. Try again.");
                BackToMenu();
            }
        }
    }

    public static (List<int> Affected, List<int> PetitionDetailsId) AddPetitionDetails(int PetitionId, List<string> EquipmentsId, DateTime InitTime, DateTime EndTime, DateTime RequestedDate, DateTime CurrentDate, List<byte?> StatusEquipments)
    {
        // declaración e inicialización de variables
        int i=0;
        List<int>? PetitionDetailsId = new List<int>();
        List<int>? Affecteds = new List<int>();
        // Si las listas que se reciben son nulas y los tamaños de las listas son diferentes
        if (EquipmentsId == null || StatusEquipments == null || EquipmentsId.Count != StatusEquipments.Count)
        {
            // Manejar el caso donde las listas no son válidas
            WriteLine("The status does not match the quantity of equipments");
            return (Affecteds, PetitionDetailsId);
        }
        using (bd_storage db = new())
        {
            // Verificar que exista la tabla
            if(db.PetitionDetails is null)
            { 
                WriteLine("Table not created");
                return(Affecteds, PetitionDetailsId);
            }
            // Para cada equipo registrado se crean diferentes objetos con los valores correspondientes
            foreach(var e in EquipmentsId)
            {
                PetitionDetail pD = new() 
                {
                    PetitionId = PetitionId,
                    EquipmentId = EquipmentsId[i],
                    StatusId = StatusEquipments[i],
                    DispatchTime = InitTime,
                    ReturnTime = EndTime,
                    RequestedDate = RequestedDate,
                    CurrentDate = CurrentDate
                };
                // Agrega el registro a la tabla
                EntityEntry<PetitionDetail> entity = db.PetitionDetails.Add(pD);
                // Guarda los cambios en la bd y se guarda la cantidad de affecteds en la lista de affecteds
                Affecteds.Add(db.SaveChanges());
                //Agrega el ID generado a la lista de ID's
                PetitionDetailsId.Add(pD.PetitionDetailsId);
                i++;
            }
        }
        return (Affecteds, PetitionDetailsId);
    }

    public static (int Affected, int PetitionId) AddPetition(int ClassroomId, string ProfessorId, string StorerId, string SubjectId)
    {
        using(bd_storage db = new())
        {
            //Si la tabla no existe se retorna la funcion
            if(db.Petitions is null)
            { 
                return(0, -1);            
            }
            // Crea el objeto de la tabla
            Petition p  = new Petition()
            {
                ClassroomId = ClassroomId,
                ProfessorId = ProfessorId,
                StorerId = StorerId,
                SubjectId = SubjectId
            };
            // Agrega el registro a la tabla
            EntityEntry<Petition> entity = db.Petitions.Add(p);
            // Guarda los cambios en la base de datos
            int Affected = db.SaveChanges();
            return (Affected, p.PetitionId);
        }
    }

    public static void DeletePetition(int PetitionID)
    {
        using (bd_storage db = new())
        {
            // Realiza una consulta donde se busca el registro en la linking table con el ID que se recibe
            var Petition = db.Petitions
                    .Where(r => r.PetitionId == PetitionID)
                    .First();

            if (Petition != null)
            {
                // Si existe un registro lo remueve de la lista y guarda los cambios en la bd
                db.Petitions.Remove(Petition);
                int Affected = db.SaveChanges();
                // Dependiendo de las filas modificadas se muestra el mensaje correspondiente
                if (Affected > 0)
                {
                    WriteLine("Petition successfully deleted");
                }
                else
                {
                    WriteLine("Petition couldn't be deleted");
                }
            }
            else
            {
                WriteLine("Petition ID not found in the database");
            }
        }
    }

    public static void DeletePetitionFormat(string UserName)
    {
        do
        {
            // Listar los permisos
            WriteLine("Here's a list of all the petitions that haven't occurred ");
            ViewPetition(UserName);

            // Escribe el ID
            WriteLine();
            WriteLine("Provide the ID of the petition that you want to delete (check the list): ");
            int PetitionId = Convert.ToInt32(ReadLine());

            using (bd_storage db = new())
            {
                // Obtener el detalle de la solicitud con el ID proporcionado
                // Verifica si existe el petitionDetail con el ID proporcionado
                var PetitionDetail = db.PetitionDetails
                    .First(e => e.PetitionDetailsId == PetitionId);

                if (PetitionDetail is not null)
                {
                    int? PetitionID = PetitionDetail.PetitionId;

                    // Eliminar todos los RequestDetails con el mismo PetitionID
                    var PetitionDetailsToDelete = db.PetitionDetails.Where(d => d.PetitionId == PetitionID);
                    db.PetitionDetails.RemoveRange(PetitionDetailsToDelete);

                    int AffectedDetails = db.SaveChanges();
                    // Mostrar los mensajes dependiendo del valor de Affected
                    if (AffectedDetails > 0)
                    {
                        WriteLine("Details of petition successfully deleted");

                        // Eliminar la solicitud principal
                        var Petition = db.Petitions.First(r => r.PetitionId == PetitionID);
                        if (Petition is not null)
                        {
                            db.Petitions.Remove(Petition);
                            int AffectedPetition = db.SaveChanges();
                            // Mostrar los mensajes necesarios por el estado de la consulta
                            if (AffectedPetition == 1)
                            {
                                WriteLine("Petition successfully deleted");
                            }
                            else
                            {
                                WriteLine("Petition couldn't be deleted");
                            }
                        }
                        else
                        {
                            WriteLine("Petition not found");
                        }
                    }
                    else
                    {
                        WriteLine("The details of the petition couldn't be deleted");
                    }
                }
                else
                {
                    WriteLine("That petition ID doesn't exist in the database, try again");
                }
            }

            // Preguntar al usuario si desea eliminar otra solicitud
            WriteLine("Do you want to delete another petition? (yes/no): ");
        } while (ReadNonEmptyLine().Trim().ToLower() == "yes");
    }
    
    public static void UpdatePetitionFormat(string UserName)
    {
        #region Variables
        int i=1, Affected = 0, Option=0, EquipId = 0;
        bool ValidatePetition=false, ValidateEq = false;;
        DateTime RequestDate=DateTime.Today;
        #endregion

        // Listar permisos no aprobados por el profesor
        WriteLine("Here's a list of all the petition format that has not occurrred ");
        ViewPetition(UserName);
        WriteLine();

        // Pedir el ID del permiso a modificar
        WriteLine("Provide the ID of the petition that you want to modify (check the list): ");
        int PetitionID = Convert.ToInt32(ReadNonEmptyLine());

        using(bd_storage db = new bd_storage()){

            // Consultar si existen los permisos que escogio el usuario y guardar los campos
            IQueryable<Petition> PetitionsQuery = db.Petitions
                .Include(rd => rd.Classroom)
                .Include(rd => rd.Professor)
                .Include(rd => rd.Subject).Where( rd => rd.PetitionId==PetitionID);

            IQueryable<PetitionDetail> PetitionDetailsQuery = db.PetitionDetails
                .Include(rd => rd.Status)
                .Include(rd=> rd.Equipment)
                .Where(r => r.PetitionId==PetitionID).Where( r => r.RequestedDate > DateTime.Today);

            // Hace listas de equipos y petitionDetails
            var PetitionList = PetitionDetailsQuery.ToList();
            List <Equipment> ListEquipments= new List<Equipment>();
            
            do{
                if (PetitionsQuery != null && PetitionsQuery.Any() && PetitionDetailsQuery != null && PetitionDetailsQuery.Any())
                {
                    // Mostrar los campos que se pueden modificar
                    WriteLine("These are the fields you can update:");
                    WriteLine($"1. Classroom: {PetitionsQuery.First().Classroom.Name}");
                    WriteLine($"2. Subject: {PetitionsQuery.First().Subject.Name}");
                    WriteLine($"3. Date of the petition: {PetitionDetailsQuery.First().RequestedDate.Date}");
                    WriteLine($"4. Dispatch time: {PetitionDetailsQuery.First().DispatchTime.TimeOfDay} and Return time: {PetitionDetailsQuery.First().ReturnTime.TimeOfDay}");

                    // Mostrar todos los equipos del permiso seleccionado
                    WriteLine($"5. Equipment(s) in the petition:");
                    foreach (var petitionDetail in PetitionDetailsQuery)
                    {
                        WriteLine($"     -{petitionDetail.Equipment.EquipmentId} ({petitionDetail.Equipment.Name})");
                        // Agregar a lista para poder manipular los datos de forma sencilla
                        ListEquipments.Add(petitionDetail.Equipment);
                        i++;
                    }
                    WriteLine("Select an option to modify: ");
                    Option = Convert.ToInt32(ReadNonEmptyLine());
                    ValidatePetition = true;
                }
                else {
                    Clear();
                    // Listar permisos no aprobados por el profesor
                    WriteLine("Here's a list of all the petition format that has not occurrred ");
                    ViewPetition(UserName);
                    WriteLine();

                    // Pedir el ID del permiso a modificar
                    WriteLine("Provide the ID of the petition that you want to modify (check the list): ");
                    PetitionID = Convert.ToInt32(ReadNonEmptyLine());

                    // Consultar si existen los permisos que escogio el usuario y guardar los campos
                    PetitionsQuery = db.Petitions
                        .Include(rd => rd.Classroom)
                        .Include(rd => rd.Professor)
                        .Include(rd => rd.Subject).Where( rd => rd.PetitionId==PetitionID);

                    PetitionDetailsQuery = db.PetitionDetails
                        .Include(rd => rd.Status)
                        .Include(rd=> rd.Equipment)
                        .Where(r => r.PetitionId==PetitionID).Where( r => r.RequestedDate > DateTime.Today);
                }
            } while (ValidatePetition==false);

            // De acuerdo a la opcion del campo a modificar
            switch(Option)
            {
                case 1:
                {
                    // Llamar la función para listar los salones y que se seleccione uno
                    int ClassroomId = AddClassroom();
                    // Cambiar la query en el primer valor con el nuevo ID del classroom
                    PetitionsQuery.First().ClassroomId=ClassroomId;
                    Affected = db.SaveChanges();
                
                }break; 
                case 2:
                {
                    // Llamar la función para buscar la nueva materia
                    string SubjectId = SearchSubjectsByName(" ", 1);
                    // Cambiar la query en el primer valor con el nuevo ID de la materia
                    PetitionsQuery.First().SubjectId = SubjectId;
                    Affected = db.SaveChanges();
                }break;
                case 3:
                {
                    // Llamar la función para establecer la nueva fecha del pedido
                    DateTime NewDate = AddDate(DateTime.Today.Date);
                    // Por todos los Petition Details de un Petition se guarda el nuevo valor de la fecha
                    foreach (var petitionsDetail in PetitionDetailsQuery)
                    {
                        petitionsDetail.RequestedDate = NewDate;
                    }
                    // Guardar los cambios en la bd
                    Affected = db.SaveChanges();
                }break;
                case 4:
                {
                    // Llamar la función para establecer las horas de la clase de un pedido
                    var Times = AddTimes(RequestDate);
                    // Por todos los Petition Details de un Petition se guarda los nuevos valores de horas 
                    foreach (var petitionsDetail in PetitionDetailsQuery)
                    {
                        petitionsDetail.DispatchTime = Times.Item1;
                        petitionsDetail.ReturnTime = Times.Item2;
                    }
                    Affected = db.SaveChanges();
                } break;
                case 5:
                {
                    i = 1;
                    // Lista los equipos del permiso y permite al usuario seleccionar uno
                    foreach (var e in PetitionDetailsQuery)
                    {
                        WriteLine($"{e.PetitionDetailsId}. {e.Equipment.Name}");
                    }
                    WriteLine("Select the number of the equipment");
                    // Ciclo para asegurarse de que ingrese un numero valido   
                    do{
                        // Lo convierte a Int y si escoge un numero de los que se le muestra sale del ciclo de do-While
                        EquipId = Convert.ToInt32(ReadNonEmptyLine());
                        if(EquipId>0)
                        {
                            ValidateEq=true;
                        }
                    } while (ValidateEq==false);
                    // Creacion de listas para mandar como parametros
                    List<string> EquipmentsId = new List<string>();
                    List<byte?> StatusIds = new List<byte?>();
                    // Llamar a la funcion para obtener el valor del nuevo equipo
                    var UpdatedEquipments = SearchEquipmentsRecursive(
                        EquipmentsId,
                        StatusIds,
                        PetitionDetailsQuery.First().RequestedDate,
                        PetitionDetailsQuery.First().DispatchTime,
                        PetitionDetailsQuery.First().ReturnTime,
                        (int)PetitionDetailsQuery.First().PetitionId,
                        1, 
                        true
                    );

                    // Escoger un solo equipo en la tabla Petition Details
                    var SingularPetitionDetail = db.PetitionDetails
                    .Where(rD => rD.PetitionDetailsId == EquipId);
                    // Cambiar el equipo en la tabla de la base de datos
                    SingularPetitionDetail.First().EquipmentId = UpdatedEquipments.EquipmentsId.First();
                    // Cambiar el estado del equipo conforme al nuevo equipo seleccionado
                    SingularPetitionDetail.First().StatusId = UpdatedEquipments.StatusEquipments.First();
                    Affected = db.SaveChanges();
                } break;
                case 6:
                {
                    return;

                }
                default:{
                    WriteLine("Not a valide option. Try again.");
                }break;
            }
            WriteLine(Affected > 0 ? "Petition changed" : "Petition not changed");
        }
    }

}
