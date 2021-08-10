﻿using ADatabaseFixture.GalacticWasteManagement.Tests.Core;
using Dapper;
using DataDude;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace ADatabaseFixture.GalacticWasteManagement.Tests
{
    public class GalacticWasteManagementTests : DatabaseTest
    {
        public GalacticWasteManagementTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task CanPersistAndFetchEmployee()
        {
            var connection = Fixture.CreateNewConnection();

            await Dude
                .Insert("Department", new { Name = "IT" })
                .Insert("Person", new { Name = "John Doe" })
                .Insert("Employee")
                .Go(connection);

            var employee = await connection.QuerySingleAsync<Employee>(@"
                SELECT
                    Employee.Id,
                    Person.Name,
                    Department.Name as Department
                FROM Employee
                INNER JOIN Person ON Person.Id = PersonId
                INNER JOIN Department ON Department.Id = DepartmentId");

            employee.Name.ShouldBe("John Doe");
            employee.Department.ShouldBe("IT");
        }

        private class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Department { get; set; }
        }
    }
}
