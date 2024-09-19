using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentorConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChatIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_User1Id1",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_User2Id1",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId1",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReceiverId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_User1Id1",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_User2Id1",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ReceiverId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SenderId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "User1Id1",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "User2Id1",
                table: "Chats");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Messages",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "Messages",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "User2Id",
                table: "Chats",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "User1Id",
                table: "Chats",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_User1Id",
                table: "Chats",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_User2Id",
                table: "Chats",
                column: "User2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_User1Id",
                table: "Chats",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_User2Id",
                table: "Chats",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_User1Id",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_User2Id",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_User1Id",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_User2Id",
                table: "Chats");

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId1",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId1",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "User2Id",
                table: "Chats",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "User1Id",
                table: "Chats",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "User1Id1",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User2Id1",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId1",
                table: "Messages",
                column: "ReceiverId1");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId1",
                table: "Messages",
                column: "SenderId1");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_User1Id1",
                table: "Chats",
                column: "User1Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_User2Id1",
                table: "Chats",
                column: "User2Id1");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_User1Id1",
                table: "Chats",
                column: "User1Id1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_User2Id1",
                table: "Chats",
                column: "User2Id1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId1",
                table: "Messages",
                column: "ReceiverId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId1",
                table: "Messages",
                column: "SenderId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
