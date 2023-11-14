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
            WriteLine();
            
            switch (op)
            {
                case "1":
                //ver todos los equipos en el inventario
                    ViewAllEquipmentsCoord();
                    BackToMenu();
                    break;
                case "2":
                //buscar equipo por su Id
                    WriteLine("Provide the number: ");
                    // Vamos a verificar que solo se esten ingresando caracteres alfanumericos y sin espacios
                    // Verificamos que sean excactamente 6 caracteres
                    string? searchTerm = VerifyAlphanumericInput(VerifyReadLengthStringExact(6));
                    SearchEquipmentsById(searchTerm);
                    BackToMenu();
                    break;
                case "3":
                //ver la lista de materiales dañados y perdidos
                    ListDandLequipment();
                    BackToMenu();
                    break;
                case "4":
                //buscar en la lista de dañados o perdidos equipos por su iD
                    WriteLine("Provide the equipment ID to search:");
                    string? equipmentIdToFind = ReadLine();
                    FindDandLequipmentById(equipmentIdToFind);
                    BackToMenu();
                    break;
                case "5":
                //buscar en la lista de dañados o perdidos equipos por su nombre
                    WriteLine("Provide the equipment name to search:");
                    string? equipmentNameToFind = ReadLine();
                    FindDandLequipmentByName(equipmentNameToFind);
                    BackToMenu();
                    break;
                case "6":
                //buscar en la lista de dañados o perdidos equipos por su fecha
                    WriteLine("Provide the date (yyyy) to search:");
                    string? dateToFind = ReadLine();
                    FindDandLequipmentByDate(dateToFind);
                    BackToMenu();
                    break;
                case "7":
                //buscar en la lista de dañados o perdidos equipos por el estudiante que lo daÑO
                    WriteLine("Provide the student name to search:");
                    string? studentNameToFind = ReadLine();
                    FindDandLequipmentByStudentName(studentNameToFind);
                    BackToMenu();
                    break;
                case "8":
                //ver la lista de request
                    ListEquipmentsRequests();
                    BackToMenu();
                    break;
                case "9": // añade, ve, modifica grupos
                    GroupsCRUD();
                    BackToMenu();
                    break;
                case "10": // ver, actualizar y borrar un profesor
                    ProfessorCRUD(); 
                    BackToMenu();
                    break;
                case "11": // ver, actualizar y borrar un storer
                    StorerCRUD();
                    BackToMenu();
                    break;
                case "12": // ver, actualizar y borrar un estudiante
                    StudentCRUD();
                    BackToMenu();
                    break;
                case "13": // ver, actualizar y borrar una materia
                    SubjectCRUD();
                    BackToMenu();
                    break;
                case "14": // Salir de la cuenta
                    return;
                default:
                    WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}