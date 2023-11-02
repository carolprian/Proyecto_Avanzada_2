using Microsoft.EntityFrameworkCore;
namespace Project_Storage;

public class bd_storage: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string path = Path.Combine(Environment.CurrentDirectory, "bd_storage.db");
        string connection = $"Filename={path}";
        ConsoleColor backgroundColor = ForegroundColor;
        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine($"Connection : {connection}");
        ForegroundColor = backgroundColor;
        //using the db motor
        optionsBuilder.UseSqlite(connection);
        // optionsBuilder.LogTo(WriteLine).EnableSensitiveDataLogging();
                

        base.OnConfiguring(optionsBuilder);
    }

/*
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
    }
*/

}

