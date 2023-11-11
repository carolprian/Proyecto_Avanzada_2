using System.Data;
using AutoGens;
using Microsoft.EntityFrameworkCore;

partial class Program{
    public static void ProfessorsPrincipal(){
        //Menu
        //1.Aprobar permisos
        //2.Listado de permisos pendientes
        WriteLine("Ingrese la opcion que desa hacer:");
        WriteLine("1.- Aprobar permisos");
        WriteLine("2.- Ver Permisos Pendientes de aprobar");
        WriteLine("3.- Sign out");
        string opString = ReadNonEmptyLine();
        int op = TryParseStringaEntero(opString);
        switch(op){
            case 1:
            break;

            case 2:

            break;

            case 3:
            return;
        }//END OF SWITCH
    }
    public static void ApprovePermissions(){
        using (bd_storage db = new())
            {
                //IQueryable<Category>? categories = db.Categories?
            //.Include(c => c.Products.Where(p => p.Stock >= stock));
                IQueryable<Request> requests = db.Requests?.Include(rd=>rd.RequestDetails.Where(nip=>nip.ProfessorNip==null));
                WriteLine($"ToQueryString: {requests.ToQueryString()}");

                if(requests is null || !requests.Any()){
                    WriteLine("There are not Permissions to aprove Teacher");
                    WriteLine();
                    return;
                }
                foreach (var request in requests)
                {
                    WriteLine($"PERMISSION HERE");
                }
            }
    }

    public static void Hi()
    {
        List<int> valuesToEncrypt = new List<int> { 1234567890, 1010101010, 0987654321, 1231231231 };

        foreach (int value in valuesToEncrypt)
        {
            string stringValue = value.ToString();
            string encryptedValue = EncryptPass(stringValue);

            Console.WriteLine($"Original Value: {value}, Encrypted Value: {encryptedValue}");
        }
    }


}