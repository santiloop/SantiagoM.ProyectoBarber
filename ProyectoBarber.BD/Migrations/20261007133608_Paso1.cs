using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProyectoBarber.BD.Migrations
{
    /// <inheritdoc />
    public partial class Paso1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Barberos");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Usuarios",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Turnos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Servicios",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "duracion",
                table: "Servicios",
                newName: "Duracion");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Servicios",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "EstadosTurnos",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "EstadosTurnos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Clientes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Barberos",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Barberos",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "Usuarios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Servicios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosTurnos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "EstadosTurnos",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Clientes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Barberos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DNI",
                table: "Barberos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Barberos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "EstadosTurnos",
                columns: new[] { "Id", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, "PEN", "Pendiente" },
                    { 2, "ATE", "Atendido" },
                    { 3, "AUS", "Ausente" },
                    { 4, "CAN", "Cancelado" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_Nombre",
                table: "Servicios",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosTurnos_Codigo",
                table: "EstadosTurnos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Telefono",
                table: "Clientes",
                column: "Telefono",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barberos_DNI",
                table: "Barberos",
                column: "DNI",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_Nombre",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_EstadosTurnos_Codigo",
                table: "EstadosTurnos");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Telefono",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Barberos_DNI",
                table: "Barberos");

            migrationBuilder.DeleteData(
                table: "EstadosTurnos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EstadosTurnos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EstadosTurnos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EstadosTurnos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "EstadosTurnos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Barberos");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Usuarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Turnos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Servicios",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Duracion",
                table: "Servicios",
                newName: "duracion");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Servicios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "EstadosTurnos",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EstadosTurnos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Clientes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Barberos",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Barberos",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "Servicios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "EstadosTurnos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Telefono",
                table: "Clientes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DNI",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
