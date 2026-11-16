using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoBarber.BD.Migrations
{
    /// <inheritdoc />
    public partial class Paso4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmado",
                table: "Turnos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaConfirmacion",
                table: "Turnos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraFinReal",
                table: "Turnos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraInicioReal",
                table: "Turnos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HistorialesEstadosTurnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TurnoId = table.Column<int>(type: "int", nullable: false),
                    EstadoAnteriorId = table.Column<int>(type: "int", nullable: false),
                    EstadoNuevoId = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesEstadosTurnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesEstadosTurnos_EstadosTurnos_EstadoAnteriorId",
                        column: x => x.EstadoAnteriorId,
                        principalTable: "EstadosTurnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialesEstadosTurnos_EstadosTurnos_EstadoNuevoId",
                        column: x => x.EstadoNuevoId,
                        principalTable: "EstadosTurnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialesEstadosTurnos_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EstadosTurnos",
                columns: new[] { "Id", "Codigo", "Nombre" },
                values: new object[] { 5, "ENA", "En atención" });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstadosTurnos_EstadoAnteriorId",
                table: "HistorialesEstadosTurnos",
                column: "EstadoAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstadosTurnos_EstadoNuevoId",
                table: "HistorialesEstadosTurnos",
                column: "EstadoNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstadosTurnos_TurnoId",
                table: "HistorialesEstadosTurnos",
                column: "TurnoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialesEstadosTurnos");

            migrationBuilder.DeleteData(
                table: "EstadosTurnos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "Confirmado",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "FechaConfirmacion",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "FechaHoraFinReal",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "FechaHoraInicioReal",
                table: "Turnos");
        }
    }
}
