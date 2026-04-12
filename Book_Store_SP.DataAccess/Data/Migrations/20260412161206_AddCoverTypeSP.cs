using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Store_SP.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverTypeSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoverTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverTypes", x => x.Id);
                });
            //Sp model 
            migrationBuilder.Sql(@"CREATE PROCEDURE CoverType
        @name varchar(50)
        AS
        INSERT INTO CoverTypes(Name) VALUES (@name)");

            migrationBuilder.Sql(@"CREATE PROCEDURE UpdateCoverType
        @id int,
        @name varchar(50)
        AS
        UPDATE CoverTypes SET Name = @name WHERE Id = @id");

            migrationBuilder.Sql(@"CREATE PROCEDURE DeleteCoverType
        @id int
        AS
        DELETE FROM CoverTypes WHERE Id = @id");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverType
        @id int
        AS
        SELECT * FROM CoverTypes WHERE Id = @id");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverTypes
        AS
        SELECT * FROM CoverTypes");
        }
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS CoverType");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS UpdateCoverType");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS DeleteCoverType");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetCoverType");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetCoverTypes");
            migrationBuilder.DropTable(
                name: "CoverTypes");
        }
    }
}
