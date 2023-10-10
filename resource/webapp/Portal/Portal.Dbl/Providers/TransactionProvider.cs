using System.Data;
using System.Data.SqlClient;
using Dapper;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Portal.Core.Providers;
using Portal.Models;

namespace Portal.Dbl.Providers;

public class TransactionProvider : ITransactionProvider
{
    private readonly String _connectionString_;
    private readonly ILogger<ITransactionProvider> _logger_;

    public TransactionProvider(
        DbConfig dbConfig,
        ILogger<ITransactionProvider> logger)
    {
        _connectionString_ = dbConfig.ConnectionString;
        _logger_ = logger;
    }

    public async Task<Result<IEnumerable<LoanLatestState>>> GetAllLoansLatestStates()
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();
            var res= await dbConnection.QueryAsync<LoanLatestState>($@"
            SELECT LoanId, SUM(Amount) AS TotalTransacted
            FROM Transact
            GROUP BY LoanId;");
            return new Result<IEnumerable<LoanLatestState>>(res);
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllLoansLatestStates)}.");
            return new Result<IEnumerable<LoanLatestState>>(e);
        }
    }

    public async Task<Result<TransactDto>> InsertTransactionAsync(TransactDto transaction)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();
            var query =
                "INSERT INTO Transact (PersonId, LoanId, UpdateDatetimeUtc, Amount) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@PersonId, @LoanId, @UpdateDatetimeUtc, @Amount)";
            var id = await dbConnection.ExecuteScalarAsync<Guid>(query, transaction);
            transaction.Id = id;
            return transaction;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(InsertTransactionAsync)}.");
            return new Result<TransactDto>();
        }
    }

    public async Task<Result<IEnumerable<PaymentVm>>> GetAllEnrichedTransactions()
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();
            var query = @"
                select 
                    t.Id,
                    t.PersonId,
                    t.LoanId,
                    t.UpdateDatetimeUtc,
                    t.Amount,
                    l.Purpose as LoanPurpose,
                    p.FullName as PersonFullName
                 FROM
                Transact as t
                inner join Loan as l on t.LoanId = l.Id
                inner join Person as p on t.PersonId = p.Id
                order by t.UpdateDatetimeUtc DESC";

            return new Result<IEnumerable<PaymentVm>>(await dbConnection.QueryAsync<PaymentVm>(query));
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllEnrichedTransactions)}.");
            return new Result<IEnumerable<PaymentVm>>(e);
        }

    }
}