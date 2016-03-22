using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
        public DbSet<ItemModName> ItemModNames { get; set; }

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
            modelBuilder.Entity<StashTab>().HasMany(e => e.Items).WithOne(e => e.StashTab).HasForeignKey(e => e.StashTabId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Item>().HasKey(e => e.Id);
            modelBuilder.Entity<Item>().HasMany(e => e.ImplicitMods).WithOne(e => e.ItemImplicit).HasForeignKey(e => e.ItemImplicitId);

            modelBuilder.Entity<ItemWithExplicitMods>().HasBaseType<Item>();
            modelBuilder.Entity<ItemWithExplicitMods>().HasMany(e => e.ExplicitMods).WithOne(e => e.ItemExplicit).HasForeignKey(e => e.ItemExplicitId).IsRequired().IsRequired().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UniqueItem>().HasBaseType<ItemWithExplicitMods>();

            modelBuilder.Entity<ItemMod>().HasKey(e => new { e.ItemId, e.Index });
            modelBuilder.Entity<ItemMod>().HasOne(e => e.ModName).WithMany(e => e.ItemMods).HasForeignKey(e => e.ModNameId);

            modelBuilder.Entity<ItemModName>().HasKey(e => e.Id);
            modelBuilder.Entity<ItemModName>().Property(e => e.Id).ValueGeneratedNever();
        }
    }
}
