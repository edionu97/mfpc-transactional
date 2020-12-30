using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseSystem.Persistence.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaitForGraph",
                columns: table => new
                {
                    WaitForGraphId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LockType = table.Column<int>(type: "int", nullable: false),
                    LockObject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockTable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionThatHasLockId = table.Column<int>(type: "int", nullable: false),
                    TransactionThatWantsLockId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitForGraph", x => x.WaitForGraphId);
                    table.ForeignKey(
                        name: "FK_WaitForGraph_Transactions_TransactionThatHasLockId",
                        column: x => x.TransactionThatHasLockId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId");
                    table.ForeignKey(
                        name: "FK_WaitForGraph_Transactions_TransactionThatWantsLockId",
                        column: x => x.TransactionThatWantsLockId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaitForGraph_TransactionThatHasLockId",
                table: "WaitForGraph",
                column: "TransactionThatHasLockId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitForGraph_TransactionThatWantsLockId",
                table: "WaitForGraph",
                column: "TransactionThatWantsLockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaitForGraph");
        }
    }
}
