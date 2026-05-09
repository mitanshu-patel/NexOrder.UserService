using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexOrder.UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class alter_users_oid_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserOid",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(@"UPDATE Users SET UserOid=NEWID()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserOid",
                table: "Users");
        }
    }
}
