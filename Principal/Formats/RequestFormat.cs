using AutoGens;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.Arm;

partial class Program
{
    public static void RequestFormat(string StudentID)
    {
        // Pide plantel si no es colomos no lo deja continuar
        string Plantel = WritePlantel();
        // La fecha en la que pide el permiso
        DateTime CurrentDate = DateTime.Now;
        // Regresa el ID del classroom seleccionado
        int ClassroomId = AddClassroom();
        // Busca la materia y regresa el ID
        string SubjectId = "";
        SubjectId = SearchSubjectsByName(SubjectId, 1);
        // Permite una busqueda de nombre por del profesor y regresa el ID
        string ProfessorId ="z";
        ProfessorId = SearchProfessorByName(ProfessorId, 0, 0);
        //selecciona el primer storer por defecto
        string? StorerId = AddStorer();
        if(StorerId is null)
        {
            WriteLine("There's not a storer to do your request. Please contact the coordinator or the storer");
            WriteLine("Going back to the menu...");
            return;
        }
        // Pide la fecha del prestamo
        DateTime RequestDate = AddDate(CurrentDate);
        // Pide los horarios de la clase
        var Times = AddTimes(RequestDate);
        List<string> EquipmentsId = new List<string>();
        List<byte?> StatusEquipments = new List<byte?>();
        // Agrega el registro a la linking table 
        var request = AddRequest(ClassroomId, ProfessorId, StudentID, StorerId, SubjectId);
        // Busca los equipos por el nombre y los agrega a una lista
        var equipments = SearchEquipmentsRecursive(EquipmentsId, StatusEquipments, RequestDate, Times.Item1, Times.Item2, request.requestId, 4, true);
        // Se pone 0 para establecer que no han sido aprobado el permiso
        int professorNip = 0;
        // Se pone este if, si se menciono en searchEquipments que quiere terminar el permiso y eliminarlo
        if(equipments.i == 1){
            return;
        } else {
            // Si se agrego el registro en Request
            if(request.affected > 0){
                // Agrega los datos a la tabla Request Details
                var requestDetailsId = AddRequestDetails(request.requestId, EquipmentsId, professorNip, Times.Item1, Times.Item2, RequestDate, CurrentDate, StatusEquipments);
                // Verifica que se haya agregado correctamente
                if(requestDetailsId.Affected.Count() >= 1){
                    WriteLine("Request added");
                } else
                {
                    WriteLine("The request was not added. Try again");
                }
            } // Si no se agrego el registro en Request
            else {
                WriteLine("The request couldnt be added. Try again.");
            }
        }
    }

    public static (List<int> Affected, List<int> RequestDetailsId) AddRequestDetails(int RequestId, List<string>? EquipmentsId, int ProfessorNip, DateTime InitTime, DateTime EndTime, DateTime RequestedDate, DateTime CurrentDate, List<byte?>? StatusEquipments){
        // Variable para establecer el índice de las listas de los nombres de los equipos y los status de estos
        int i=0;
        // Lista que almacena los requestDetails Id de los registros nuevos agregados en la tabla
        List<int>? RequestDetailsId = new List<int>();
        // Lista que almacena cuantas filas fueron afectadas en la tabla de Request Details al momento de re
        List<int>? Affecteds = new List<int>();
        // Verifica que la lista de equipmentId y statusId sean del mismo tamaño y no tengan valores nulos para poder usarlos
        if (EquipmentsId == null || StatusEquipments == null || EquipmentsId.Count != StatusEquipments.Count)
        {
            // Manejar el caso donde las listas no son válidas
            // Si no son validas muestra el mensaje y retorna los valores
            WriteLine("The equipment selected was not correctly added. Try again.");
            DeleteRequest(RequestId);
            return (Affecteds, RequestDetailsId);
        } else {
            using (bd_storage db = new()){
                // Si la tabla no existe se lanza una excepcion
                if(db.RequestDetails is null){ 
                    throw new InvalidOperationException ("The table Request Details does not exist. Verify it");
                }
                // Por cada equipo registrado en la tabla se genera una fila de datos en Request Details, donde se llenan los campos 
                //con los datos recibidos por los parametros
                foreach(var e in EquipmentsId){
                    RequestDetail rD = new() {
                        RequestId = RequestId,
                        EquipmentId = EquipmentsId[i],
                        StatusId = StatusEquipments[i],
                        ProfessorNip = ProfessorNip,
                        DispatchTime = InitTime,
                        ReturnTime = EndTime,
                        RequestedDate = RequestedDate,
                        CurrentDate = CurrentDate
                    };
                    // Se añaden los datos a la tabla
                    EntityEntry<RequestDetail> entity = db.RequestDetails.Add(rD);
                    // Se guardan los datos en la bd y el valor de las filas affectadas se guarda en la lista de affecteds
                    Affecteds.Add(db.SaveChanges());
                    // Se guardan los ID generados en la lista de ID's
                    RequestDetailsId.Add(rD.RequestDetailsId);
                    // Aumenta el indice de las listas
                    i++;
                }
            }
        }
        return (Affecteds, RequestDetailsId);
    }

    public static string WritePlantel()
    {
        // Declaración de variables
        string Plantel="";
        bool Ban = false;
        do{
            // Pide que se ponga el valor
            WriteLine("Insert the plantel: ");
            Plantel = ReadNonEmptyLine();
            // De acuerdo a la verificacion si es valido se termina el ciclo, si no se queda hasta que sea correcto
            Ban = VerifyPlantel(Plantel);
        }while(Ban==false);
        return Plantel;
    }

    public static bool VerifyPlantel(string Plantel){
        // Verifica que sea colomos con Trim que es quitar espacios en blanco y ToLower que pone todo en minusculas
        if(Plantel.Trim().ToLower()=="colomos"){
            return true;
        } else 
        {
            WriteLine("Plantel not registered. Try again");
            return false;
        }
    }

    public static int AddClassroom(){
        int ClassroomId=0;
        bool Ban=true;
        using (bd_storage db = new()){
            // Lista los salones
            int Count = ListClassrooms();
            // Pide que seleccione uno
            do{
                WriteLine("Select the number of the classroom: ");
                ClassroomId = TryParseStringaEntero(ReadNonEmptyLine());
                // Verifica que exista
                IQueryable<Classroom> classroomsId = db.Classrooms.Where(c => c.ClassroomId==ClassroomId);
                // Si no existe le pide que ingrese otra vez el valor
                if(classroomsId is null || !classroomsId.Any())
                {
                    WriteLine("Not a valid key. Try again");
                } else // Si existe termina el bucle y retorna el valor
                {
                    Ban=false;
                }
            } while(Ban==true);
        }
        return ClassroomId;
    }

    public static string? AddStorer(){
        using(bd_storage db= new()){
            //Obtiene la consulta de Storers
            IQueryable<Storer> storers = db.Storers;
            //Si no es nulo agarra el primer ID y lo retorna
            if(storers is not null && storers.Any())
            {
                return storers.First().StorerId;
            } else // Retorna un valor nulo si no hay algun storer registrado
            {
                return null;
            }
        }
    }

    public static (int affected, int requestId) AddRequest(int ClassroomId, string ProfessorId, string StudentId, string StorerId, string SubjectId){
        using(bd_storage db = new()){
            // Verifica que exista la tabla
            if(db.Requests is null){ 
                throw new InvalidOperationException("The table request is not found");
            }
            else{ // Si existe la tabla
                // Crea el objeto y le asigna valores
                Request r  = new Request()
                {
                    ClassroomId = ClassroomId,
                    ProfessorId = ProfessorId,
                    StudentId = StudentId,
                    StorerId = StorerId,
                    SubjectId = SubjectId
                };
                // Agrega el objeto a la tabla
                EntityEntry<Request> Entity = db.Requests.Add(r);
                // Cambia los valores de la bd
                int Affected = db.SaveChanges();
                // Retorna los valores de filas aceptadas y el ID del nuevo request
                return (Affected, r.RequestId);
            }
        }
    }

    public static DateTime AddDate(DateTime CurrentDate)
    {
        //Inicializa variables
        DateTime DateValue= DateTime.Today;
        bool ValideDate = false;
        while (ValideDate==false)
        {
            // Pide que inserte la fecha
            WriteLine("Insert the date of the class: yyyy/MM/dd");
            string DateInput = ReadNonEmptyLine();
            // Lo trata de convertir al formato especificado, teniendo en cuenta la zona horaria de la computadora, sin ningun estilo especial
            // Y si es correcto entra al if
            if (DateTime.TryParseExact(DateInput, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateValue))
            {
                // Verifica que sea minimo un día antes y maximo 14
                if(DateValue > CurrentDate.Date && CurrentDate.AddDays(14) >= DateValue.Date){
                    // Verifica que no se hagan permisos en sabado o domingo
                    if (DateValue.DayOfWeek != DayOfWeek.Saturday && DateValue.DayOfWeek != DayOfWeek.Sunday )
                    {
                        // Si es válida la fecha termina el ciclo
                        ValideDate = true;
                    } else {
                        WriteLine("The request must be between monday to friday");
                    }
                }else{
                    WriteLine("It must be one day appart of the minimum and 2 weeks maximum. Try again");
                }
            } else{
                WriteLine("The format must be yyyy/mm/dd. Try again.");
            }
        }
        return DateValue;
    }

    public static (DateTime, DateTime) AddTimes(DateTime dateValue)
    {
        // Variables para almacenar los identificadores de las horas de inicio y fin,
        // así como las horas de inicio y fin reales.
        int scheduleIdInit = 0, scheduleIdEnd = 0, take = 9;
        DateTime initTimeValue = DateTime.MinValue, endTimeValue = DateTime.MinValue;

        // Obtener el día de la semana de la fecha proporcionada.
        DayOfWeek day = dateValue.DayOfWeek;

        // Indicador para saber si estamos seleccionando la hora de inicio o fin.
        bool validHours = false;
        bool selectingInit = true;

        // Bucle principal para la selección de horas.
        while (!validHours)
        {
            // Se utiliza el contexto de base de datos con una declaración 'using' para
            // asegurar que se libere correctamente después de su uso.
            using (bd_storage db = new())
            {
                IQueryable<Schedule> schedules = db.Schedules;

                // Obtener todas las horas de inicio o fin disponibles para el día y la fase de selección.
                int offset = GetOffset(day, selectingInit);
                var selectedSchedules = schedules.Skip(offset).Take(take);

                // Mostrar las opciones de horas de inicio/fin disponibles.
                // Le paso el horario del inicio de la clase para que no le muestre horas menores para elegir el final de la clase
                ShowScheduleOptions(selectedSchedules, offset, scheduleIdInit);

                // Leer la entrada del usuario para seleccionar una opción.
                string scheduleIdInput = ReadNonEmptyLine();
                int selectedScheduleId = TryParseStringaEntero(scheduleIdInput) + offset;

                // Validar la opción seleccionada.
                if (IsValidScheduleId(selectedScheduleId, offset, offset + take) && 
                    (selectingInit || selectedScheduleId > scheduleIdInit))
                {
                    // Procesar la opción seleccionada.
                    if (selectingInit)
                    {
                        WriteLine("Class start hour added");
                        scheduleIdInit = selectedScheduleId;
                    }
                    else
                    {
                        WriteLine("Class end hour added");
                        scheduleIdEnd = selectedScheduleId;
                    }

                    // Cambiar a la fase de selección opuesta.
                    selectingInit = !selectingInit;

                    // Si se han seleccionado ambas horas, verificar y calcular las horas de inicio y fin.
                    if (scheduleIdInit > 0 && scheduleIdEnd > 0)
                    {
                        if (scheduleIdInit < scheduleIdEnd)
                        {
                            validHours = true;
                            InitAndEndTimesFromIds(scheduleIdInit, scheduleIdEnd, dateValue, out initTimeValue, out endTimeValue);
                        }
                        else
                        {
                            WriteLine("It can't be first the end of the class. Try again.");
                        }
                    }
                }
                else
                {
                    WriteLine("Invalid option. Try again.");
                }
            }
        }

        // Devolver las horas de inicio y fin seleccionadas.
        return (initTimeValue, endTimeValue);
    }

    private static int GetOffset(DayOfWeek day, bool selectingInit)
    {
        // Método para determinar el desplazamiento (offset) en función del día de la semana y
        // de si estamos seleccionando la hora de inicio o fin.
        int offset = 0;

        switch (day)
        {
            // Los números específicos son asignados para evitar superposiciones y garantizar la coherencia
            // "condición ? expresión_si_verdadero : expresión_si_falso"
            case DayOfWeek.Monday:
                offset = selectingInit ? 0 : 1; 
            break;

            case DayOfWeek.Tuesday:
                offset = selectingInit ? 10 : 11; 
            break;
            
            case DayOfWeek.Wednesday:
                offset = selectingInit ? 20 : 21; 
            break;

            case DayOfWeek.Thursday: 
                offset = selectingInit ? 30 : 31;
            break;

            case DayOfWeek.Friday: 
                offset = selectingInit ? 40 : 41; 
            break;
        }

        return offset;
    }

    private static bool IsValidScheduleId(int selectedScheduleId, int min, int max)
    {
        // Método para validar si el identificador de la hora seleccionada es válido.
        return selectedScheduleId >= min && selectedScheduleId <= max;
    }

    private static void InitAndEndTimesFromIds(int initId, int endId, DateTime dateValue, out DateTime initTime, out DateTime endTime)
    {
        // Método para obtener las horas de inicio y fin reales a partir de los identificadores seleccionados.
        using (bd_storage db = new())
        {
            var initSchedule = db.Schedules.FirstOrDefault(s => s.ScheduleId == initId);
            var endSchedule = db.Schedules.FirstOrDefault(s => s.ScheduleId == endId);

            // Calcular las horas de inicio y fin basadas en las horas de inicio de la base de datos.
            initTime = dateValue.Date + (initSchedule?.InitTime ?? DateTime.MinValue).TimeOfDay;
            endTime = dateValue.Date + (endSchedule?.InitTime ?? DateTime.MinValue).TimeOfDay;

        }
    }

    public static void DeleteRequest(int? RequestID)
    {
        using (bd_storage db = new())
        {
            // Guarda el request donde sea igual al requestId
            var Request = db.Requests
                    .Where(r => r.RequestId == RequestID)
                    .FirstOrDefault();
            // Verifica que no sea nulo
            if (Request != null)
            {
                // Lo elimina de la tabla
                db.Requests.Remove(Request);
                // Guarda los cambios en la bd
                int Affected = db.SaveChanges();
                // Verifica que si se hayan realizado cambios y manda el mensaje
                if (Affected > 0)
                {
                    WriteLine("Request successfully deleted");
                }
                else
                {
                    WriteLine("Request couldn't be deleted");
                }
            }
            else
            {
                WriteLine("Request ID not found in the database");
            }
        }
    }

    public static void DeleteRequestFormat(string UserName)
    {
        do
        {
            // Mostrar una lista de los formatos de solicitud que aún no han sido aceptados
            WriteLine("Here's a list of all the request formats that have not been accepted yet. ");
            ViewRequestFormatNotAcceptedYet(UserName);

            // Solicitar el ID de la solicitud a eliminar
            WriteLine();
            WriteLine("Provide the ID of the request that you want to delete (check the list): ");
            int DetailsId = TryParseStringaEntero(ReadNonEmptyLine());

            using (bd_storage db = new())
            {
                // Obtener el detalle de la solicitud con el ID proporcionado
                var RequestDetail = db.RequestDetails.FirstOrDefault(e => e.RequestDetailsId == DetailsId);

                if (RequestDetail is not null)
                {
                    int? RequestId = RequestDetail.RequestId;

                    // Eliminar todos los RequestDetails con el mismo RequestId
                    var RequestDetailsToDelete = db.RequestDetails.Where(d => d.RequestId == RequestId);
                    db.RequestDetails.RemoveRange(RequestDetailsToDelete);

                    int AffectedDetails = db.SaveChanges();
                    // Mostrar los mensajes dependiendo del valor de Affected
                    if (AffectedDetails > 0)
                    {
                        WriteLine("RequestDetails successfully deleted");

                        // Eliminar la solicitud principal
                        var Request = db.Requests.FirstOrDefault(r => r.RequestId == RequestId);
                        if (Request is not null)
                        {
                            db.Requests.Remove(Request);
                            int AffectedRequest = db.SaveChanges();
                            // Mostrar los mensajes necesarios por el estado de la consulta
                            if (AffectedRequest == 1)
                            {
                                WriteLine("Request successfully deleted");
                            }
                            else
                            {
                                WriteLine("Request couldn't be deleted");
                            }
                        }
                        else
                        {
                            WriteLine("Request not found");
                        }
                    }
                    else
                    {
                        WriteLine("The details of the request couldn't be deleted");
                    }
                }
                else
                {
                    WriteLine("That request ID doesn't exist in the database, try again");
                }
            }

            // Preguntar al usuario si desea eliminar otra solicitud
            WriteLine("Do you want to delete another request? (yes/no): ");
        } while (ReadNonEmptyLine().Trim().ToLower() == "yes");
    }

    public static void UpdateRequestFormat(string UserName){
       
        #region Variables
        int i=1, Affected = 0, Option=0, EquipId = 0;
        bool ValidateRequest=false, ValidateEq = false;;
        DateTime RequestDate=DateTime.Today;
        #endregion

        // Listar permisos no aprobados por el profesor
        WriteLine("Here's a list of all the request format that has not been accepted yet. ");
        ViewRequestFormatNotAcceptedYet(UserName);
        WriteLine();

        // Pedir el ID del permiso a modificar
        WriteLine("Provide the ID of the request that you want to modify (check the list): ");
        int RequestID = Convert.ToInt32(ReadNonEmptyLine());

        using(bd_storage db = new bd_storage()){

            // Consultar si existen los permisos que escogio el usuario y guardar los campos
            IQueryable<Request> RequestsQuery = db.Requests
                .Include(rd => rd.Classroom)
                .Include(rd => rd.Professor)
                .Include(rd => rd.Subject)
                .Include(rd => rd.Student).Where( rd => rd.RequestId==RequestID);
            IQueryable<RequestDetail> RequestDetailsQuery = db.RequestDetails
                .Include(rd => rd.Status)
                .Include(rd=> rd.Equipment)
                .Where(r => r.RequestId==RequestID).Where(r=> r.ProfessorNip==0);

            // Hace listas de equipos y requestDetails
            var RequestList = RequestDetailsQuery.ToList();
            List <Equipment> ListEquipments= new List<Equipment>();
            
            do{
                if (RequestsQuery != null && RequestsQuery.Any() && RequestDetailsQuery != null && RequestDetailsQuery.Any())
                {
                    // Mostrar los campos que se pueden modificar
                    WriteLine("These are the fields you can update:");
                    WriteLine($"1. Classroom: {RequestsQuery.First().Classroom.Name}");
                    WriteLine($"2. Professor: {RequestsQuery.First().Professor.Name} {RequestsQuery.First().Professor.LastNameP}");
                    WriteLine($"3. Subject: {RequestsQuery.First().Subject.Name}");
                    WriteLine($"4. Date of the request: {RequestDetailsQuery.First().RequestedDate.Date}");
                    WriteLine($"5. Dispatch time: {RequestDetailsQuery.First().DispatchTime.TimeOfDay} and Return time: {RequestDetailsQuery.First().ReturnTime.TimeOfDay}");

                    // Mostrar todos los equipos del permiso seleccionado
                    WriteLine($"6. Equipment(s) in the request:");
                    foreach (var requestDetail in RequestDetailsQuery)
                    {
                        WriteLine($"     -{requestDetail.Equipment.EquipmentId} ({requestDetail.Equipment.Name})");
                        // Agregar a lista para poder manipular los datos de forma sencilla
                        ListEquipments.Add(requestDetail.Equipment);
                        i++;
                    }
                    WriteLine("Select an option to modify: ");
                    Option = Convert.ToInt32(ReadNonEmptyLine());
                    ValidateRequest = true;
                }
                else {
                    Clear();
                    WriteLine("Request not found. Try again.");
                    // Se realiza el mismo proceso del inicio si esta mal el ID hasta que se encuentre un valor
                    WriteLine("Here's a list of all the request format that has not been accepted yet. ");
                    ViewRequestFormatNotAcceptedYet(UserName);
                    WriteLine();
                    WriteLine("Provide the ID of the request that you want to modify (check the list): ");
                    RequestID = Convert.ToInt32(ReadNonEmptyLine());

                    RequestsQuery = db.Requests
                    .Include(rd => rd.Classroom)
                    .Include(rd => rd.Professor)
                    .Include(rd => rd.Subject)
                    .Include(rd => rd.Student).Where( rd => rd.RequestId==RequestID);

                    RequestDetailsQuery = db.RequestDetails
                    .Include(rd => rd.Status)
                    .Include(rd=> rd.Equipment)
                    .Where(r => r.RequestId==RequestID).Where(r=> r.ProfessorNip==0);
                    RequestList = RequestDetailsQuery.ToList();
                }

            } while (ValidateRequest==false);

            // De acuerdo a la opcion del campo a modificar
            switch(Option)
            {
                case 1:
                {
                    // Llamar la función para listar los salones y que se seleccione uno
                    int ClassroomId = AddClassroom();
                    // Cambiar la query en el primer valor con el nuevo ID del classroom
                    RequestsQuery.First().ClassroomId=ClassroomId;
                    Affected = db.SaveChanges();
                
                }break; 
                case 2:
                {
                    // Llama la función para buscar al profesor de la clase
                    string ProfessorId = SearchProfessorByName(" ", 0, 0);
                    // Cambiar la query en el primer valor con el nuevo ID del profesor
                    RequestsQuery.First().ProfessorId = ProfessorId;
                    Affected = db.SaveChanges();
                }break;
                case 3:
                {
                    // Llamar la función para buscar la nueva materia
                    string SubjectId = SearchSubjectsByName(" ", 1);
                    // Cambiar la query en el primer valor con el nuevo ID de la materia
                    RequestsQuery.First().SubjectId = SubjectId;
                    Affected = db.SaveChanges();
                }break;
                case 4:
                {
                    // Llamar la función para establecer la nueva fecha del pedido
                    DateTime NewDate = AddDate(DateTime.Today.Date);
                    // Por todos los Request Details de un Request se guarda el nuevo valor de la fecha
                    foreach (var requestDetail in RequestDetailsQuery)
                    {
                        requestDetail.RequestedDate = NewDate;
                    }
                    // Guardar los cambios en la bd
                    Affected = db.SaveChanges();
                }break;
                case 5:
                {
                    // Llamar la función para establecer las horas de la clase de un pedido
                    var Times = AddTimes(RequestDate);
                    // Por todos los Request Details de un Request se guarda los nuevos valores de horas 
                    foreach (var requestDetail in RequestDetailsQuery)
                    {
                        requestDetail.DispatchTime = Times.Item1;
                        requestDetail.ReturnTime = Times.Item2;
                    }
                    Affected = db.SaveChanges();
                } break;
                case 6:
                {
                    i = 1;
                    // Lista los equipos del permiso y permite al usuario seleccionar uno
                    foreach (var e in RequestDetailsQuery)
                    {
                        WriteLine($"{e.RequestDetailsId}. {e.Equipment.Name}");
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
                        RequestDetailsQuery.First().RequestedDate,
                        RequestDetailsQuery.First().DispatchTime,
                        RequestDetailsQuery.First().ReturnTime,
                        RequestDetailsQuery.First().RequestId,
                        1, 
                        true
                    );

                    // Escoger un solo equipo en la tabla Request Details
                    var SingularRequestDetail = db.RequestDetails
                    .Where(rD => rD.RequestDetailsId == EquipId);
                    // Cambiar el equipo en la tabla de la base de datos
                    SingularRequestDetail.First().EquipmentId = UpdatedEquipments.EquipmentsId.First();
                    // Cambiar el estado del equipo conforme al nuevo equipo seleccionado
                    SingularRequestDetail.First().StatusId = UpdatedEquipments.StatusEquipments.First();
                    Affected = db.SaveChanges();
                } break;
                case 7:
                {
                    return;

                }
                default:{
                    WriteLine("Not a valide option. Try again.");
                }break;
            }
            WriteLine(Affected > 0 ? "Request changed" : "Request not changed");
        }
    }
}