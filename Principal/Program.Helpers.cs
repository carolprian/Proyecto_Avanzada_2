using System.Globalization;
using System.Security.Cryptography;
using System.Text;


partial class Program
{
    public static string VerifyAlphabeticInput()
    {
        string? input;
        do
        {
            input = ReadLine();

            if (!IsAlphabetic(input))
            {
                WriteLine("Invalid input. Please enter alphabetic characters and ensure it's not empty. Try again:");
            }

        } while (!IsAlphabetic(input));

        return input;
    }

    public static string VerifyAlphanumericInput(string input)
    {
        while (string.IsNullOrWhiteSpace(input) || !input.All(char.IsLetterOrDigit))
        {
            WriteLine("Invalid input. Please enter a valid alphanumeric string without spaces or special characters:");
            input = ReadLine();
        }

        return input;
    }




    public static bool IsAlphabetic(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        foreach (char c in input)
        {
            if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
            {
                return false;
            }
        }

        return true;
    }

    public static string VerifyNumericInput()
    {
        string? text;
        do
        {
            text = ReadLine();
            if (string.IsNullOrEmpty(text) || !text.All(char.IsDigit) || text.Length < 10)
            {
                WriteLine("Invalid input. Please enter numeric characters only. Try again:");
            }
        } while (string.IsNullOrEmpty(text) || !text.All(char.IsDigit) || text.Length < 10);
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
    public static string VerifyReadLengthStringExact(int Characters)
    {
        string? text;
        do
        {
            text = ReadLine();
            if (text.Length < Characters || text.Length > Characters)
            {
                WriteLine($"The input must have {Characters} caracteres. Try again:");
            }
        } while (text.Length < Characters || text.Length > Characters);
        return text;
    }

    public static string VerifyReadLengthString(int Characters)
    {
        string? text;
        do
        {
            text = ReadLine();
            if (text.Length < Characters)
            {
                WriteLine($"The input must have minimum {Characters} caracteres. Try again:");
            }
        } while (text.Length < Characters);
        return text;
    }

    public static string VerifyReadMaxLengthString(int Characters)
    {
        string? text;
        do
        {
            text = ReadLine();
            if (text.Length > Characters)
            {
                WriteLine($"The input must have maximum {Characters} caracteres. Try again:");
            }
        } while (text.Length > Characters);
        return text;
    }

    public static string ReadNonEmptyLine()
    {
        string? input = "";
        input = ReadLine();
        while (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(input) || input == "")
        {
            WriteLine(" It cant be empty. ");
            input = ReadLine();
        }
        return input;
    }

    public static int TryParseStringaEntero(string? Op)
    {
        int input;
        while (true) // Infinite loop until there is a return, that there is a valid number
        {
            if (int.TryParse(Op, out input))
            {
                return input;
            }
            else
            {
                WriteLine("That's not a correct form of number. Try again:");
                Op = ReadNonEmptyLine();
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

    public static DateTime ReturnMaintenanceDate(DateTime CurrentDate)
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

    public static string EncryptPass(string PlainText)
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
                        swEncrypt.Write(PlainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string Decrypt(string? CipherText)
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