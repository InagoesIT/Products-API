using Microsoft.EntityFrameworkCore;
using ProductsAPI.Entities;

namespace ProductsAPI.Data;

public class DatabaseContext : DbContext
{
    private static string DEFAULT_DATABASE_NAME = "products";
    private readonly string DatabaseName;
    public virtual DbSet<Product> Products { get; set; }

    public DatabaseContext()
    {
        this.DatabaseName = DEFAULT_DATABASE_NAME;
        this.Database.EnsureCreated();
    }

    public DatabaseContext(string databaseName)
    {
        this.DatabaseName = databaseName;
        this.Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // * connection string builder, helps with sql server
        optionsBuilder.UseSqlite($"Data Source = {this.DatabaseName}.db");
    }
}