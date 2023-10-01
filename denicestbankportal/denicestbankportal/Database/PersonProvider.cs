using System.Data;
using System.Data.SqlClient;
using Dapper;
using denicestbankportal.Models;

namespace denicestbankportal.Database;

public class PersonProvider
{
    private readonly string _connectionString;

    public PersonProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Person>> GetAllPersonsAsync()
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();
        return await dbConnection.QueryAsync<Person>("SELECT * FROM Person");
    }

    public async Task<Person> GetPersonByIdAsync(Guid id)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();
        return await dbConnection.QueryFirstOrDefaultAsync<Person>("SELECT * FROM Person WHERE Id = @Id", new { Id = id });
    }

    public async Task<Person> CreatePersonAsync(Person person)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();
        var query = "INSERT INTO Person (FullName, Email, RoleId, Ssn) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@FullName, @Email, @RoleId, @Ssn)";
        var id = await dbConnection.ExecuteScalarAsync<Guid>(query, person);
        person.Id = id;
        return person;
    }

    public async Task<Person> UpdatePersonAsync(Person person)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();
        await dbConnection.ExecuteAsync(
            "UPDATE Person SET FullName = @FullName, Email = @Email, RoleId = @RoleId, Ssn = @Ssn " +
            "WHERE Id = @Id",
            person
        );
        return person;
    }

    public async Task DeletePersonAsync(Guid id)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();
        await dbConnection.ExecuteAsync("DELETE FROM Person WHERE Id = @Id", new { Id = id });
    }
}