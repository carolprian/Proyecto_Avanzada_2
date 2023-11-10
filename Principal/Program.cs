// namespace LogInPrincipal;
//using Principal.AutoGens;
using AutoGens;

public partial class Program
{
    public static void Main()
    {
        bd_storage db = new();
        WriteLine($"Provider : {db.Database.ProviderName}"); 
        WriteLine();
        // ApprovePermissions();
        // StorersPrincipal();
        //MenuStudents("20300826");
        //Hi();
        while (true)
        {
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
                    string? rol;
                    var logIn = IniciarSesion(out rol);
                    if (rol!=null)
                    {
                        WriteLine($"Congratulations {rol}, you logged in succesfully!"); 
                        switch(rol)
                        {
                            case "students":
                            StudentsPrincipal(logIn.username);
                            break;

                            case "professors":
                            ProfessorsPrincipal();
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
                    }

                break;
                
                case "2":
                    MenuRegistro();
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