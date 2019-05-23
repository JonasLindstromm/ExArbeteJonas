using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExArbeteJonas.Migrations
{
    public partial class SaveDeletedAds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RemovedAdv",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemberName = table.Column<string>(nullable: true),
                    AdvTypeId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    Place = table.Column<string>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    ImageFileName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemovedAdv", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemovedAdv_AdType_AdvTypeId",
                        column: x => x.AdvTypeId,
                        principalTable: "AdType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RemovedEqm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RemovedAdId = table.Column<int>(nullable: false),
                    EqTypeId = table.Column<int>(nullable: false),
                    Make = table.Column<string>(nullable: false),
                    Model = table.Column<string>(nullable: true),
                    Size = table.Column<string>(nullable: true),
                    Length = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemovedEqm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemovedEqm_EquipmentType_EqTypeId",
                        column: x => x.EqTypeId,
                        principalTable: "EquipmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemovedEqm_RemovedAdv_RemovedAdId",
                        column: x => x.RemovedAdId,
                        principalTable: "RemovedAdv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemovedAdv_AdvTypeId",
                table: "RemovedAdv",
                column: "AdvTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RemovedEqm_EqTypeId",
                table: "RemovedEqm",
                column: "EqTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RemovedEqm_RemovedAdId",
                table: "RemovedEqm",
                column: "RemovedAdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemovedEqm");

            migrationBuilder.DropTable(
                name: "RemovedAdv");
        }
    }
}
