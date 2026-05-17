using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEApi.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Ambulancias_AmbulanciaId",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_UserInfoDto_UserId",
                table: "Chamados");

            migrationBuilder.DropTable(
                name: "UserInfoDto");

            migrationBuilder.AlterColumn<Guid>(
                name: "AmbulanciaId",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Ambulancias_AmbulanciaId",
                table: "Chamados",
                column: "AmbulanciaId",
                principalTable: "Ambulancias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_AspNetUsers_UserId",
                table: "Chamados",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Ambulancias_AmbulanciaId",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_AspNetUsers_UserId",
                table: "Chamados");

            migrationBuilder.AlterColumn<Guid>(
                name: "AmbulanciaId",
                table: "Chamados",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "UserInfoDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Cargo = table.Column<string>(type: "TEXT", nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    IsAuthenticated = table.Column<bool>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Roles = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfoDto", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Ambulancias_AmbulanciaId",
                table: "Chamados",
                column: "AmbulanciaId",
                principalTable: "Ambulancias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_UserInfoDto_UserId",
                table: "Chamados",
                column: "UserId",
                principalTable: "UserInfoDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
