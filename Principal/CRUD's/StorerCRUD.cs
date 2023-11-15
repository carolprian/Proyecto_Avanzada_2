using AutoGens;

partial class Program
{
    public static void StorerCRUD()
    {
        string op = "";
        bool b = false;

        while (!b)
        {
            WriteLine("1. List all storers");
            WriteLine("2. Update a storer");
            WriteLine("3. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3")
            {
                b = true;
            }

            switch (op)
            {
                case "1":
                    ListStorers();
                    break;
                case "2":
                    UpdateStorer();
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

    private static void UpdateStorer()
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
            }
            else
            {
                UpdateStorerFields(storers);
            }
        }
    }

    private static void UpdateStorerFields(List<Storer> storers)
    {
        string op = "a";
        while (op != "6")
        {
            WriteLine("What field do you want to update? (except ID)");
            WriteLine($"1. Storer ID: {storers.First().StorerId}");
            WriteLine($"2. Name: {storers.First().Name}");
            WriteLine($"3. Last Name (Paternal): {storers.First().LastNameP}");
            WriteLine($"4. Last Name (Maternal): {storers.First().LastNameM}");
            WriteLine($"5. Password: {storers.First().Password}");
            WriteLine($"6. None. Exit the Update of the Storer");
            Write("Choose the option:");

            op = VerifyReadLengthStringExact(1);

            switch (op)
            {
                case "1":
                    WriteLine("Updating Storer ID is not allowed.");
                    break;
                case "2":
                    UpdateStorerField("Name", value => storers.First().Name = value);
                    break;
                case "3":
                    UpdateStorerField("Paternal Last Name", value => storers.First().LastNameP = value);
                    break;
                case "4":
                    UpdateStorerField("Maternal Last Name", value => storers.First().LastNameM = value);
                    break;
                case "5":
                    UpdateStorerField("Password", value => storers.First().Password = value);
                    break;
                case "6":
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
                    WriteLine("Storer information updated successfully!");
                }
                else
                {
                    WriteLine("Update was not successful, sorry.");
                }
            }
        }
    }

    private static void UpdateStorerField(string fieldName, Action<string> updateAction)
    {
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyReadMaxLengthString(30);
        updateAction(newValue);
    }
}
