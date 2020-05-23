CREATE TABLE Employee (
	Id INT IDENTITY PRIMARY KEY,
    PersonId INT NOT NULL,
    DepartmentId INT NOT NULL,
    CONSTRAINT FK_Employee_Person FOREIGN KEY (PersonId) REFERENCES Person(Id),
    CONSTRAINT FK_Employee_Department FOREIGN KEY (DepartmentId) REFERENCES Department(Id)
)