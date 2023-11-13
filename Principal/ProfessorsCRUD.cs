using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

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

            if (op == "1")
            {
                ListProfessors();
            }

            if (op == "2")
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
                        b = false;
                    }
                    else
                    {
                        op = "a";
                        while (op != "7")
                        {
                            WriteLine("These are the fields you can update:");
                            WriteLine($"1. Professor ID: {professors.First().ProfessorId}");
                            WriteLine($"2. Name: {professors.First().Name}");
                            WriteLine($"3. Last Name (Paternal): {professors.First().LastNameP}");
                            WriteLine($"4. Last Name (Maternal): {professors.First().LastNameM}");
                            WriteLine($"5. NIP: {professors.First().Nip}");
                            WriteLine($"6. Password: {professors.First().Password}");
                            WriteLine($"7. None. Exit the Update of the Professor");
                            WriteLine("Choose the option you want to update:");

                            op = VerifyReadLengthStringExact(1);

                            switch (op)
                            {
                                case "1":
                                    Write("Write the new Professor ID: ");
                                    string newId = VerifyReadMaxLengthString(10);
                                    professors.First().ProfessorId = newId;
                                    break;
                                case "2":
                                    Write("Write the new name: ");
                                    string newName = VerifyReadMaxLengthString(30);
                                    professors.First().Name = newName;
                                    break;
                                case "3":
                                    Write("Write the new paternal last name: ");
                                    string newLastNameP = VerifyReadMaxLengthString(30);
                                    professors.First().LastNameP = newLastNameP;
                                    break;
                                case "4":
                                    Write("Write the new maternal last name: ");
                                    string newLastNameM = VerifyReadMaxLengthString(30);
                                    professors.First().LastNameM = newLastNameM;
                                    break;
                                case "5":
                                    Write("Write the new NIP: ");
                                    string newNIP = VerifyReadMaxLengthString(4);
                                    professors.First().Nip = newNIP;
                                    break;
                                case "6":
                                    Write("Write the new password: ");
                                    string newPassword = VerifyReadMaxLengthString(50);
                                    professors.First().Password = newPassword;
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
            else if (op == "3")
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
    }


}

