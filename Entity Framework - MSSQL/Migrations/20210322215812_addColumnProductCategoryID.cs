using Microsoft.EntityFrameworkCore.Migrations;

namespace efCore_SQLITE.Migrations
{
    public partial class addColumnProductCategoryID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category_Id",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category_Id",
                table: "Products");
        }
    }
}
