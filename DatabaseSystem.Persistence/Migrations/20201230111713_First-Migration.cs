using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseSystem.Persistence.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "Locks",
                columns: table => new
                {
                    LockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LockType = table.Column<int>(type: "int", nullable: false),
                    Object = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locks", x => x.LockId);
                    table.ForeignKey(
                        name: "FK_Locks_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Operation",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    DatabaseQuery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operation", x => x.OperationId);
                    table.ForeignKey(
                        name: "FK_Operation_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaitForGraphs",
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
                    table.PrimaryKey("PK_WaitForGraphs", x => x.WaitForGraphId);
                    table.ForeignKey(
                        name: "FK_WaitForGraphs_Transactions_TransactionThatHasLockId",
                        column: x => x.TransactionThatHasLockId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId");
                    table.ForeignKey(
                        name: "FK_WaitForGraphs_Transactions_TransactionThatWantsLockId",
                        column: x => x.TransactionThatWantsLockId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locks_TransactionId",
                table: "Locks",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_TransactionId",
                table: "Operation",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitForGraphs_TransactionThatHasLockId",
                table: "WaitForGraphs",
                column: "TransactionThatHasLockId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitForGraphs_TransactionThatWantsLockId",
                table: "WaitForGraphs",
                column: "TransactionThatWantsLockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locks");

            migrationBuilder.DropTable(
                name: "Operation");

            migrationBuilder.DropTable(
                name: "WaitForGraphs");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
