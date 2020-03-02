using Microsoft.EntityFrameworkCore.Migrations;

namespace myAuthExampleApi.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder?.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => new { x.UserId, x.Token });
                });

            migrationBuilder.CreateTable(
                name: "SimpleTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleTokens", x => new { x.UserId, x.Token });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(maxLength: 128, nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder?.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "SimpleTokens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
