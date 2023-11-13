using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

partial class Program
{
    public static void CoordinatorsPrincipal()
    {
        bool exitRequested = false;
        WriteLine("Welcome Coordinator!");

        while (!exitRequested)
        {
            string op = MenuCoordinators();
            Console.Clear();
            WriteLine();
            switch (op)
            {
                case "1":
                    ViewAllEquipmentsCoord();
                    break;
                case "2":
                    WriteLine("Provide the number: ");
                    string searchTerm = ReadLine();
                    SearchEquipmentsById(searchTerm);
                    break;
                case "3":
                    ListDandLequipment();
                    break;
                case "4":
                    WriteLine("Provide the equipment ID to search:");
                    string equipmentIdToFind = ReadLine();
                    FindDandLequipmentById(equipmentIdToFind);
                    break;
                case "5":
                    WriteLine("Provide the equipment name to search:");
                    string equipmentNameToFind = ReadLine();
                    FindDandLequipmentByName(equipmentNameToFind);
                    break;
                case "6":
                    WriteLine("Provide the date (yyyy) to search:");
                    string dateToFind = ReadLine();
                    FindDandLequipmentByDate(dateToFind);
                    break;
                case "7":
                    WriteLine("Provide the student name to search:");
                    string studentNameToFind = ReadLine();
                    FindDandLequipmentByStudentName(studentNameToFind);
                    break;
                case "8":
                    ListEquipmentsRequests();
                    break;
                case "9":
                    GroupsCRUD();
                    break;
                case "10":
                    ProfessorCRUD(); // Listar grupos
                    break;
                case "11":
                    StorerCRUD();
                    break;
                case "12":
                    StudentCRUD();
                    break;
                case "13":
                    SubjectCRUD();
                    break;
                case "14":
                    exitRequested = true;
                    Console.Clear();
                    break;
                default:
                    WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    public static string MenuCoordinators()
    {
        string op = "";
        WriteLine(" 1. View inventory");
        WriteLine(" 2. Search equipment by serial number");
        WriteLine(" 3. View damaged and lost equipment");
        WriteLine(" 4. Search damaged or lost equipment by serial number");
        WriteLine(" 5. Search damaged or lost equipment by equipment name");
        WriteLine(" 6. Search damaged or lost equipment by date of event");
        WriteLine(" 7. Search damaged or lost equipment by student name");
        WriteLine(" 8. View loan history");
        WriteLine(" 9. Manage groups");
        WriteLine(" 10. Manage professors");
        WriteLine(" 11. Manage storers");
        WriteLine(" 12. Manage students");
        WriteLine(" 13. Manage subjects");
        WriteLine(" 14. Exit");
        bool valid = false;
        do
        {
            op = ReadNonEmptyLine();
            if (op != "1" && op != "2" && op != "3" && op != "4" && op != "5" && op != "6" && op != "7" && op != "8" && op != "9" && op != "10" && op != "11" && op != "12" && op != "13" && op != "14")
            {
                WriteLine("Please choose a valid option (1 - 14)");
                op = ReadNonEmptyLine();
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
    }

    public static void GroupsCRUD()
    {
        string op = "";
        bool b = false;

        while (!b)
        {
            WriteLine("1. List all groups");
            WriteLine("2. Update a group");
            WriteLine("3. Remove a group");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
            {
                b = true;
            }

            if (op == "1")
            {
                ListGroups();
            }
            else if (op == "2")
            {
                WriteLine("Enter the ID of the group you want to update:");
                string groupIdString = ReadLine();

                int groupId = TryParseStringaEntero(groupIdString);

                using (bd_storage db = new())
                {
                    var groups = db.Groups
                        .Where(g => g.GroupId == groupId)
                        .ToList();

                    if (groups.Count == 0)
                    {
                        WriteLine("Group with that ID is not registered");
                    }
                    else
                    {
                        op = "a";
                        while (op != "3")
                        {
                            WriteLine("These are the fields you can update:");
                            WriteLine($"1. Group ID: {groups.First().GroupId}");
                            WriteLine($"2. Name: {groups.First().Name}");
                            WriteLine($"3. None. Exit the Update of the Group");
                            WriteLine("Choose the option you want to update:");

                            op = VerifyReadLengthStringExact(1);

                            switch (op)
                            {
                                case "1":
                                    WriteLine("Updating Group ID is not allowed.");
                                    break;
                                case "2":
                                    Write("Write the new name: ");
                                    string newName = VerifyReadMaxLengthString(3);
                                    groups.First().Name = newName;
                                    break;
                                case "3":
                                    return;
                                default:
                                    WriteLine("Not a viable option");
                                    break;
                            }

                            int affected = db.SaveChanges();

                            if (affected == 1)
                            {
                                WriteLine("Group information updated successfully!");
                            }
                            else
                            {
                                WriteLine("Update was not successful, sorry.");
                            }
                        }
                    }
                }
            }
            else if (op == "3")
            {
                WriteLine("Enter the ID of the group you want to remove:");
                string groupIdString = ReadLine();

                int groupId = TryParseStringaEntero(groupIdString);

                using (bd_storage db = new())
                {
                    var group = db.Groups
                        .FirstOrDefault(g => g.GroupId == groupId);

                    if (group == null)
                    {
                        WriteLine("Group with that ID is not registered");
                    }
                    else
                    {
                        db.Groups.Remove(group);
                        int affected = db.SaveChanges();
                        if (affected == 0)
                        {
                            WriteLine("The group was not removed, sorry");
                        }
                        else
                        {
                            WriteLine("The group was removed successfully");
                        }
                    }
                }
            }
        }
    }

    public static void ListGroups()
    {
        using (bd_storage db = new())
        {
            IQueryable<Group> groups = db.Groups;

            if ((groups is null) || !groups.Any())
            {
                WriteLine("There are no groups");
                return;
            }

            WriteLine("| {0,-3} | {1,-10} |", "Id", "Group Name");

            foreach (var group in groups)
            {
                WriteLine("| {0:000} | {1,-10} |", group.GroupId, group.Name);
            }
        }
    }

    public static void ProfessorCRUD()
    {
        string op = "";
        bool b = false;

        while (!b)
        {
            WriteLine("1. List all professors");
            WriteLine("2. Update a professor");
            WriteLine("3. Remove a professor");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
            {
                b = true;
            }

            if (op == "1")
            {
                ListProfessors();
            }

            if (op == "2")
            {
                WriteLine("Enter the ID of the professor you want to update:");
                string professorId = VerifyReadMaxLengthString(10);
                using (bd_storage db = new())
                {
                    var professors = db.Professors
                        .Where(p => p.ProfessorId.Equals(professorId))
                        .ToList();

                    if (professors.Count == 0)
                    {
                        WriteLine("Professor with that ID is not registered");
                        b = false;
                    }
                    else
                    {
                        op = "a";
                        while (op != "7")
                        {
                            WriteLine("These are the fields you can update:");
                            WriteLine($"1. Professor ID: {professors.First().ProfessorId}");
                            WriteLine($"2. Name: {professors.First().Name}");
                            WriteLine($"3. Last Name (Paternal): {professors.First().LastNameP}");
                            WriteLine($"4. Last Name (Maternal): {professors.First().LastNameM}");
                            WriteLine($"5. NIP: {professors.First().Nip}");
                            WriteLine($"6. Password: {professors.First().Password}");
                            WriteLine($"7. None. Exit the Update of the Professor");
                            WriteLine("Choose the option you want to update:");

                            op = VerifyReadLengthStringExact(1);

                            switch (op)
                            {
                                case "1":
                                    Write("Write the new Professor ID: ");
                                    string newId = VerifyReadMaxLengthString(10);
                                    professors.First().ProfessorId = newId;
                                    break;
                                case "2":
                                    Write("Write the new name: ");
                                    string newName = VerifyReadMaxLengthString(30);
                                    professors.First().Name = newName;
                                    break;
                                case "3":
                                    Write("Write the new paternal last name: ");
                                    string newLastNameP = VerifyReadMaxLengthString(30);
                                    professors.First().LastNameP = newLastNameP;
                                    break;
                                case "4":
                                    Write("Write the new maternal last name: ");
                                    string newLastNameM = VerifyReadMaxLengthString(30);
                                    professors.First().LastNameM = newLastNameM;
                                    break;
                                case "5":
                                    Write("Write the new NIP: ");
                                    string newNIP = VerifyReadMaxLengthString(4);
                                    professors.First().Nip = newNIP;
                                    break;
                                case "6":
                                    Write("Write the new password: ");
                                    string newPassword = VerifyReadMaxLengthString(50);
                                    professors.First().Password = newPassword;
                                    break;
                                case "7":
                                    return;
                                default:
                                    WriteLine("Not a viable option");
                                    break;
                            }

                            int affected = db.SaveChanges();

                            if (affected == 1)
                            {
                                WriteLine("Professor information updated successfully!");
                            }
                            else
                            {
                                WriteLine("Update was not successful, sorry.");
                            }
                        }
                    }
                }
            }
            else if (op == "3")
            {
                WriteLine("Enter the ID of the professor you want to remove:");
                string professorId = VerifyReadMaxLengthString(10);
                using (bd_storage db = new())
                {
                    var professor = db.Professors
                        .FirstOrDefault(p => p.ProfessorId.Equals(professorId));

                    if (professor == null)
                    {
                        WriteLine("Professor with that ID is not registered");
                    }
                    else
                    {
                        db.Professors.Remove(professor);
                        int affected = db.SaveChanges();
                        if (affected == 0)
                        {
                            WriteLine("The professor was not removed, sorry");
                        }
                        else
                        {
                            WriteLine("The professor was removed successfully");
                        }
                    }
                }
            }
        }
    }

    public static void ListProfessors()
    {
        using (bd_storage db = new())
        {
            IQueryable<Professor> professors = db.Professors;

            if ((professors is null) || !professors.Any())
            {
                WriteLine("There are no professors");
                return;
            }

            WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-4} | {5,-50} |", "Id", "Name", "Last Name P", "Last Name M", "NIP", "Password");

            foreach (var professor in professors)
            {
                WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-4} | {5,-50} |", professor.ProfessorId, professor.Name, professor.LastNameP, professor.LastNameM, Decrypt(professor.Nip), Decrypt(professor.Password));
            }
        }
    }

    public static void StorerCRUD()
    {
        string op = "";
        bool b = false;

        while (!b)
        {
            WriteLine("1. List all storers");
            WriteLine("2. Update a storer");
            WriteLine("3. Remove a storer");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
            {
                b = true;
            }

            if (op == "1")
            {
                ListStorers();
            }

            if (op == "2")
            {
                WriteLine("Enter the ID of the storer you want to update:");
                string storerId = VerifyReadMaxLengthString(10);
                using (bd_storage db = new())
                {
                    var storers = db.Storers
                        .Where(s => s.StorerId.Equals(storerId))
                        .ToList();

                    if (storers.Count == 0)
                    {
                        WriteLine("Storer with that ID is not registered");
                        b = false;
                    }
                    else
                    {
                        op = "a";
                        while (op != "6")
                        {
                            WriteLine("These are the fields you can update:");
                            WriteLine($"1. Storer ID: {storers.First().StorerId}");
                            WriteLine($"2. Name: {storers.First().Name}");
                            WriteLine($"3. Last Name (Paternal): {storers.First().LastNameP}");
                            WriteLine($"4. Last Name (Maternal): {storers.First().LastNameM}");
                            WriteLine($"5. Password: {storers.First().Password}");
                            WriteLine($"6. None. Exit the Update of the Storer");
                            WriteLine("Choose the option you want to update:");

                            op = VerifyReadLengthStringExact(1);

                            switch (op)
                            {
                                case "1":
                                    Write("Write the new Storer ID: ");
                                    string newId = VerifyReadMaxLengthString(10);
                                    storers.First().StorerId = newId;
                                    break;
                                case "2":
                                    Write("Write the new name: ");
                                    string newName = VerifyReadMaxLengthString(30);
                                    storers.First().Name = newName;
                                    break;
                                case "3":
                                    Write("Write the new paternal last name: ");
                                    string newLastNameP = VerifyReadMaxLengthString(30);
                                    storers.First().LastNameP = newLastNameP;
                                    break;
                                case "4":
                                    Write("Write the new maternal last name: ");
                                    string newLastNameM = VerifyReadMaxLengthString(30);
                                    storers.First().LastNameM = newLastNameM;
                                    break;
                                case "5":
                                    Write("Write the new password: ");
                                    string newPassword = VerifyReadMaxLengthString(50);
                                    storers.First().Password = newPassword;
                                    break;
                                case "6":
                                    return;
                                default:
                                    WriteLine("Not a viable option");
                                    break;
                            }

                            int affected = db.SaveChanges();

                            if (affected == 1)
                            {
                                WriteLine("Storer information updated successfully!");
                            }
                            else
                            {
                                WriteLine("Update was not successful, sorry.");
                            }
                        }
                    }
                }
            }
            else if (op == "3")
            {
                WriteLine("Enter the ID of the storer you want to remove:");
                string storerId = VerifyReadMaxLengthString(10);
                storerId= EncryptPass(storerId);
                using (bd_storage db = new())
                {
                    var storer = db.Storers
                        .FirstOrDefault(s => s.StorerId.Equals(storerId));

                    if (storer == null)
                    {
                        WriteLine("Storer with that ID is not registered");
                    }
                    else
                    {
                        db.Storers.Remove(storer);
                        int affected = db.SaveChanges();
                        if (affected == 0)
                        {
                            WriteLine("The storer was not removed, sorry");
                        }
                        else
                        {
                            WriteLine("The storer was removed successfully");
                        }
                    }
                }
            }
        }
    }

    public static void ListStorers()
    {
        using (bd_storage db = new())
        {
            IQueryable<Storer> storers = db.Storers;

            if ((storers is null) || !storers.Any())
            {
                WriteLine("There are no storers");
                return;
            }

            WriteLine("| {0,-10} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", "Id", "Name", "Last Name P", "Last Name M", "Password");

            foreach (var storer in storers)
            {
                WriteLine("| {0:0000000000} | {1,-30} | {2,-30} | {3,-30} | {4,-50} |", storer.StorerId, storer.Name, storer.LastNameP, storer.LastNameM, Decrypt(storer.Password));
            }
        }
    }

    public static void StudentCRUD()
    {
        string op = "";
        bool b = false;

        while (!b)
        {
            WriteLine("1. List all students");
            WriteLine("2. Update a student");
            WriteLine("3. Remove a student");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
            {
                b = true;
            }

            if (op == "1")
            {
                ListStudentsforCoord();
            }

            if (op == "2")
            {
                WriteLine("Enter the ID of the student you want to update:");
                string studentId = VerifyReadMaxLengthString(8);
                using (bd_storage db = new())
                {
                    var students = db.Students
                        .Where(s => s.StudentId.Equals(studentId))
                        .ToList();

                    if (students.Count == 0)
                    {
                        WriteLine("Student with that ID is not registered");
                        b = false;
                    }
                    else
                    {
                        op = "a";
                        while (op != "7")
                        {
                            WriteLine("These are the fields you can update:");
                            WriteLine($"1. Student ID: {students.First().StudentId}");
                            WriteLine($"2. Name: {students.First().Name}");
                            WriteLine($"3. Last Name (Paternal): {students.First().LastNameP}");
                            WriteLine($"4. Last Name (Maternal): {students.First().LastNameM}");
                            WriteLine($"5. Password: {students.First().Password}");
                            WriteLine($"6. Group ID: {students.First().GroupId}");
                            WriteLine("7. None. Exit the Update of the Student");
                            WriteLine("Choose the option you want to update:");

                            op = VerifyReadLengthStringExact(1);

                            switch (op)
                            {
                                case "1":
                                    Write("Write the new Student ID: ");
                                    string newId = VerifyReadMaxLengthString(8);
                                    students.First().StudentId = newId;
                                    break;
                                case "2":
                                    Write("Write the new name: ");
                                    string newName = VerifyReadMaxLengthString(30);
                                    students.First().Name = newName;
                                    break;
                                case "3":
                                    Write("Write the new paternal last name: ");
                                    string newLastNameP = VerifyReadMaxLengthString(30);
                                    students.First().LastNameP = newLastNameP;
                                    break;
                                case "4":
                                    Write("Write the new maternal last name: ");
                                    string newLastNameM = VerifyReadMaxLengthString(30);
                                    students.First().LastNameM = newLastNameM;
                                    break;
                                case "5":
                                    Write("Write the new password: ");
                                    string newPassword = VerifyReadMaxLengthString(50);
                                    students.First().Password = newPassword;
                                    break;
                                case "6":
                                    Write("Write the new Group ID: ");
                                    WriteLine("Enter the ID of the group you want to remove:");
                                    string groupIdString = ReadLine();
                                    int newGroupId = TryParseStringaEntero(groupIdString);
                                    students.First().GroupId = newGroupId;
                                    break;
                                case "7":
                                    return;
                                default:
                                    WriteLine("Not a viable option");
                                    break;
                            }

                            int affected = db.SaveChanges();

                            if (affected == 1)
                            {
                                WriteLine("Student information updated successfully!");
                            }
                            else
                            {
                                WriteLine("Update was not successful, sorry.");
                            }
                        }
                    }
                }
            }else if (op == "3")
            {
                WriteLine("It is not posible to remove a student");
            }
        }
    }

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
                                        .Where(a => a.Name.StartsWith(academy));
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
                    .Where(s => s.Name.StartsWith(subjectid));
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

    public static void ListSubjects()
    {
        using (bd_storage db = new())
        {
            IQueryable<Subject> subjects = db.Subjects
                .Include(su => su.Academy);
            if ((subjects is null) || !subjects.Any())
            {
                WriteLine("There are no subjects");
                return;
            }

            WriteLine("| {0,-13} | {1,-55} | {2,-7} |", "Id", "Name", "Academy");

            foreach (var subject in subjects)
            {
                WriteLine("| {0:0000000000000} | {1,-55} | {2,-7} |", subject.SubjectId, subject.Name, subject.Academy?.Name);
            }
        }
    }

}