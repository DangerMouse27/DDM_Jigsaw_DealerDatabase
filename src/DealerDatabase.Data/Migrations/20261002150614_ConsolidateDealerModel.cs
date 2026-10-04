using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealerDatabase.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateDealerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvgDaysInStock",
                table: "Dealers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AvgListedPrice",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AvgSoldPrice",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyNumber",
                table: "Dealers",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyStatus",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Dealers",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FcaBusinessType",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FcaFrn",
                table: "Dealers",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FcaPermissions",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FcaStatus",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinanceLenders",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FranchiseMake",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GoogleRating",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoogleReviewCount",
                table: "Dealers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IcoPaymentTier",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IcoRegistrationNumber",
                table: "Dealers",
                type: "TEXT",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ImportedAt",
                table: "Dealers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "InventoryCount",
                table: "Dealers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalName",
                table: "Dealers",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketcheckDealerId",
                table: "Dealers",
                type: "TEXT",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "MarketcheckLastSeen",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OffersFinance",
                table: "Dealers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Dealers",
                type: "TEXT",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "Dealers",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredAddressLine1",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredAddressLine2",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredCountry",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredCounty",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredPostcode",
                table: "Dealers",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredTown",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeApr",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "SafExpiry",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SafMemberId",
                table: "Dealers",
                type: "TEXT",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SafStatus",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerType",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SicCodes",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoldLast30Days",
                table: "Dealers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sources",
                table: "Dealers",
                type: "TEXT",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StockFeedProvider",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradingName",
                table: "Dealers",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TrustpilotScore",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatNumber",
                table: "Dealers",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleTypes",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Dealers",
                type: "TEXT",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteDomain",
                table: "Dealers",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsitePlatform",
                table: "Dealers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DealerSourceLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DealerId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    SourceRecordKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealerSourceLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealerSourceLinks_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_CompanyNumber",
                table: "Dealers",
                column: "CompanyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_FcaFrn",
                table: "Dealers",
                column: "FcaFrn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_IcoRegistrationNumber",
                table: "Dealers",
                column: "IcoRegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_MarketcheckDealerId",
                table: "Dealers",
                column: "MarketcheckDealerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_Name",
                table: "Dealers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_Postcode",
                table: "Dealers",
                column: "Postcode");

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_SafMemberId",
                table: "Dealers",
                column: "SafMemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_VatNumber",
                table: "Dealers",
                column: "VatNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DealerSourceLinks_DealerId",
                table: "DealerSourceLinks",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_DealerSourceLinks_SourceName_SourceRecordKey",
                table: "DealerSourceLinks",
                columns: new[] { "SourceName", "SourceRecordKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DealerSourceLinks");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_CompanyNumber",
                table: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_FcaFrn",
                table: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_IcoRegistrationNumber",
                table: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_MarketcheckDealerId",
                table: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_Name",
                table: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_Postcode",
                table: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_SafMemberId",
                table: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_VatNumber",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "AvgDaysInStock",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "AvgListedPrice",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "AvgSoldPrice",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "CompanyNumber",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "CompanyStatus",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "County",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "FcaBusinessType",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "FcaFrn",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "FcaPermissions",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "FcaStatus",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "FinanceLenders",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "FranchiseMake",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "GoogleRating",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "GoogleReviewCount",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "IcoPaymentTier",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "IcoRegistrationNumber",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "ImportedAt",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "InventoryCount",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "LegalName",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "MarketcheckDealerId",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "MarketcheckLastSeen",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "OffersFinance",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "RegisteredAddressLine1",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "RegisteredAddressLine2",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "RegisteredCountry",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "RegisteredCounty",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "RegisteredPostcode",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "RegisteredTown",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "RepresentativeApr",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "SafExpiry",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "SafMemberId",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "SafStatus",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "SellerType",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "SicCodes",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "SoldLast30Days",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "Sources",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "StockFeedProvider",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "TradingName",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "TrustpilotScore",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "VatNumber",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "VehicleTypes",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "WebsiteDomain",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "WebsitePlatform",
                table: "Dealers");
        }
    }
}
