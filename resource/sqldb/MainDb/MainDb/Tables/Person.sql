CREATE TABLE Person
(
    Id       UNIQUEIDENTIFIER PRIMARY KEY   NOT NULL,
    FullName NVARCHAR(100)                  NOT NULL,
    Email    NVARCHAR(100)  UNIQUE          NOT NULL,
    [Role]   NVARCHAR(20)                   NOT NULL,
    Ssn      NVARCHAR(11)   UNIQUE          NOT NULL
);