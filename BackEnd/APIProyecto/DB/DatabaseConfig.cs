namespace APIProyecto.DB;

public static class DatabaseConfig
{
    public static string BuildConnectionString(IConfiguration configuration)
    {
        var section = configuration.GetSection("Database");
        var password = configuration["Database:Password"];

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Database password not configured. Set 'Database:Password' via User Secrets or environment variables.");
        }

        return $"Host={section["Host"]};" +
               $"Port={section["Port"]};" +
               $"Database={section["Name"]};" +
               $"Username={section["Username"]};" +
               $"Password={password}";
    }
}