using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoBarber.BD.Datos.Entity
{
    public class Turno : EntityBase
    {
        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [ForeignKey(nameof(Barbero))]
        public int BarberoId { get; set; }
        public barbero Barbero { get; set; }

        [ForeignKey(nameof(Servicio))]
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }

        [ForeignKey(nameof(EstadoTurno))]
        public int EstadoTurnoId { get; set; }
        public EstadoTurno EstadoTurno { get; set; }

        public DateTime FechaHora { get; set; }
    }
}



