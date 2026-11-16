using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // DTO de EstadoTurno, solo de lectura (para filtros y combos de la agenda, CU03).
    public class EstadoTurnoDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}
