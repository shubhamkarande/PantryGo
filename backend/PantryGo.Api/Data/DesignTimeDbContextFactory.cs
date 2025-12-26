using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace PantryGo.Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = configuration["Database:Host"] ?? "localhost",
            Port = int.Parse(configuration["Database:Port"] ?? "5432"),
            Database = configuration["Database:Name"] ?? "pantrygo",
            Username = configuration["Database:Username"] ?? "postgres",
            Password = configuration["Database:Password"] ?? "postgres"
        };

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connStringBuilder.ConnectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
