namespace LogInPrincipal;
public class LogInPrincipal
{
    static void Main(string[] args)
    {
        bd_storage db = new();
        WriteLine($"Provider : {db.Database.ProviderName}");

        Login Login = new Login();

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
                    string rol;
                    if (Login.IniciarSesion(out rol))
                    {
                        WriteLine("Inició sesión como " + rol);
                    }
                    else
                    {
                        WriteLine("Inicio de sesión fallido.");
                    }
                break;
                
                case "2":
                    Login.MenuRegistro();
                break;

                case "3":
                    Login.RecuperarContraseña();
                break;

                case "4":{
                return;
                } break;
                default:
                    WriteLine("Opcion no valida. Intentalo de nuevo.");
                break;
            }
        }
    }
}