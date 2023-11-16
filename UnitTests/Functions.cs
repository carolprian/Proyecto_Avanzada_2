// pongan todas sus funciones cambiadas aqui

//using AutoGens;
using Microsoft.Data.Sqlite;
using UnitTests;
using console;

//new
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using AutoGens;
using Data;
using ConsoleTables;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Globalization;
using System.Security.Cryptography; // for encryption and decryption
using System.Text;

namespace FunctionsName
{
    partial class Functions
    {
        public static (int affected, string storerId) ChangeStorerPsw(string username, IAll db)
        {
            int incorrect = 0;
            // checks if it exists
            IQueryable<Storer> storers = db.Storers
            .Where(s => s.StorerId == EncryptPass(username)); // finds the storer that logged in
                                    
            if(storers is null || !storers.Any()) // checks if the query returned any results
            {
                WriteLine("Storer not found");
            } 
            else
            {
                string oldPassword = "";
                do
                {
                    WriteLine();
                    WriteLine("Please enter your old password"); 
                    oldPassword = ReadNonEmptyLine();
                    if(EncryptPass(oldPassword) != storers.First().Password) // validates old password before changing to a new one
                    {
                        incorrect++;
                        if(incorrect >= 3) return (0,"0"); // if user has entered old password 3 times in a row, return to storer main menu
                        WriteLine($"Incorrect Password, please try again ({incorrect})");
                    }
                    else
                    {
                        string newPassword = "";
                        WriteLine("Please enter your new password");
                        newPassword = EncryptPass(VerifyReadLengthString(8)); // reads a new password and verifies it is at least 8 characters long
                        storers.First().Password = newPassword; // saves the new password on the database field
                        int affected = db.SaveChanges(); // saves changes on database
                        return(affected, storers.First().StorerId); 
                    }
                } while (true);   
            }
            return (0,"0");
        }


        public static string? AddStorerForUt(IAll db){
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

        public static int ListClassroomsForUt(IAll db)
        {
            // Indice de la lista
            int i = 1;
                // verifica que exista la tabla de Classroom
                if( db.Classrooms is null)
                {
                    throw new InvalidOperationException("The table does not exist.");
                } 
                else 
                {
                    // Muestra toda la lista de classrooms con un indice y la clave de este
                    IQueryable<Classroom> Classrooms = db.Classrooms;
                    
                    foreach (var cl in Classrooms)
                    {
                        WriteLine($"{i}. {cl.Clave}");
                        i++;
                    }
                    return Classrooms.Count();
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
    

        public static void ApprovePermissions(string? User, IAll db)
        {
            //funcion para aprobar o denegar permisos pendientes 
            
                int index; // Índice para realizar un seguimiento de la solicitud actual
                string option = "";
                while (option != "2")
                {
                    int i = 1;
                    IQueryable<RequestDetail> requests = db.RequestDetails
                    .Include(r => r.Request).ThenInclude(s=>s.Student).ThenInclude(g=>g.Group).Include(e=>e.Equipment).Where(d => d.ProfessorNip == 0)
                    .Where(d =>d.Request.ProfessorId == EncryptPass(User));

                    if (requests == null || !requests.Any())
                    {
                        WriteLine("There are no permissions to approve");
                        WriteLine();
                        return;
                    }

                    var table = new ConsoleTable("NO. ", "Student Name", 
                    "Last Name P","Last Name M", "Group", 
                    "Equipment Name","Current Date", "Dispatch Time", "Return Time");


                    foreach (var element in requests)
                    {
                        table.AddRow(i, element.Request.Student.Name, element.Request.Student.LastNameP, 
                            element.Request.Student.LastNameM, element.Request.Student.Group.Name, element.Equipment.Name, $"{element.RequestedDate.Day}/{element.RequestedDate.Month}/{element.RequestedDate.Year}",
                            element.DispatchTime.TimeOfDay, element.ReturnTime.TimeOfDay);

                        i++;
                    }
                    Clear();
                    table.Write();
                    WriteLine();
                    
                    WriteLine("Type the number of the permission you want to modify");

                    string indexop = ReadLine();
                    index = TryParseStringaEntero(indexop);
                    var requestslist = requests.ToList();

                    if (index > requestslist.Count())
                    {
                        WriteLine("Option not valid, you will be redirected to the menu");
                        return;
                    }
                    else
                    {
                        var permission = requestslist[index - 1];
                        string status;

                        do
                        {
                            WriteLine("Type the option you want to do with the permission");
                            WriteLine("1.- Approve it");
                            WriteLine("2 .- Deny it");
                            status = VerifyReadLengthStringExact(1);

                        } while (status != "1" && status != "2");

                        if (permission.ProfessorNip == 0)
                        {
                            WriteLine("Enter your PIN to be able to modify permissions: ");
                            string choice = ReadNonEmptyLine();

                            IQueryable<Professor> prof = db.Professors
                            .Where(r => r.Nip == EncryptPass(choice));

                            if (prof is null || !prof.Any())
                            {
                                WriteLine("Unvalid entry. It will remain unchanged.");
                                return;
                            }
                            else
                            {
                                permission.ProfessorNip = Convert.ToInt32(status);
                            }
                        }

                        // Guardar los cambios en la base de datos
                        db.SaveChanges();
                        WriteLine($"The permission has been changed.");
                        WriteLine("Press 2 to exit, press any other to continue");
                        option = VerifyReadLengthStringExact(1);
                    }
                }
        }
        public static bool ListStorers(IAll db)
        {
            bool aux = true;
                IQueryable<Storer> storers = db.Storers;

                if ((storers is null) || !storers.Any())
                {
                    WriteLine("There are no storers");
                    aux = false;
                    return aux;
                }

                WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", "Id", "Name", "Last Name P", "Last Name M", "Password");

                foreach (var storer in storers)
                {
                    WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", storer.StorerId, storer.Name, storer.LastNameP, storer.LastNameM, Decrypt(storer.Password));
                }
            return aux;
        }
        public static bool WatchPermissions(string User, IAll db)
        {
            bool aux = true;
            int i = 1;
            IQueryable<RequestDetail> requests = db.RequestDetails
                .Include(r => r.Request).ThenInclude(s=>s.Student).ThenInclude(g=>g.Group)
                .Include(e=>e.Equipment).Where(d =>d.Request.ProfessorId == EncryptPass(User));
                
            if (requests == null || !requests.Any())
                {
                    WriteLine("There are no permissions");
                    WriteLine();
                    aux = false;
                    return aux;
                }
                var table = new ConsoleTable("NO. ", "Student Name", 
                "Last Name P","Last Name M", "Group", 
                "Equipment Name","Current Date", "Dispatch Time", "Return Time");

                foreach (var element in requests)
                {
                    table.AddRow(i, element.Request.Student.Name, element.Request.Student.LastNameP, 
                        element.Request.Student.LastNameM, element.Request.Student.Group.Name, element.Equipment.Name, $"{element.RequestedDate.Day}/{element.RequestedDate.Month}/{element.RequestedDate.Year}",
                        element.DispatchTime.TimeOfDay, element.ReturnTime.TimeOfDay);

                    i++;
                }
                Clear();
                table.Write();
                WriteLine();
            return aux;
        }

















        // Vali
    public static(int Affected, List<string> ListEquipmentsId) UpdateRequestEquipmentsStatus( string RequestId, IAll db)
    {
        int Affected = -1;
        List<string> EquipmentsIds = new List<string>();
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
        return (Affected, EquipmentsIds);
    }

    public static bool StudentsLateReturn(IAll db)
    {
        bool aux = false;
            var currentDate = DateTime.Now;

            IQueryable<RequestDetail> requestDetails = db.RequestDetails
            .Include(r => r.Request.Student.Group)
            .Include(r => r.Equipment)
            .Where(s => s.StatusId == 2 && s.ProfessorNip == 1 && s.RequestedDate < currentDate);

            if (!requestDetails.Any())
            {
                WriteLine("No students found with overdue equipment.");
                //SubMenuStudentsUsingEquipment();
                aux = true;
                return aux;
            }

            var table = new ConsoleTable("Id", "Name", "LastName P", "Group", "Equipment", "Return Time", "Return Date");
            int i = 0;
            foreach (var use in requestDetails)
            {
                    i++;

                    table.AddRow(use.Request.Student.StudentId, use.Request.Student.Name, use.Request.Student.LastNameP, 
                    use.Request.Student.Group.Name, use.Equipment.Name, $"{use.ReturnTime.Hour}:{use.ReturnTime.Minute}", use.RequestedDate);
            }

            table.Write();
            WriteLine();
        return aux;
    }

    public static int ListAreas(IAll db)
    {
        IQueryable<Area> areas = db.Areas;
            if ((areas is null) || !areas.Any())
            {
                WriteLine("There are no areas found");
                return 0;
            }
            // Use the data
            foreach (var area in areas)
            {
                WriteLine($"{area.AreaId} . {area.Name} ");
            }
            return areas.Count();
    }
    public static int UpdateRequestFormatStatus(string RequestId, IAll db)
    {
        int affected = -1;
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
        
        return affected;
    }

        public static bool VerifyControlNumberExistence(string controlnumber, IAll db)
        {
            IQueryable<Equipment> repeatedEquipment = db.Equipments.Where(e => e.ControlNumber == controlnumber); // checks the controll numbe does not already exist on any of the other equipments
                                        
            if(repeatedEquipment is null || !repeatedEquipment.Any()) //  checks if the query returned anything, if it did, user must enter another control number that is not on the database
            {
                return false;
            }
            else
            {
                WriteLine("That control number is already in use, try again.");
                return true;
            }                                    
        } 

        public static bool ListSubjects(IAll db)
        {
            bool aux = false;
                IQueryable<Subject> subjects = db.Subjects
                    .Include(su => su.Academy);
                if ((subjects is null) || !subjects.Any())
                {
                    WriteLine("There are no subjects");
                    aux = true;
                    return aux;
                }

                WriteLine("| {0,-13} | {1,-55} | {2,-7} |", "Id", "Name", "Academy");

                foreach (var subject in subjects)
                {
                    WriteLine("| {0:0000000000000} | {1,-55} | {2,-7} |", subject.SubjectId, subject.Name, subject.Academy?.Name);
                }
            
            return aux;
        }



    public static string? ListEquipmentsRequests(IAll db)
    {
            IQueryable<RequestDetail>? requestDetails = db.RequestDetails
                .Include( e => e.Equipment)
                .Include( s => s.Status)
                .Include(e=> e.Request.Student)
                .Where( r => r.ProfessorNip == 1)
                .OrderByDescending( f  => f.RequestedDate);

                if ((requestDetails is null) || !requestDetails.Any())
                {
                    WriteLine("There are no request found");
                    return null;
                }

                var groupedRequests = requestDetails.GroupBy(r => r.RequestId);

                int i = 0;
                string count = "";

                foreach (var group in groupedRequests)
                {
                    i++;
                    count = i + "";
                    string nip = "";
                    var firstRequest = group.First();
                    if(firstRequest.ProfessorNip == 1)
                    {
                        nip = "aceptado";
                    }
                    else if (firstRequest.ProfessorNip == 0 )
                    {
                        nip = "Pendiente";
                    }

                    var table = new ConsoleTable("NO. Request", count);

                    table.AddRow("RequestId", firstRequest.RequestId);
                    table.AddRow("Student", $"{firstRequest.Request?.Student?.Name} {firstRequest.Request?.Student?.LastNameP} {firstRequest.Request?.Student?.LastNameM}" );
                    table.AddRow("StatusId", $"{firstRequest.Status?.Value}");
                    table.AddRow("ProfessorNip", nip);
                    table.AddRow("DispatchTime", $"{firstRequest.DispatchTime.TimeOfDay}");
                    table.AddRow("Return Time", $"{firstRequest.ReturnTime.TimeOfDay}");
                    table.AddRow("RequestedDate", $"{firstRequest.RequestedDate.Day}/{firstRequest.RequestedDate.Month}/{firstRequest.RequestedDate.Year}");
                    table.AddRow("", "");
                    foreach (var r in group)
                    {
                        // Adding an empty string as the first column to match the table structure
                        table.AddRow("Equipment Name", r.Equipment.Name);
                    }

                    table.Write();
                    WriteLine();
                }
                return count;
        }

    public static bool SearchEquipmentsById(IConsoleInput consoleInput, IConsoleOutput consoleOutput, IAll db, string? searchTerm)
    {
        // Variable to indicate if the equipment is found
        bool aux = false;

        // Check if the search term is null or empty, and throw an exception if so
        if (string.IsNullOrEmpty(searchTerm))
        {
            WriteLine("Search term cannot be null or empty.");
            throw new InvalidOperationException("Search term cannot be null or empty.");
        }

        // Use Entity Framework to query the database for equipments by ID
        IQueryable<Equipment>? equipments = db.Equipments
            .Include(e => e.Area)
            .Include(e => e.Status)
            .Include(e => e.Coordinator)
            .Where(e => e.EquipmentId.StartsWith(searchTerm));

        // Check if no equipment is found with the given ID
        if (!equipments.Any())
        {
            WriteLine("No equipment found matching the search term: " + searchTerm);
            aux = true;
            return aux;
        }
        else
        {
            // Display table header
            WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                "EquipmentId", "Equipment Name", "Year", "Status");
            WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");

            // Display details of the equipments with the given ID
            foreach (var e in equipments)
            {
                WriteLine("| {0,-15} | {1,-80} | {2,7} | {3,-22} |",
                    e.EquipmentId, e.Name, e.Year, e.Status?.Value);
            }

            // Ask the user if they want to see more information about any of the equipments
            WriteLine("Do you want to see more information about any of the equipments? (y/n)");
            string read = consoleInput.ReadLine();

            // If the user wants more information, prompt for the equipment ID
            if (read == "y" || read == "Y")
            {
                consoleOutput.WriteLine("Provide the equipment ID you want to see more info:");
                read = consoleInput.ReadLine();
            }
        }

        // Return the flag indicating if the equipment is found
        return aux;
    
    }




    public static int MenuSignUp(IConsoleInput consoleInput, IConsoleOutput consoleOutput)
    {
        int option = 0;
        bool valid = false;

        consoleOutput.WriteLine("Welcome to the registration");
        consoleOutput.WriteLine("What kind of user are you?");
        consoleOutput.WriteLine("1. Student");
        consoleOutput.WriteLine("2. Professor");
        consoleOutput.WriteLine("3. Coordinator");
        consoleOutput.WriteLine("4. Storer");
        consoleOutput.WriteLine("5. Exit");

        do
        {
            consoleOutput.WriteLine("Option: ");
            string input = consoleInput.ReadLine();

            if (int.TryParse(input, out option) && option >= 1 && option <= 5)
            {
                valid = true;
            }
            else
            {
                consoleOutput.WriteLine("Please choose a valid option (1 - 5)");
            }

        } while (!valid);

        return option;
    }

          
    public static string VerifyAlphabeticInput() // checks all characters on a string entered by user are alphabetic (letters)
    {
        string? input;
        do
        {
            input = ReadNonEmptyLine(); // reads line from user

            if (!IsAlphabetic(input)) // checks if the string is alphabetic
            {
                WriteLine("Invalid input. Please enter alphabetic characters and ensure it's not empty. Try again:");
            }

        } while (!IsAlphabetic(input));

        return input;
    }

    public static string VerifyAlphanumericInput(string input) // verifies a string is alphanumeric and does not contain special characters
    {
        while (string.IsNullOrWhiteSpace(input) || !input.All(char.IsLetterOrDigit)) // checks it is not an empty line and that it is only letters and digits
        {
            WriteLine("Invalid input. Please enter a valid alphanumeric string without spaces or special characters:");
            input = ReadLine(); // reads line again if necessary
        }

        return input;
    }

    public static bool IsAlphabetic(string? input) // checks if the string is alphabetic
    {
        if (string.IsNullOrEmpty(input)) // checks it isnt an empty line
        {
            return false;
        }

        foreach (char c in input) // verifies if each char is a letter of the alphabet
        {
            if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
            {
                return false;
            }
        }

        return true;
    }

    public static string VerifyNumericInput() // checks if a string is only numbers
    {
        string? text;
        do
        {
            text = ReadNonEmptyLine();  // reads line in an infinite loop until the string is only numbers
            if (string.IsNullOrEmpty(text) || !text.All(char.IsDigit) || (text.Length < 8 || text.Length > 10)) // verifies string is not empty an it is inlly numbers
            {
                WriteLine("Invalid input. Please enter numeric characters only, between 8 and 10 digits. Try again:");
            }
        } while (string.IsNullOrEmpty(text) || !text.All(char.IsDigit) || (text.Length < 8 || text.Length > 10));
        return text;
    }

    public static string VerifyAllNumbers() // checks if a string is only numbers
    {
        string? text;
        do
        {
            text = ReadNonEmptyLine();  // reads line in an infinite loop until the string is only numbers
            if (string.IsNullOrEmpty(text) || !text.All(char.IsDigit)) // verifies string is not empty an it is inlly numbers
            {
                WriteLine("Invalid input. Please enter numeric characters only. Try again:");
            }
        } while (string.IsNullOrEmpty(text) || !text.All(char.IsDigit));
        return text;
    }

    public static string VerifyNumericInput10()
    {
        string? text;
        do
        {
            text = ReadLine();
            if (string.IsNullOrEmpty(text) || !text.All(char.IsDigit) || text.Length < 10 || text.Length > 10 )
            {
                WriteLine("Invalid input. Please enter numeric characters only and 10 of them. Try again:");
            }
        } while (string.IsNullOrEmpty(text) || !text.All(char.IsDigit) || text.Length < 10 || text.Length > 10);
        return text;
    }

    public static string VerifyNumericInput8()
    {
        string? text;
        do
        {
            text = ReadLine();
            if (string.IsNullOrEmpty(text) || !text.All(char.IsDigit) || text.Length < 8 || text.Length > 8)
            {
                WriteLine("Invalid input. Please enter numeric characters only and 8 of them. Try again:");
            }
        } while (string.IsNullOrEmpty(text) || !text.All(char.IsDigit) || text.Length < 8 || text.Length > 8);
        return text;
    }

    public static string VerifyUpperCaseAndNumeric(string text)
    {
        while (true)
        {
            // Verifica si el texto contiene al menos una letra mayúscula
            bool hasUpperCase = text.Any(char.IsUpper);

            // Verifica si el texto contiene al menos un carácter numérico
            bool hasNumeric = text.Any(char.IsDigit);

            // Si cumple con los requisitos, devuelve el texto
            if (hasUpperCase && hasNumeric)
            {
                return text;
            }
            else
            {
                WriteLine("El texto no cumple con los requisitos. Debe tener al menos una letra mayúscula y al menos un carácter numérico.");
                WriteLine("Try again:");
                text = ReadLine();
            }
        }
    }

    public static string VerifyReadLengthStringExact(int Characters) // verifies a string entered by user has x amount of characters
    {
        string text = "";
        do
        {
            text = ReadNonEmptyLine(); // reads a string from the user
            if (text.Length < Characters || text.Length > Characters || string.IsNullOrEmpty(text)) // checks if the string has the specified amount of characters and verifies it isnt null or an empty string
            {
                WriteLine($"The input must have {Characters} characters. Try again:");
            }
        } while (text.Length < Characters || text.Length > Characters); // infinte loop until conditions are met
        return text;
    }

    public static string VerifyReadLengthString(int Characters) // verifies a string entered by the user has at least x amount of characters
    {
        string? text;
        do
        {
            text = ReadNonEmptyLine(); // reads a string from the user
            if (text.Length < Characters) //  checks if the string has at least the sepcified amount of characters
            {
                WriteLine($"The input must have minimum {Characters} caracteres. Try again:");
            }
        } while (text.Length < Characters); // infinte loop until conditions are met
        return text;
    }

    public static string VerifyReadMaxLengthString(int Characters) // verifies a string entered by the user does not exceed x amount of characters
    {
        string? text;
        do
        {
            text = ReadNonEmptyLine();  // reads a string from the user
            if (text.Length > Characters || string.IsNullOrEmpty(text)) // checks the string is not emptyy or null and that it does not exceed the specified amount of characters
            {
                WriteLine($"The input must have maximum {Characters} caracteres. Try again:");
            }
        } while (text.Length > Characters); // infinte loop until conditions are met
        return text;
    }

    public static string ReadNonEmptyLine() // reads a line from the user until the user enters a line with something on it, not empty, not a whitespace nor null
    {
        string? input = "";
        input = ReadLine(); // reads a line from the user
        while (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(input) || input == "") // checks user didnt enter a null, a whitespace nor an empty line
        {
            WriteLine(" It cant be empty. ");
            input = ReadLine(); // reads again until conditions are met
        }
        return input;
    }

    public static int TryParseStringaEntero(string? Op) // tries converting a string to an integer
    {
        int input;
        while (true) // Infinite loop until there is a return, that there is a valid number
        {
            if (int.TryParse(Op, out input)) // tries converting the string, if it can, it return the integer value
            {
                return input;
            }
            else // otherwise it reads it again
            {
                WriteLine("That's not a correct form of number. Try again:");
                Op = ReadNonEmptyLine();
            }
        }
    }

    public static string ReadPassword() // reads password from user and turns every character entered by them to an asterisk (*)
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = ReadKey(true);

            if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
            {
                password += key.KeyChar;
                Write("*"); // Muestra un asterisco en lugar del carácter
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                // Si se presiona Backspace y hay caracteres en la contraseña, elimina el último carácter
                password = password.Substring(0, password.Length - 1);
                Write("\b \b"); // Borra el último asterisco mostrado
            }
        } while (key.Key != ConsoleKey.Enter);

        WriteLine(); // Agrega una nueva línea después de ingresar la contraseña
        return password;
    }

    public static DateTime ProgrammedMaintenanceDate() // reads a string from the user a tries converting it to a datetime value
    {
        DateTime dateValue = new();
        bool valideDate = false;
        while (!valideDate)
        {
            string dateInput = ReadNonEmptyLine(); // reads a valid line from the user
            if (
                DateTime.TryParseExact( // tries converting the line to DateTime
                    dateInput,
                    "yyyy/MM/dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateValue
                )
            )
            {
                valideDate = true;
            }
            else // re-reads if it couldn't convert it
            {
                WriteLine("The format must be yyyy/mm/dd. Try again.");
            }
        }
        return dateValue;
    }

    public static DateTime ReturnMaintenanceDate(DateTime CurrentDate) // reads a string from the user a tries converting it to a datetime value and checks it is a date after the programmed date
    {
        DateTime dateValue = new();
        bool valideDate = false;
        while (!valideDate)
        {
            string dateInput = ReadNonEmptyLine(); // reads a valid line from the user
            if (
                DateTime.TryParseExact( // tries converting the line to DateTime
                    dateInput,
                    "yyyy/MM/dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateValue
                )
            )
            {
                if (dateValue > CurrentDate.Date)
                {
                    if (
                        dateValue.DayOfWeek != DayOfWeek.Saturday
                        && dateValue.DayOfWeek != DayOfWeek.Sunday
                    )
                    {
                        valideDate = true;
                    }
                }
                else // re-reads if it wasn't a date after the specified date
                {
                    WriteLine("It must be after the programmed date. Try again");
                }
            }
            else // re-reads if it couldn't convert it
            {
                WriteLine("The format must be yyyy/mm/dd. Try again.");
            }
        }
        return dateValue;
    }

    // Función para limpiar la consola y esperar la pulsación de Enter
    public static void BackToMenu()
    {
        WriteLine();
        WriteLine("Press enter to come back to the menu...");
        if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
        {
            Clear();
        }
    }

    public static string EncryptPass(string PlainText) // encrypts with a specified key a string
    {
        using (Aes aesAlg = Aes.Create())
        {

            aesAlg.Key = Encoding.UTF8.GetBytes("llave secreta".PadRight(32));//32 caracteres hexadecimales
            //cada byte esta representado por 2 hex, cada hex son 4 bytes
            //con esta cantidad de bytes se tienen 32 digitos (0-9 y A-F)
            //32 caracteres hex * 4 bytes que recordemos que cada hex son 4 bytes=128
            aesAlg.IV = new byte[16]; 

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(PlainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string Decrypt(string? CipherText) // decrypts previously encrypted text with the key it was encrypted with
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes("llave secreta".PadRight(32));
            aesAlg.IV = new byte[16];

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(CipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    }

    }



        // fin VAli

    
