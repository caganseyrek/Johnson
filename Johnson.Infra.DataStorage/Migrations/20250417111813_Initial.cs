using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Johnson.Infra.DataStorage.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventName = table.Column<string>(type: "TEXT", nullable: true),
                    Outcome = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventPublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OriginService = table.Column<string>(type: "TEXT", nullable: false),
                    EffectedDataId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistryServiceEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    URL = table.Column<string>(type: "TEXT", nullable: false),
                    IP = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsHealthy = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    APIVersion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistryServiceEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsedAPIKeys",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedAPIKeys", x => x.Key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAuditLogs");

            migrationBuilder.DropTable(
                name: "RegistryServiceEntries");

            migrationBuilder.DropTable(
                name: "UsedAPIKeys");
        }
    }
}
