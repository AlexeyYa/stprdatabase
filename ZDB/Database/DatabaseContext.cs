using System.Data.Entity;

namespace ZDB.Database
{
    class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DefaultConnection") { }

        public DbSet<Entry> Entries { get; set; }
    }
}
