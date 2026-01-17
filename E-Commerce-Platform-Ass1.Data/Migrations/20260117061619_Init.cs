using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace E_Commerce_Platform_Ass1.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carts_users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orders_users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "shops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shops_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payments_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shipments_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AvgRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 0m),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_products_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_products_shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_variants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_variants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_variants_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductVariantId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_product_variants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "product_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItems_product_variants_ProductVariantId1",
                        column: x => x.ProductVariantId1,
                        principalTable: "product_variants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_items_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_items_product_variants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "product_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SpamScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    ToxicityScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    ModerationReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ModeratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModeratedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reviews_order_items_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "order_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reviews_users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "Id", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("10101010-1010-1010-1010-101010101010"), "Glasses", "Active" },
                    { new Guid("20202020-2020-2020-2020-202020202020"), "Contact Lens", "Active" },
                    { new Guid("30303030-3030-3030-3030-303030303030"), "Accessories", "Active" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "RoleId", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("99999999-1111-1111-1111-111111111111"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Normal customer", "Customer" },
                    { new Guid("99999999-2222-2222-2222-222222222222"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Shop owner", "Shop" },
                    { new Guid("99999999-3333-3333-3333-333333333333"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "System administrator", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "RoleId", "Status" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "a@mail.com", "Customer A", "$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC", new Guid("99999999-1111-1111-1111-111111111111"), true },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "shopA@mail.com", "Shop Owner A", "$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC", new Guid("99999999-2222-2222-2222-222222222222"), true },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "shopB@mail.com", "Shop Owner B", "$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC", new Guid("99999999-2222-2222-2222-222222222222"), true },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "shopC@mail.com", "Shop Owner C", "$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC", new Guid("99999999-2222-2222-2222-222222222222"), true },
                    { new Guid("55554444-5555-5555-5555-555555555555"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "mod@mail.com", "Moderator", "$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC", new Guid("99999999-3333-3333-3333-333333333333"), true }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "CreatedAt", "Status", "UserId", "UserId1" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Active", new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Active", new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "InActive", new Guid("11111111-1111-1111-1111-111111111111"), null }
                });

            migrationBuilder.InsertData(
                table: "orders",
                columns: new[] { "Id", "CreatedAt", "ShippingAddress", "Status", "TotalAmount", "UserId", "UserId1" },
                values: new object[,]
                {
                    { new Guid("c0000000-0000-0000-0000-000000000001"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "HCM", "Paid", 1200000m, new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("c0000000-0000-0000-0000-000000000002"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Hanoi", "Pending", 700000m, new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("c0000000-0000-0000-0000-000000000003"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Danang", "Delivered", 150000m, new Guid("11111111-1111-1111-1111-111111111111"), null }
                });

            migrationBuilder.InsertData(
                table: "shops",
                columns: new[] { "Id", "CreatedAt", "Description", "ShopName", "Status", "UserId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Eyewear Shop", "Vision Store", "Active", new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Contact Lens", "Lens World", "Active", new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Optic Accessories", "Optic Pro", "Active", new Guid("44444444-4444-4444-4444-444444444444") }
                });

            migrationBuilder.InsertData(
                table: "payments",
                columns: new[] { "Id", "Amount", "Method", "OrderId", "PaidAt", "Status", "TransactionCode" },
                values: new object[,]
                {
                    { new Guid("e0000000-0000-0000-0000-000000000001"), 1200000m, "VNPay", new Guid("c0000000-0000-0000-0000-000000000001"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Success", "TXN001" },
                    { new Guid("e0000000-0000-0000-0000-000000000002"), 700000m, "Momo", new Guid("c0000000-0000-0000-0000-000000000002"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Pending", "TXN002" },
                    { new Guid("e0000000-0000-0000-0000-000000000003"), 150000m, "Momo", new Guid("c0000000-0000-0000-0000-000000000003"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Success", "TXN003" }
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "AvgRating", "BasePrice", "CategoryId", "CreatedAt", "Description", "ImageUrl", "Name", "ShopId", "Status" },
                values: new object[,]
                {
                    { new Guid("aaaa1111-1111-1111-1111-111111111111"), 4.5m, 1200000m, new Guid("10101010-1010-1010-1010-101010101010"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Men Glasses", "rayban.png", "Rayban Glasses", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Active" },
                    { new Guid("bbbb2222-2222-2222-2222-222222222222"), 4.7m, 350000m, new Guid("20202020-2020-2020-2020-202020202020"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Monthly Lens", "acuvue.png", "Acuvue Lens", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Active" },
                    { new Guid("cccc3333-3333-3333-3333-333333333333"), 4.2m, 150000m, new Guid("30303030-3030-3030-3030-303030303030"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "Leather Case", "case.png", "Glasses Case", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Active" }
                });

            migrationBuilder.InsertData(
                table: "shipments",
                columns: new[] { "Id", "Carrier", "OrderId", "Status", "TrackingCode", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("f0000000-0000-0000-0000-000000000001"), "GHTK", new Guid("c0000000-0000-0000-0000-000000000001"), "Delivering", "TRK001", new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("f0000000-0000-0000-0000-000000000002"), "VNPost", new Guid("c0000000-0000-0000-0000-000000000002"), "Pending", "TRK002", new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("f0000000-0000-0000-0000-000000000003"), "GHN", new Guid("c0000000-0000-0000-0000-000000000003"), "Delivered", "TRK003", new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "product_variants",
                columns: new[] { "Id", "Color", "ImageUrl", "Price", "ProductId", "Size", "Sku", "Status", "Stock", "VariantName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Black", "rb-black.png", 1200000m, new Guid("aaaa1111-1111-1111-1111-111111111111"), "M", "RB-BLK-M", "Active", 10, "Black-M" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Clear", "acu-clear.png", 350000m, new Guid("bbbb2222-2222-2222-2222-222222222222"), "Standard", "ACU-CLR", "Active", 50, "Clear" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Brown", "case-brown.png", 150000m, new Guid("cccc3333-3333-3333-3333-333333333333"), "Standard", "CASE-BR", "Active", 30, "Brown" }
                });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "Id", "CartId", "CreatedAt", "ProductVariantId", "ProductVariantId1", "Quantity" },
                values: new object[,]
                {
                    { new Guid("b0000000-0000-0000-0000-000000000001"), new Guid("a0000000-0000-0000-0000-000000000001"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), null, 1 },
                    { new Guid("b0000000-0000-0000-0000-000000000002"), new Guid("a0000000-0000-0000-0000-000000000002"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), new Guid("22222222-2222-2222-2222-222222222222"), null, 2 },
                    { new Guid("b0000000-0000-0000-0000-000000000003"), new Guid("a0000000-0000-0000-0000-000000000003"), new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), new Guid("33333333-3333-3333-3333-333333333333"), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "order_items",
                columns: new[] { "Id", "OrderId", "Price", "ProductName", "ProductVariantId", "Quantity" },
                values: new object[,]
                {
                    { new Guid("d0000000-0000-0000-0000-000000000001"), new Guid("c0000000-0000-0000-0000-000000000001"), 1200000m, "Rayban Black M", new Guid("11111111-1111-1111-1111-111111111111"), 1 },
                    { new Guid("d0000000-0000-0000-0000-000000000002"), new Guid("c0000000-0000-0000-0000-000000000002"), 350000m, "Acuvue Lens", new Guid("22222222-2222-2222-2222-222222222222"), 2 },
                    { new Guid("d0000000-0000-0000-0000-000000000003"), new Guid("c0000000-0000-0000-0000-000000000003"), 150000m, "Glasses Case", new Guid("33333333-3333-3333-3333-333333333333"), 1 }
                });

            migrationBuilder.InsertData(
                table: "reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "ModeratedAt", "ModeratedBy", "ModerationReason", "OrderItemId", "ProductId", "Rating", "Status", "UserId", "UserId1" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Great!", new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), null, new Guid("55554444-5555-5555-5555-555555555555"), "", new Guid("d0000000-0000-0000-0000-000000000001"), new Guid("aaaa1111-1111-1111-1111-111111111111"), 5, "Approved", new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Good lens", new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), null, new Guid("55554444-5555-5555-5555-555555555555"), "", new Guid("d0000000-0000-0000-0000-000000000002"), new Guid("bbbb2222-2222-2222-2222-222222222222"), 4, "Approved", new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Nice case", new DateTime(2025, 1, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), null, new Guid("55554444-5555-5555-5555-555555555555"), "", new Guid("d0000000-0000-0000-0000-000000000003"), new Guid("cccc3333-3333-3333-3333-333333333333"), 5, "Approved", new Guid("11111111-1111-1111-1111-111111111111"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductVariantId",
                table: "CartItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductVariantId1",
                table: "CartItems",
                column: "ProductVariantId1");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId1",
                table: "Carts",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_categories_Name",
                table: "categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_items_OrderId",
                table: "order_items",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_ProductVariantId",
                table: "order_items",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CreatedAt",
                table: "orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status",
                table: "orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_orders_UserId",
                table: "orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_UserId1",
                table: "orders",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_payments_OrderId",
                table: "payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_TransactionCode",
                table: "payments",
                column: "TransactionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_ProductId",
                table: "product_variants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_ProductId_Size_Color",
                table: "product_variants",
                columns: new[] { "ProductId", "Size", "Color" },
                unique: true,
                filter: "[Size] IS NOT NULL AND [Color] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_Sku",
                table: "product_variants",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_CategoryId",
                table: "products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_products_Name",
                table: "products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_products_ShopId",
                table: "products",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_OrderItemId",
                table: "reviews",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_ProductId",
                table: "reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_UserId",
                table: "reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_UserId1",
                table: "reviews",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                table: "roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shipments_OrderId",
                table: "shipments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shipments_TrackingCode",
                table: "shipments",
                column: "TrackingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shops_ShopName",
                table: "shops",
                column: "ShopName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shops_UserId",
                table: "shops",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                table: "users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "shipments");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "product_variants");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "shops");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
