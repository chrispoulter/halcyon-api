using Microsoft.EntityFrameworkCore.Migrations;

namespace Halcyon.Api.Migrations
{
    public partial class AddUserFullText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE FULLTEXT CATALOG FullTextCatalog AS DEFAULT;",
                suppressTransaction: true
            );

            migrationBuilder.Sql(
                @"CREATE FULLTEXT INDEX ON Users
                (
                    FirstName LANGUAGE 1033,
                    LastName LANGUAGE 1033,
                    EmailAddress LANGUAGE 1033
                )
                KEY INDEX PK_Users
                ON FullTextCatalog;",
                suppressTransaction: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Users;", suppressTransaction: true);

            migrationBuilder.Sql(
                "DROP FULLTEXT CATALOG FullTextCatalog;",
                suppressTransaction: true
            );
        }
    }
}
