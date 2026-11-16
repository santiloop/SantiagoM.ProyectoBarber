using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProyectoBarber.BD.Migrations
{
    /// <inheritdoc />
    public partial class Paso2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiasNoLaborables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasNoLaborables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HorariosBarberos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarberoId = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraDesde = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraHasta = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosBarberos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosBarberos_Barberos_BarberoId",
                        column: x => x.BarberoId,
                        principalTable: "Barberos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HorariosLocal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraDesde = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraHasta = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosLocal", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "HorariosLocal",
                columns: new[] { "Id", "DiaSemana", "HoraDesde", "HoraHasta" },
                values: new object[,]
                {
                    { 1, 1, new TimeOnly(10, 0, 0), new TimeOnly(13, 0, 0) },
                    { 2, 1, new TimeOnly(15, 0, 0), new TimeOnly(21, 0, 0) },
                    { 3, 2, new TimeOnly(10, 0, 0), new TimeOnly(13, 0, 0) },
                    { 4, 2, new TimeOnly(15, 0, 0), new TimeOnly(21, 0, 0) },
                    { 5, 3, new TimeOnly(10, 0, 0), new TimeOnly(13, 0, 0) },
                    { 6, 3, new TimeOnly(15, 0, 0), new TimeOnly(21, 0, 0) },
                    { 7, 4, new TimeOnly(10, 0, 0), new TimeOnly(13, 0, 0) },
                    { 8, 4, new TimeOnly(15, 0, 0), new TimeOnly(21, 0, 0) },
                    { 9, 5, new TimeOnly(10, 0, 0), new TimeOnly(13, 0, 0) },
                    { 10, 5, new TimeOnly(15, 0, 0), new TimeOnly(21, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiasNoLaborables_Fecha",
                table: "DiasNoLaborables",
                column: "Fecha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HorariosBarberos_BarberoId",
                table: "HorariosBarberos",
                column: "BarberoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiasNoLaborables");

            migrationBuilder.DropTable(
                name: "HorariosBarberos");

            migrationBuilder.DropTable(
                name: "HorariosLocal");
        }
    }
}
