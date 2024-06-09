using FluentMigrator;

namespace ADatabaseFixture.FluentMigrator.Tests.Database.Migrations;

[Migration(3)]
public class CreateEmployeeTable : Migration
{
    public override void Up()
    {
        Create.Table("Employee")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("PersonId").AsInt32().NotNullable().ForeignKey("FK_Employee_Person", "Person", "Id")
            .WithColumn("DepartmentId").AsInt32().NotNullable().ForeignKey("FK_Employee_Department", "Department", "Id");
    }

    public override void Down()
    {
        Delete.Table("Employee");
    }
}
