using AutoGens;
using Microsoft.EntityFrameworkCore;

partial class Program
{
    public static void SubjectCRUD()
    {
        string op = "";
        bool b = false;

        while (b == false)
        {
            WriteLine("1. List all subjects");
            WriteLine("2. Update a subject");
            WriteLine("3. Remove permanently a subject");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
            {
                b = true;
            }

            switch (op)
            {
                case "1":
                    ListSubjects();
                    break;
                case "2":
                    UpdateSubject();
                    break;
                case "3":
                    RemoveSubject();
                    break;
                case "4":
                    // Exit
                    return;
                default:
                    WriteLine("Invalid option");
                    break;
            }
        }
    }

    private static void UpdateSubject()
    {
        WriteLine("Enter the subject id of the subject you want to update:");
        string subjectId = VerifyReadMaxLengthString(13);

        using (bd_storage db = new())
        {
            IQueryable<Subject> subjects = db.Subjects
                .Include(s => s.Academy)
                .Where(s => s.SubjectId.Equals(subjectId));

            if (subjects is null || !subjects.Any())
            {
                WriteLine("That subject id is not registered");
            }
            else
            {
                UpdateSubjectFields(subjects.First());
            }
        }
    }

    private static void UpdateSubjectFields(Subject subject)
    {
        string op = "a";
        while (op != "4")
        {
            WriteLine("What field do you want to update? (except ID)");
            WriteLine($"1. Subject id : {subject.SubjectId}");
            WriteLine($"2. Name: {subject.Name}");
            WriteLine($"3. Academy: {subject.Academy?.Name}");
            WriteLine("4. None. Exit the Update of the Subject");
            WriteLine("Choose the option you want to update:");

            op = VerifyReadLengthStringExact(1);

            switch (op)
            {
                case "1":
                    WriteLine("Updating Subject ID is not allowed.");
                    break;
                case "2":
                    UpdateSubjectField("Name", value => subject.Name = value);
                    break;
                case "3":
                    UpdateSubjectField("Academy", value =>
                    {
                        int academyId = int.Parse(value); // Assuming the input is a valid integer
                        subject.AcademyId = academyId;
                    });
                    break;
                case "4":
                    return;
                default:
                    WriteLine("Not a viable option");
                    break;
            }

            using (bd_storage db = new())
            {
                int affected = db.SaveChanges();

                if (affected == 1)
                {
                    WriteLine("Subject information updated successfully!");
                }
                else
                {
                    WriteLine("Update was not successful, sorry.");
                }
            }
        }
    }

    private static void UpdateSubjectField(string fieldName, Action<string> updateAction)
    {
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyReadMaxLengthString(30);
        updateAction(newValue);
    }

    private static void RemoveSubject()
    {
        WriteLine("Enter the subject name of the subject you want to delete:");
        string subjectName = VerifyReadMaxLengthString(55);

        using (bd_storage db = new())
        {
            IQueryable<Subject> subjects = db.Subjects
                .Include(s => s.Academy)
                .Where(s => s.Name != null && s.Name.StartsWith(subjectName));

            if (subjects is null || !subjects.Any())
            {
                WriteLine("That subject name is not registered");
            }
            else
            {
                List<string> idsSubjects = new List<string>();
                foreach (var s in subjects)
                {
                    WriteLine($"{s.SubjectId} . {s.Name}");
                    idsSubjects.Add(s.SubjectId);
                }
                string subjectId = "";
                while (!idsSubjects.Contains(subjectId))
                {
                    WriteLine("Write the selected subject id :");
                    subjectId = VerifyReadMaxLengthString(13);
                }

                db.Subjects.Remove(subjects.First());
                int affected = db.SaveChanges();
                if (affected == 0)
                {
                    WriteLine("The subject was not deleted, sorry");
                }
                else
                {
                    WriteLine("The subject was deleted successfully");
                }
            }
        }
    }
}
