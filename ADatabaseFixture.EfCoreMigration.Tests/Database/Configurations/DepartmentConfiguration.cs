using ADatabaseFixture.EfCoreMigration.Tests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADatabaseFixture.EfCoreMigration.Tests.Database.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
    }
}
