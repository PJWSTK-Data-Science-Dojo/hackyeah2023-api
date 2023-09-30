using Microsoft.EntityFrameworkCore;

namespace HackYeah_API.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

}