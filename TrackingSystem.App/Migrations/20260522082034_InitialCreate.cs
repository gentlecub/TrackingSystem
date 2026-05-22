using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrackingSystem.App.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Offices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasePriceUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LocalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WarrantyExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OfficeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Assets_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "Offices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    MaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceRecords_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Department", "Email", "FullName" },
                values: new object[,]
                {
                    { 1, "IT", "anna.andersson@company.com", "Anna Andersson" },
                    { 2, "Finance", "erik.eriksson@company.com", "Erik Eriksson" },
                    { 3, "HR", "maria.garcia@company.com", "Maria Garcia" },
                    { 4, "Sales", "john.smith@company.com", "John Smith" }
                });

            migrationBuilder.InsertData(
                table: "Offices",
                columns: new[] { "Id", "City", "Country", "CurrencyCode", "ExchangeRate", "Name" },
                values: new object[,]
                {
                    { 1, "Stockholm", "Sweden", "SEK", 10.5m, "Stockholm HQ" },
                    { 2, "London", "UK", "GBP", 0.79m, "London Office" },
                    { 3, "Madrid", "Spain", "EUR", 0.92m, "Madrid Office" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "EmployeeId", "Password", "Role", "Username" },
                values: new object[] { 1, null, "admin123", 1, "admin" });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "Brand", "EmployeeId", "LocalPrice", "ModelName", "OfficeId", "PurchaseDate", "PurchasePriceUSD", "SerialNumber", "Type", "WarrantyExpirationDate" },
                values: new object[,]
                {
                    { 1, "Dell", 1, 15750.00m, "XPS 15", 1, new DateTime(2023, 6, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5203), 1500.00m, "DELL-XPS-001", 1, new DateTime(2025, 6, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5302) },
                    { 2, "Apple", 2, 25200.00m, "MacBook Pro 14", 1, new DateTime(2023, 9, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5311), 2400.00m, "APPLE-MBP-001", 1, new DateTime(2026, 9, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5317) },
                    { 3, "HP", null, 12600.00m, "EliteDesk 800", 1, new DateTime(2025, 5, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5323), 1200.00m, "HP-ED-001", 2, new DateTime(2028, 5, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5328) },
                    { 4, "Apple", 3, 869.00m, "iPhone 15 Pro", 2, new DateTime(2025, 11, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5333), 1100.00m, "APPLE-IP15-001", 3, new DateTime(2027, 11, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5337) },
                    { 5, "Lenovo", 4, 1422.00m, "ThinkPad X1 Carbon", 2, new DateTime(2023, 7, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5342), 1800.00m, "LEN-X1C-001", 1, new DateTime(2025, 7, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5346) },
                    { 6, "Apple", null, 1012.00m, "iPad Pro 12.9", 3, new DateTime(2024, 11, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5351), 1100.00m, "APPLE-IPAD-001", 4, new DateTime(2026, 11, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5356) },
                    { 7, "HP", null, 414.00m, "LaserJet Pro M428", 3, new DateTime(2023, 11, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5361), 450.00m, "HP-PRINT-001", 5, new DateTime(2025, 11, 22, 10, 20, 33, 485, DateTimeKind.Local).AddTicks(5365) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "EmployeeId", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 2, 1, "manager123", 2, "manager" },
                    { 3, 2, "employee123", 3, "employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_EmployeeId",
                table: "Assets",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_OfficeId",
                table: "Assets",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SerialNumber",
                table: "Assets",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_AssetId",
                table: "MaintenanceRecords",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceRecords");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Offices");
        }
    }
}
