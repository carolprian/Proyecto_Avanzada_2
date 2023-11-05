using AutoGens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Globalization;
using System.Text;

partial class Program
{
    public static void RequestFormat(string username)
    {
        const int maxEquipment = 4;
        int count = 0, scheduleIdInitI, scheduleIdEndI;
        string r = "y";
        bool ban=false;
        string[] equipmentId = new string[maxEquipment];
        string[] quantity = new string[maxEquipment];
        int[] quantitys = new int[maxEquipment];
        
        do{
            WriteLine("Insert the plantel: ");
            string plantel = ReadNonEmptyLine();
            if(plantel.ToLower()=="colomos"){
                ban=true;
            } else 
            {
                WriteLine("Plantel not registered. Try again");
            }
        }while(ban==false);
        WriteLine("Insert today's date: yyyy/mm/dd ");
        string currentDate = ReadNonEmptyLine();
        using(bd_storage db = new()){
            IQueryable<Student> students = db.Students.Where(s => s.StudentId==username);
            var studentss = students.FirstOrDefault();
            if(students is not null && students.Any()){
                IQueryable<Group> groups = db.Groups.Where(g => g.GroupId == studentss.GroupId);
                var groupss = groups.FirstOrDefault();
                if(groups is not null && groups.Any())
                {
                    if(studentss.LastNameM is not null){
                        WriteLine($"Name: {studentss.Name} {studentss.LastNameP} {studentss.LastNameM}");
                        WriteLine($"Register {studentss.StudentId}");
                        WriteLine($"Group: {groupss.GroupId}");
                    } else{
                        WriteLine($"Name: {studentss.Name} {studentss.LastNameP}");
                        WriteLine($"Register {studentss.StudentId}");
                        WriteLine($"Group: {groupss.GroupId}");
                    }
            }
            }
            
        }
        do
        {
            WriteLine("Check if your credentials are correct:");
        }while(ban==true);
        //listado de materias
        WriteLine("Select the number of the subject: ");
        using (bd_storage db = new()){
            IQueryable<Subject> subjects = db.Subjects;
            var result = subjects.ToList(); // Ejemplo: Obtiene la lista de asignaturas.
            int i=0;
            foreach (var subject in result)
            {
                WriteLine($"{i}. {subject.Name}");
                i++;
            }
        }
        string? subjectStg = ReadLine();
        WriteLine("Insert the name of the teacher: ");
        string? teacherName = ReadLine();
        do{
            WriteLine("Select the number of the classroom: ");
            string classroom = ReadNonEmptyLine();
            using (bd_storage db = new()){
                IQueryable<Classroom> classrooms = db.Classrooms;
                var result = classrooms; 
                int i=0;
                foreach (var cl in result)
                {
                    WriteLine($"{i}. {cl.Clave}");
                    i++;
                }
                IQueryable<Classroom> classroomsId = db.Classrooms.Where(c => c.Clave==classroom);
                if(classroomsId is null || !classroomsId.Any())
                {
                        WriteLine("Not a valid key. Try again");
                } else {
                    ban=false;
                }
            }
        }while(ban==true);
        WriteLine("Insert the date of the class: ");
        string date = ReadNonEmptyLine();
        //agregar verificacion que inserte bien, que no sea sabado o domingo
        DateTime.TryParse(date, out DateTime dateT);
        DayOfWeek day = dateT.DayOfWeek;
        string dayString = day.ToString();
        while(ban==false){
            WriteLine("Insert the class start hour: ");
            string initHour = ReadNonEmptyLine();
            WriteLine("Insert the class finish hour: ");
            string endHour = ReadNonEmptyLine();
            //agregar hora de las 14:30
            DateTime initTime = DateTime.ParseExact(initHour, "HH:mm", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(endHour, "HH:mm", CultureInfo.InvariantCulture);
            using (bd_storage db = new()){
                IQueryable<Schedule> schedulesInit = db.Schedules.Where(g=> g.InitTime == initTime && g.WeekDay==dayString);//12:50
                IQueryable<Schedule> schedulesEnd = db.Schedules.Where(g=> g.InitTime == endTime && g.WeekDay==dayString);//2:30
                var scheduleIdInit = schedulesInit.FirstOrDefault();
                var scheduleIdEnd = schedulesEnd.FirstOrDefault();
                scheduleIdInitI= scheduleIdInit.ScheduleId;
                scheduleIdEndI= scheduleIdEnd.ScheduleId;
            }
            int difference = scheduleIdEndI - scheduleIdInitI;
            if(difference <= 5 && difference >= 1){
                ban=true;
            }
            else{
                WriteLine("The bare minimun of loans are 50 min and the maximun are 3 hours and 20 min");
                WriteLine("Try again with different hour");
            }
        }
        bool isDuplicate = false;
        //Listado de equipos
        for (int i = 0; i < maxEquipment && r == "y"; i++)
        {
            WriteLine("Select the number of equipment: ");
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
            WriteLine("You are only allowed to request up to 4 equipments");
        }  
        /*using (bd_storage db = new()){
            Request request = new(){

            };
        }*/
    }

    public static void AddRequest(){

    }
}
