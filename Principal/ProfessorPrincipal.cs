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
        //hacer todo lo de pedir materiales de profesores
        string opString = ReadNonEmptyLine();
        int op = TryParseStringaEntero(opString);
        switch(op){
            case 1:
            break;

            case 2:

            break;
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
}