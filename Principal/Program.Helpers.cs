partial class Program
{
    public static string VerifyReadLengthStringExact(int characters){
        string? text;
        do
        {
            text = ReadLine();
            if (text.Length < characters || text.Length > characters)
            {
                WriteLine($"The input must have {characters} caracteres. Try again:");
            }

        } while (text.Length < characters);
        return text;
    }

    public static string VerifyReadLengthString(int characters){
        string? text;
        do
        {
            text = ReadLine();
            if (text.Length < characters)
            {
                WriteLine($"The input must have {characters} caracteres. Try again:");
            }

        } while (text.Length < characters);
        return text;
    }
   
    public static string ReadNonEmptyLine()
    {
        string? input= "";
        while(string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(input) || input == "" ){
        input = ReadLine();
        }
        return input;
    }

    public static int TryParseStringaEntero(string op)
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
            }
        }
    }

}
