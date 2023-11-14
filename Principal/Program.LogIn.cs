using AutoGens;

partial class Program
{
    public static (bool logged, string? username) IniciarSesion(out string? Rol)
    {
        using (bd_storage db = new())
        {
            WriteLine("Write your ID: ");
            string? username = ReadNonEmptyLine();
            string encyptUsr = EncryptPass(username);

            WriteLine("Write your password: ");
            string pass = EncryptPass(ReadPassword());

            string idUser = "studentId";
            while ( true )
            {
                if (idUser == "studentId")
                {
                    IQueryable<Student> students = db.Students
                    .Where(s => s.StudentId == username && s.Password == pass);
                    
                    if (students is null || !students.Any())
                    {
                        idUser = "professorId";
                        Rol = null;
                    }
                    else
                    {
                        Rol = "students";
                        return (true, username);
                    }

                }
                else if (idUser == "professorId")
                {
                    IQueryable<Professor> professors = db.Professors
                    .Where(p => p.ProfessorId == encyptUsr && p.Password == pass);

                    if (professors is null || !professors.Any())
                    {
                        idUser = "storerId";
                        Rol = null;
                    }
                    else
                    {
                        Rol = "professors";
                        return (true, username);
                    }
                }
                else if (idUser == "storerId")
                {
                    IQueryable<Storer> storers = db.Storers
                    .Where(s => s.StorerId == encyptUsr && s.Password == pass);
                    if (storers is null || !storers.Any())
                    {
                        idUser = "coordinatorId";
                        Rol = null;
                    }
                    else
                    {
                        Rol = "storers";
                        return (true, username);
                    }
                }
                else if (idUser == "coordinatorId")
                {
                    IQueryable<Coordinator> coordinators = db.Coordinators
                    .Where(s => s.CoordinatorId == encyptUsr && s.Password == pass);
                    if (coordinators is null || !coordinators.Any())
                    {
                        Rol = null;
                        idUser = "none";
                        break;
                    }
                    else
                    {
                        Rol = "coordinators";
                        return (true, username);
                    }
                }
            }
        }
        return (false, null);
    }

    public static void RecuperarContraseña()
    {
        using (bd_storage db = new())
        {
            WriteLine("Write your ID: ");
            string username = ReadNonEmptyLine();

            string[] tables = { "students", "professors", "storers", "coordinators" };

            foreach (string tableName in tables)
            {
                string idUser = "";
                switch (tableName)
                {
                    case "students":
                        idUser = "studentId";
                        break;
                    case "professors":
                        idUser = "professorId";
                        break;
                    case "storers":
                        idUser = "storerId";
                        break;
                    case "coordinators":
                        idUser = "coordinatorId";
                        break;
                    default:
                        idUser = "";
                        break;
                }

                // query to find user id                
                if (idUser == "studentId")
                {
                    // SELECT * FROM [UserTable] WHERE [UserTablePK] = UsernameInput
                    IQueryable<Student> students = db.Students.Where(s => s.StudentId == username);
                    // checks if there are any registers with the desired ID
                    if (students is not null && students.Any())
                    {
                        var firstStudent = students.First();
                        if (firstStudent.StudentId != null)
                        {
                            WriteLine($"StudentID : {firstStudent.StudentId} - Password : {Decrypt(firstStudent.Password)}");
                        }
                        else
                        {
                            WriteLine("Student ID is null");
                        }
                    }
                }
                else if (idUser == "professorId")
                {
                    IQueryable<Professor>? professors = db.Professors.Where(p => p.ProfessorId == EncryptPass(username));

                    if (professors is not null || professors.Any())
                    {
                        foreach (var professor in professors)
                        {
                            WriteLine($"ProfessorID : {Decrypt(professor.ProfessorId)} - Password : {Decrypt(professor.Password)}");
                        }
                    }
                }
                else if (idUser == "storerId")
                {
                    IQueryable<Storer>? storers = db.Storers.Where(s => s.StorerId == EncryptPass(username));

                    if (storers is not null || storers.Any())
                    {
                        foreach (var storer in storers)
                        {
                            WriteLine($"StorerID : {Decrypt(storer.StorerId)} - Password : {Decrypt(storer.Password)}");
                        }
                    }
                }
                else if (idUser == "coordinatorId")
                {
                    IQueryable<Coordinator>? coordinators = db.Coordinators
                    .Where(c => c.CoordinatorId == EncryptPass(username));
                    if (coordinators is not null || coordinators.Any())
                    {
                        foreach (var coordinator in coordinators)
                        {
                            WriteLine($"CoordinatorId : {Decrypt(coordinator.CoordinatorId)} - Password : {Decrypt(coordinator.Password)}");
                        }
                    }
                }
            }
        }
    }
}