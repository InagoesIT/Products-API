using Microsoft.EntityFrameworkCore;
using ProductsAPI.Entities;

namespace ProductsAPI.Data;

public class DatabaseContext : DbContext
{
    private static string DEFAULT_DATABASE_NAME = "Products";
    private readonly string databaseName;
    public virtual DbSet<Product> products { get; set; }

    public DatabaseContext()
    {
        this.databaseName = DEFAULT_DATABASE_NAME;
        this.Database.EnsureCreated();
    }

    public DatabaseContext(string databaseName)
    {
        this.databaseName = databaseName;
        this.Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // * connection string builder, helps with sql server
        optionsBuilder.UseSqlite($"Data Source = {this.databaseName}.db");
    }
}