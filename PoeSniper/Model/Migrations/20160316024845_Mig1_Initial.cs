using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Model.Migrations
{
    public partial class Mig1_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountName = table.Column<string>(nullable: false),
                    LastCharacterName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountName);
                });

            migrationBuilder.CreateTable(
                name: "ItemFeedChunks",
                columns: table => new
                {
                    ChunkId = table.Column<string>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    NextChunkId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFeedChunks", x => new { x.ChunkId, x.Index });
                });

            migrationBuilder.CreateTable(
                name: "ItemModName",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemModName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stashes",
                columns: table => new
                {
                    AccountName = table.Column<string>(nullable: false),
                    League = table.Column<string>(nullable: false),
                    ItemFeedChunkChunkId = table.Column<string>(nullable: true),
                    ItemFeedChunkIndex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stashes", x => new { x.AccountName, x.League });
                    table.ForeignKey(
                        name: "FK_Stashes_Accounts_AccountName",
                        column: x => x.AccountName,
                        principalTable: "Accounts",
                        principalColumn: "AccountName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stashes_ItemFeedChunks_ItemFeedChunkChunkId_ItemFeedChunkIndex",
                        columns: x => new { x.ItemFeedChunkChunkId, x.ItemFeedChunkIndex },
                        principalTable: "ItemFeedChunks",
                        principalColumns: new[] { "ChunkId", "Index" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StashTabs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccountName = table.Column<string>(nullable: true),
                    League = table.Column<string>(nullable: true),
                    TabName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StashTabs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StashTabs_Stashes_AccountName_League",
                        columns: x => new { x.AccountName, x.League },
                        principalTable: "Stashes",
                        principalColumns: new[] { "AccountName", "League" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Corrupted = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Ilvl = table.Column<int>(nullable: false),
                    StashTabId = table.Column<string>(nullable: false),
                    TypeLine = table.Column<string>(nullable: true),
                    Identified = table.Column<bool>(nullable: true),
                    FlavorText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => new { x.Id, x.Name });
                    table.ForeignKey(
                        name: "FK_Items_StashTabs_StashTabId",
                        column: x => x.StashTabId,
                        principalTable: "StashTabs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemMod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemId = table.Column<string>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    MagicalItemId = table.Column<string>(nullable: true),
                    MagicalItemName = table.Column<string>(nullable: true),
                    NameId = table.Column<int>(nullable: true),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemMod_ItemModName_NameId",
                        column: x => x.NameId,
                        principalTable: "ItemModName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemMod_Items_ItemId_ItemName",
                        columns: x => new { x.ItemId, x.ItemName },
                        principalTable: "Items",
                        principalColumns: new[] { "Id", "Name" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemMod_Items_MagicalItemId_MagicalItemName",
                        columns: x => new { x.MagicalItemId, x.MagicalItemName },
                        principalTable: "Items",
                        principalColumns: new[] { "Id", "Name" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemRequirement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ItemId = table.Column<string>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemRequirement_Items_ItemId_ItemName",
                        columns: x => new { x.ItemId, x.ItemName },
                        principalTable: "Items",
                        principalColumns: new[] { "Id", "Name" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_StashTabId",
                table: "Items",
                column: "StashTabId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMod_NameId",
                table: "ItemMod",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMod_ItemId_ItemName",
                table: "ItemMod",
                columns: new[] { "ItemId", "ItemName" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemMod_MagicalItemId_MagicalItemName",
                table: "ItemMod",
                columns: new[] { "MagicalItemId", "MagicalItemName" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequirement_ItemId_ItemName",
                table: "ItemRequirement",
                columns: new[] { "ItemId", "ItemName" });

            migrationBuilder.CreateIndex(
                name: "IX_Stashes_AccountName",
                table: "Stashes",
                column: "AccountName");

            migrationBuilder.CreateIndex(
                name: "IX_Stashes_ItemFeedChunkChunkId_ItemFeedChunkIndex",
                table: "Stashes",
                columns: new[] { "ItemFeedChunkChunkId", "ItemFeedChunkIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_StashTabs_AccountName_League",
                table: "StashTabs",
                columns: new[] { "AccountName", "League" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemMod");

            migrationBuilder.DropTable(
                name: "ItemRequirement");

            migrationBuilder.DropTable(
                name: "ItemModName");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "StashTabs");

            migrationBuilder.DropTable(
                name: "Stashes");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "ItemFeedChunks");
        }
    }
}
