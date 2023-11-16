using AutoGens;

partial class Program
{
    // CRUD Method
    public static void GroupsCRUD()
    {
        // Initialize variables for operation and loop control
        string op = "";
        bool b = false;

        while (!b)
        {
            // Display menu options to the user and validates the user input
            WriteLine("1. List all groups");
            WriteLine("2. Update a group");
            WriteLine("3. Exit");
            op = VerifyReadLengthStringExact(1);

            // Check if the option is valid
            if (op == "1" || op == "2" || op == "3")
            {
                b = true;
            }

            // Switch based on the selected operation
            switch (op)
            {
                case "1":
                    // Call the method to list all groups
                    ListGroups();
                    break;
                case "2":
                    // Call the method to update a group
                    UpdateGroup();
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

    // Updates the group
    private static void UpdateGroup()
    {
        // Prompt the user to enter the ID of the group to be updated
        WriteLine("Enter the ID of the group you want to update:");
        string? groupIdString = ReadNonEmptyLine();

        // Parse the ID into an integer
        int groupId = TryParseStringaEntero(groupIdString);

        using (bd_storage db = new())
        {
            var groups = db.Groups
                .Where(g => g.GroupId == groupId)
                .ToList();

            // Check if a group with the specified ID exists
            if (groups.Count == 0)
            {
                WriteLine("Group with that ID is not registered");
            }
            else
            {
                // Updates the fields of the group
                UpdateGroupFields(groups);
            }
        }
    }

    // Updates individual fields of a group.
    private static void UpdateGroupFields(List<Group> groups)
    {
        // Initialize variable for operation within the field update loop
        string op = "a";

        // Loop for updating group fields
        while (op != "3")
        {
            // Display options for updating group fields
            WriteLine("What field do you want to update? (except ID)");
            WriteLine($"1. Group ID: {groups.First().GroupId}");
            WriteLine($"2. Name: {groups.First().Name}");
            WriteLine($"3. None. Exit the Update of the Group");

            // Read and validate user input
            op = VerifyReadLengthStringExact(1);

            switch (op)
            {
                case "1":
                    // Display a message indicating that updating Group ID is not allowed
                    WriteLine("Updating Group ID is not allowed.");
                    break;
                case "2":
                    // Ask for the new name of the group
                    Write("Write the new name: ");
                    string newName = VerifyReadMaxLengthString(3);
                    // Update the name of the group.
                    groups.First().Name = newName;
                    break;
                case "3":
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
