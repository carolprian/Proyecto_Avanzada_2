
using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

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
            WriteLine("3. Remove a storer");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
            {
                b = true;
            }

            if (op == "1")
            {
                ListStorers();
            }

            if (op == "2")
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
                        b = false;
                    }
                    else
                    {
                        op = "a";
                        while (op != "6")
                        {
                            WriteLine("These are the fields you can update:");
                            WriteLine($"1. Storer ID: {storers.First().StorerId}");
                            WriteLine($"2. Name: {storers.First().Name}");
                            WriteLine($"3. Last Name (Paternal): {storers.First().LastNameP}");
                            WriteLine($"4. Last Name (Maternal): {storers.First().LastNameM}");
                            WriteLine($"5. Password: {storers.First().Password}");
                            WriteLine($"6. None. Exit the Update of the Storer");
                            WriteLine("Choose the option you want to update:");

                            op = VerifyReadLengthStringExact(1);

                            switch (op)
                            {
                                case "1":
                                    Write("Write the new Storer ID: ");
                                    string newId = VerifyReadMaxLengthString(10);
                                    storers.First().StorerId = newId;
                                    break;
                                case "2":
                                    Write("Write the new name: ");
                                    string newName = VerifyReadMaxLengthString(30);
                                    storers.First().Name = newName;
                                    break;
                                case "3":
                                    Write("Write the new paternal last name: ");
                                    string newLastNameP = VerifyReadMaxLengthString(30);
                                    storers.First().LastNameP = newLastNameP;
                                    break;
                                case "4":
                                    Write("Write the new maternal last name: ");
                                    string newLastNameM = VerifyReadMaxLengthString(30);
                                    storers.First().LastNameM = newLastNameM;
                                    break;
                                case "5":
                                    Write("Write the new password: ");
                                    string newPassword = VerifyReadMaxLengthString(50);
                                    storers.First().Password = newPassword;
                                    break;
                                case "6":
                                    return;
                                default:
                                    WriteLine("Not a viable option");
                                    break;
                            }

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
            }
            else if (op == "3")
            {
                WriteLine("Enter the ID of the storer you want to remove:");
                string storerId = VerifyReadMaxLengthString(10);
                storerId= EncryptPass(storerId);
                using (bd_storage db = new())
                {
                    var storer = db.Storers
                        .FirstOrDefault(s => s.StorerId.Equals(storerId));

                    if (storer == null)
                    {
                        WriteLine("Storer with that ID is not registered");
                    }
                    else
                    {
                        db.Storers.Remove(storer);
                        int affected = db.SaveChanges();
                        if (affected == 0)
                        {
                            WriteLine("The storer was not removed, sorry");
                        }
                        else
                        {
                            WriteLine("The storer was removed successfully");
                        }
                    }
                }
            }
        }
    }
}