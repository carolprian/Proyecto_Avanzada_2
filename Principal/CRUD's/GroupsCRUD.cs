using AutoGens;

partial class Program
{
    public static void GroupsCRUD()
    {
        string op = "";
        bool b = false;

        while (!b)
        {
            WriteLine("1. List all groups");
            WriteLine("2. Update a group");
            WriteLine("3. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3")
            {
                b = true;
            }

            switch (op)
            {
                case "1":
                    ListGroups();
                    break;
                case "2":
                    UpdateGroup();
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

    private static void UpdateGroup()
    {
        WriteLine("Enter the ID of the group you want to update:");
        string? groupIdString = ReadLine();

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
                UpdateGroupFields(groups);
            }
        }
    }

    private static void UpdateGroupFields(List<Group> groups)
    {
        string op = "a";
        while (op != "3")
        {
            WriteLine("What field do you want to update? (except ID)");
            WriteLine($"1. Group ID: {groups.First().GroupId}");
            WriteLine($"2. Name: {groups.First().Name}");
            WriteLine($"3. None. Exit the Update of the Group");
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

            using (bd_storage db = new())
            {
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
