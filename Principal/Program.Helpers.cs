using System.Globalization;
using System.Security.Cryptography;
using System.Text;


partial class Program
{
    public static string VerifyReadLengthStringExact(int characters)
    {
        string? text;
        do
        {
            text = ReadLine();
            if (text.Length < characters || text.Length > characters)
            {
                WriteLine($"The input must have {characters} caracteres. Try again:");
            }
        } while (text.Length < characters || text.Length > characters);
        return text;
    }

    public static string VerifyReadLengthString(int characters)
    {
        string? text;
        do
        {
            text = ReadLine();
            if (text.Length < characters)
            {
                WriteLine($"The input must have minimum {characters} caracteres. Try again:");
            }
        } while (text.Length < characters);
        return text;
    }

    public static string VerifyReadMaxLengthString(int characters)
    {
        string? text;
        do
        {
            text = ReadLine();
            if (text.Length > characters)
            {
                WriteLine($"The input must have maximum {characters} caracteres. Try again:");
            }
        } while (text.Length > characters);
        return text;
    }

    public static string ReadNonEmptyLine()
    {
        string? input = "";
        while (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(input) || input == "")
        {
            input = ReadLine();
        }
        return input;
    }

    public static int TryParseStringaEntero(string? op)
    {
        int input;
        while (true) // Infinite loop until there is a return, that there is a valid number
        {
            if (int.TryParse(op, out input))
            {
                return input;
            }
            else
            {
                WriteLine("That's not a correct form of number. Try again:");
                op = ReadNonEmptyLine();
            }
        }
    }

    public static string ReadPassword()
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

    public static DateTime ProgrammedMaintenanceDate()
    {
        DateTime dateValue = new();
        bool valideDate = false;
        while (!valideDate)
        {
            string dateInput = ReadNonEmptyLine();
            if (
                DateTime.TryParseExact(
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
            else
            {
                WriteLine("The format must be yyyy/mm/dd. Try again.");
            }
        }
        return dateValue;
    }

    public static DateTime ReturnMaintenanceDate(DateTime currentDate)
    {
        DateTime dateValue = new();
        bool valideDate = false;
        while (!valideDate)
        {
            string dateInput = ReadNonEmptyLine();
            if (
                DateTime.TryParseExact(
                    dateInput,
                    "yyyy/MM/dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateValue
                )
            )
            {
                if (dateValue > currentDate.Date)
                {
                    if (
                        dateValue.DayOfWeek != DayOfWeek.Saturday
                        && dateValue.DayOfWeek != DayOfWeek.Sunday
                    )
                    {
                        valideDate = true;
                    }
                }
                else
                {
                    WriteLine("It must be after the programmed date. Try again");
                }
            }
            else
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
}