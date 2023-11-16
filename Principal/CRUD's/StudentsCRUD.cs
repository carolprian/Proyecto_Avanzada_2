using AutoGens;

partial class Program
{
    public static void StudentCRUD()
    {
        // Initialize variables for operation and loop control
        string? op = "";
        bool b = false;

        while (!b)
        {
            // Display menu options to the user and validates the user input
            WriteLine("1. List all students");
            WriteLine("2. Update a student");
            WriteLine("3. Exit");
            op = VerifyReadLengthStringExact(1);

            // Check if the entered option is valid.
            if (op == "1" || op == "2" || op == "3")
            {
                b = true;
            }

            switch (op)
            {
                case "1":
                    // Call the method to list all students
                    ListStudents();
                    break;
                case "2":
                    // Call the method to update a student
                    UpdateStudent();
                    break;
                case "3":
                    // Exit
                    return;
                default:
                    // Error
                    WriteLine("Invalid option");
                    break;
            }
        }
    }

    // Updates for a specific student.
    private static void UpdateStudent()
    {
        // Prompt the user to enter the ID of the student to be updated
        WriteLine("Enter the ID of the student you want to update:");
        string studentId = VerifyReadMaxLengthString(8);

        using (bd_storage db = new())
        {
            var students = db.Students
                .Where(s => s.StudentId.Equals(studentId))
                .ToList();

            // Check if a student with the specified ID exists
            if (students.Count == 0)
            {
                WriteLine("Student with that ID is not registered");
            }
            else
            {
                // Call a method to update the fields of the specified student
                UpdateStudentFields(students);
            }
        }
    }

    // Updates individual fields of a student
    private static void UpdateStudentFields(List<Student> students)
    {
        string op = "a";
        while (op != "7")
        {
            // Display options for updating student fields
            WriteLine("What field do you want to update? (except ID)");
            WriteLine($"1. Student ID: {students.First().StudentId}");
            WriteLine($"2. Name: {students.First().Name}");
            WriteLine($"3. Paternal Last Name: {students.First().LastNameP}");
            WriteLine($"4. Maternal Last Name: {students.First().LastNameM}");
            WriteLine($"5. Password: {students.First().Password}");
            WriteLine($"6. Group ID: {students.First().GroupId}");
            WriteLine("7. None. Exit the Update of the Student");
            Write("Choose the option:");

            // Read and validate user input for the selected field update
            op = VerifyReadLengthStringExact(1);

            switch (op)
            {
                case "1":
                    // Display a message indicating that updating Student ID is not allowed
                    WriteLine("Updating Student ID is not allowed.");
                    break;
                case "2":
                    // Call a method to update the student's name
                    UpdateStudentField("Name", value => students.First().Name = value);
                    break;
                case "3":
                    // Call a method to update the student's paternal last name
                    UpdateStudentField("Paternal Last Name", value => students.First().LastNameP = value);
                    break;
                case "4":
                    // Call a method to update the student's maternal last name
                    UpdateStudentField("Maternal Last Name", value => students.First().LastNameM = value);
                    break;
                case "5":
                    // Call a method to update the student's password
                    UpdateStudentField("Password", value => students.First().Password = value);
                    break;
                case "6":
                    // Call a method to update the student's group ID
                    UpdateStudentField("Group ID", value =>
                    {
                        WriteLine("Write the new Group ID: ");
                        string? groupIdString = ReadLine();
                        int newGroupId = TryParseStringaEntero(groupIdString);
                        students.First().GroupId = newGroupId;
                    });
                    break;
                case "7":
                    // Exit
                    return;
                default:
                    // Error
                    WriteLine("Not a viable option");
                    break;
            }

            using (bd_storage db = new())
            {
                int affected = db.SaveChanges();

                // Check if the update was successful
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

    // Updates a generic student field
    private static void UpdateStudentField(string fieldName, Action<string> updateAction)
    {
        // Prompt the user to enter the new value for the field
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyReadMaxLengthString(30);
        // Call the update action to update the student's field
        updateAction(newValue);
    }
}
