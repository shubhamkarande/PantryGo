using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PantryGo.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AddressLine = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Pincode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    PaymentId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RazorpayOrderId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    DeliveryPartnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Users_DeliveryPartnerId",
                        column: x => x.DeliveryPartnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), "Fruits", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(5222), "Crisp and sweet red apples", "/images/products/apples.jpg", true, "Fresh Apples", 120.00m, 100, "kg" },
                    { new Guid("11111111-1111-1111-1111-111111111102"), "Fruits", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6819), "Ripe yellow bananas", "/images/products/bananas.jpg", true, "Bananas", 40.00m, 150, "dozen" },
                    { new Guid("11111111-1111-1111-1111-111111111103"), "Fruits", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6841), "Juicy oranges rich in Vitamin C", "/images/products/oranges.jpg", true, "Oranges", 80.00m, 80, "kg" },
                    { new Guid("11111111-1111-1111-1111-111111111104"), "Fruits", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6859), "Sweet Alphonso mangoes", "/images/products/mangoes.jpg", true, "Mangoes", 200.00m, 50, "kg" },
                    { new Guid("11111111-1111-1111-1111-111111111105"), "Fruits", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6862), "Fresh green seedless grapes", "/images/products/grapes.jpg", true, "Grapes", 90.00m, 60, "kg" },
                    { new Guid("11111111-1111-1111-1111-111111111201"), "Vegetables", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6865), "Fresh red tomatoes", "/images/products/tomatoes.jpg", true, "Tomatoes", 30.00m, 200, "kg" },
                    { new Guid("11111111-1111-1111-1111-111111111202"), "Vegetables", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6868), "Fresh onions", "/images/products/onions.jpg", true, "Onions", 35.00m, 300, "kg" },
                    { new Guid("11111111-1111-1111-1111-111111111203"), "Vegetables", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6872), "Farm fresh potatoes", "/images/products/potatoes.jpg", true, "Potatoes", 25.00m, 250, "kg" },
                    { new Guid("11111111-1111-1111-1111-111111111204"), "Vegetables", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6879), "Fresh green spinach leaves", "/images/products/spinach.jpg", true, "Spinach", 20.00m, 100, "bunch" },
                    { new Guid("11111111-1111-1111-1111-111111111205"), "Vegetables", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6883), "Crunchy orange carrots", "/images/products/carrots.jpg", true, "Carrots", 45.00m, 120, "kg" },
                    { new Guid("11111111-1111-1111-1111-111111111301"), "Dairy", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6886), "Fresh full cream milk", "/images/products/milk.jpg", true, "Milk", 60.00m, 200, "liter" },
                    { new Guid("11111111-1111-1111-1111-111111111302"), "Dairy", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6890), "Creamy salted butter", "/images/products/butter.jpg", true, "Butter", 55.00m, 80, "200g" },
                    { new Guid("11111111-1111-1111-1111-111111111303"), "Dairy", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6895), "Processed cheese slices", "/images/products/cheese.jpg", true, "Cheese", 120.00m, 60, "200g" },
                    { new Guid("11111111-1111-1111-1111-111111111304"), "Dairy", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6900), "Fresh plain yogurt", "/images/products/yogurt.jpg", true, "Yogurt", 45.00m, 100, "400g" },
                    { new Guid("11111111-1111-1111-1111-111111111305"), "Dairy", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6913), "Fresh cottage cheese", "/images/products/paneer.jpg", true, "Paneer", 90.00m, 70, "200g" },
                    { new Guid("11111111-1111-1111-1111-111111111401"), "Snacks", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6917), "Crispy salted potato chips", "/images/products/chips.jpg", true, "Potato Chips", 30.00m, 150, "pack" },
                    { new Guid("11111111-1111-1111-1111-111111111402"), "Snacks", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6921), "Cream filled biscuits", "/images/products/biscuits.jpg", true, "Biscuits", 25.00m, 200, "pack" },
                    { new Guid("11111111-1111-1111-1111-111111111403"), "Snacks", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6924), "Mixed crunchy namkeen", "/images/products/namkeen.jpg", true, "Namkeen", 40.00m, 100, "200g" },
                    { new Guid("11111111-1111-1111-1111-111111111404"), "Snacks", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6928), "Milk chocolate bar", "/images/products/chocolate.jpg", true, "Chocolates", 50.00m, 120, "piece" },
                    { new Guid("11111111-1111-1111-1111-111111111405"), "Snacks", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6932), "Roasted mixed nuts", "/images/products/nuts.jpg", true, "Nuts Mix", 150.00m, 80, "200g" },
                    { new Guid("11111111-1111-1111-1111-111111111501"), "Beverages", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6936), "Fresh orange juice", "/images/products/orange-juice.jpg", true, "Orange Juice", 80.00m, 100, "liter" },
                    { new Guid("11111111-1111-1111-1111-111111111502"), "Beverages", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6941), "Ready to drink cold coffee", "/images/products/cold-coffee.jpg", true, "Cold Coffee", 45.00m, 120, "250ml" },
                    { new Guid("11111111-1111-1111-1111-111111111503"), "Beverages", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6944), "Natural coconut water", "/images/products/coconut-water.jpg", true, "Coconut Water", 35.00m, 150, "pack" },
                    { new Guid("11111111-1111-1111-1111-111111111504"), "Beverages", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6947), "Organic green tea bags", "/images/products/green-tea.jpg", true, "Green Tea", 120.00m, 80, "box" },
                    { new Guid("11111111-1111-1111-1111-111111111505"), "Beverages", new DateTime(2025, 12, 25, 18, 33, 15, 848, DateTimeKind.Utc).AddTicks(6952), "Sparkling lemon soda", "/images/products/soda.jpg", true, "Soda", 25.00m, 200, "can" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryPartnerId",
                table: "Orders",
                column: "DeliveryPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
