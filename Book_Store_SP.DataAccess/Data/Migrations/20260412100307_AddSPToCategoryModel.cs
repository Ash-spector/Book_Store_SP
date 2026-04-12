using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Store_SP.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSPToCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });
            // SPs cateegory table
            migrationBuilder.Sql(@"CREATE PROCEDURE Category
        @name varchar(50)
        AS
        INSERT INTO Categories(Name) VALUES (@name)");

            migrationBuilder.Sql(@"CREATE PROCEDURE UpdateCategory
        @id int,
        @name varchar(50)
        AS
        UPDATE Categories SET Name = @name WHERE Id = @id");

            migrationBuilder.Sql(@"CREATE PROCEDURE DeleteCategory
        @id int
        AS
        DELETE FROM Categories WHERE Id = @id");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetCategory
        @id int
        AS
        SELECT * FROM Categories WHERE Id = @id");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetCategories
        AS
        SELECT * FROM Categories");



        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
