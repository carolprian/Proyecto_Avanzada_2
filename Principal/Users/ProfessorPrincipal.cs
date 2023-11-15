using AutoGens;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;

partial class Program
{
    public static void ProfessorsPrincipal(string username)
    {
        bool exitRequested = false;
        WriteLine("Welcome Professor!");

        while (!exitRequested)
        {
            int op = MenuProfessors();
            Console.Clear();
            WriteLine();
            switch (op)
            {
                case 1:
                    WatchPermissions(username);
                    break;
                case 2:
                    ApprovePermissions(username);
                    break;
                case 3:
                    PetitionFormat(username);
                    break;
                case 4:
                    UpdatePetitionFormat(username);
                    break;
                case 5:
                    DeletePetitionFormat(username);
                    break;
                case 6:
                    ViewAllEquipments();
                    break;
                case 7:
                    UpdateProfessorFields(username);
                    break;
                case 8:
                    exitRequested = true;
                    break;
                default:
                    WriteLine("Please type a correct option");
                    break;
            }

            if (!exitRequested)
            {
                WriteLine();
                WriteLine("Press enter to come back to the menu...");
                if (ReadKey(intercept: true).Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                }
            }
        }
    }

    public static void ApprovePermissions(string? user)
    {
        //funcion para aprobar o denegar permisos pendientes 
        using (bd_storage db = new bd_storage())
        {
            int index; // Índice para realizar un seguimiento de la solicitud actual
            string option = "";
            while (option != "2")
            {
                int i = 1;
                IQueryable<RequestDetail> requests = db.RequestDetails
                .Include(r => r.Request).ThenInclude(s=>s.Student).ThenInclude(g=>g.Group).Include(e=>e.Equipment).Where(d => d.ProfessorNip == 0)
                .Where(d =>d.Request.ProfessorId == EncryptPass(user));

                if (requests == null || !requests.Any())
                {
                    WriteLine("There are no permissions to approve");
                    WriteLine();
                    return;
                }

                var table = new ConsoleTable("NO. ", "Student Name", 
                "Last Name P","Last Name M", "Group", 
                "Equipment Name","Current Date", "Dispatch Time", "Return Time");


                foreach (var element in requests)
                {
                    table.AddRow(i, element.Request.Student.Name, element.Request.Student.LastNameP, 
                        element.Request.Student.LastNameM, element.Request.Student.Group.Name, element.Equipment.Name, $"{element.RequestedDate.Day}/{element.RequestedDate.Month}/{element.RequestedDate.Year}",
                        element.DispatchTime.TimeOfDay, element.ReturnTime.TimeOfDay);

                    i++;
                }
                Clear();
                table.Write();
                WriteLine();
                
                WriteLine("Type the number of the permission you want to modify");

                string indexop = ReadLine();
                index = TryParseStringaEntero(indexop);
                var requestslist = requests.ToList();

                if (index > requestslist.Count())
                {
                    WriteLine("Option not valid, you will be redirected to the menu");
                    return;
                }
                else
                {
                    var permission = requestslist[index - 1];
                    string status;

                    do
                    {
                        WriteLine("Type the option you want to do with the permission");
                        WriteLine("1.- Approve it");
                        WriteLine("2 .- Deny it");
                        status = VerifyReadLengthStringExact(1);

                    } while (status != "1" && status != "2");

                    if (permission.ProfessorNip == 0)
                    {
                        WriteLine("Enter your PIN to be able to modify permissions: ");
                        string choice = ReadNonEmptyLine();

                        IQueryable<Professor> prof = db.Professors
                        .Where(r => r.Nip == EncryptPass(choice));

                        if (prof is null || !prof.Any())
                        {
                            WriteLine("Unvalid entry. It will remain unchanged.");
                            return;
                        }
                        else
                        {
                            permission.ProfessorNip = Convert.ToInt32(status);
                        }
                    }

                    // Guardar los cambios en la base de datos
                    db.SaveChanges();
                    WriteLine($"The permission has been changed.");
                    WriteLine("Press 2 to exit, press any other to continue");
                    option = VerifyReadLengthStringExact(1);
                }
            }
        }
    }
}