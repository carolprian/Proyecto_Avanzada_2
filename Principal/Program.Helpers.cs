using System.Globalization;
using System.Security.Cryptography; // for encryption and decryption
using System.Text;


partial class Program
{
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
        string text;
        do
        {
            text = ReadLine(); // reads a string from the user
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
            text = ReadLine(); // reads a string from the user
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
            text = ReadLine();  // reads a string from the user
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