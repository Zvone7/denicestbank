CREATE TABLE Loan
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LoanBaseAmount   DECIMAL(18, 5) NOT NULL,
    DurationInDays   INT            NOT NULL,
    StartDatetimeUtc DATETIME2      NOT NULL,
    Interest         DECIMAL(18, 5) NOT NULL,
    LoanTotalAmount  DECIMAL(18, 5) NOT NULL,
    IsApproved       BIT            NOT NULL
);