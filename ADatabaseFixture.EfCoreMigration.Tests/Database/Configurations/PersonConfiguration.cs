using ADatabaseFixture.EfCoreMigration.Tests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADatabaseFixture.EfCoreMigration.Tests.Database.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Person");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
    }
}
