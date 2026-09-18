using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KRT.Onboarding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountStatusCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_Status",
                table: "Accounts",
                sql: "[Status] IN (0, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_Status",
                table: "Accounts");
        }
    }
}
