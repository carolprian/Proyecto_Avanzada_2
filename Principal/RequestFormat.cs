partial class Program
{
    public static void RequestFormat()
    {
        WriteLine("Insert the plantel:");
        string plantel = ReadNonEmptyLine();
        WriteLine("Insert today's date:");
        string currentDate = ReadNonEmptyLine();
        WriteLine("Insert your name(s):");
        string nameStudent = ReadNonEmptyLine();
        WriteLine("Insert your patern last name:");
        string lastNameP = ReadNonEmptyLine();
        WriteLine("Insert your matern last name:");
        string? lastNameM = ReadLine();
        WriteLine("Insert your register:");
        string studentId = ReadNonEmptyLine();
        WriteLine("Insert your grade and group:");
        string groupName = ReadNonEmptyLine();
        WriteLine("Insert the date of the class:");
        string date = ReadNonEmptyLine();
        WriteLine("Insert the class start hour :");
        string initHour = ReadNonEmptyLine();
        WriteLine("Insert the class finish hour :");
        string endHour = ReadNonEmptyLine();
        

    }
}