using ADatabaseFixture.EfCoreMigration.Tests.Database.Configurations;
using ADatabaseFixture.EfCoreMigration.Tests.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ADatabaseFixture.EfCoreMigration.Tests.Database;

public class StaffDbContext(DbContextOptions<StaffDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonConfiguration).Assembly);
    }
}
