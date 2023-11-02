using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public partial class Login
{
    public static SQLiteConnection? dbConnection;
    private static string secretKey = "llave secreta"; // Reemplaza con tu clave secreta

    public Login()
    {
        // Establecemos la conexión a la base de datos SQLite
        dbConnection = new SQLiteConnection("Data Source=bd_storage.sql");
        dbConnection.Open();
    }

    static public void MenuRegistro()
    {
        string tableName;
        string ID;
        while (true)
        {
            WriteLine("Que tipo de usuario eres? ");
            WriteLine("1.Alumno");
            WriteLine("2.Profesor");
            WriteLine("3.Coordinador");
            WriteLine("4.Almacenista");
            WriteLine("5.Salir");
            WriteLine("Selecciona una opcion: ");

            string? opcion = ReadLine();
            bool bdquery = false;
            switch (opcion)
            {
                case "1":
                    bdquery = RegistroStudent();
                    break;

                case "2":
                    tableName = "professor";
                    ID = "professorId";
                    bdquery = RegistroStorerProfCoord(tableName, ID);
                    break;

                case "3":
                    tableName = "coordinator";
                    ID = "coordinatorId";
                    bdquery = RegistroStorerProfCoord(tableName, ID);
                    break;

                case "4":
                    tableName = "storer";
                    ID = "storerId";
                    bdquery = RegistroStorerProfCoord(tableName, ID);
                    break;
                case "5":

                    break;

                default:
                    WriteLine("Opcion no valida. Intentalo de nuevo.");
                    break;
            }
        }
    }

    public static bool RegistroStorerProfCoord(string tableName, string idUser)
    {
        WriteLine("Ingrese su nomina (será su futuro ID): ");
        string username = VerifyReadLengthStringExact(10);

        WriteLine("Ingrese su Nombre: ");
        string firstname = ReadNonEmptyLine();

        WriteLine("Ingrese su Apellido Paterno: ");
        string lastnameP = ReadNonEmptyLine();

        WriteLine("Ingrese su Apellido Materno: ");
        string lastnameM = ReadNonEmptyLine();

        WriteLine("Ingrese su contrasena: ");
        string encryptedPassword = EncryptPass(VerifyReadLengthString(8));

        string query = $"INSERT INTO {tableName} ({idUser}, name, lastNameP, lastNameM, password)" +
        $"VALUES (@{username}, @{firstname}, @{lastnameP}, @{lastnameM}, @{encryptedPassword})";

        using (SQLiteCommand cmd = new SQLiteCommand(query, dbConnection))
        {
            cmd.Parameters.AddWithValue($"@{idUser}", username);
            cmd.Parameters.AddWithValue("@name", firstname);
            cmd.Parameters.AddWithValue("@lastNameP", lastnameP);
            cmd.Parameters.AddWithValue("@lastNameM", lastnameM);
            cmd.Parameters.AddWithValue("@password", encryptedPassword);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }

    public static bool RegistroStudent()
    {
        WriteLine("Ingrese su registro (será su futuro ID): ");
        string username = VerifyReadLengthStringExact(8);

        WriteLine("Ingrese su Nombre: ");
        string firstname = ReadNonEmptyLine();

        WriteLine("Ingrese su Apellido Paterno: ");
        string lastnameP = ReadNonEmptyLine();

        WriteLine("Ingrese su Apellido Materno: ");
        string lastnameM = ReadNonEmptyLine();

        WriteLine("Ingrese su contrasena: ");
        string encryptedPassword = EncryptPass(VerifyReadLengthString(8));

        // string query = $"INSERT INTO Students(studentId, name, lastNameP, lastNameM, password)" +
        // $"VALUES(@{username}, @{firstname}, @{lastnameP}, @{lastnameM}, @{encryptedPassword})";
        
        string query = "INSERT INTO students(studentId, name, lastNameP, lastNameM, password) VALUES (@studentId, @name, @lastNameP, @lastNameM, @password)";

        using (SQLiteCommand cmd = new SQLiteCommand(query, dbConnection))
        {
            cmd.Parameters.AddWithValue($"@studentId", username);
            cmd.Parameters.AddWithValue("@name", firstname);
            cmd.Parameters.AddWithValue("@lastNameP", lastnameP);
            cmd.Parameters.AddWithValue("@lastNameM", lastnameM);
            cmd.Parameters.AddWithValue("@password", encryptedPassword);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }

    public static bool IniciarSesion(out string? rol)
    {
        WriteLine("Ingrese su ID: ");
        string username = ReadLine();

        WriteLine("Ingrese su contrasena: ");
        string encryptedPassword = EncryptPass(ReadLine());

        string[] tables = { "students", "professors", "storers", "coordinators" };

        foreach (string tableName in tables)
        {
            string idUser;

            switch (tableName)
            {
                case "students":
                    idUser = "studentId";
                    break;
                case "professor":
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

            string query = $"SELECT {idUser}, password FROM {tableName} WHERE {idUser} = @{idUser} AND password = @Password";

            using (SQLiteCommand cmd = new SQLiteCommand(query, dbConnection))
            {
                cmd.Parameters.AddWithValue($"@{idUser}", username);
                cmd.Parameters.AddWithValue("@password", encryptedPassword);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        rol = tableName;
                        return true;
                    }
                }
            }
        }
        rol = null;
        return false;
    }

    public static string RecuperarContraseña()
    {
        WriteLine("Ingrese su ID: ");
        string? username = ReadLine();

        string[] tables = { "students", "professors", "storer", "coordinator" };

        foreach (string tableName in tables)
        {
            string idUser;

            switch (tableName)
            {
                case "studentd":
                    idUser = "studentId";
                    break;
                case "professor":
                    idUser = "professorId";
                    break;
                case "Storer":
                    idUser = "storerId";
                    break;
                case "coordinator":
                    idUser = "coordinatorId";
                    break;
                default:
                    idUser = "";
                    break;
            }

            string query = $"SELECT Password FROM {tableName} WHERE {idUser} = @{idUser}";

            using (SQLiteCommand cmd = new SQLiteCommand(query, dbConnection))
            {
                cmd.Parameters.AddWithValue($"@{idUser}", username);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string res = result.ToString();
                    return Decrypt(res);
                }
            }
        }

        return null;
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
    
    public static string Decrypt(string cipherText)
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
}
