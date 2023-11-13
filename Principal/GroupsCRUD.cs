using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

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
            WriteLine("3. Remove a group");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
            {
                b = true;
            }

            if (op == "1")
            {
                ListGroups();
            }
            else if (op == "2")
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
                        op = "a";
                        while (op != "3")
                        {
                            WriteLine("These are the fields you can update:");
                            WriteLine($"1. Group ID: {groups.First().GroupId}");
                            WriteLine($"2. Name: {groups.First().Name}");
                            WriteLine($"3. None. Exit the Update of the Group");
                            WriteLine("Choose the option you want to update:");

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
            else if (op == "3")
            {
                WriteLine("Enter the ID of the group you want to remove:");
                string? groupIdString = ReadLine();

                int groupId = TryParseStringaEntero(groupIdString);

                using (bd_storage db = new())
                {
                    var group = db.Groups
                        .FirstOrDefault(g => g.GroupId == groupId);

                    if (group == null)
                    {
                        WriteLine("Group with that ID is not registered");
                    }
                    else
                    {
                        db.Groups.Remove(group);
                        int affected = db.SaveChanges();
                        if (affected == 0)
                        {
                            WriteLine("The group was not removed, sorry");
                        }
                        else
                        {
                            WriteLine("The group was removed successfully");
                        }
                    }
                }
            }
        }
    }
}