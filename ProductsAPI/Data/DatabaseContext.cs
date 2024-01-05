using Microsoft.EntityFrameworkCore;
using ProductsAPI.Entities;

namespace ProductsAPI.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Product> products { get; set; }

    public DatabaseContext()
    {
        this.Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // * connection string builder, helps with sql server
        optionsBuilder.UseSqlite("Data Source = Products.db");
    }
}
