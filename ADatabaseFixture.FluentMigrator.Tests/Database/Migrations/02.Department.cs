using FluentMigrator;

namespace ADatabaseFixture.FluentMigrator.Tests.Database.Migrations;

[Migration(2)]
public class CreateDepartmentTable : Migration
{
    public override void Up()
    {
        Create.Table("Department")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Department");
    }
}
