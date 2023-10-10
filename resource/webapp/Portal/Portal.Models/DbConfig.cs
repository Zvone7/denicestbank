namespace Portal.Models;

public class DbConfig
{
    public String ConnectionString { get; set; }
    public DbConfig(String connectionString)
    {
        ConnectionString = connectionString;
    }
}