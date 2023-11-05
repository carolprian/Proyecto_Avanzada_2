partial class Program{
    public static void StudentsPrincipal(string username){
        //menu de acciones
        //1.Llenar permisos__Caro
        //2.Reportes de material dañado
        //3.Listado de materiales
        //4.Listado de permisos
       // MenuStudents(username);
    } 

    public static void MenuStudents(string username){
        WriteLine();
        WriteLine("Welcome Student!");
        WriteLine("********Menu********");
        WriteLine("1. Fill a request format");
        WriteLine("2. Report damage and lost equipment");
        WriteLine("3. View equipments");
        WriteLine("4. View request formats");
        string opString = ReadNonEmptyLine();
        int op = TryParseStringaEntero(opString);
        switch(op){
            case 1:
                RequestFormat(username);
            break;
            case 2:

            break;
            case 3:

            break;
            case 4:

            break;
            default:

            break;
        }
    }
}