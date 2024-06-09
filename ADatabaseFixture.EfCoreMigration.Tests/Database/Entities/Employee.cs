namespace ADatabaseFixture.EfCoreMigration.Tests.Database.Entities;

public class Employee
{
    public required int Id { get; init; }

    public required int PersonId { get; init; }
    public Person? Person { get; init; }

    public int DepartmentId { get; init; }
    public Department? Department { get; init; }
}
