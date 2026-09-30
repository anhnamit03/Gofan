using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoFan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductImageAndRemovePrimary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "ProductMedias");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "ProductMedias",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
