using System.Data;
using System.Data.SqlClient;
using Dapper;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Portal.Core.Providers;
using Portal.Models;

namespace Portal.Dbl.Providers;

public class PersonProvider : IPersonProvider
{
    private readonly String _connectionString_;
    private readonly ILogger<IPersonProvider> _logger_;

    public PersonProvider(DbConfig dbConfig, ILogger<IPersonProvider> logger)
    {
        _connectionString_ = dbConfig.ConnectionString;
        _logger_ = logger;
    }

    public async Task<Result<PersonDto>> GetPersonByIdAsync(Guid personId)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();
            return new Result<PersonDto>(await dbConnection.QueryFirstOrDefaultAsync<PersonDto>("SELECT * FROM Person WHERE Id = @Id", 
                new { Id = personId }));
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetPersonByIdAsync)}.");
            return new Result<PersonDto>(e);
        }
    }
    
    public async Task<Result<PersonDto>> CreatePersonAsync(PersonDto personDto)
    {
        try
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString_);
            dbConnection.Open();
            var query = "INSERT INTO Person (Id, FullName, Email, Role, Ssn) " +
                        "OUTPUT INSERTED.Id " +
                        "VALUES (@Id, @FullName, @Email, @Role, @Ssn)";
            var id = await dbConnection.ExecuteScalarAsync<Guid>(query, personDto);
            personDto.Id = id;
            return new Result<PersonDto>(personDto);
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(CreatePersonAsync)}.");
            return new Result<PersonDto>(e);
        }
    }

}