using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoBarber.BD.Migrations
{
    /// <inheritdoc />
    public partial class RelacionesTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarberoDNI",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "ClienteDNI",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Turnos");

            migrationBuilder.RenameColumn(
                name: "ServicioID",
                table: "Turnos",
                newName: "ServicioId");

            migrationBuilder.RenameColumn(
                name: "Hora",
                table: "Turnos",
                newName: "EstadoTurnoId");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Turnos",
                newName: "ClienteId");

            migrationBuilder.AddColumn<int>(
                name: "BarberoId",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHora",
                table: "Turnos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_BarberoId",
                table: "Turnos",
                column: "BarberoId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ClienteId",
                table: "Turnos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_EstadoTurnoId",
                table: "Turnos",
                column: "EstadoTurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ServicioId",
                table: "Turnos",
                column: "ServicioId");

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

            migrationBuilder.DropIndex(
                name: "IX_Turnos_BarberoId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_ClienteId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_EstadoTurnoId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_ServicioId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "BarberoId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "FechaHora",
                table: "Turnos");

            migrationBuilder.RenameColumn(
                name: "ServicioId",
                table: "Turnos",
                newName: "ServicioID");

            migrationBuilder.RenameColumn(
                name: "EstadoTurnoId",
                table: "Turnos",
                newName: "Hora");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Turnos",
                newName: "Fecha");

            migrationBuilder.AddColumn<string>(
                name: "BarberoDNI",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClienteDNI",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
