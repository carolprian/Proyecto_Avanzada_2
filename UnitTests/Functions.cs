// pongan todas sus funciones cambiadas aqui

//using AutoGens;
using Microsoft.Data.Sqlite;
using UnitTests;

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

        public static bool RegistroProf(IAll db)
        {
            //pide datos para registro, ReadNonEmptyLine() para que no se dejen campos vacios
            WriteLine("Provide your payroll number, this will be your ID: ");
            string username = EncryptPass(VerifyNumericInput10());

            WriteLine("Provide your name: ");
            string firstname = VerifyAlphabeticInput();

            WriteLine("Provide your paternal last name: ");
            string lastnameP = VerifyAlphabeticInput();

            WriteLine("Provide your maternal last name: ");
            string lastnameM = VerifyAlphabeticInput();

            WriteLine("Create your password: ");
            string encryptedPassword = EncryptPass(VerifyUpperCaseAndNumeric(VerifyReadLengthString(8)));
            
            string nip = "";
            int op = 0;

            while (op == 0)
            {
                //mientras no sea valido lo pide todo el rato
                WriteLine("Create your confirmation NIP for your students requests (4 digits): ");
                nip = EncryptPass(VerifyReadLengthStringExact(4));
                IQueryable<Professor> professors = db.Professors.Where(p => p.Nip == nip);
                if (((professors is null) || !professors.Any()))
                {
                    op = 1;
                }
            }

            // add to the database the register
            int affected = 0;
            Professor p = new()
            {
                ProfessorId = username,
                Name = firstname,
                LastNameP = lastnameP,
                LastNameM = lastnameM,
                Password = encryptedPassword,
                Nip = nip
            };

            EntityEntry<Professor> entity = db.Professors.Add(p);
            WriteLine($"State: {entity.State} ProfessorId: {p.ProfessorId}");
            // SAVE THE CHANGES ON DB
            affected = db.SaveChanges();

            WriteLine();
            bool changes;
            if (affected == 1)
            {
                WriteLine($"Congratulations Professor, you were added succesfully!");
                changes = true;
            }
            else
            {
                WriteLine($"Professor was not registered");
                changes = false;
            }

            return changes;

        }

        public static bool RegistroStorerCoord(string TableName, IAll db)
        {
            //pide datos para registr, ReadNonEmptyLine() poara que se regustren datos
            //es el mismo 
            WriteLine("Provide your payroll number, this will be your ID: ");
            string username = EncryptPass(VerifyNumericInput10());
    
            WriteLine("Provide your name: ");
            string firstname = VerifyAlphabeticInput();

            WriteLine("Provide your paternal last name: ");
            string lastnameP = VerifyAlphabeticInput();

            WriteLine("Provide your maternal last name: ");
            string lastnameM = VerifyAlphabeticInput();

            WriteLine("Create your password: ");
            string encryptedPassword = EncryptPass(VerifyUpperCaseAndNumeric(VerifyReadLengthString(8)));

            // add to the database the register
            int affected = 0;
            
            if (TableName == "Storer")
            {
                //agrega a la bd
                Storer s = new()
                {
                    StorerId = username,
                    Name = firstname,
                    LastNameP = lastnameP,
                    LastNameM = lastnameM,
                    Password = encryptedPassword
                };

                EntityEntry<Storer> entity = db.Storers.Add(s);
                WriteLine($"State: {entity.State} StorerId: {s.StorerId}");
                // SAVE THE CHANGES ON DB
                affected = db.SaveChanges();
            }
            else if (TableName == "Coordinator")
            {
                Coordinator c = new()
                {
                    CoordinatorId = username,
                    Name = firstname,
                    LastNameP = lastnameP,
                    LastNameM = lastnameM,
                    Password = encryptedPassword
                };

                EntityEntry<Coordinator> entity = db.Coordinators.Add(c);
                WriteLine($"State: {entity.State} CoordinatorId: {c.CoordinatorId}");
                // SAVE THE CHANGES ON DB
                affected = db.SaveChanges();
            }
            WriteLine();
            bool changes;
            if (affected == 1)
            {
                WriteLine($"Congratulations {TableName}, you were added succesfully!");
                changes = true;
            }
            else
            {
                WriteLine($"{TableName} was not registered");
                changes = false;
            }
            return changes;
        }

        public static bool RegistroStudent(IAll db)
        {
            WriteLine("Provide your register, this will be your ID: ");
            string username = VerifyNumericInput8();

            WriteLine("Provide your name: ");
            string firstname = VerifyAlphabeticInput();

            WriteLine("Provide your paternal last name: ");
            string lastnameP = VerifyAlphabeticInput();

            WriteLine("Provide your maternal last name: ");
            string lastnameM = VerifyAlphabeticInput();
            int? groupid=0;
            int op=0;
            
            while(op==0)
            {
                WriteLine("Provide your group:");
                string? group = VerifyReadLengthStringExact(3);
                IQueryable<Group> groups = db.Groups.Where(g=> g.Name == group);
                
                if(groups is null || !groups.Any())
                {
                    Group g = new()
                    {
                        Name = group
                    };

                    EntityEntry<Group> entiti = db.Groups.Add(g);
                    int changed = db.SaveChanges();  
                }
                IQueryable<Group>? groupsid = db.Groups.Where(g=> g.Name == group);
                if(groupsid is not null && groupsid.Any())
                {
                    var groupfirst = groupsid.First();
                    groupid = groupfirst.GroupId;
                    op=1; 
                }
            }

            WriteLine("Create your password: ");
            string? encryptedPassword = EncryptPass(VerifyUpperCaseAndNumeric(VerifyReadLengthString(8)));
            int affected = 0;
            bool changes;
            Student s = new()
            {
                StudentId = username,
                Name = firstname,
                LastNameP = lastnameP,
                LastNameM = lastnameM,
                GroupId = groupid,
                Password = encryptedPassword
            };

            EntityEntry<Student> entity = db.Students.Add(s);
            WriteLine($"State: {entity.State} StudentId: {s.StudentId}");
            // SAVE THE CHANGES ON DB
            affected = db.SaveChanges();
            
            WriteLine();
            if (affected == 1)
            {
                WriteLine("Congratulations Student, you were added succesfully!");
                changes = true;

            }
            else
            {
                WriteLine("Your student ID was not registered");
                changes = false;
            }

            return changes;
        }






















        // Vali

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





        // fin VAli

    }
}