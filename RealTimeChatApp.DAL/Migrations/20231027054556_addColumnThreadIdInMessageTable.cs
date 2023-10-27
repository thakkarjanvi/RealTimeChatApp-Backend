using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addColumnThreadIdInMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                  name: "ThreadId", 
                  table: "Messages",
                  type: "int", 
                  nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ThreadId",
                table: "Messages",
                column: "ThreadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_ThreadId",
                table: "Messages",
                column: "ThreadId",
                principalTable: "Messages",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                  name: "FK_Messages_Messages_ThreadId",
                  table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ThreadId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "Messages");
        }
    }
}
