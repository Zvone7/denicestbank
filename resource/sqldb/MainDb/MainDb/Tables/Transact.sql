CREATE TABLE Transact
(
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PersonId            UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [Person](Id),
    LoanId              UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Loan(Id),
    CreatedBy           UNIQUEIDENTIFIER,
    CreatedDateTimeUtc  DATETIME2      NOT NULL,
    Amount              DECIMAL(18, 5) NOT NULL
);