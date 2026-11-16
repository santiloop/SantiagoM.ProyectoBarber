using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 10 - Roles del sistema. Se usan en el servidor ([Authorize(Roles = Roles.Dueno)])
    // y en el cliente (para mostrar u ocultar secciones del menu).
    //   Dueno:         todo (servicios, barberos, horarios, reporte, usuarios).
    //   Recepcionista: agenda, turnos, clientes y dias no laborables; ve servicios,
    //                  barberos y horarios pero no los modifica.
    // El rol Cliente se agrega en el paso 11.
    public static class Roles
    {
        public const string Dueno = "Dueno";
        public const string Recepcionista = "Recepcionista";

        public static readonly string[] Todos = { Dueno, Recepcionista };

        public static string Nombre(string rol) => rol == Dueno ? "Dueño" : rol;
    }
}
