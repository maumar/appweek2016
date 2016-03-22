using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IndexerModel
{
    public class IndexerPoeSniperContext : DbContext
    {
        public DbSet<IndexerAccount> Accounts { get; set; }
        public DbSet<IndexerFeedChunk> FeedChunks { get; set; }
        public DbSet<IndexerFeedChunkAccount> FeedChunkAccounts { get; set; }
        public DbSet<IndexerItem> Items { get; set; }
        public DbSet<IndexerItemMod> ItemMods { get; set; }
        public DbSet<IndexerItemModName> ItemModNames { get; set; }
        public DbSet<IndexerStashTab> StashTabs { get; set; }

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
            modelBuilder.Entity<IndexerAccount>().ToTable("Accounts");
            modelBuilder.Entity<IndexerAccount>().HasKey(e => e.AccountName);

            modelBuilder.Entity<IndexerFeedChunk>().ToTable("FeedChunks");
            modelBuilder.Entity<IndexerFeedChunk>().HasKey(e => new { e.ChunkId, e.Index });

            modelBuilder.Entity<IndexerFeedChunkAccount>().ToTable("FeedChunkAccounts");
            modelBuilder.Entity<IndexerFeedChunkAccount>().HasKey(e => new { e.ChunkId, e.Index, e.AccountName });

            modelBuilder.Entity<IndexerStashTab>().ToTable("StashTabs");
            modelBuilder.Entity<IndexerStashTab>().HasKey(e => e.Id);

            modelBuilder.Entity<IndexerItem>().ToTable("Items");
            modelBuilder.Entity<IndexerItem>().HasKey(e => e.Id);
            modelBuilder.Entity<IndexerItem>().HasDiscriminator().HasValue("Item");

            modelBuilder.Entity<IndexerItemWithExplicitMods>().HasBaseType<IndexerItem>().HasDiscriminator().HasValue("ItemWithExplicitMods");
            modelBuilder.Entity<IndexerUniqueItem>().HasBaseType<IndexerItemWithExplicitMods>().HasDiscriminator().HasValue("UniqueItem");

            modelBuilder.Entity<IndexerItemMod>().ToTable("ItemMods");
            modelBuilder.Entity<IndexerItemMod>().HasKey(e => new { e.ItemId, e.Index });

            modelBuilder.Entity<IndexerItemModName>().ToTable("ItemModNames");
            modelBuilder.Entity<IndexerItemModName>().HasKey(e => e.Id);
            modelBuilder.Entity<IndexerItemModName>().Property(e => e.Id).ValueGeneratedNever();
        }
    }
}
