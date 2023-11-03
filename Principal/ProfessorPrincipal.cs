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
                IQueryable<Request> requests = db.Requests.Include(c=>c.RequestDetails.Where(p=>p.ProfessorNip == null));
                foreach (var request in requests)
                {
                    WriteLine($"Info about the request");
                }
            }
    } 
}