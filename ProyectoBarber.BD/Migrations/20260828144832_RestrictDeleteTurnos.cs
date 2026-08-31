using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoBarber.BD.Migrations
{
    /// <inheritdoc />
    public partial class RestrictDeleteTurnos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Barberos_BarberoId",
                table: "Turnos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Clientes_ClienteId",
                table: "Turnos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_EstadosTurnos_EstadoTurnoId",
                table: "Turnos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Servicios_ServicioId",
                table: "Turnos");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Barberos_BarberoId",
                table: "Turnos",
                column: "BarberoId",
                principalTable: "Barberos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Clientes_ClienteId",
                table: "Turnos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_EstadosTurnos_EstadoTurnoId",
                table: "Turnos",
                column: "EstadoTurnoId",
                principalTable: "EstadosTurnos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Servicios_ServicioId",
                table: "Turnos",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Barberos_BarberoId",
                table: "Turnos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Clientes_ClienteId",
                table: "Turnos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_EstadosTurnos_EstadoTurnoId",
                table: "Turnos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Servicios_ServicioId",
                table: "Turnos");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Barberos_BarberoId",
                table: "Turnos",
                column: "BarberoId",
                principalTable: "Barberos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Clientes_ClienteId",
                table: "Turnos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_EstadosTurnos_EstadoTurnoId",
                table: "Turnos",
                column: "EstadoTurnoId",
                principalTable: "EstadosTurnos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Servicios_ServicioId",
                table: "Turnos",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
