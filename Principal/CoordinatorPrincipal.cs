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
        bool valid = false;
        do
        {
            op = ReadNonEmptyLine();
            if (op != "1" && op != "2" && op != "3" && op != "4" && op != "5" && op != "6" && op != "7" && op != "8" && op != "9" && op != "10" && op != "11" && op != "12" && op != "13")
            {
                WriteLine("Please choose a valid option (1 - 13)");
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

}