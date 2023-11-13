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
            WriteLine("3. Remove a professor");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
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
                    RemoveProfessor();
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

    private static void UpdateProfessor()
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
            }
            else
            {
                UpdateProfessorFields(professors);
            }
        }
    }

    private static void UpdateProfessorFields(List<Professor> professors)
    {
        string op = "a";
        while (op != "7")
        {
            WriteLine("What field do you want to update? (except ID):");
            WriteLine($"1. Professor ID: {professors.First().ProfessorId}");
            WriteLine($"2. Name: {professors.First().Name}");
            WriteLine($"3. Last Name (Paternal): {professors.First().LastNameP}");
            WriteLine($"4. Last Name (Maternal): {professors.First().LastNameM}");
            WriteLine($"5. NIP: {professors.First().Nip}");
            WriteLine($"6. Password: {professors.First().Password}");
            WriteLine($"7. None. Exit the Update of the Professor");
            Write("Choose the option:");

            op = VerifyReadLengthStringExact(1);

            switch (op)
            {
                case "1":
                    WriteLine("Updating Professor ID is not allowed.");
                    break;
                case "2":
                    UpdateProfessorField("Name", value => professors.First().Name = value);
                    break;
                case "3":
                    UpdateProfessorField("Paternal Last Name", value => professors.First().LastNameP = value);
                    break;
                case "4":
                    UpdateProfessorField("Maternal Last Name", value => professors.First().LastNameM = value);
                    break;
                case "5":
                    UpdateProfessorField("NIP", value => professors.First().Nip = value);
                    break;
                case "6":
                    UpdateProfessorField("Password",  value => professors.First().Password = value);
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
                    WriteLine("Professor information updated successfully!");
                }
                else
                {
                    WriteLine("Update was not successful, sorry.");
                }
            }
        }
    }

    private static void UpdateProfessorField(string fieldName, Action<string> updateAction)
    {
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyReadMaxLengthString(30);
        updateAction(newValue);
    }


    private static void RemoveProfessor()
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
