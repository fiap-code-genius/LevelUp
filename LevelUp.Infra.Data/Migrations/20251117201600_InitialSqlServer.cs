using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_LEVELUP_REWARDS",
                columns: table => new
                {
                    reward_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    point_cost = table.Column<int>(type: "int", nullable: false),
                    stock_quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_ACTIVE = table.Column<string>(type: "CHAR(1)", nullable: false, defaultValue: "Y")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LEVELUP_REWARDS", x => x.reward_id);
                });

            migrationBuilder.CreateTable(
                name: "TB_LEVELUP_TEAMS",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    team_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LEVELUP_TEAMS", x => x.team_id);
                });

            migrationBuilder.CreateTable(
                name: "TB_LEVELUP_USERS",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    job_title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    point_balance = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    team_id = table.Column<int>(type: "int", nullable: true),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "USER"),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_ACTIVE = table.Column<string>(type: "CHAR(1)", nullable: false, defaultValue: "Y")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LEVELUP_USERS", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_USERS_TEAM",
                        column: x => x.team_id,
                        principalTable: "TB_LEVELUP_TEAMS",
                        principalColumn: "team_id");
                });

            migrationBuilder.CreateTable(
                name: "TB_LEVELUP_REWARD_REDEMPTIONS",
                columns: table => new
                {
                    redemption_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    reward_id = table.Column<int>(type: "int", nullable: false),
                    redeemed_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    points_spent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LEVELUP_REWARD_REDEMPTIONS", x => x.redemption_id);
                    table.ForeignKey(
                        name: "FK_REDEMPTIONS_REWARD",
                        column: x => x.reward_id,
                        principalTable: "TB_LEVELUP_REWARDS",
                        principalColumn: "reward_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_REDEMPTIONS_USER",
                        column: x => x.user_id,
                        principalTable: "TB_LEVELUP_USERS",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_LEVELUP_REWARD_REDEMPTIONS_reward_id",
                table: "TB_LEVELUP_REWARD_REDEMPTIONS",
                column: "reward_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_LEVELUP_REWARD_REDEMPTIONS_user_id",
                table: "TB_LEVELUP_REWARD_REDEMPTIONS",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_LEVELUP_TEAMS_team_name",
                table: "TB_LEVELUP_TEAMS",
                column: "team_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_LEVELUP_USERS_email",
                table: "TB_LEVELUP_USERS",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_LEVELUP_USERS_team_id",
                table: "TB_LEVELUP_USERS",
                column: "team_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_LEVELUP_REWARD_REDEMPTIONS");

            migrationBuilder.DropTable(
                name: "TB_LEVELUP_REWARDS");

            migrationBuilder.DropTable(
                name: "TB_LEVELUP_USERS");

            migrationBuilder.DropTable(
                name: "TB_LEVELUP_TEAMS");
        }
    }
}
