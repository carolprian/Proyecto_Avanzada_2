using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
partial class Program
{
    public static string MenuCoordinators()
    {
        string op = "";
        WriteLine(" 1. View inventory");
        WriteLine(" 2. Search equipment by serial number");
        WriteLine(" 3. View damaged and lost equipment");
        WriteLine(" 4. Search damaged or lost equipment by serial number");
        WriteLine(" 5. Search damaged or lost equipment by equipment name");
        WriteLine(" 6. Search damaged or lost equipment by date of event");
        WriteLine(" 7. Search damaged or lost equipment by student name");
        WriteLine(" 8. View loan history");
        WriteLine(" 9. Manage groups");
        WriteLine(" 10. Manage professors");
        WriteLine(" 11. Manage storers");
        WriteLine(" 12. Manage students");
        WriteLine(" 13. Manage subjects");
        WriteLine(" 14. Exit");
        bool valid = false;
        do
        {
            op = ReadNonEmptyLine();
            if (op != "1" && op != "2" && op != "3" && op != "4" && op != "5" && op != "6" && op != "7" && op != "8" && op != "9" && op != "10" && op != "11" && op != "12" && op != "13" && op != "14")
            {
                WriteLine("Please choose a valid option (1 - 14)");
                op = ReadNonEmptyLine();
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
    }
}