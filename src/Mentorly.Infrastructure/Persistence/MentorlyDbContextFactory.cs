using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mentorly.Infrastructure.Persistence;

public class MentorlyDbContextFactory : IDesignTimeDbContextFactory<MentorlyDbContext>
{
    public MentorlyDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        // Si estamos en el directorio Infrastructure, subir al UI
        if (basePath.EndsWith("Mentorly.Infrastructure"))
        {
            basePath = Path.Combine(basePath, "..", "Mentorly.UI");
        }
        // Si estamos en la raíz de la solución, ir a UI
        else if (Directory.Exists(Path.Combine(basePath, "src", "Mentorly.UI")))
        {
            basePath = Path.Combine(basePath, "src", "Mentorly.UI");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<MentorlyDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new MentorlyDbContext(optionsBuilder.Options);
    }
}
