// namespace LogInPrincipal;
//using Principal.AutoGens;
using AutoGens;

public partial class Program
{
    public static void Main()
    {
        bd_storage db = new();
        WriteLine($"Provider : {db.Database.ProviderName}"); 
        
        while (true)
        {
            WriteLine("Menu principal: ");
            WriteLine("1.Iniciar sesion");
            WriteLine("2.Registrarme ");
            WriteLine("3.Recuperar Contraseña");
            WriteLine("4. Salir");
            WriteLine("Selecciona una opcion: ");

            string? opcion = ReadLine();
            switch (opcion)
            {
                case "1":
                    string? rol;
                    if (IniciarSesion(out rol))
                    {
                        WriteLine($"Congratulation {rol}, you logged in succesfully!");                 
                    }
                    else
                    {
                        WriteLine("Inicio de sesión fallido.");
                    }
                    switch(rol)
                    {
                        case "students":
                        StudentsPrincipal();
                        break;

                        case "prfoessors":
                        ProfessorsPrincipal();
                        break;

                        case "storers":
                        StorersPrincipal();
                        break;

                        case "coordinators":
                        CoordinatorsPrincipal();
                        break;
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
                    WriteLine("Opcion no valida. Intentalo de nuevo.");
                break;
            }
        }
    }
}