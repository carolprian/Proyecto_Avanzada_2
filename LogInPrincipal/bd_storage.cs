using Microsoft.EntityFrameworkCore;
namespace LogInPrincipal;

public class bd_storage : DbContext
{
    public object RequestDetails { get; internal set; }
    public IEnumerable<object> Students { get; internal set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string path = Path.Combine(Environment.CurrentDirectory, "bd_storage.db");
        string connection = $"Filename={path}";
        ConsoleColor backgoundColor = ForegroundColor;
        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine($"Connection : {connection}");
        ForegroundColor = backgoundColor;
        // Use The DB Motor
        optionsBuilder.UseSqlite(connection);
    }
}