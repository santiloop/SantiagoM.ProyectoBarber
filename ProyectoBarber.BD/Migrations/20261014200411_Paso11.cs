using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoBarber.BD.Migrations
{
    /// <inheritdoc />
    public partial class Paso11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PedidoPorCliente",
                table: "Turnos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PedidoPorCliente",
                table: "Turnos");
        }
    }
}
