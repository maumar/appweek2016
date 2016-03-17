using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class PoeSniperContext : DbContext
    {
        public DbSet<ItemFeedChunk> ItemFeedChunks { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ItemFeedChunkAccounts> ChunkAccounts { get; set; }
        public DbSet<Stash> Stashes { get; set; }
        public DbSet<StashTab> StashTabs { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemMod> ItemMods { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = ".",
                InitialCatalog = "PoeSniper6",
                MultipleActiveResultSets = true,
                IntegratedSecurity = true
            }.ToString();

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemFeedChunk>().HasKey(e => new { e.ChunkId, e.Index });

            modelBuilder.Entity<Account>().HasKey(e => e.AccountName);

            modelBuilder.Entity<Stash>().HasKey(e => new { e.AccountName, e.League });
            modelBuilder.Entity<Stash>().HasOne(e => e.Account).WithMany(e => e.Stashes).HasForeignKey("AccountName");
            modelBuilder.Entity<Stash>().HasMany(e => e.StashTabs).WithOne(e => e.Stash).HasForeignKey("AccountName", "League");

            modelBuilder.Entity<StashTab>().HasKey(e => new { e.Id });
            modelBuilder.Entity<StashTab>().HasMany(e => e.Items).WithOne().HasForeignKey("StashTabId").IsRequired();

            modelBuilder.Entity<Item>().HasKey(e => new { e.Id, e.Name });
            modelBuilder.Entity<MagicalItem>().HasBaseType<Item>();
            modelBuilder.Entity<UniqueItem>().HasBaseType<MagicalItem>();

            modelBuilder.Entity<ItemFeedChunkAccounts>().HasKey(e => new { e.ChunkId, e.Index, e.AccountName });
        }
    }
}
