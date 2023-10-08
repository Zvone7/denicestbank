using System.Data;
using System.Data.SqlClient;
using Dapper;
using Portal.Core.Providers;
using Portal.Models;

namespace Portal.Dbl.Providers;

public class PersonProvider : IPersonProvider
{
    private readonly string _connectionString_;

    public PersonProvider(string connectionString)
    {
        _connectionString_ = connectionString;
    }

    public async Task<PersonDto> GetPersonByIdAsync(Guid personId)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString_);
        dbConnection.Open();
        return await dbConnection.QueryFirstOrDefaultAsync<PersonDto>("SELECT * FROM Person WHERE Id = @Id", new { Id = personId });
    }
    
    public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString_);
        dbConnection.Open();
        var query = "INSERT INTO Person (Id, FullName, Email, Role, Ssn) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@Id, @FullName, @Email, @Role, @Ssn)";
        var id = await dbConnection.ExecuteScalarAsync<Guid>(query, personDto);
        personDto.Id = id;
        return personDto;
    }

}