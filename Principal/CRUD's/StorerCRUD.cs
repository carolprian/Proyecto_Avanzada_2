using AutoGens;

partial class Program
{
    // CRUD Method
    public static void StorerCRUD()
    {
        // Initialize variables for operation and loop control
        string op = "";
        bool b = false;

        while (!b)
        {
            // Display menu options to the user and validates the user input
            WriteLine("1. List all storers");
            WriteLine("2. Update a storer");
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
                    // Call the method to list all storers
                    ListStorers();
                    break;
                case "2":
                    // Call the method to update a storer
                    UpdateStorer();
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

    private static void UpdateStorer()
    {
        // Prompt the user to enter the ID of the storer to be updated
        WriteLine("Enter the ID of the storer you want to update:");
        string storerId = VerifyReadMaxLengthString(10);

        using (bd_storage db = new())
        {
            var storers = db.Storers
                .Where(s => s.StorerId.Equals(storerId))
                .ToList();

            // Check if a storer with the specified ID exists
            if (storers.Count == 0)
            {
                WriteLine("Storer with that ID is not registered");
            }
            else
            {
                // Call a method to update the fields of the specified storer
                UpdateStorerFields(storers);
            }
        }
    }

    // Updates individual fields of a storer
    private static void UpdateStorerFields(List<Storer> storers)
    {
        string op = "a";
        while (op != "6")
        {
            // Display options for updating storer fields and validates user input
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
                    // Display a message indicating that updating Storer ID is not allowed
                    WriteLine("Updating Storer ID is not allowed.");
                    break;
                case "2":
                    // Call a method to update the storer's name
                    UpdateStorerField("Name", value => storers.First().Name = value);
                    break;
                case "3":
                    // Call a method to update the storer's paternal last name
                    UpdateStorerField("Paternal Last Name", value => storers.First().LastNameP = value);
                    break;
                case "4":
                    // Call a method to update the storer's maternal last name
                    UpdateStorerField("Maternal Last Name", value => storers.First().LastNameM = value);
                    break;
                case "5":
                    // Call a method to update the storer's password
                    UpdateStorerField("Password", value => storers.First().Password = value);
                    break;
                case "6":
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
                    WriteLine("Storer information updated successfully!");
                }
                else
                {
                    WriteLine("Update was not successful, sorry.");
                }
            }
        }
    }

    // Updates a generic storer field
    private static void UpdateStorerField(string fieldName, Action<string> updateAction)
    {
        // Prompt the user to enter the new value for the field
        Write($"Write the new {fieldName}: ");
        string newValue = VerifyReadMaxLengthString(30);
        // Call the update action to update the storer's field
        updateAction(newValue);
    }
}
