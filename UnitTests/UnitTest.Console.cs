using FunctionsName;
using UnitTests;
namespace console
{
    public interface IConsoleInput //interfaces puede definir metodos, propiedades, etc. pero no como una implementacion "real"
    {
        string ReadLine(); //Cualquier clase que implemente esta interface debera si o si usarse en un readline
    }

    public interface IConsoleOutput
    {
        void WriteLine(string message);
    }

    public class ConsoleInput : IConsoleInput
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }

    public class ConsoleOutput : IConsoleOutput
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}