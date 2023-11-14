using AutoGens;

partial class Program
{
    public static void StudentCRUD()
    {
        string? op = "";
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

            switch (op)
            {
                case "1":
                    ListStudents();
                    break;
                case "2":
                    UpdateStudent();
                    break;
                case "3":
                    RemoveStudent();
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

    private static void UpdateStudent()
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
            }
            else
            {
                UpdateStudentFields(students);
            }
        }
    }

    private static void UpdateStudentFields(List<Student> students)
    {
        string op = "a";
        while (op != "7")
        {
            WriteLine("What field do you want to update? (except ID)");
            WriteLine($"1. Student ID: {students.First().StudentId}");
            WriteLine($"2. Name: {students.First().Name}");
            WriteLine($"3. Paternal Last Name: {students.First().LastNameP}");
            WriteLine($"4. Maternal Last Name: {students.First().LastNameM}");
            WriteLine($"5. Password: {students.First().Password}");
            WriteLine($"6. Group ID: {students.First().GroupId}");
            WriteLine("7. None. Exit the Update of the Student");
            Write("Choose the option:");

            op = VerifyReadLengthStringExact(1);

            switch (op)
            {
                case "1":
                    WriteLine("Updating Student ID is not allowed.");
                    break;
                case "2":
                    UpdateStudentField("Name", value => students.First().Name = value);
                    break;
                case "3":
                    UpdateStudentField("Paternal Last Name", value => students.First().LastNameP = value);
                    break;
                case "4":
                    UpdateStudentField("Maternal Last Name", value => students.First().LastNameM = value);
                    break;
                case "5":
                    UpdateStudentField("Password", value => students.First().Password = value);
                    break;
                case "6":
                    UpdateStudentField("Group ID", value =>
                    {
                        WriteLine("Write the new Group ID: ");
                        string? groupIdString = ReadLine();
                        int newGroupId = TryParseStringaEntero(groupIdString);
                        students.First().GroupId = newGroupId;
                    });
                    break;
                case "7":
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
                    WriteLine("Student information updated successfully!");
                }
                else
                {
                    WriteLine("Update was not successful, sorry.");
                }
            }
        }
    }

    private static void UpdateStudentField(string fieldName, Action<string> updateAction)
    {
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyReadMaxLengthString(30);
        updateAction(newValue);
    }

    private static void RemoveStudent()
    {
        WriteLine("It is not possible to remove a student");
    }
}
