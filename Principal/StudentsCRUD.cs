using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

partial class Program
{
    public static void StudentCRUD()
    {
        string? op = "";
        bool b = false;

        while (!b)
        {
            WriteLine("1. List all students");
            WriteLine("2. Update a student");
            WriteLine("3. Remove a student");
            WriteLine("4. Exit");
            op = VerifyReadLengthStringExact(1);

            if (op == "1" || op == "2" || op == "3" || op == "4")
            {
                b = true;
            }

            if (op == "1")
            {
                ListStudentsforCoord();
            }

            if (op == "2")
            {
                WriteLine("Enter the ID of the student you want to update:");
                string studentId = VerifyReadMaxLengthString(8);
                using (bd_storage db = new())
                {
                    var students = db.Students
                        .Where(s => s.StudentId.Equals(studentId))
                        .ToList();

                    if (students.Count == 0)
                    {
                        WriteLine("Student with that ID is not registered");
                        b = false;
                    }
                    else
                    {
                        op = "a";
                        while (op != "7")
                        {
                            WriteLine("These are the fields you can update:");
                            WriteLine($"1. Student ID: {students.First().StudentId}");
                            WriteLine($"2. Name: {students.First().Name}");
                            WriteLine($"3. Last Name (Paternal): {students.First().LastNameP}");
                            WriteLine($"4. Last Name (Maternal): {students.First().LastNameM}");
                            WriteLine($"5. Password: {students.First().Password}");
                            WriteLine($"6. Group ID: {students.First().GroupId}");
                            WriteLine("7. None. Exit the Update of the Student");
                            WriteLine("Choose the option you want to update:");

                            op = VerifyReadLengthStringExact(1);

                            switch (op)
                            {
                                case "1":
                                    Write("Write the new Student ID: ");
                                    string newId = VerifyReadMaxLengthString(8);
                                    students.First().StudentId = newId;
                                    break;
                                case "2":
                                    Write("Write the new name: ");
                                    string newName = VerifyReadMaxLengthString(30);
                                    students.First().Name = newName;
                                    break;
                                case "3":
                                    Write("Write the new paternal last name: ");
                                    string newLastNameP = VerifyReadMaxLengthString(30);
                                    students.First().LastNameP = newLastNameP;
                                    break;
                                case "4":
                                    Write("Write the new maternal last name: ");
                                    string newLastNameM = VerifyReadMaxLengthString(30);
                                    students.First().LastNameM = newLastNameM;
                                    break;
                                case "5":
                                    Write("Write the new password: ");
                                    string newPassword = VerifyReadMaxLengthString(50);
                                    students.First().Password = newPassword;
                                    break;
                                case "6":
                                    Write("Write the new Group ID: ");
                                    WriteLine("Enter the ID of the group you want to remove:");
                                    string? groupIdString = ReadLine();
                                    int newGroupId = TryParseStringaEntero(groupIdString);
                                    students.First().GroupId = newGroupId;
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
                                WriteLine("Student information updated successfully!");
                            }
                            else
                            {
                                WriteLine("Update was not successful, sorry.");
                            }
                        }
                    }
                }
            }else if (op == "3")
            {
                WriteLine("It is not posible to remove a student");
            }
        }
    }
}