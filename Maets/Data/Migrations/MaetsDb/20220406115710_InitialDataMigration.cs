using Maets.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace Maets.Data.Migrations.MaetsDb;

public partial class InitialDataMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var sql = EmbeddedResourceHelper.ReadEmbeddedResourceAsString("initialize_db.sql");
        migrationBuilder.Sql(sql);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
        name: "App_Screenshots");

        migrationBuilder.DropTable(
        name: "Apps_Developers");

        migrationBuilder.DropTable(
        name: "Apps_Labels");

        migrationBuilder.DropTable(
        name: "CompanyEmployees");

        migrationBuilder.DropTable(
        name: "Reviews");

        migrationBuilder.DropTable(
        name: "Labels");

        migrationBuilder.DropTable(
        name: "Apps");

        migrationBuilder.DropTable(
        name: "Users");

        migrationBuilder.DropTable(
        name: "Companies");

        migrationBuilder.DropTable(
        name: "MediaFiles");
    }
}
