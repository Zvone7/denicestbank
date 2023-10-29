Create Table LoanLog
(
    Id                  INT PRIMARY KEY IDENTITY(1,1),
    LoanId              UNIQUEIDENTIFIER NOT NULL FOREIGN KEY (LoanId) REFERENCES Loan(Id),
    CreatedBy           UNIQUEIDENTIFIER,
    FieldName           NVARCHAR(100)    NOT NULL,
    NewFieldValue       NVARCHAR(100)    NOT NULL,
    CreatedDatetimeUtc  DATETIME2        NOT NULL
);