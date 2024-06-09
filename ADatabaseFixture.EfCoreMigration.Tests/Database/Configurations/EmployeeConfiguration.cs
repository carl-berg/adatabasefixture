using ADatabaseFixture.EfCoreMigration.Tests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADatabaseFixture.EfCoreMigration.Tests.Database.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employee");
        builder.HasOne(x => x.Person);
        builder.HasOne(x => x.Department);
    }
}
