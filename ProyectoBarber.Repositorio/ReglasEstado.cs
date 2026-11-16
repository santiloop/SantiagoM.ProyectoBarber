using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;

namespace ProyectoBarber.Repositorio
{
    // Paso 4 - Reglas de cambio de estado del turno (CU05, CU06 y correcciones C7).
    //
    //   Pendiente   -> En atencion   (Iniciar atencion; se puede antes de la hora)
    //   Pendiente   -> Ausente       (recien desde la hora del turno)
    //   Pendiente   -> Cancelado     (en cualquier momento; motivo opcional)
    //   En atencion -> Atendido      (Finalizar atencion)
    //   En atencion -> Cancelado     (motivo OBLIGATORIO)
    //   En atencion -> Ausente       (motivo OBLIGATORIO; desde la hora del turno)
    //   Atendido / Ausente / Cancelado -> Pendiente  (correccion de un error, sin limite de tiempo)
    public static class ReglasEstado
    {
        public const string Pendiente = "PEN";
        public const string EnAtencion = "ENA";
        public const string Atendido = "ATE";
        public const string Ausente = "AUS";
        public const string Cancelado = "CAN";

        private static readonly Dictionary<string, string[]> Permitidos = new()
        {
            [Pendiente] = new[] { EnAtencion, Ausente, Cancelado },
            [EnAtencion] = new[] { Atendido, Cancelado, Ausente },
            [Atendido] = new[] { Pendiente },
            [Ausente] = new[] { Pendiente },
            [Cancelado] = new[] { Pendiente }
        };

        // Devuelve null si el cambio es valido, o el mensaje de por que no.
        public static string? Validar(string actual, string nuevo, Turno turno, string? motivo, DateTime ahora)
        {
            if (actual == nuevo)
                return "El turno ya está en ese estado.";

            if (!Permitidos.TryGetValue(actual, out var destinos) || !destinos.Contains(nuevo))
                return $"No se puede pasar de {Nombre(actual)} a {Nombre(nuevo)}.";

            if (actual == EnAtencion && (nuevo == Cancelado || nuevo == Ausente) && string.IsNullOrWhiteSpace(motivo))
                return $"Para pasar un turno En atención a {Nombre(nuevo)} hay que escribir el motivo.";

            if (nuevo == Ausente && ahora < turno.FechaHora)
                return $"Todavía no es la hora del turno ({turno.FechaHora:HH:mm}). Si el cliente avisó que no viene, cancelalo.";

            return null;
        }

        public static string Nombre(string codigo)
        {
            return codigo switch
            {
                Pendiente => "Pendiente",
                EnAtencion => "En atención",
                Atendido => "Atendido",
                Ausente => "Ausente",
                Cancelado => "Cancelado",
                _ => codigo
            };
        }
    }
}
