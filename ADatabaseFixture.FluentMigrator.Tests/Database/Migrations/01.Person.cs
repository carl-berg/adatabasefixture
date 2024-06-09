using FluentMigrator;

namespace ADatabaseFixture.FluentMigrator.Tests.Database.Migrations;

[Migration(1)]
public class CreatePersonTable : Migration
{
    public override void Up()
    {
        Create.Table("Person")
              .WithColumn("Id").AsInt32().PrimaryKey().Identity()
              .WithColumn("Name").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Person");
    }
}
