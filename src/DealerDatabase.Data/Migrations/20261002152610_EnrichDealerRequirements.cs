using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealerDatabase.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnrichDealerRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Directors",
                table: "Dealers",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinanceCalculator",
                table: "Dealers",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IcoExpiry",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IcoRegistrationStart",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IncorporationDate",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineageTag",
                table: "Dealers",
                type: "TEXT",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VatValidationStatus",
                table: "Dealers",
                type: "TEXT",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DealerFieldAttributions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DealerId = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    SourceName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    SourceRecordKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ValuePreview = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealerFieldAttributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealerFieldAttributions_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_LineageTag",
                table: "Dealers",
                column: "LineageTag");

            migrationBuilder.CreateIndex(
                name: "IX_DealerFieldAttributions_DealerId_FieldName",
                table: "DealerFieldAttributions",
                columns: new[] { "DealerId", "FieldName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DealerFieldAttributions");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_LineageTag",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "Directors",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "FinanceCalculator",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "IcoExpiry",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "IcoRegistrationStart",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "IncorporationDate",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "LineageTag",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "VatValidationStatus",
                table: "Dealers");
        }
    }
}
