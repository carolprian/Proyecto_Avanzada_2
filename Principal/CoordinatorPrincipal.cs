using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

partial class Program
{
    public static void CoordinatorsPrincipal()
    {
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
                    ViewAllEquipmentsCoord();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "2":
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
                    ListDandLequipment();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "4":
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
                    ListEquipmentsRequests();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "9":
                    GroupsCRUD();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "10":
                    ProfessorCRUD(); // Listar grupos
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "11":
                    StorerCRUD();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "12":
                    StudentCRUD();
                    WriteLine();
                    WriteLine("Press enter to come back to the menu...");
                    if(ReadKey(intercept: true).Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                    }
                    break;
                case "13":
                    SubjectCRUD();
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