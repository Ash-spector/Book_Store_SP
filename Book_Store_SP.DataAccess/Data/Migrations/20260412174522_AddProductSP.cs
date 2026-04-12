using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Store_SP.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Table first
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CoverTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_CoverTypes_CoverTypeId",
                        column: x => x.CoverTypeId,
                        principalTable: "CoverTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CoverTypeId",
                table: "Products",
                column: "CoverTypeId");

            // 2. SPs after table
            migrationBuilder.Sql(@"
        CREATE PROCEDURE Product
            @title nvarchar(100),
            @description nvarchar(max),
            @isbn nvarchar(50),
            @author nvarchar(100),
            @listPrice float,
            @price float,
            @price50 float,
            @price100 float,
            @imageUrl nvarchar(max),
            @categoryId int,
            @coverTypeId int
        AS
            INSERT INTO Products (Title, Description, ISBN, Author, ListPrice, Price, Price50, Price100, ImageUrl, CategoryId, CoverTypeId)
            VALUES (@title, @description, @isbn, @author, @listPrice, @price, @price50, @price100, @imageUrl, @categoryId, @coverTypeId)
    ");

            migrationBuilder.Sql(@"
        CREATE PROCEDURE UpdateProduct
            @id int,
            @title nvarchar(100),
            @description nvarchar(max),
            @isbn nvarchar(50),
            @author nvarchar(100),
            @listPrice float,
            @price float,
            @price50 float,
            @price100 float,
            @imageUrl nvarchar(max),
            @categoryId int,
            @coverTypeId int
        AS
            UPDATE Products SET
                Title = @title,
                Description = @description,
                ISBN = @isbn,
                Author = @author,
                ListPrice = @listPrice,
                Price = @price,
                Price50 = @price50,
                Price100 = @price100,
                ImageUrl = @imageUrl,
                CategoryId = @categoryId,
                CoverTypeId = @coverTypeId
            WHERE Id = @id
    ");

            migrationBuilder.Sql(@"
        CREATE PROCEDURE DeleteProduct
            @id int
        AS
            DELETE FROM Products WHERE Id = @id
    ");

            migrationBuilder.Sql(@"
        CREATE PROCEDURE GetProduct
            @id int
        AS
            SELECT p.*, c.Name as CategoryName, ct.Name as CoverTypeName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            INNER JOIN CoverTypes ct ON p.CoverTypeId = ct.Id
            WHERE p.Id = @id
    ");

            migrationBuilder.Sql(@"
        CREATE PROCEDURE GetProducts
        AS
            SELECT p.*, c.Name as CategoryName, ct.Name as CoverTypeName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            INNER JOIN CoverTypes ct ON p.CoverTypeId = ct.Id
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS Product");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateProduct");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteProduct");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetProduct");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetProducts");

            migrationBuilder.DropTable(name: "Products");
        }
    }
}
