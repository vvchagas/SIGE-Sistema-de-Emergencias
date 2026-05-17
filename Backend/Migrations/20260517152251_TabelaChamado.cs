using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEApi.Migrations
{
    /// <inheritdoc />
    public partial class TabelaChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "endereco",
                table: "Chamados",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "ChamadoId",
                table: "Paramedicos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AmbulanciaId",
                table: "Chamados",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAbertura",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEncerramento",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NivelPrioridade",
                table: "Chamados",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NomeSolicitante",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Numero",
                table: "Chamados",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PontoReferencia",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StatusChamado",
                table: "Chamados",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusPaciente",
                table: "Chamados",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneSolicitante",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoEmergencia",
                table: "Chamados",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserInfoDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    IsAuthenticated = table.Column<bool>(type: "INTEGER", nullable: false),
                    Roles = table.Column<string>(type: "TEXT", nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", nullable: false),
                    Cargo = table.Column<string>(type: "TEXT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfoDto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paramedicos_ChamadoId",
                table: "Paramedicos",
                column: "ChamadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_AmbulanciaId",
                table: "Chamados",
                column: "AmbulanciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_UserId",
                table: "Chamados",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Paramedicos_Chamados_ChamadoId",
                table: "Paramedicos",
                column: "ChamadoId",
                principalTable: "Chamados",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Ambulancias_AmbulanciaId",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_UserInfoDto_UserId",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Paramedicos_Chamados_ChamadoId",
                table: "Paramedicos");

            migrationBuilder.DropTable(
                name: "UserInfoDto");

            migrationBuilder.DropIndex(
                name: "IX_Paramedicos_ChamadoId",
                table: "Paramedicos");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_AmbulanciaId",
                table: "Chamados");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_UserId",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "ChamadoId",
                table: "Paramedicos");

            migrationBuilder.DropColumn(
                name: "AmbulanciaId",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "DataAbertura",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "DataEncerramento",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "NivelPrioridade",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "NomeSolicitante",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "PontoReferencia",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "StatusChamado",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "StatusPaciente",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "TelefoneSolicitante",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "TipoEmergencia",
                table: "Chamados");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Chamados",
                newName: "endereco");
        }
    }
}
