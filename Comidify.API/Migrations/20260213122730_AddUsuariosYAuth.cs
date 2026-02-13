using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comidify.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuariosYAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComidaIngredientes_Ingredientes_IngredienteId",
                table: "ComidaIngredientes");

            migrationBuilder.DropIndex(
                name: "IX_MenuComidas_MenuSemanalId_DiaSemana_TipoComida",
                table: "MenuComidas");

            migrationBuilder.DropIndex(
                name: "IX_Ingredientes_Nombre",
                table: "Ingredientes");

            migrationBuilder.DropIndex(
                name: "IX_Comidas_Nombre",
                table: "Comidas");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "MenusSemanales",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Ingredientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Comidas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: true),
                    ProviderId = table.Column<string>(type: "TEXT", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenusSemanales_UsuarioId",
                table: "MenusSemanales",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuComidas_MenuSemanalId",
                table: "MenuComidas",
                column: "MenuSemanalId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredientes_UsuarioId",
                table: "Ingredientes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Comidas_UsuarioId",
                table: "Comidas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ComidaIngredientes_Ingredientes_IngredienteId",
                table: "ComidaIngredientes",
                column: "IngredienteId",
                principalTable: "Ingredientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comidas_Usuarios_UsuarioId",
                table: "Comidas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredientes_Usuarios_UsuarioId",
                table: "Ingredientes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenusSemanales_Usuarios_UsuarioId",
                table: "MenusSemanales",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComidaIngredientes_Ingredientes_IngredienteId",
                table: "ComidaIngredientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Comidas_Usuarios_UsuarioId",
                table: "Comidas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredientes_Usuarios_UsuarioId",
                table: "Ingredientes");

            migrationBuilder.DropForeignKey(
                name: "FK_MenusSemanales_Usuarios_UsuarioId",
                table: "MenusSemanales");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_MenusSemanales_UsuarioId",
                table: "MenusSemanales");

            migrationBuilder.DropIndex(
                name: "IX_MenuComidas_MenuSemanalId",
                table: "MenuComidas");

            migrationBuilder.DropIndex(
                name: "IX_Ingredientes_UsuarioId",
                table: "Ingredientes");

            migrationBuilder.DropIndex(
                name: "IX_Comidas_UsuarioId",
                table: "Comidas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "MenusSemanales");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Ingredientes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Comidas");

            migrationBuilder.CreateIndex(
                name: "IX_MenuComidas_MenuSemanalId_DiaSemana_TipoComida",
                table: "MenuComidas",
                columns: new[] { "MenuSemanalId", "DiaSemana", "TipoComida" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredientes_Nombre",
                table: "Ingredientes",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Comidas_Nombre",
                table: "Comidas",
                column: "Nombre");

            migrationBuilder.AddForeignKey(
                name: "FK_ComidaIngredientes_Ingredientes_IngredienteId",
                table: "ComidaIngredientes",
                column: "IngredienteId",
                principalTable: "Ingredientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
