using AutoGens;

partial class Program
{
    // CRUD Method
    public static void ProfessorCRUD()
    {
        // Initialize variables for operation and loop control
        string op = "";
        bool b = false;

        while (!b)
        {
            // Display menu options to the user and validates the user input
            WriteLine("1. List all professors");
            WriteLine("2. Update a professor");
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
                    // Call the method to list all professors
                    ListProfessors();
                    break;
                case "2":
                    // Call the method to update a professor
                    UpdateProfessor();
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

    // Updates for a specific professor
    private static void UpdateProfessor()
    {
        // Prompt the user to enter the ID of the professor to be updated
        WriteLine("Enter the ID of the professor you want to update:");
        string ProfessorId = VerifyNumericInput();
        UpdateProfessorFields(ProfessorId);
    }

    // Updates individual fields of a professor
    private static void UpdateProfessorFields(string ProfessorId)
    {
        // Encripts the professor ID
        string UserName = EncryptPass(ProfessorId);

        using (bd_storage db = new())
        {
            var professor = db.Professors
                .FirstOrDefault(p => p.ProfessorId.Equals(UserName));

            // Checks if the ID exist
            if (professor == null)
            {
                WriteLine("Professor with that ID is not registered");
            }
            else
            {
                // Decrypt the professor's password
                string DescryptPass = Decrypt(professor.Password);
                
                string op = "";
                while (op != "7")
                {
                    // Display options for updating professor fields
                    WriteLine();
                    WriteLine("What field do you want to update? (except ID):");
                    WriteLine($"1. Professor ID: {ProfessorId}");
                    WriteLine($"2. Name: {professor.Name}");
                    WriteLine($"3. Last Name (Paternal): {professor.LastNameP}");
                    WriteLine($"4. Last Name (Maternal): {professor.LastNameM}");
                    WriteLine($"5. NIP: {professor.Nip}");
                    WriteLine($"6. Password: {DescryptPass}");
                    WriteLine($"7. None. Exit the Update of the Professor");
                    Write("Choose the option:");

                    // Validates the user input
                    op = VerifyReadLengthStringExact(1);
                    WriteLine();

                    switch (op)
                    {
                        case "1":
                            // Display a message indicating that updating Professor ID is not allowed
                            WriteLine("Updating Professor ID is not allowed.");
                            break;
                        case "2":
                            // Call a method to update the professor's name
                            UpdateProfessorField("Name", value => professor.Name = value);
                            break;
                        case "3":
                            // Call a method to update the professor's paternal last name
                            UpdateProfessorField("Paternal Last Name", value => professor.LastNameP = value);
                            break;
                        case "4":
                            // Call a method to update the professor's maternal last name
                            UpdateProfessorField("Maternal Last Name", value => professor.LastNameM = value);
                            break;
                        case "5":
                            // Call a method to update the professor's NIP
                            UpdateProfessorNIP("NIP", value => professor.Nip = value);
                            break;
                        case "6":
                            // Call a method to update the professor's password
                            UpdateProfessorPass("Password",  value => professor.Password = value);
                            break;
                        case "7":
                            // Exit
                            return;
                        default:
                            // Error
                            WriteLine("Not a viable option");
                            break;
                    }

                    // Saves the changes to the database
                    int affected = db.SaveChanges();

                    // Checks if the update was successful
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

    // Updates a generic professor field.
    private static void UpdateProfessorField(string fieldName, Action<string> updateAction)
    {
        // Prompt the user to enter the new value for the field
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyAlphabeticInput();
        // Call the update action to update the professor's field
        updateAction(newValue);
    }

    // Updates the NIP field of a professor.
    private static void UpdateProfessorNIP(string fieldName, Action<string> updateAction)
    {
        // Prompt the user to enter the new NIP value
        Write($"Write the new {fieldName}: ");
        string newValue = EncryptPass(VerifyReadLengthStringExact(4));
        // Call the update action to update the professor's NIP
        updateAction(newValue);
    }

    private static void UpdateProfessorPass(string fieldName, Action<string> updateAction)
    {
        // Prompt the user to enter the new password value
        Write($"Write the new {fieldName}: ");
        string newValue = EncryptPass(VerifyUpperCaseAndNumeric(VerifyReadLengthString(8)));
        // Call the update action to update the professor's password
        updateAction(newValue);
    }
}
