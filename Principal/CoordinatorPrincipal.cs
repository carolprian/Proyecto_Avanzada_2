using AutoGens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

partial class Program
{
    public static void CoordinatorsPrincipal()
    {
        string op = MenuCoordinators();
        WriteLine();
        switch (op)
        {
            case "1":
                ListStudentsforCoord();
                break;
            case "2":
                ViewAllEquipmentsCoord();
                break;
            case "3":
                WriteLine("Provide the number: ");
                string searchTerm = ReadLine();
                SearchEquipmentsById(searchTerm);
                break;
            case "4":
                ListDandLequipment();
                break;
            case "5":
                WriteLine("Provide the equipment ID to search:");
                string equipmentIdToFind = ReadLine();
                FindDandLequipmentById(equipmentIdToFind);
                break;
            case "6":
                WriteLine("Provide the equipment name to search:");
                string equipmentNameToFind = ReadLine();
                FindDandLequipmentByName(equipmentNameToFind);
                break;
            case "7":
                WriteLine("Provide the date (yyyy) to search:");
                string dateToFind = ReadLine();
                FindDandLequipmentByDate(dateToFind);
                break;
            case "8":
                WriteLine("Provide the student name to search:");
                string studentNameToFind = ReadLine();
                FindDandLequipmentByStudentName(studentNameToFind);
                break;
            case "9":

                break;
            case "10":
                ListGroups(); // Listar grupos
                break;
            case "11":
                WriteLine("Enter the name of the group to add:");
                string groupNameToAdd = ReadLine();
                AddGroup(groupNameToAdd); // Agregar grupo
                break;
            case "12":
                WriteLine("Enter the current name of the group to update:");
                string currentGroupName = ReadLine();
                WriteLine("Enter the new name for the group:");
                string newGroupName = ReadLine();
                int updateResult = UpdateGroupName(currentGroupName, newGroupName); // Actualizar grupo
                if (updateResult > 0)
                {
                    WriteLine("Group updated successfully.");
                }
                else
                {
                    WriteLine("Group not found or update failed.");
                }
                break;
            case "13":
                WriteLine("Enter the name of the group to delete:");
                string groupNameToDelete = ReadLine();
                int deleteResult = DeleteGroup(groupNameToDelete); // Eliminar grupo
                if (deleteResult > 0)
                {
                    WriteLine("Group deleted successfully.");
                }
                else
                {
                    WriteLine("Group not found or delete failed.");
                }
                break;
                case "14":
                return;
            default:

                break;
        }
    }

    public static string MenuCoordinators()
    {
        string op = "";
        /*
        academies
        areas
        classrooms
        divisions
        equipments
        groups
        professors
        storers
        students
        subjects
        se puede buscar por parametros
        historial de prestamos
        ver material perdido y dañado
        */
        WriteLine(" 1. View all students");
        WriteLine(" 2. View inventory");
        WriteLine(" 3. Search equipment by serial number");
        WriteLine(" 4. View damaged and lost equipment");
        WriteLine(" 5. Search damaged or lost equipment by serial number");
        WriteLine(" 6. Search damaged or lost equipment by equipment name");
        WriteLine(" 7. Search damaged or lost equipment by date of event");
        WriteLine(" 8. Search damaged or lost equipment by student name");
        WriteLine(" 9. View loan history");
        WriteLine(" 10. Ver grupos");
        WriteLine(" 11. Agregar grupos");
        WriteLine(" 12. Actualizar grupos");
        WriteLine(" 13. Eliminar grupos");
        WriteLine(" 14. Sign out");
        bool valid = false;
        do
        {
            op = ReadNonEmptyLine();
            if (op != "1" && op != "2" && op != "3" && op != "4" && op != "5" && op != "6" && op != "7" && op != "8" && op != "9" && op != "10" && op != "11" && op != "12" && op != "13"&& op != "14")
            {
                WriteLine("Please choose a valid option (1 - 14)");
                op = ReadNonEmptyLine();
            }
            else
            {
                valid = true;
            }
        } while (!valid);

        return op;
    }

    public static void ListGroups()
    {
        using (bd_storage db = new())
        {
            if ((db.Groups is null) || !db.Groups.Any())
            {
                WriteLine("There are no groups");
                return;
            }

            WriteLine("| {0,-3} | {1,-10} |", "Id", "Group Name");

            foreach (var group in db.Groups)
            {
                WriteLine("| {0:000} | {1,-10} |", group.GroupId, group.Name);
            }
        }
    }

    public static void AddGroup(string groupName)
    {
        using (bd_storage db = new())
        {
            if (db.Groups is null)
            {
                return;
            }

            Group g = new()
            {
                Name = groupName
            };

            EntityEntry<Group> entity = db.Groups.Add(g);
            int affected = db.SaveChanges();
            WriteLine("Group added with ID: {0}", entity.Entity.GroupId);
        }
    }

    public static int UpdateGroupName(string oldName, string newName)
    {
        using (bd_storage db = new())
        {
            if (db.Groups is null)
            {
                return 0;
            }

            Group groupToUpdate = db.Groups.FirstOrDefault(g => g.Name == oldName);

            if (groupToUpdate != null)
            {
                groupToUpdate.Name = newName;
                int affected = db.SaveChanges();
                return affected;
            }

            return 0;
        }
    }

    public static int DeleteGroup(string groupName)
    {
        using (bd_storage db = new())
        {
            IQueryable<Group>? groupsToDelete = db.Groups?.Where(g => g.Name == groupName);

            if (groupsToDelete is null || !groupsToDelete.Any())
            {
                WriteLine("No groups to delete");
                return 0;
            }
            else
            {
                if (db.Groups is null) return 0;
                db.Groups.RemoveRange(groupsToDelete);
            }

            int affected = db.SaveChanges();
            return affected;
        }
    }

    public static void SubjectCRUD()
    {
        string op = ""; bool b = false;
        while(b == false)
        {
            WriteLine("1. Update a subject");
            WriteLine("2. Remove permanently a subject");
            WriteLine("3. Exit");
            op = VerifyReadLengthStringExact(1);
            if(op=="1" || op=="2" || op=="3"){b = true;}
        

        if(op=="1")
        {
            WriteLine("Enter the subject id of the subject you want to update:");
            string subjectid = VerifyReadMaxLengthString(13);
            using(bd_storage db = new())
            {    
                IQueryable<Subject> subjects = db.Subjects
                .Include(s=>s.Academy)
                .Where(s=>s.SubjectId.Equals(subjectid));
                if(subjects is null || !subjects.Any())
                {
                    WriteLine("That subject id is not registered");
                    b = false;
                }
                else
                {
                op="a";
                while(op!="4")
                {
                WriteLine("This are the fields you can update:");
                WriteLine($"1. Subject id : {subjects.First().SubjectId}");
                WriteLine($"2. Name: {subjects.First().Name}");
                WriteLine($"3. Academy: {subjects.First().Academy?.Name}");
                WriteLine("4. None. Exit the Update of the Subject");
                WriteLine("Choose the option you want to update:");
                    bool ban = false;
                    op = VerifyReadLengthStringExact(1);
                    switch(op)
                    {
                        case "1":
                            while(ban == false)
                            {
                                Write("Write the new subject id :  ");
                                string subject = VerifyReadMaxLengthString(13);
                                IQueryable<Subject> subjectsids = db.Subjects.Where(u=>u.SubjectId == subject);
                                if(subjectsids is null || !subjectsids.Any())
                                {
                                    subjects.First().SubjectId = subject;
                                    int affected = db.SaveChanges();
                                    ban = true;
                                    if(affected == 1)
                                    {
                                        WriteLine("The id of the subject has been updated!");
                                    }
                                    else 
                                    {
                                        WriteLine("Unsuccesful update, sorry");
                                    }
                                }
                                else { WriteLine("That subject ID is already in use, try again.");}
                            }
                        break;
                        case "2":
                            while(ban == false)
                            {
                                Write("Write the new subject name :  ");
                                string subject = VerifyReadMaxLengthString(55);
                                IQueryable<Subject> subjectsids = db.Subjects.Where(u=>u.Name == subject);
                                if(subjectsids is null || !subjectsids.Any())
                                {
                                    subjects.First().Name = subject;
                                    int affected = db.SaveChanges();
                                    ban = true;
                                    if(affected == 1)
                                    {
                                        WriteLine("The name of the subject has been updated!");
                                    }
                                    else 
                                    {
                                        WriteLine("Unsuccesful update, sorry");
                                    }
                                }
                                else { WriteLine("That subject Name is already registered, try again.");}
                            }
                        break;
                        case "3":
                            while(ban == false)
                            {
                                WriteLine("Search for a academy by it's name: ");
                                string academy = VerifyReadMaxLengthString(55);
                                IQueryable<Academy> academies = db.Academies
                                .Where(a=>a.Name.StartsWith(academy));
                                if(academies is null || !academies.Any())
                                {
                                    WriteLine("There are no matching academies registered.");
                                }
                                else
                                {
                                    List<int> idsAca = new List<int>();
                                    foreach(var a in academies)
                                    {
                                        WriteLine($"{a.AcademyId} . {a.Name}");
                                        idsAca.Add(a.AcademyId);
                                    }
                                    int subjectaca = 0;
                                    while(!idsAca.Contains(subjectaca))
                                    {
                                        WriteLine("Choose an academy id :");
                                        subjectaca = TryParseStringaEntero(VerifyReadMaxLengthString(2));
                                    }
                                    subjects.First().AcademyId = subjectaca;     
                                    int affected = db.SaveChanges();
                                    ban = true;
                                    if(affected == 1)
                                    {
                                        WriteLine("The academy of the subject has been updated!");
                                    }
                                    else 
                                    {
                                        WriteLine("Unsuccesful update, sorry");
                                    }                          
                                }     
                            }
                        break;
                        case "4":
                        return;
                        default:
                        WriteLine("Not a viable option");
                        break;
                    }
                }
                

                }
            }
        }
        else if(op=="2")
        {
           WriteLine("Enter the subject name of the subject you want to update:");
            string subjectid = VerifyReadMaxLengthString(55);
            using(bd_storage db = new())
            {    
                IQueryable<Subject> subjects = db.Subjects
                .Include(s=>s.Academy)
                .Where(s=>s.Name.StartsWith(subjectid));
                if(subjects is null || !subjects.Any())
                {
                    WriteLine("That subject id is not registered");
                }
                else
                { 
                    List<string> idsSubjects = new List<string>();
                    foreach(var s in subjects)
                    {
                        WriteLine($"{s.SubjectId} . {s.Name}");
                        idsSubjects.Add(s.SubjectId);
                    }
                    string subjectaca = "";
                    while(!idsSubjects.Contains(subjectaca))
                    {
                        WriteLine("Write the selected subject id :");
                        subjectaca = VerifyReadMaxLengthString(13);
                    }
                    db.Subjects.Remove(subjects.First());
                    int affected = db.SaveChanges();
                    if(affected == 0)
                    {
                        WriteLine("The subject was not deleted, sorry");
                    }
                    else{
                        WriteLine("The subject was deleted succesfully");
                    }
                }
            }
        
        }
        }
    }
}