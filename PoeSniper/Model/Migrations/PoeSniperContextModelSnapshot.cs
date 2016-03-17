using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Model;

namespace Model.Migrations
{
    [DbContext(typeof(PoeSniperContext))]
    partial class PoeSniperContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20207")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Model.Account", b =>
                {
                    b.Property<string>("AccountName");

                    b.Property<string>("LastCharacterName");

                    b.HasKey("AccountName");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Model.Item", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Name");

                    b.Property<bool>("Corrupted");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("Ilvl");

                    b.Property<string>("StashTabId")
                        .IsRequired();

                    b.Property<string>("TypeLine");

                    b.HasKey("Id", "Name");

                    b.HasIndex("StashTabId");

                    b.ToTable("Items");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Item");
                });

            modelBuilder.Entity("Model.ItemFeedChunk", b =>
                {
                    b.Property<string>("ChunkId");

                    b.Property<int>("Index");

                    b.Property<string>("NextChunkId");

                    b.HasKey("ChunkId", "Index");

                    b.ToTable("ItemFeedChunks");
                });

            modelBuilder.Entity("Model.ItemMod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ItemId");

                    b.Property<string>("ItemName");

                    b.Property<string>("MagicalItemId");

                    b.Property<string>("MagicalItemName");

                    b.Property<int?>("NameId");

                    b.Property<decimal>("Value");

                    b.HasKey("Id");

                    b.HasIndex("NameId");

                    b.HasIndex("ItemId", "ItemName");

                    b.HasIndex("MagicalItemId", "MagicalItemName");

                    b.ToTable("ItemMod");
                });

            modelBuilder.Entity("Model.ItemModName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("ItemModName");
                });

            modelBuilder.Entity("Model.ItemRequirement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ItemId");

                    b.Property<string>("ItemName");

                    b.Property<string>("Name");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ItemId", "ItemName");

                    b.ToTable("ItemRequirement");
                });

            modelBuilder.Entity("Model.Stash", b =>
                {
                    b.Property<string>("AccountName");

                    b.Property<string>("League");

                    b.Property<string>("ItemFeedChunkChunkId");

                    b.Property<int?>("ItemFeedChunkIndex");

                    b.HasKey("AccountName", "League");

                    b.HasIndex("AccountName");

                    b.HasIndex("ItemFeedChunkChunkId", "ItemFeedChunkIndex");

                    b.ToTable("Stashes");
                });

            modelBuilder.Entity("Model.StashTab", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("AccountName");

                    b.Property<string>("League");

                    b.Property<string>("TabName");

                    b.HasKey("Id");

                    b.HasIndex("AccountName", "League");

                    b.ToTable("StashTabs");
                });

            modelBuilder.Entity("Model.MagicalItem", b =>
                {
                    b.HasBaseType("Model.Item");

                    b.Property<bool>("Identified");

                    b.ToTable("MagicalItem");

                    b.HasDiscriminator().HasValue("MagicalItem");
                });

            modelBuilder.Entity("Model.UniqueItem", b =>
                {
                    b.HasBaseType("Model.MagicalItem");

                    b.Property<string>("FlavorText");

                    b.ToTable("UniqueItem");

                    b.HasDiscriminator().HasValue("UniqueItem");
                });

            modelBuilder.Entity("Model.Item", b =>
                {
                    b.HasOne("Model.StashTab")
                        .WithMany()
                        .HasForeignKey("StashTabId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.ItemMod", b =>
                {
                    b.HasOne("Model.ItemModName")
                        .WithMany()
                        .HasForeignKey("NameId");

                    b.HasOne("Model.Item")
                        .WithMany()
                        .HasForeignKey("ItemId", "ItemName");

                    b.HasOne("Model.MagicalItem")
                        .WithMany()
                        .HasForeignKey("MagicalItemId", "MagicalItemName");
                });

            modelBuilder.Entity("Model.ItemRequirement", b =>
                {
                    b.HasOne("Model.Item")
                        .WithMany()
                        .HasForeignKey("ItemId", "ItemName");
                });

            modelBuilder.Entity("Model.Stash", b =>
                {
                    b.HasOne("Model.Account")
                        .WithMany()
                        .HasForeignKey("AccountName")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.ItemFeedChunk")
                        .WithMany()
                        .HasForeignKey("ItemFeedChunkChunkId", "ItemFeedChunkIndex");
                });

            modelBuilder.Entity("Model.StashTab", b =>
                {
                    b.HasOne("Model.Stash")
                        .WithMany()
                        .HasForeignKey("AccountName", "League");
                });
        }
    }
}
