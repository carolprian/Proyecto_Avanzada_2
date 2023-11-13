using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

partial class Program
{
    public static void SubjectCRUD()
    {
        string op = ""; bool b = false;
        while (b == false)
        {
            WriteLine("1. List all subjects");
            WriteLine("2. Update a subject");
            WriteLine("3. Remove permanently a subject");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);
            if (op == "1" || op == "2" || op == "3" || op == "4") { b = true; }

            if (op == "1")
            {
                ListSubjects();
            }
            if (op == "2")
            {
                WriteLine("Enter the subject id of the subject you want to update:");
                string subjectid = VerifyReadMaxLengthString(13);
                using (bd_storage db = new())
                {
                    IQueryable<Subject> subjects = db.Subjects
                    .Include(s => s.Academy)
                    .Where(s => s.SubjectId.Equals(subjectid));
                    if (subjects is null || !subjects.Any())
                    {
                        WriteLine("That subject id is not registered");
                        b = false;
                    }
                    else
                    {
                        op = "a";
                        while (op != "4")
                        {
                            WriteLine("This are the fields you can update:");
                            WriteLine($"1. Subject id : {subjects.First().SubjectId}");
                            WriteLine($"2. Name: {subjects.First().Name}");
                            WriteLine($"3. Academy: {subjects.First().Academy?.Name}");
                            WriteLine("4. None. Exit the Update of the Subject");
                            WriteLine("Choose the option you want to update:");
                            bool ban = false;
                            op = VerifyReadLengthStringExact(1);
                            switch (op)
                            {
                                case "1":
                                    while (ban == false)
                                    {
                                        Write("Write the new subject id :  ");
                                        string subject = VerifyReadMaxLengthString(13);
                                        IQueryable<Subject> subjectsids = db.Subjects.Where(u => u.SubjectId == subject);
                                        if (subjectsids is null || !subjectsids.Any())
                                        {
                                            subjects.First().SubjectId = subject;
                                            int affected = db.SaveChanges();
                                            ban = true;
                                            if (affected == 1)
                                            {
                                                WriteLine("The id of the subject has been updated!");
                                            }
                                            else
                                            {
                                                WriteLine("Unsuccesful update, sorry");
                                            }
                                        }
                                        else { WriteLine("That subject ID is already in use, try again."); }
                                    }
                                    break;
                                case "2":
                                    while (ban == false)
                                    {
                                        Write("Write the new subject name :  ");
                                        string subject = VerifyReadMaxLengthString(55);
                                        IQueryable<Subject> subjectsids = db.Subjects.Where(u => u.Name == subject);
                                        if (subjectsids is null || !subjectsids.Any())
                                        {
                                            subjects.First().Name = subject;
                                            int affected = db.SaveChanges();
                                            ban = true;
                                            if (affected == 1)
                                            {
                                                WriteLine("The name of the subject has been updated!");
                                            }
                                            else
                                            {
                                                WriteLine("Unsuccesful update, sorry");
                                            }
                                        }
                                        else { WriteLine("That subject Name is already registered, try again."); }
                                    }
                                    break;
                                case "3":
                                    while (ban == false)
                                    {
                                        WriteLine("Search for a academy by it's name: ");
                                        string academy = VerifyReadMaxLengthString(55);
                                        IQueryable<Academy> academies = db.Academies
                                        .Where(a => a.Name != null && a.Name.StartsWith(academy));
                                        if (academies is null || !academies.Any())
                                        {
                                            WriteLine("There are no matching academies registered.");
                                        }
                                        else
                                        {
                                            List<int> idsAca = new List<int>();
                                            foreach (var a in academies)
                                            {
                                                WriteLine($"{a.AcademyId} . {a.Name}");
                                                idsAca.Add(a.AcademyId);
                                            }
                                            int subjectaca = 0;
                                            while (!idsAca.Contains(subjectaca))
                                            {
                                                WriteLine("Choose an academy id :");
                                                subjectaca = TryParseStringaEntero(VerifyReadMaxLengthString(2));
                                            }
                                            subjects.First().AcademyId = subjectaca;
                                            int affected = db.SaveChanges();
                                            ban = true;
                                            if (affected == 1)
                                            {
                                                WriteLine("The academy of the subject has been updated!");
                                            }
                                            else
                                            {
                                                WriteLine("Unsuccesful update, sorry");
                                            }
                                        }
                                    }
                                    break;
                                case "4":
                                    return;
                                default:
                                    WriteLine("Not a viable option");
                                    break;
                            }
                        }


                    }
                }
            }
            else if (op == "3")
            {
                WriteLine("Enter the subject name of the subject you want to delete:");
                string subjectid = VerifyReadMaxLengthString(55);
                using (bd_storage db = new())
                {
                    IQueryable<Subject> subjects = db.Subjects
                    .Include(s => s.Academy)
                    .Where(s => s.Name != null && s.Name.StartsWith(subjectid));
                    if (subjects is null || !subjects.Any())
                    {
                        WriteLine("That subject id is not registered");
                    }
                    else
                    {
                        List<string> idsSubjects = new List<string>();
                        foreach (var s in subjects)
                        {
                            WriteLine($"{s.SubjectId} . {s.Name}");
                            idsSubjects.Add(s.SubjectId);
                        }
                        string subjectaca = "";
                        while (!idsSubjects.Contains(subjectaca))
                        {
                            WriteLine("Write the selected subject id :");
                            subjectaca = VerifyReadMaxLengthString(13);
                        }
                        db.Subjects.Remove(subjects.First());
                        int affected = db.SaveChanges();
                        if (affected == 0)
                        {
                            WriteLine("The subject was not deleted, sorry");
                        }
                        else
                        {
                            WriteLine("The subject was deleted succesfully");
                        }
                    }
                }

            }
        }
    }
}