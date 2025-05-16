using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Halcyon.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_address = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: true),
                    password_reset_token = table.Column<Guid>(type: "uuid", nullable: true),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    is_locked_out = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false
                    ),
                    roles = table.Column<List<string>>(type: "text[]", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    search_vector = table
                        .Column<NpgsqlTsVector>(type: "tsvector", nullable: true)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation(
                            "Npgsql:TsVectorProperties",
                            new[] { "first_name", "last_name", "email_address" }
                        ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_email_address",
                table: "users",
                column: "email_address",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "users");
        }
    }
}
