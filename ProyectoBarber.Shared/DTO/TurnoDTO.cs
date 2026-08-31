using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    public class TurnoDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int BarberoId { get; set; }
        public int ServicioId { get; set; }
        public int EstadoTurnoId { get; set; }
        public DateTime FechaHora { get; set; }
    }
}