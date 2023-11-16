// pongan todas sus funciones cambiadas aqui

//using AutoGens;
using Microsoft.Data.Sqlite;
using LogInUnitTestings;

//new
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using AutoGens;
using Data;
using System.Collections.Generic;
using System.Linq;

public class Functions()
{
    static (int affected, string storerId) ChangeStorerPsw(string username, IAll db)
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

    public static void ApprovePermissions(string? User)
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

                EntityEntry<Group> entity = db.Groups.Add(g);
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
        Equipment repeatedEquipment = db.Equipments.Where(e => e.ControlNumber == controlnumber); // checks the controll numbe does not already exist on any of the other equipments
                                    
        if(repeatedEquipment is null || !repeatedEquipment.Any()) //  checks if the query returned anything, if it did, user must enter another control number that is not on the database
        {
            return false
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
            Iqueryable<Subject> subjects = db.Subjects
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
        using (bd_storage db = new())
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

            foreach (var group in groupedRequests)
            {
                i++;
                string count = i + "";
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
    }







    // fin VAli






}