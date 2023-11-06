using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Cryptography;
using AutoGens;
using System.Text;

partial class Program
{
    private const bool V = true;

    static public void MenuRegistro()
    {
        string tableName;
        while (true)
        {
            WriteLine("Welcome to the register ");
            WriteLine("What kind of user are you?");
            WriteLine("1.Student");
            WriteLine("2.Professor");
            WriteLine("3.Coordinator");
            WriteLine("4.Storer");
            WriteLine("5.Exit");
            WriteLine("Pick an option: ");

            string? opcion = ReadLine();
            bool bdquery = false;
            string safecode;
            switch (opcion)
            {
                case "1":
                    bdquery = RegistroStudent();
                    return;

                case "2":
                    WriteLine("Please enter the safecode in order to create a new Professor");
                    safecode = ReadNonEmptyLine();
                    if(safecode == "Brambi12345")
                    {
                        bdquery = RegistroProf();
                    }
                    else
                    {
                        WriteLine("Safecode is not correct, going back to main menu...");
                        WriteLine();
                    }
                    return;

                case "3":
                    tableName = "Coordinator";
                    WriteLine("Please enter the safecode in order to create a new Coordinator");
                    safecode = ReadNonEmptyLine();
                    if(safecode == "Brambi12345")
                    {
                        bdquery = RegistroStorerCoord(tableName);
                    }
                    else
                    {
                        WriteLine("Safecode is not correct, going back to main menu...");
                        WriteLine();
                    }                    
                    return;

                case "4":
                    tableName = "Storer";
                    WriteLine("Please enter the safecode in order to create a new Storer");
                    safecode = ReadNonEmptyLine();
                    if(safecode == "Brambi12345")
                    {
                        bdquery = RegistroStorerCoord(tableName);
                    }
                    else
                    {
                        WriteLine("Safecode is not correct, going back to main menu...");
                        WriteLine();
                    }                    
                    return;
                    
                case "5":
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
        WriteLine("Provide your payroll number, this will be your ID: ");
        string username = EncryptPass(VerifyReadLengthStringExact(10));

        WriteLine("Provide your name: ");
        string firstname = ReadNonEmptyLine();

        WriteLine("Provide your paternal last name: ");
        string lastnameP = ReadNonEmptyLine();

        WriteLine("Provide your maternal last name: ");
        string lastnameM = ReadNonEmptyLine();

        WriteLine("Create your password: ");
        string encryptedPassword = EncryptPass(VerifyReadLengthString(8));
        string nip = "";
        int op = 0;
        while (op == 0)
        {
            WriteLine("Create your confirmation NIP for your students requests (4 digits): ");
            nip = VerifyReadLengthStringExact(4);

            using (bd_storage db = new())
            {
                IQueryable<Professor> professors = db.Professors.Where(p => p.Nip == nip);
                if (((professors is null) || professors.Any()))
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

    public static bool RegistroStorerCoord(string tableName)
    {
        WriteLine("Provide your payroll number, this will be your ID: ");
        string username = EncryptPass(VerifyReadLengthStringExact(10));

        WriteLine("Provide your name: ");
        string firstname = ReadNonEmptyLine();

        WriteLine("Provide your paternal last name: ");
        string lastnameP = ReadNonEmptyLine();

        WriteLine("Provide your maternal last name: ");
        string lastnameM = ReadNonEmptyLine();

        WriteLine("Create your password: ");
        string encryptedPassword = EncryptPass(VerifyReadLengthString(8));

        // add to the database the register
        int affected = 0;

        if (tableName == "Storer")
        {
            using (bd_storage db = new())
            {
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
        else if (tableName == "Coordinator")
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
            WriteLine($"Congratulations {tableName}, you were added succesfully!");
            changes = true;
        }
        else
        {
            WriteLine($"{tableName} was not registered");
            changes = false;
        }
        return changes;
    }
    
    public static bool RegistroStudent()
    {
        WriteLine("Provide your register, this will be your ID: ");
        string username = VerifyReadLengthStringExact(8);

        WriteLine("Provide your name: ");
        string firstname = ReadNonEmptyLine();

        WriteLine("Provide your paternal last name: ");
        string lastnameP = ReadNonEmptyLine();

        WriteLine("Provide your maternal last name: ");
        string lastnameM = ReadNonEmptyLine();
        int groupid=0;
        int op=0;
        
        while(op==0)
        {
            WriteLine("Provide your group:");
            string group = VerifyReadLengthStringExact(3);
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
                IQueryable<Group> groupsid = db.Groups.Where(g=> g.Name == group);
                if(groupsid is not null && groupsid.Any())
                {
                    var groupfirst = groupsid.FirstOrDefault();
                    groupid = groupfirst.GroupId;
                    op=1; 
                }
                        
            }
        }

        WriteLine("Create your password: ");
        string encryptedPassword = EncryptPass(VerifyReadLengthString(8));
        int affected = 0;
        bool changes;
        using (bd_storage db = new())
        {
            Student s = new()
            {
                StudentId = username,
                Name = firstname,
                LastNameP = lastnameM,
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

    public static (bool logged, string username) IniciarSesion(out string? rol)
    {
        using (bd_storage db = new())
        {
            WriteLine("Write your ID: ");
            string? username = ReadNonEmptyLine();

            WriteLine("Write your password: ");
            string pass = EncryptPass(ReadNonEmptyLine());

            string idUser = "studentId";
            while ( true )
            {
                if (idUser == "studentId")
                {
                    IQueryable<Student> students = db.Students
                    .Where(s => s.StudentId == username && s.Password == pass);
                    
                    if (students is null || !students.Any())
                    {
                        idUser = "professorId";
                        rol = null;
                    }
                    else
                    {
                        rol = "students";
                        return (true, username);
                    }

                }
                else if (idUser == "professorId")
                {
                    IQueryable<Professor> professors = db.Professors
                    .Where(p => p.ProfessorId == EncryptPass(username) && p.Password == pass);

                    if (professors is null || !professors.Any())
                    {
                        idUser = "storerId";
                        rol = null;
                    }
                    else
                    {
                        rol = "professors";
                        return (true, username);
                    }
                }
                else if (idUser == "storerId")
                {
                    IQueryable<Storer> storers = db.Storers
                    .Where(s => s.StorerId == EncryptPass(username) && s.Password == pass);

                    if (storers is null || !storers.Any())
                    {
                        idUser = "coordinatorId";
                        rol = null;
                    }
                    else
                    {
                        rol = "storers";
                        return (true, username);
                    }
                }
                else if (idUser == "coordinatorId")
                {
                    IQueryable<Coordinator> coordinators = db.Coordinators
                    .Where(s => s.CoordinatorId == EncryptPass(username) && s.Password == pass);

                    if (coordinators is null || !coordinators.Any())
                    {
                        rol = null;
                        idUser = "none";
                        break;
                    }
                    else
                    {
                        rol = "coordinators";
                        return (true, username);
                    }
                }
            }
        }
        return (false, null);
    }

    public static string EncryptPass(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes("llave secreta".PadRight(32));
            aesAlg.IV = new byte[16];

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string Decrypt(string? cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes("llave secreta".PadRight(32));
            aesAlg.IV = new byte[16];

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
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

    public static void RecuperarContraseña()
    {
        using (bd_storage db = new())
        {
            WriteLine("Write your ID: ");
            string username = ReadNonEmptyLine();

            string[] tables = { "students", "professors", "storers", "coordinators" };

            foreach (string tableName in tables)
            {
                string idUser = "";
                switch (tableName)
                {
                    case "students":
                        idUser = "studentId";
                        break;
                    case "professors":
                        idUser = "professorId";
                        break;
                    case "storers":
                        idUser = "storerId";
                        break;
                    case "coordinators":
                        idUser = "coordinatorId";
                        break;
                    default:
                        idUser = "";
                        break;
                }

                // query to find user id                
                if (idUser == "studentId")
                {
                    // SELECT * FROM [UserTable] WHERE [UserTablePK] = UsernameInput
                    IQueryable<Student> students = db.Students.Where(s => s.StudentId == username);
                    // checks if there are any registers with the desired ID
                    if (students is not null && students.Any())
                    {
                        var firstStudent = students.First();
                        if (firstStudent.StudentId != null)
                        {
                            WriteLine($"StudentID : {firstStudent.StudentId} - Password : {Decrypt(firstStudent.Password)}");
                        }
                        else
                        {
                            WriteLine("Student ID is null");
                        }
                    }
                }
                else if (idUser == "professorId")
                {
                    IQueryable<Professor> professors = db.Professors.Where(p => p.ProfessorId == EncryptPass(username));

                    if (professors is not null || professors.Any())
                    {
                        foreach (var professor in professors)
                        {
                            WriteLine($"ProfessorID : {Decrypt(professor.ProfessorId)} - Password : {Decrypt(professor.Password)}");
                        }
                    }
                }
                else if (idUser == "storerId")
                {
                    IQueryable<Storer> storers = db.Storers.Where(s => s.StorerId == EncryptPass(username));

                    if (storers is not null || storers.Any())
                    {
                        foreach (var storer in storers)
                        {
                            WriteLine($"StorerID : {Decrypt(storer.StorerId)} - Password : {Decrypt(storer.Password)}");
                        }
                    }
                }
                else if (idUser == "coordinatorId")
                {
                    IQueryable<Coordinator> coordinators = db.Coordinators.Where(c => c.CoordinatorId == EncryptPass(username));
                    if (coordinators is not null || coordinators.Any())
                    {
                        foreach (var coordinator in coordinators)
                        {
                            WriteLine($"CoordinatorId : {Decrypt(coordinator.CoordinatorId)} - Password : {Decrypt(coordinator.Password)}");
                        }
                    }
                }
            }
        }
    }
}






// using System;
// using Microsoft.Data.Sqlite;
// using System.IO;
// using System.Security.Cryptography;
// using System.Text;

// public partial class Login
// {
//     public static SqliteConnection? dbConnection;
//     private static string secretKey = "llave secreta"; // Reemplaza con tu clave secreta

//     public Login()
//     {
//         // Establecemos la conexión a la base de datos SQLite
//         dbConnection = new SQLiteConnection("Data Source=storage.sql");
//         dbConnection.Open();
//     }

//     static public void MenuRegistro()
//     {
//         string tableName;
//         string ID;
//         while (true)
//         {
//             WriteLine("Que tipo de usuario eres? ");
//             WriteLine("1.Alumno");
//             WriteLine("2.Profesor");
//             WriteLine("3.Coordinador");
//             WriteLine("4.Almacenista");
//             WriteLine("5.Salir");
//             WriteLine("Selecciona una opcion: ");

//             string? opcion = ReadLine();
//             bool bdquery = false;
//             switch (opcion)
//             {
//                 case "1":
//                     bdquery = RegistroStudent();
//                     break;

//                 case "2":
//                     tableName = "professor";
//                     ID = "professorId";
//                     bdquery = RegistroStorerProfCoord(tableName, ID);
//                     break;

//                 case "3":
//                     tableName = "coordinator";
//                     ID = "coordinatorId";
//                     bdquery = RegistroStorerProfCoord(tableName, ID);
//                     break;

//                 case "4":
//                     tableName = "storer";
//                     ID = "storerId";
//                     bdquery = RegistroStorerProfCoord(tableName, ID);
//                     break;
//                 case "5":

//                     break;

//                 default:
//                     WriteLine("Opcion no valida. Intentalo de nuevo.");
//                     break;
//             }
//         }
//     }

//     public static bool RegistroStorerProfCoord(string tableName, string idUser)
//     {
//         WriteLine("Ingrese su nomina (será su futuro ID): ");
//         string username = VerifyReadLengthStringExact(10);

//         WriteLine("Ingrese su Nombre: ");
//         string firstname = ReadNonEmptyLine();

//         WriteLine("Ingrese su Apellido Paterno: ");
//         string lastnameP = ReadNonEmptyLine();

//         WriteLine("Ingrese su Apellido Materno: ");
//         string lastnameM = ReadNonEmptyLine();

//         WriteLine("Ingrese su contrasena: ");
//         string encryptedPassword = EncryptPass(VerifyReadLengthString(8));

//         string query = $"INSERT INTO {tableName} ({idUser}, name, lastNameP, lastNameM, password)" +
//         $"VALUES (@{username}, @{firstname}, @{lastnameP}, @{lastnameM}, @{encryptedPassword})";

//         using (SQLiteCommand cmd = new SQLiteCommand(query, dbConnection))
//         {
//             cmd.Parameters.AddWithValue($"@{idUser}", username);
//             cmd.Parameters.AddWithValue("@name", firstname);
//             cmd.Parameters.AddWithValue("@lastNameP", lastnameP);
//             cmd.Parameters.AddWithValue("@lastNameM", lastnameM);
//             cmd.Parameters.AddWithValue("@password", encryptedPassword);

//             int rowsAffected = cmd.ExecuteNonQuery();
//             return rowsAffected > 0;
//         }
//     }

//     public static bool RegistroStudent()
//     {
//         WriteLine("Ingrese su registro (será su futuro ID): ");
//         string username = VerifyReadLengthStringExact(8);

//         WriteLine("Ingrese su Nombre: ");
//         string firstname = ReadNonEmptyLine();

//         WriteLine("Ingrese su Apellido Paterno: ");
//         string lastnameP = ReadNonEmptyLine();

//         WriteLine("Ingrese su Apellido Materno: ");
//         string lastnameM = ReadNonEmptyLine();

//         WriteLine("Ingrese su contrasena: ");
//         string encryptedPassword = EncryptPass(VerifyReadLengthString(8));

//         // string query = $"INSERT INTO Students(studentId, name, lastNameP, lastNameM, password)" +
//         // $"VALUES(@{username}, @{firstname}, @{lastnameP}, @{lastnameM}, @{encryptedPassword})";

//         string query = "INSERT INTO students(studentId, name, lastNameP, lastNameM, password) VALUES (@studentId, @name, @lastNameP, @lastNameM, @password)";

//         using (SQLiteCommand cmd = new SQLiteCommand(query, dbConnection))
//         {
//             cmd.Parameters.AddWithValue($"@studentId", username);
//             cmd.Parameters.AddWithValue("@name", firstname);
//             cmd.Parameters.AddWithValue("@lastNameP", lastnameP);
//             cmd.Parameters.AddWithValue("@lastNameM", lastnameM);
//             cmd.Parameters.AddWithValue("@password", encryptedPassword);

//             int rowsAffected = cmd.ExecuteNonQuery();
//             return rowsAffected > 0;
//         }
//     }

//     public static bool IniciarSesion(out string? rol)
//     {
//         WriteLine("Ingrese su ID: ");
//         string username = ReadLine();

//         WriteLine("Ingrese su contrasena: ");
//         string encryptedPassword = EncryptPass(ReadLine());

//         string[] tables = { "students", "professors", "storers", "coordinators" };

//         foreach (string tableName in tables)
//         {
//             string idUser;

//             switch (tableName)
//             {
//                 case "students":
//                     idUser = "studentId";
//                     break;
//                 case "professor":
//                     idUser = "professorId";
//                     break;
//                 case "storers":
//                     idUser = "storerId";
//                     break;
//                 case "coordinators":
//                     idUser = "coordinatorId";
//                     break;
//                 default:
//                     idUser = "";
//                     break;
//             }

//             string query = $"SELECT {idUser}, password FROM {tableName} WHERE {idUser} = @{idUser} AND password = @Password";

//             using (SQLiteCommand cmd = new SQLiteCommand(query, dbConnection))
//             {
//                 cmd.Parameters.AddWithValue($"@{idUser}", username);
//                 cmd.Parameters.AddWithValue("@password", encryptedPassword);

//                 using (SQLiteDataReader reader = cmd.ExecuteReader())
//                 {
//                     if (reader.HasRows)
//                     {
//                         rol = tableName;
//                         return true;
//                     }
//                 }
//             }
//         }
//         rol = null;
//         return false;
//     }

//     public static string RecuperarContraseña()
//     {
//         WriteLine("Ingrese su ID: ");
//         string? username = ReadLine();

//         string[] tables = { "students", "professors", "storer", "coordinator" };

//         foreach (string tableName in tables)
//         {
//             string idUser;

//             switch (tableName)
//             {
//                 case "studentd":
//                     idUser = "studentId";
//                     break;
//                 case "professor":
//                     idUser = "professorId";
//                     break;
//                 case "Storer":
//                     idUser = "storerId";
//                     break;
//                 case "coordinator":
//                     idUser = "coordinatorId";
//                     break;
//                 default:
//                     idUser = "";
//                     break;
//             }

//             string query = $"SELECT Password FROM {tableName} WHERE {idUser} = @{idUser}";

//             using (SQLiteCommand cmd = new SQLiteCommand(query, dbConnection))
//             {
//                 cmd.Parameters.AddWithValue($"@{idUser}", username);

//                 object result = cmd.ExecuteScalar();
//                 if (result != null)
//                 {
//                     string res = result.ToString();
//                     return Decrypt(res);
//                 }
//             }
//         }

//         return null;
//     }

//     public static string EncryptPass(string plainText)
//     {
//         using (Aes aesAlg = Aes.Create())
//         {
//             aesAlg.Key = Encoding.UTF8.GetBytes("llave secreta".PadRight(32));
//             aesAlg.IV = new byte[16];

//             ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

//             using (MemoryStream msEncrypt = new MemoryStream())
//             {
//                 using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
//                 {
//                     using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
//                     {
//                         swEncrypt.Write(plainText);
//                     }
//                 }
//                 return Convert.ToBase64String(msEncrypt.ToArray());
//             }
//         }
//     }

//     public static string Decrypt(string cipherText)
//         {
//             using (Aes aesAlg = Aes.Create())
//             {
//                 aesAlg.Key = Encoding.UTF8.GetBytes("llave secreta".PadRight(32));
//                 aesAlg.IV = new byte[16];

//                 ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

//                 using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
//                 {
//                     using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
//                     {
//                         using (StreamReader srDecrypt = new StreamReader(csDecrypt))
//                         {
//                             return srDecrypt.ReadToEnd();
//                         }
//                     }
//                 }
//             }
//         }
// }
