using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

partial class Program
{
    public static void RequestFormat()
    {
        const int maxEquipment = 15;
        int count = 0, scheduleIdInitI, scheduleIdEndI;
        string r = "y";
        bool ban=false;
        string[] equipmentId = new string[maxEquipment];
        string[] quantity = new string[maxEquipment];
        int[] quantitys = new int[maxEquipment];
        WriteLine("Insert the plantel:");
        string plantel = ReadNonEmptyLine();
        WriteLine("Insert today's date:");
        string currentDate = ReadNonEmptyLine();
        WriteLine("Insert your name(s):");
        string nameStudent = ReadNonEmptyLine();
        WriteLine("Insert your patern last name:");
        string lastNameP = ReadNonEmptyLine();
        WriteLine("Insert your matern last name:");
        string? lastNameM = ReadLine();
        WriteLine("Insert your register:");
        string studentId = ReadNonEmptyLine();
        WriteLine("Insert your grade and group:");
        string groupName = ReadNonEmptyLine();
        WriteLine("Insert the date of the class:");
        string date = ReadNonEmptyLine();
        DateTime.TryParse(date, out DateTime dateT);
        DayOfWeek day = dateT.DayOfWeek;
        string dayString = day.ToString();
        while(ban==false){
            WriteLine("Insert the class start hour :");
            string initHour = ReadNonEmptyLine();
            WriteLine("Insert the class finish hour :");
            string endHour = ReadNonEmptyLine();
            DateTime initTime = DateTime.ParseExact(initHour, "HH:mm", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(endHour, "HH:mm", CultureInfo.InvariantCulture);
            using (bd_storage db = new()){
                IQueryable<Schedule> schedulesInit = db.Schedules.Where(g=> g.InitTime == initTime && g.WeekDay==dayString);
                IQueryable<Schedule> schedulesEnd = db.Schedules.Where(g=> g.InitTime == endTime && g.WeekDay==dayString);
                var scheduleIdInit = schedulesInit.FirstOrDefault();
                var scheduleIdEnd = schedulesEnd.FirstOrDefault();
                scheduleIdInitI= scheduleIdInit.ScheduleId;
                scheduleIdEndI= scheduleIdEnd.ScheduleId;
            }
            int difference = scheduleIdEndI - scheduleIdInitI;
            if(difference <= 4 && difference > 1){
                ban=true;
            }
            else{
                WriteLine("The bare minimun of loans are 50 min and the maximun are 3 hours and 50 min");
                WriteLine("Try again with different hour");
            }
        }
        bool isDuplicate = false;

        for (int i = 0; i < maxEquipment && r == "y"; i++)
        {
            WriteLine("Select the number of equipment :");
            string newEquipmentId = ReadNonEmptyLine();

            // Verificar duplicados
            isDuplicate = false;
            for (int j = 0; j < count; j++)
            {
                if (equipmentId[j] == newEquipmentId)
                {
                    isDuplicate = true;
                    break;  // Salir del bucle si se encuentra un duplicado.
                }
            }

            if (isDuplicate)
            {
                WriteLine("Este equipmentId ya ha sido agregado anteriormente.");
            }
            else
            {
                equipmentId[count] = newEquipmentId;

                WriteLine("Insert how many :");
                quantity[count] = ReadNonEmptyLine();
                if (int.TryParse(quantity[count], out quantitys[count]))
                {
                    count++;
                }

                WriteLine("Do you want to add another equipment? y/n");
                r = ReadNonEmptyLine();
            }
        }

        if (count == maxEquipment)
        {
            WriteLine("You are only allowed to request up to 15 equipments");
        }        
        
    }
}
