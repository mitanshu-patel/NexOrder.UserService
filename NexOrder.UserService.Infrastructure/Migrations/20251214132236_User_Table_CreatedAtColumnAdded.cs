using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexOrder.UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class User_Table_CreatedAtColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"update Users set CreatedAtUtc = (select GetUtcDate())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Users");
        }
    }
}
