using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Store_SP.DataAccess.Migrations
{
    public partial class AddProductSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create table first
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(50)", nullable: false),
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

            // 2. Create SPs after table
            migrationBuilder.Sql(@"
                CREATE PROCEDURE CreateProduct
                    @title varchar(200),
                    @description varchar(max),
                    @author varchar(100),
                    @isbn varchar(50),
                    @listPrice float,
                    @price float,
                    @price50 float,
                    @price100 float,
                    @imageUrl varchar(max),
                    @categoryId int,
                    @coverTypeId int
                AS
                    INSERT INTO Products(Title, Description, Author, ISBN, ListPrice, Price, Price50, Price100, ImageUrl, CategoryId, CoverTypeId)
                    VALUES(@title, @description, @author, @isbn, @listPrice, @price, @price50, @price100, @imageUrl, @categoryId, @coverTypeId)
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdateProduct
                    @id int,
                    @title varchar(200),
                    @description varchar(max),
                    @author varchar(100),
                    @isbn varchar(50),
                    @listPrice float,
                    @price float,
                    @price50 float,
                    @price100 float,
                    @imageUrl varchar(max),
                    @categoryId int,
                    @coverTypeId int
                AS
                    UPDATE Products SET
                        Title=@title, Description=@description, Author=@author, ISBN=@isbn,
                        ListPrice=@listPrice, Price=@price, Price50=@price50, Price100=@price100,
                        ImageUrl=@imageUrl, CategoryId=@categoryId, CoverTypeId=@coverTypeId
                    WHERE Id=@id
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE DeleteProduct
                    @id int
                AS
                    DELETE FROM Products WHERE Id=@id
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetProduct
                    @id int
                AS
                    SELECT * FROM Products WHERE Id=@id
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetProducts
                AS
                    SELECT * FROM Products
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CreateProduct");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateProduct");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteProduct");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetProduct");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetProducts");

            migrationBuilder.DropTable(name: "Products");
        }
    }
}