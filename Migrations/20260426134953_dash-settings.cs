using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dash_backend.Migrations
{
    /// <inheritdoc />
    public partial class dashsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Columns",
                table: "Dashes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DisplayGrid",
                table: "Dashes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RowHeight",
                table: "Dashes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Columns",
                table: "Dashes");

            migrationBuilder.DropColumn(
                name: "DisplayGrid",
                table: "Dashes");

            migrationBuilder.DropColumn(
                name: "RowHeight",
                table: "Dashes");
        }
    }
}
