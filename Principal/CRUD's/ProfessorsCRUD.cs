using AutoGens;

partial class Program
{
    public static void ProfessorCRUD()
    {
        string op = "";
        bool b = false;

        while (!b)
        {
            WriteLine("1. List all professors");
            WriteLine("2. Update a professor");
            WriteLine("3. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3")
            {
                b = true;
            }

            switch (op)
            {
                case "1":
                    ListProfessors();
                    break;
                case "2":
                    UpdateProfessor();
                    break;
                case "3":
                    // Exit
                    return;
                default:
                    WriteLine("Invalid option");
                    break;
            }
        }
    }

    private static void UpdateProfessor()
    {
        WriteLine("Enter the ID of the professor you want to update:");
        string ProfessorId = VerifyNumericInput();
        UpdateProfessorFields(ProfessorId);
    }

    private static void UpdateProfessorFields(string ProfessorId)
    {
        string UserName = EncryptPass(ProfessorId);

        using (bd_storage db = new())
        {
            var professor = db.Professors
                .FirstOrDefault(p => p.ProfessorId.Equals(UserName));

            if (professor == null)
            {
                WriteLine("Professor with that ID is not registered");
            }
            else
            {
                string DescryptPass = Decrypt(professor.Password);
                string DescryptNip = Decrypt(professor.Nip);
                string op = "";
                while (op != "7")
                {
                    WriteLine();
                    WriteLine("What field do you want to update? (except ID):");
                    WriteLine($"1. Professor ID: {ProfessorId}");
                    WriteLine($"2. Name: {professor.Name}");
                    WriteLine($"3. Last Name (Paternal): {professor.LastNameP}");
                    WriteLine($"4. Last Name (Maternal): {professor.LastNameM}");
                    WriteLine($"5. NIP: {DescryptNip}");
                    WriteLine($"6. Password: {DescryptPass}");
                    WriteLine($"7. None. Exit the Update of the Professor");
                    Write("Choose the option:");

                    op = VerifyReadLengthStringExact(1);
                    WriteLine();

                    switch (op)
                    {
                        case "1":
                            WriteLine("Updating Professor ID is not allowed.");
                            break;
                        case "2":
                            UpdateProfessorField("Name", value => professor.Name = value);
                            break;
                        case "3":
                            UpdateProfessorField("Paternal Last Name", value => professor.LastNameP = value);
                            break;
                        case "4":
                            UpdateProfessorField("Maternal Last Name", value => professor.LastNameM = value);
                            break;
                        case "5":
                            UpdateProfessorNIP("NIP", value => professor.Nip = value);
                            break;
                        case "6":
                            UpdateProfessorPass("Password",  value => professor.Password = value);
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

    private static void UpdateProfessorField(string fieldName, Action<string> updateAction)
    {
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyAlphabeticInput();
        updateAction(newValue);
    }

    private static void UpdateProfessorNIP(string fieldName, Action<string> updateAction)
    {
        Write($"Write the new {fieldName}: ");
        string newValue = EncryptPass(VerifyReadLengthStringExact(4));
        updateAction(newValue);
    }

    private static void UpdateProfessorPass(string fieldName, Action<string> updateAction)
    {
        Write($"Write the new {fieldName}: ");
        string newValue = EncryptPass(VerifyUpperCaseAndNumeric(VerifyReadLengthString(8)));
        updateAction(newValue);
    }
}
