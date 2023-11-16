using AutoGens;

public partial class Program
{
    
    public static void Main()
    {
        Clear();
        bd_storage db = new();
        WriteLine($"Provider : {db.Database.ProviderName}"); 
        WriteLine();
        ForegroundColor = ConsoleColor.Gray;     
        
        while (true)
        {
            //menu principal para usar el sistema
            WriteLine("Principal Menu: ");
            WriteLine("1.Log In");
            WriteLine("2.Sign up ");
            WriteLine("3.Password Recovery");
            WriteLine("4. Exit");
            WriteLine("Pick an option: ");

            string? opcion = ReadLine();
            switch (opcion)
            {
                case "1":
                    string? Rol;
                    var logIn = IniciarSesion(out Rol);
                    if (Rol!=null)
                    {
                        WriteLine($"Congratulations {Rol}, you logged in succesfully!"); 
                        switch(Rol)
                        {
                            case "students":
                            StudentsPrincipal(logIn.username);
                            break;

                            case "professors":
                            ProfessorsPrincipal(logIn.username);
                            break;

                            case "storers":
                            StorersPrincipal(logIn.username);
                            break;

                            case "coordinators":
                            CoordinatorsPrincipal();
                            break;
                        }                
                    }
                    else
                    {
                        WriteLine("Log in unsuccesful.");
                        WriteLine("Invalid password or username");
                    }

                break;
                
                case "2":
                    Registro();
                break;

                case "3":
                    RecuperarContraseña();
                break;

                case "4":
                return;

                default:
                    WriteLine("Not a valid option. Try again");
                break;
            }
        }
    }
}