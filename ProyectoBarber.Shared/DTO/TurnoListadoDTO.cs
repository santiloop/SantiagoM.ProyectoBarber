using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    public class TurnoListadoDTO
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Barbero { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
    }
}
