using System.Data;
using System.Data.SqlClient;
using Dapper;
using Portal.Models;

namespace Portal.Dbl.Providers;

public class PersonProvider
{
    private readonly string _connectionString;

    public PersonProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();
        return await dbConnection.QueryAsync<PersonDto>("SELECT * FROM Person");
    }

    public async Task<PersonDto> GetPersonByIdAsync(Guid personId)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();
        return await dbConnection.QueryFirstOrDefaultAsync<PersonDto>("SELECT * FROM Person WHERE Id = @Id", new { Id = personId });
    }

    public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();
        var query = "INSERT INTO Person (Id, FullName, Email, Role, Ssn) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@Id, @FullName, @Email, @Role, @Ssn)";
        var id = await dbConnection.ExecuteScalarAsync<Guid>(query, personDto);
        personDto.Id = id;
        return personDto;
    }

    // public async Task<Person> UpdatePersonAsync(Person person)
    // {
    //     using IDbConnection dbConnection = new SqlConnection(_connectionString);
    //     dbConnection.Open();
    //     await dbConnection.ExecuteAsync(
    //         "UPDATE Person SET FullName = @FullName, Email = @Email, RoleId = @RoleId, Ssn = @Ssn " +
    //         "WHERE Id = @Id",
    //         person
    //     );
    //     return person;
    // }

    // public async Task DeletePersonAsync(Guid id)
    // {
    //     using IDbConnection dbConnection = new SqlConnection(_connectionString);
    //     dbConnection.Open();
    //     await dbConnection.ExecuteAsync("DELETE FROM Person WHERE Id = @Id", new { Id = id });
    // }
}