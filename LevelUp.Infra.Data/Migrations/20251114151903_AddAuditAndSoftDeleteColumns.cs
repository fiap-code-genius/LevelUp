using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditAndSoftDeleteColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CREATED_AT",
                table: "TB_LEVELUP_USERS",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValueSql: "SYSDATE");

            migrationBuilder.AddColumn<string>(
                name: "IS_ACTIVE",
                table: "TB_LEVELUP_USERS",
                type: "CHAR(1)",
                nullable: false,
                defaultValue: "Y");

            migrationBuilder.AddColumn<DateTime>(
                name: "UPDATED_AT",
                table: "TB_LEVELUP_USERS",
                type: "TIMESTAMP(7)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CREATED_AT",
                table: "TB_LEVELUP_REWARDS",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValueSql: "SYSDATE");

            migrationBuilder.AddColumn<string>(
                name: "IS_ACTIVE",
                table: "TB_LEVELUP_REWARDS",
                type: "CHAR(1)",
                nullable: false,
                defaultValue: "Y");

            migrationBuilder.AddColumn<DateTime>(
                name: "UPDATED_AT",
                table: "TB_LEVELUP_REWARDS",
                type: "TIMESTAMP(7)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "TB_LEVELUP_USERS");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "TB_LEVELUP_USERS");

            migrationBuilder.DropColumn(
                name: "UPDATED_AT",
                table: "TB_LEVELUP_USERS");

            migrationBuilder.DropColumn(
                name: "CREATED_AT",
                table: "TB_LEVELUP_REWARDS");

            migrationBuilder.DropColumn(
                name: "IS_ACTIVE",
                table: "TB_LEVELUP_REWARDS");

            migrationBuilder.DropColumn(
                name: "UPDATED_AT",
                table: "TB_LEVELUP_REWARDS");
        }
    }
}
