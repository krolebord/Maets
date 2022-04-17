using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace Maets.Data.Migrations.MaetsDb;

public partial class InitialDataMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith("initialize_db.sql"));

        using Stream stream = assembly.GetManifestResourceStream(resourcePath);
        using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
        var sql = reader.ReadToEnd();

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
