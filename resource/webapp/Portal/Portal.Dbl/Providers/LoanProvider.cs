using System.Data;
using System.Data.SqlClient;
using Dapper;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Portal.Core.Providers;
using Portal.Models;

namespace Portal.Dbl.Providers;

public class LoanProvider : ILoanProvider
{
    private readonly string _connectionString_;
    private readonly ILogger _logger_;

    public LoanProvider(
        String connectionString,
        ILogger logger)
    {
        _connectionString_ = connectionString;
        _logger_ = logger;
    }

    public async Task<Result<List<LoanWithPersons>>> GetAllLoansWithPersonsAsync()
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();
            var loans = await dbConnection.QueryAsync<LoanDto>("SELECT * FROM Loan");

            var loansWithPersons = new List<LoanWithPersons>();
            foreach (var loan in loans)
            {
                var personIds = await dbConnection.QueryAsync<Guid>(
                    "SELECT PersonId FROM PersonToLoan WHERE LoanId = @LoanId",
                    new { LoanId = loan.Id }
                );
                var persons = new List<PersonDto>();
                foreach (var personId in personIds)
                {
                    var person = await dbConnection.QueryFirstOrDefaultAsync<PersonDto>(
                        "SELECT * FROM Person WHERE Id = @Id",
                        new { Id = personId }
                    );
                    if (person != null)
                    {
                        persons.Add(person);
                    }
                }
                loansWithPersons.Add(new LoanWithPersons { LoanDto = loan, Persons = persons });
            }
            return loansWithPersons;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllLoansWithPersonsAsync)}");
            return new Result<List<LoanWithPersons>>(e);
        }
    }

    public async Task<Result<LoanWithPersons>> GetLoanWithPersonsByIdAsync(Guid loanId)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();
            var loan = await dbConnection.QueryFirstOrDefaultAsync<LoanDto>("SELECT * FROM Loan where Id = @LoanId", new { LoanId = loanId });

            var personIds = await dbConnection.QueryAsync<Guid>(
                "SELECT PersonId FROM PersonToLoan WHERE LoanId = @LoanId",
                new { LoanId = loanId }
            );
            var persons = new List<PersonDto>();
            foreach (var personId in personIds)
            {
                var person = await dbConnection.QueryFirstOrDefaultAsync<PersonDto>(
                    "SELECT * FROM Person WHERE Id = @Id",
                    new { Id = personId }
                );
                if (person != null)
                {
                    persons.Add(person);
                }
            }

            return new LoanWithPersons()
            {
                LoanDto = loan,
                Persons = persons
            };
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetLoanWithPersonsByIdAsync)}");
            return new Result<LoanWithPersons>(e);
        }
    }

    public async Task<Result<IEnumerable<LoanWithPersons>>> GetLoansByPersonIdAsync(Guid personId)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();

            var loanIds = await dbConnection.QueryAsync<Guid>(
                "SELECT DISTINCT(LoanId) FROM PersonToLoan WHERE PersonId = @PersonId",
                new { PersonId = personId }
            );
            var loans = new List<LoanWithPersons>();
            foreach (var loanId in loanIds)
            {
                var loan = await dbConnection.QueryFirstOrDefaultAsync<LoanDto>(
                    "SELECT * FROM Loan WHERE Id = @Id",
                    new { Id = loanId }
                );
                if (loan != null)
                {
                    loans.Add(
                        new LoanWithPersons()
                        {
                            LoanDto = loan,
                            Persons = null
                        });
                }
            }

            return loans;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetLoansByPersonIdAsync)}");
            return new Result<IEnumerable<LoanWithPersons>>(e);
        }
    }

    public async Task<Result<LoanDto>> CreateLoanAsync(LoanDto loanDto, IEnumerable<Guid> personIds)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString_);
        dbConnection.Open();
        using var transaction = dbConnection.BeginTransaction();
        try
        {
            await dbConnection.ExecuteAsync(
                "INSERT INTO Loan (LoanBaseAmount, Purpose, DurationInDays, StartDatetimeUtc, Interest, LoanTotalAmount, IsApproved) " +
                "VALUES (@LoanBaseAmount, @Purpose, @DurationInDays, @StartDatetimeUtc, @Interest, @LoanTotalAmount, @IsApproved)",
                loanDto,
                transaction
            );
            
            var createdLoan = await dbConnection.QueryFirstOrDefaultAsync<LoanDto>(
                "SELECT TOP 1 * FROM Loan ORDER BY StartDatetimeUtc DESC",
                transaction: transaction
            );

            foreach (var personId in personIds)
            {
                await dbConnection.ExecuteAsync(
                    "INSERT INTO PersonToLoan (PersonId, LoanId) " +
                    "VALUES (@PersonId, @LoanId)",
                    new { PersonId = personId, LoanId = createdLoan.Id },
                    transaction
                );
            }

            transaction.Commit();
            return createdLoan;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger_.LogError(e, $"Exception on {nameof(CreateLoanAsync)}");
            return new Result<LoanDto>(e);
        }
    }

    public async Task<Result<Boolean>> SetLoanToApprovedAsync(Guid loanId)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();
            await dbConnection.ExecuteAsync(
                "UPDATE Loan SET IsApproved = 1 " +
                "WHERE Id = @Id",
                new { Id = loanId }
            );
            return true;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(SetLoanToApprovedAsync)}");
            return new Result<Boolean>(e);
        }
    }
}