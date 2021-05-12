using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Model.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Landen",
                columns: table => new
                {
                    ISOLandCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    NISLandCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AantalInwoners = table.Column<int>(type: "int", nullable: false),
                    Oppervlakte = table.Column<float>(type: "real", nullable: false),
                    Aangepast = table.Column<byte[]>(type: "timestamp", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Landen", x => x.ISOLandCode);
                });

            migrationBuilder.CreateTable(
                name: "Talen",
                columns: table => new
                {
                    ISOTaalCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    NaamNL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NaamTaal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talen", x => x.ISOTaalCode);
                });

            migrationBuilder.CreateTable(
                name: "Steden",
                columns: table => new
                {
                    StadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ISOLandCode = table.Column<string>(type: "nvarchar(2)", nullable: true),
                    Aangepast = table.Column<byte[]>(type: "timestamp", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steden", x => x.StadId);
                    table.ForeignKey(
                        name: "FK_Steden_LandCode",
                        column: x => x.ISOLandCode,
                        principalTable: "Landen",
                        principalColumn: "ISOLandCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LandenTalen",
                columns: table => new
                {
                    LandCode = table.Column<string>(type: "nvarchar(2)", nullable: false),
                    TaalCode = table.Column<string>(type: "nvarchar(2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandenTalen", x => new { x.LandCode, x.TaalCode });
                    table.ForeignKey(
                        name: "FK_LandTaal_Land",
                        column: x => x.LandCode,
                        principalTable: "Landen",
                        principalColumn: "ISOLandCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandTaal_Taal",
                        column: x => x.TaalCode,
                        principalTable: "Talen",
                        principalColumn: "ISOTaalCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Landen_Naam",
                table: "Landen",
                column: "Naam",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Landen_NISLandCode",
                table: "Landen",
                column: "NISLandCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandenTalen_TaalCode",
                table: "LandenTalen",
                column: "TaalCode");

            migrationBuilder.CreateIndex(
                name: "IX_Steden_ISOLandCode",
                table: "Steden",
                column: "ISOLandCode");

            migrationBuilder.CreateIndex(
                name: "IX_Steden_Naam",
                table: "Steden",
                column: "Naam",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talen_NaamNL",
                table: "Talen",
                column: "NaamNL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talen_NaamTaal",
                table: "Talen",
                column: "NaamTaal",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LandenTalen");

            migrationBuilder.DropTable(
                name: "Steden");

            migrationBuilder.DropTable(
                name: "Talen");

            migrationBuilder.DropTable(
                name: "Landen");
        }
    }
}
