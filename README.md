# denicestbank


## database description

### table Person:
Id - Guid, primary key
FullName - string 100
email - string 100
roleId - foreign key to Role, int
ssn - string 11

### table Borower:
Id - Guid, primary key
PersonId - Guid, foreign key to Person
YearlySalary - decimal number, 5 digits of precision
CurrentEquity - decimal number, 5 digits of precision

### table Role:
Id - primary key, int
Description - string 30

#### values filled out during creation
1 - admin
2 - Person
3 - Adviser

### table Loan:
Id - guid, primary key
LoanBaseAmount - decimal number, 5 digits of precision
StartDatetimeUtc - datetime2
Interest - decimalnumber, 5 digits of precision
LoanTotalAmount - decimalnumber, 5 digits of precision
IsApproved - boolean


### table PersonToLoan:
Id - int, primary key
PersonId - guid, foreign key to Person
LoanId - guid, foreign key to Loan

### table Transaction:
Id - guid, primary key
PersonId - guid, foreign key to Person
UpdateDatetimeUtc - datetime2
Amount - decimalnumber, 5 digits of precision