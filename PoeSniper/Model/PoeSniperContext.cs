using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class PoeSniperContext : DbContext
    {
        public DbSet<FeedChunk> FeedChunks { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<FeedChunkAccount> FeedChunkAccounts { get; set; }
        public DbSet<StashTab> StashTabs { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemMod> ItemMods { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = ".",
                InitialCatalog = "PoeSniperNew",
                MultipleActiveResultSets = true,
                IntegratedSecurity = true
            }.ToString();

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedChunk>().HasKey(e => new { e.ChunkId, e.Index });
            modelBuilder.Entity<FeedChunk>().HasMany(e => e.FeedChunkAccounts).WithOne(e => e.Chunk).HasForeignKey(e => new { e.ChunkId, e.Index });

            modelBuilder.Entity<FeedChunkAccount>().HasKey(e => new { e.ChunkId, e.Index, e.AccountName });

            modelBuilder.Entity<Account>().HasKey(e => e.AccountName);
            modelBuilder.Entity<Account>().HasMany(e => e.ChunkAccounts).WithOne(e => e.Account).HasForeignKey(e => e.AccountName);
            modelBuilder.Entity<Account>().HasMany(e => e.StashTabs).WithOne(e => e.Account).HasForeignKey(e => e.AccountName);

            modelBuilder.Entity<StashTab>().HasKey(e => new { e.Id });
            modelBuilder.Entity<StashTab>().HasMany(e => e.Items).WithOne().HasForeignKey(e => e.StashTabId);

            modelBuilder.Entity<Item>().HasKey(e => e.Id);
            modelBuilder.Entity<Item>().HasMany(e => e.ImplicitMods).WithOne(e => e.ItemImplicit).HasForeignKey(e => e.ItemImplicitId);

            modelBuilder.Entity<ItemWithExplicitMods>().HasBaseType<Item>();
            modelBuilder.Entity<ItemWithExplicitMods>().HasMany(e => e.ExplicitMods).WithOne(e => e.ItemExplicit).HasForeignKey(e => e.ItemExplicitId);

            modelBuilder.Entity<UniqueItem>().HasBaseType<ItemWithExplicitMods>();
        }
    }
}
