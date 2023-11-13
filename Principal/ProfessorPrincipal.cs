using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

partial class Program
{
    public static void ProfessorsPrincipal(string username)
    {
        while(true){
            WriteLine("Type the option you want to do");
            WriteLine("1.- Watch permissions");
            WriteLine("2.- Approve or Deny Permissions");
            WriteLine("3.- Request for Material");
            WriteLine("4.- Exit");
            string op = VerifyReadLengthStringExact(1);
            switch (op)
            {
                case "1":
                    WatchPermissions(username);
                    break;
                case "2":
                    ApprovePermissions(username);
                    break;
                case "3":
                    PetitionFormat(username);
                    break;
                case "4":
                    return;
                default:
                    WriteLine("Please type a correct option");
                    break;
            }//END OF SWITCH
        }
        
    }

    public static void WatchPermissions(string user)
    {
        int i = 1;
        using (bd_storage db = new bd_storage())
        {
            IQueryable<RequestDetail> requests = db.RequestDetails
                .Include(r => r.Request).ThenInclude(s=>s.Student).ThenInclude(g=>g.Group)
                .Include(e=>e.Equipment).Where(d =>d.Request.ProfessorId == EncryptPass(user));
                WriteLine("| {0,-1} | {1,-15} | {2,-26} | {3,-10} | {4,-3} | {5,-22} | {6,-22} | {7, -15}",
                        "Number of permission | "," Students Name | ","Students Paternal Last Name | ",
                         "Students Maternal Last Name | ", "Students Group | ","Equipments Name | ", "Dispatch Time | ", "Return Time");

                foreach (var element in requests)
                {
                    WriteLine("| {0,-1} | {1,-23} | {2,-12} | {3,-10} | {4,-3} | {5,-41} | {6,-22} | {7, -22}",
                        i, element.Request.Student.Name,element.Request.Student.LastNameP, 
                        element.Request.Student.LastNameM,element.Request.Student.Group.Name, element.Equipment.Name,
                        element.DispatchTime,element.ReturnTime);
                        i++;
                }
        }
    }

    public static void ApprovePermissions(string? user)
    {
        using (bd_storage db = new bd_storage())
        {
            int index; // Índice para realizar un seguimiento de la solicitud actual
            string option = "";
            while (option != "2")
            {
                int i = 1;
                IQueryable<RequestDetail> requests = db.RequestDetails
                .Include(r => r.Request).ThenInclude(s=>s.Student).ThenInclude(g=>g.Group).Include(e=>e.Equipment).Where(d => d.ProfessorNip == 0);
               // WriteLine($"ToQueryString: {requests.ToQueryString()}");
                if (requests == null || !requests.Any())
                {
                    WriteLine("There are no permissions to approve");
                    WriteLine();
                    return;
                }
                //var element = requests.ElementAt(index);
                foreach (var element in requests)
                {
                            WriteLine("| {0,-2} | {1,-15} | {2,-13} | {3,-13} | {4,-3} | {5,-41} | {6,-23} | {7, -23}",
                        i, element.Request.Student.Name,element.Request.Student.LastNameP, 
                        element.Request.Student.LastNameM,element.Request.Student.Group.Name, element.Equipment.Name,
                        element.DispatchTime,element.ReturnTime);
                        i++;
                }
                WriteLine("Type the number of the permission you want to modify");
                string indexop = ReadLine();
                index = TryParseStringaEntero(indexop);
                var requestslist = requests.ToList();
                if(index > requestslist.Count()){
                    WriteLine("Option not valid, you will be redirected to the menu");
                    return;
                }
                else{
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
                    WriteLine("Ingrese su NIP para poder modificar los permisos");
                    string choice = ReadNonEmptyLine();
                    IQueryable<Professor> prof = db.Professors?.Where(r => r.Nip == EncryptPass(choice));
                    if (prof is null || !prof.Any())
                    {
                        WriteLine("Entrada no válida. Se mantendrá sin cambios.");
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
    public static void Hi()
    {
        List<int> valuesToEncrypt = new List<int> { 1111111111 };

        foreach (int value in valuesToEncrypt)
        {
            string stringValue = value.ToString();
            string encryptedValue = EncryptPass(stringValue);

            Console.WriteLine($"Original Value: {value}, Encrypted Value: {encryptedValue}");
        }
    }
    public static void Hi2(){
        using (bd_storage db = new bd_storage()){
            IQueryable<Professor> professors = db.Professors;
            foreach (var item in professors)
            {
                WriteLine(Decrypt(item.ProfessorId));
                WriteLine(Decrypt(item.Password));
                WriteLine(Decrypt(item.Nip));
            }
        }
    }

}