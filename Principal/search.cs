using Microsoft.EntityFrameworkCore;
using AutoGens;
using System.Linq;
partial class Program
{
    public static void SearchStudentGeneral()
    {
        using (bd_storage db = new())
        {
            WriteLine("Provide the ID of the student you want to search:");
            string studentId = ReadNonEmptyLine();

            var student = db.Students
            .Include( g => g.Group)
            .SingleOrDefault(s => s.StudentId == studentId);

            if (student == null)
            {
                WriteLine("No student found");
                SubMenuStudentsHistory();
            }

            WriteLine("Student Information: ");
            
            WriteLine($"Name: {student.Name}, Paternal Last Name: {student.LastNameP}, Maternal Last Name: {student.LastNameM}, Group: {student.Group.Name}");

            var requests = db.Requests.Where(r => r.StudentId == student.StudentId).ToList();

            if (requests.Count == 0)
            {
                WriteLine("No history found for the student.");
                SubMenuStudentsusingEquipment();
            }

            List<int> requestIds = requests.Select(r => r.RequestId).ToList();
            
            IQueryable<RequestDetail> RequestDetails = db.RequestDetails
            .Where(rd => requestIds.Contains((int)rd.RequestId))
            .Include(rd => rd.Equipment);

            var groupedRequests = RequestDetails.GroupBy(r => r.RequestId);

            int i = 0;
            WriteLine("");
            WriteLine("Student History: ");

            foreach (var group in groupedRequests)
            {
                i++;
                var firstRequest = group.First();

                WriteLine($"Request {i}: ");
                WriteLine($"Request Detail: {firstRequest.RequestId}");
                WriteLine($"Dispatch Time: {firstRequest.DispatchTime}");
                WriteLine($"Return Time: {firstRequest.ReturnTime}");
                WriteLine($"Requested Date: {firstRequest.RequestedDate}");
                WriteLine($"Current Date: {firstRequest.CurrentDate}");

                WriteLine("Equipment:");
                foreach (var r in group)
                {
                    WriteLine($"Equipment Name: {r.Equipment.Name}");
                }
            }  
        }
    }

    public static void SearchStudentUsingEquipment()
    {
        using (bd_storage db = new())
        {
            WriteLine("Provide the ID of the student you want to search:");
            string studentId = ReadNonEmptyLine();

            var student = db.Students
            .Include( g => g.Group)
            .SingleOrDefault(s => s.StudentId == studentId);

            if (student == null)
            {
                WriteLine("No student found");
                SubMenuStudentsusingEquipment();
            }

            var requests = db.Requests.Where(r => r.StudentId == student.StudentId).ToList();

            if (requests.Count == 0)
            {
                WriteLine("No history found for the student.");
                SubMenuStudentsusingEquipment();
            }

            List<int> requestIds = requests.Select(r => r.RequestId).ToList();
            
            IQueryable<RequestDetail>? RequestDetails = db.RequestDetails
            .Where(rd => requestIds.Contains((int)rd.RequestId) && rd.StatusId == 2)
            .Include(rd => rd.Equipment);

            var groupedRequests = RequestDetails.GroupBy(r => r.RequestId);

            int i = 0;
            WriteLine("");
            WriteLine("Students with Equipments in use: ");

            foreach (var group in groupedRequests)
            {
                i++;
                var firstRequest = group.First();

                WriteLine($"Student {i} Information: ");
                WriteLine("");
                WriteLine($"Name: {student.Name}, Last Name: {student.LastNameP}, Group: {student.Group.Name}");
                WriteLine("Equipment(s):");
                foreach (var r in group)
                {
                    WriteLine($" Equipment Name: {r.Equipment.Name}");
                }

                WriteLine($"Return Time: {firstRequest.ReturnTime.Hour}:{firstRequest.ReturnTime.Minute}");
                WriteLine($"Date: {firstRequest.RequestedDate.Date}");

            } 
        }
    }
}

