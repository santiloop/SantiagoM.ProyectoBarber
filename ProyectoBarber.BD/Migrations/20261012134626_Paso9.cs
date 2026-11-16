using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoBarber.BD.Migrations
{
    /// <inheritdoc />
    public partial class Paso9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Turnos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Paso 9 - Los turnos que ya existian toman el precio actual de su servicio
            // (sin esto quedarian en $0 y el reporte no mostraria recaudacion).
            migrationBuilder.Sql(
                "UPDATE t SET t.Precio = s.Precio " +
                "FROM Turnos t INNER JOIN Servicios s ON s.Id = t.ServicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Turnos");
        }
    }
}
