using Microsoft.EntityFrameworkCore.ChangeTracking;
using AutoGens;

partial class Program 
{
    static public void Registro()
    {
        string tableName;
        while (true)
        {
            int opcion = MenuSignUp();
            bool bdquery = false;
            string safecode;
            switch (opcion)
            {
                case 1:
                    bdquery = RegistroStudent();
                    return;

                case 2:
                //pide safecode para que se pueda registrar un profesor, un storer y un coordinator
                    WriteLine("Please enter the safecode in order to create a new Professor");
                    safecode = ReadPassword();
                    if(safecode == "Brambi12345")
                    {
                        //si es correcto deja registrarlo
                        bdquery = RegistroProf();
                    }
                    else
                    {
                        WriteLine("Safecode is not correct, going back to main menu...");
                        WriteLine();
                    }
                    return;

                case 3:
                //pide safecode para que se peuda registrar un profesor, un storer y un coordinator
                    tableName = "Coordinator";
                    WriteLine("Please enter the safecode in order to create a new Coordinator");
                    safecode = ReadPassword();
                    if(safecode == "Brambi12345")
                    {
                        //si es correcto deja registrarlo
                        bdquery = RegistroStorerCoord(tableName);
                    }
                    else
                    {
                        WriteLine("Safecode is not correct, going back to main menu...");
                        WriteLine();
                    }                    
                    return;

                case 4:
                //pide safecode para que se peuda registrar un profesor, un storer y un coordinator
                    tableName = "Storer";
                    WriteLine("Please enter the safecode in order to create a new Storer");
                    safecode = ReadPassword();
                    if(safecode == "Brambi12345")
                    {
                        //si es correcto deja registrarlo
                        bdquery = RegistroStorerCoord(tableName);
                    }
                    else
                    {
                        WriteLine("Safecode is not correct, going back to main menu...");
                        WriteLine();
                    }                    
                    return;
                    
                case 5:
                    return;

                default:
                    WriteLine("Not a valid option. Try again");
                    WriteLine();
                    break;
            }
        }
    }

    public static bool RegistroProf()
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

            using (bd_storage db = new())
            {
                IQueryable<Professor> professors = db.Professors.Where(p => p.Nip == nip);
                if (((professors is null) || !professors.Any()))
                {
                    op = 1;
                }
            }
        }

        // add to the database the register
        int affected = 0;
        using (bd_storage db = new())
        {
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
        }
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

    public static bool RegistroStorerCoord(string TableName)
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
            using (bd_storage db = new())
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
        }
        else if (TableName == "Coordinator")
        {
            using (bd_storage db = new())
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
    
    public static bool RegistroStudent()
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
            using(bd_storage db = new())
            {
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
        }

        WriteLine("Create your password: ");
        string? encryptedPassword = EncryptPass(VerifyUpperCaseAndNumeric(VerifyReadLengthString(8)));
        int affected = 0;
        bool changes;
        using (bd_storage db = new())
        {
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
        }
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
}