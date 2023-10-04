# denicestbank

# Solution Description

Solution consists of 3 components
1. SQL Database hosted in Azure
2. Portal API written in C# hosted in Azure App Service
3. Transaction Generator Logic Function running as a cron job


# Component description - SQL Database

Database is an Azure SQL Database. It consists of following tables with schema.

### table Person:
Id - Guid, primary key
AadId - Guid, Azure Active Directory ID
FullName - string 100
email - string 100
role - string 20
ssn - string 11

### table Borower:
Id - Guid, primary key
PersonId - Guid, foreign key to Person
YearlySalary - decimal number, 5 digits of precision
CurrentEquity - decimal number, 5 digits of precision

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

# Component description - Portal API

Portal API is an MVC application written in net7.0.
It is hosted in Azure App Service. It uses Service Prinicipal registered through App Registration to identify towards the Azure Active Directory as well as the Azure SQL Database.
It has a simple UI interface on top of it, protected by the Azure AD Login.

Upon user loging in with their AAD user, a profile is generated in the database. For now, we decide if a user is a customer or an adviser based on their name. 

Main interraction is meant to happen through Customer and Adviser view.

## Customer view

Customers can apply for a loan by selecting two values - Loan Base Amount and Interest Over Years.

After applying for a loan, they can view the state of (only) their loans in the Customer View.

## Adviser view

Advisers are considered to be "administrators" in the bank. They have the ability to view All the loans and approve them.

Once the loan is approved, and Customer has made transactions towards it, they can also see how much of the loan has been paid off.


# Component description - Logic Function

Logic function is a time trigerred console application written in net7.0, hosted as an Azure Function. It is named transactiongenerator.

It is authenticated using a Service Principal registered through App Registration to identify towards Active Directory and Azure SQL Database.

It triggers once an hour and it randomly generates one transaction per loan, emulating users paying off loans.

Flow of the logic function is like this:
    1. Connect to the database and retrieve all loans.
    2. Filter out loans which are not approved
    3. Filter out loans which are paid off 90% or more.
    4. For each Person connected with a loan, generate a random transaction in amount 0.01% - 0.09% of the base amount.
    5. Attempt to commit those transactions towards the Portal API.
Logic Function will never generate a transaction larger than 0.1% of total amount of the loan. It will also not generate a transaction if a loan has been paid off more than 90%.