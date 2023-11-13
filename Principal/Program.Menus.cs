partial class Program
{

    public static string MenuSignUp()
    {
        string op = "";
        bool valid = false;
        //menu de para registrar
        WriteLine("Welcome to the register ");
        WriteLine("What kind of user are you?");
        WriteLine("1.Student");
        WriteLine("2.Professor");
        WriteLine("3.Coordinator");
        WriteLine("4.Storer");
        WriteLine("5.Exit");

        do
        {
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "1" && op != "2" && op != "3" && op != "4" && op != "5")
            {
                WriteLine("Please choose a valid option (1 - 5)");
                op = ReadNonEmptyLine();
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
    }

    public static string MenuCoordinators()
    {
        string op = "";
        bool valid = false;

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

        do
        {
            WriteLine();
            Write("Option : ");
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

    public static string MenuProfessors()
    {
        string op = "";
        bool valid = false;

        WriteLine(" 1. Watch permissions");
        WriteLine(" 2. Approve or Deny Permissions");
        WriteLine(" 3. Request for Material");
        WriteLine(" 4. Edit Request for Material");
        WriteLine(" 5. Delete Request for Material");
        WriteLine(" 6. View All materials of the storage");
        WriteLine(" 7. Exit");

        do
        {
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "1" && op != "2" && op != "3" && op != "4" && op != "5" && op != "6" && op != "7")
            {
                WriteLine("Please choose a valid option (1 - 7)");
                op = ReadNonEmptyLine();
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
    }

    public static string MenuStorer()
    {
        string op = "";
        bool valid = false;

        WriteLine();
        WriteLine("**********************************MENU**********************************");
        WriteLine("Please choose an option, a number between 1 and 14");
        WriteLine(" 1. Add new equipment");
        WriteLine(" 2. Update equipment information");
        WriteLine(" 3. View equipments");
        WriteLine(" 4. Delete equipment");
        WriteLine(" 5. Search for a equipment by equipment ID or equipment Name");
        WriteLine(" 6. View Equipment Requests");
        WriteLine(" 7. View Tomorrows Equipment Requests");
        WriteLine(" 8. View and Search for a Students History");
        WriteLine("         a. See all students");
        WriteLine("         b. Search for a student in specific");
        WriteLine("         c. See students that have lost or damaged an equipment (and haven't made up for it)");
        WriteLine(" 9. View and Search for Students using Equipment at this moment");
        WriteLine("         a. See all students using equipments ");
        WriteLine("         b. Search for a specific student in this list");
        WriteLine("         c. See the list of students that are late for returning equipments");
        WriteLine(" 10. Delivery equipment.");
        WriteLine("         a. To students ");
        WriteLine("         b. To professor ");
        WriteLine(" 11. Register the Return of Equipment(s) of a request");
        WriteLine(" 12. Create report of damaged or lost equipment");
        WriteLine(" 13. Student debt Of LostDamagedEquipment");
        WriteLine(" 14. Program maintenance for a equipment");
        WriteLine("         a. Program a new maintenance");
        WriteLine("         b. Report Finished Maintenance");
        WriteLine(" 15. View Maintenance History");
        WriteLine(" 16. Change password");
        WriteLine(" 17. Sign out");

        do
        {
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "1"  &&  op != "2" &&  op != "3" &&  op != "4" &&  op != "5" &&  op != "6" &&  op != "7" &&  op != "8" &&  op != "9" &&  op != "10" &&  op != "11" &&  op != "12" &&  op != "13" &&  op != "14" && op!="15" && op!="16" && op!="17")
            {
                WriteLine("Please choose a valid option (1 - 17)");
                op = ReadNonEmptyLine(); 
            }
            else valid = true;
        } while (!valid);

        return op;
    }

    public static string SubMenuMantenance()
    {
        string op = "";
        bool valid = false;

        WriteLine("a. Program a new maintenance");
        WriteLine("b. Report Finished Maintenance");
        WriteLine("c. Cancel");

        do
        {
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if(op != "a" && op != "b" && op != "c")
            {
                WriteLine("Please select a valid option a, b or c");
                op = ReadNonEmptyLine(); 
            }
            else valid = true;

        } while (!valid);

        return op;
    }


    public static string SubMenuDeliveryAndReturn()
    {
        string op = "";
        bool valid = false;

        WriteLine("Choose an option: ");
        WriteLine("a. student");
        WriteLine("b. teacher");
        WriteLine("d. Exit");

        do{
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c")
            {
                WriteLine("Please choose a valid option (a, b or c )");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
    }

    public static string SubMenuStudentsusingEquipment()
    {
        string op = "";
        bool valid = false;
        WriteLine("Choose an option: ");
        WriteLine("a. See all students using equipments ");
        WriteLine("b. Search for a specific student in this list");
        WriteLine("c. See the list of students that are late for returning equipments");
        WriteLine("d. Exit");

        do{
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c" &&  op != "d")
            {
                WriteLine("Please choose a valid option (a, b, c or d)");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
    }

    public static string SubMenuStudentsHistory()
    {
        string op = "";
        bool valid = false;
        WriteLine("Choose an option: ");
        WriteLine("a. See all students");
        WriteLine("b. Search for a student in specific");
        WriteLine("c. See students that have lost or damaged an equipment (and haven't made up for it)");
        WriteLine("d. Exit");
        
        do{
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c" &&  op != "d")
            {
                WriteLine("Please choose a valid option (a, b, c or d)");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
    }

    public static string SubMenuSearchEquipment()
    {
        string op = "";
        bool valid = false;
        WriteLine("a. Search for a equipment info by the equipment ID");
        WriteLine("b. Search for a equipments info by the equipment name");
        WriteLine("c. Exit");

        do{
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c")
            {
                WriteLine("Please choose a valid option (a, b or c)");
                op = ReadNonEmptyLine(); 
            }
            else
            {
                valid = true;
            }
        } while (!valid);
        return op;
    }

    public static string MenuStudents()
    { 
        string op = "";
        bool valid = false;
        WriteLine();
        WriteLine("********Menu********");
        WriteLine("1. Fill a request format"); //SI
        WriteLine("2. View equipments"); //SI
        WriteLine("3. View request formats"); //sam SI
        WriteLine("4. Edit request formats that aren't signed yet"); //CARO
        WriteLine("5. Delete request formats that aren't signed yet"); //SAM SI
        WriteLine("6. See a list of equipments that are late for returning"); //SAM ver sus request tardios Si
        WriteLine("7. Sign out");

        do{
            WriteLine();
            Write("Option : ");
            op = ReadNonEmptyLine();
            if (op != "a"  &&  op != "b" &&  op != "c")
            {
                WriteLine("Please choose a valid option (a, b or c)");
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