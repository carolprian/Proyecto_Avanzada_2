using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

partial class Program
{
    public static void CoordinatorsPrincipal()
    {
        //menu del coordinador
        bool exitRequested = false;
        WriteLine("Welcome Coordinator!");

        while (!exitRequested)
        {
            string op = MenuCoordinators();
            Console.Clear();
            WriteLine();
            switch (op)
            {
                case "1":
                //ver todos los equipos en el inventario
                    ViewAllEquipmentsCoord();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "2":
                //buscar equipo por su Id
                    WriteLine("Provide the number: ");
                    string? searchTerm = ReadLine();
                    SearchEquipmentsById(searchTerm);
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "3":
                //ver la lista de materiales dañados y perdidos
                    ListDandLequipment();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "4":
                //buscar en la lista de dañados o perdidos equipos por su iD
                    WriteLine("Provide the equipment ID to search:");
                    string? equipmentIdToFind = ReadLine();
                    FindDandLequipmentById(equipmentIdToFind);
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "5":
                //buscar en la lista de dañados o perdidos equipos por su nombre
                    WriteLine("Provide the equipment name to search:");
                    string? equipmentNameToFind = ReadLine();
                    FindDandLequipmentByName(equipmentNameToFind);
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "6":
                //buscar en la lista de dañados o perdidos equipos por su fecha
                    WriteLine("Provide the date (yyyy) to search:");
                    string? dateToFind = ReadLine();
                    FindDandLequipmentByDate(dateToFind);
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "7":
                //buscar en la lista de dañados o perdidos equipos por el estudiante que lo daÑO
                    WriteLine("Provide the student name to search:");
                    string? studentNameToFind = ReadLine();
                    FindDandLequipmentByStudentName(studentNameToFind);
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "8":
                //ver la lista de request
                    ListEquipmentsRequests();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "9":
                //añade, ve, modifica grupos
                    GroupsCRUD();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "10":
                    ProfessorCRUD(); // Hace que que pueda hacer el crud en un perfil de profesor
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "11":
                    StorerCRUD(); // Hace que que pueda hacer el crud en un perfil de storer
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "12":
                    StudentCRUD(); // Hace que que pueda hacer el crud en un perfil de estudiantes
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "13":
                    SubjectCRUD(); // Hace que que pueda hacer el crud en un perfil de profesor
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "14":
                    exitRequested = true;
                    Console.Clear();
                    break;
                default:
                    WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}