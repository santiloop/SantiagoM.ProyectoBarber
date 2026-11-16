using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProyectoBarber.BD.Datos.Entity
{
    // HorarioBarbero: una franja en la que trabaja un barbero un dia de la semana.
    // CU08 - Gestionar barberos (el Dueno carga los dias y horarios de cada uno).
    // Regla: la franja tiene que quedar dentro de una franja del horario del local ese dia.
    public class HorarioBarbero : EntityBase
    {
        [ForeignKey(nameof(Barbero))]
        public int BarberoId { get; set; }
        public Barbero Barbero { get; set; }

        // Domingo = 0, Lunes = 1 ... Sabado = 6 (System.DayOfWeek)
        [Required(ErrorMessage = "El dato es obligatorio")]
        public DayOfWeek DiaSemana { get; set; }

        [Required(ErrorMessage = "El dato es obligatorio")]
        public TimeOnly HoraDesde { get; set; }

        [Required(ErrorMessage = "El dato es obligatorio")]
        public TimeOnly HoraHasta { get; set; }
    }
}
