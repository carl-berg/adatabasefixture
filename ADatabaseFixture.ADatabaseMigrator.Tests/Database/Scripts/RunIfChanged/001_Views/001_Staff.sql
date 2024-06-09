CREATE OR ALTER VIEW Staff AS
    SELECT 
        Person.Id,
        Person.Name,
        Department.Name AS Department
    FROM Person
    INNER JOIN Employee ON Employee.PersonId = Person.Id
    INNER JOIN Department ON Department.Id = Employee.DepartmentId