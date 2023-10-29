CREATE PROCEDURE CleanupIntegrationTestData
AS
BEGIN
    DECLARE @PersonId UNIQUEIDENTIFIER
    SELECT @PersonId = Id FROM Person WHERE [FullName] = 'Test'

    DECLARE @LoanIds TABLE (Id UNIQUEIDENTIFIER)
    INSERT INTO @LoanIds (Id)
    SELECT LoanId FROM PersonToLoan WHERE PersonId = @PersonId

    DELETE FROM Transact WHERE PersonId = @PersonId
    DELETE FROM PersonToLoan WHERE PersonId = @PersonId
    DELETE FROM Person WHERE Id = @PersonId
    DELETE FROM LoanLog WHERE LoanId IN (SELECT Id FROM @LoanIds)
    DELETE FROM Loan WHERE Id IN (SELECT Id FROM @LoanIds)
END