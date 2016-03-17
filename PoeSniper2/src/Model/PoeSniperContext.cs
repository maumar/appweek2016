using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class PoeSniperContext : DbContext
    {
        public DbSet<ItemFeedChunk> ItemFeedChunks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = ".",
                InitialCatalog = "PoeSniper",
                MultipleActiveResultSets = true,
                IntegratedSecurity = true
            }.ToString();

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
