using AutoGens;
using Microsoft.EntityFrameworkCore;

partial class Program
{
    public static void SubjectCRUD()
    {
        // Initialize variables for operation and loop control
        string op = "";
        bool b = false;

        while (b == false)
        {
            // Display menu options to the user and validates user input
            WriteLine("1. List all subjects");
            WriteLine("2. Update a subject");
            WriteLine("3. Exit");
            op = VerifyReadLengthStringExact(1);

            // Check if the entered option is valid
            if (op == "1" || op == "2" || op == "3")
            {
                b = true;
            }

            switch (op)
            {
                case "1":
                    // Call the method to list all subjects
                    ListSubjects();
                    break;
                case "2":
                    // Call the method to update a subject
                    UpdateSubject();
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

    // Updates for a specific subject.
    private static void UpdateSubject()
    {
        // Prompt the user to enter the ID of the subject to be updated
        WriteLine("Enter the subject id of the subject you want to update:");
        string subjectId = VerifyReadMaxLengthString(13);

        using (bd_storage db = new())
        {
            // Include related Academy information in the query
            IQueryable<Subject> subjects = db.Subjects
                .Include(s => s.Academy)
                .Where(s => s.SubjectId.Equals(subjectId));
            
            // Check if a subject with the specified ID exists
            if (subjects is null || !subjects.Any())
            {
                WriteLine("That subject id is not registered");
            }
            else
            {
                // Call a method to update the fields of the specified subject
                UpdateSubjectFields(subjects.First());
            }
        }
    }

    // Updates individual fields of a subject
    private static void UpdateSubjectFields(Subject subject)
    {
        string op = "a";
        while (op != "4")
        {
            // Display options for updating subject fields
            WriteLine("What field do you want to update? (except ID)");
            WriteLine($"1. Subject id : {subject.SubjectId}");
            WriteLine($"2. Name: {subject.Name}");
            WriteLine($"3. Academy: {subject.Academy?.Name}");
            WriteLine("4. None. Exit the Update of the Subject");
            WriteLine("Choose the option you want to update:");

            // Read and validate user input for the selected field update
            op = VerifyReadLengthStringExact(1);

            switch (op)
            {
                case "1":
                    // Display a message indicating that updating Subject ID is not allowed
                    WriteLine("Updating Subject ID is not allowed.");
                    break;
                case "2":
                    // Call a method to update the subject's name
                    UpdateSubjectField("Name", value => subject.Name = value);
                    break;
                case "3":
                    // Call a method to update the subject's academy
                    UpdateSubjectField("Academy", value =>
                    {
                        int academyId = int.Parse(value);
                        subject.AcademyId = academyId;
                    });
                    break;
                case "4":
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
                    WriteLine("Subject information updated successfully!");
                }
                else
                {
                    WriteLine("Update was not successful, sorry.");
                }
            }
        }
    }

    // Updates a generic subject field
    private static void UpdateSubjectField(string fieldName, Action<string> updateAction)
    {
        // Prompt the user to enter the new value for the field
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyReadMaxLengthString(30);
        // Call the update action to update the subject's field
        updateAction(newValue);
    }
}
