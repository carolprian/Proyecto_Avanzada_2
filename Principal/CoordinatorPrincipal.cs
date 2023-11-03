partial class Program
{
    public static void CoordinatorsPrincipal()
    {
        string op = MenuCoordinators();
        WriteLine();
        switch (op)
        {
            case "1": // View students list
                ViewStudents();
                break;
            case "2": // View equipment list
                ViewEquipment();
                break;
            case "3": // View damaged and lost equipment list
                ViewDandLEquipment();
                break;
            case "4": // View loan history
                LoanHistory();
                break;
            case "5":  // Add groups
                AddGroups();
                break;
            default:
                break;
        }
        public static int MenuCoordinators()
        {
            string op = "";
            WriteLine(" 1. View all students");
            WriteLine(" 2. View inventory");//buscar con numero de serie
            WriteLine(" 3. View damaged and lost equipment");// tmb buscar con numero de serie o lo q sea
            WriteLine(" 4. Historial de préstamos");
            WriteLine(" 5. Agregar grupos");
            bool valid = false;
        do{
            op = ReadNonEmptyLine();
            if (op != "1"  &&  op != "2" &&  op != "3" &&  op != "4" &&  op != "5")
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
    }
}