using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.BD.Datos.Entity
{
    // HorarioLocal: una franja de atencion del local en un dia de la semana.
    // CU15 - Configurar horario del local (Dueno).
    // Un dia puede tener varias franjas (ej. lunes 10-13 y 15-21).
    // Un dia SIN franjas es un dia que no se trabaja (ej. martes).
    // Se carga de entrada lunes a viernes de 10 a 13 y de 15 a 21 (HasData en AppDbContext).
    public class HorarioLocal : EntityBase
    {
        // Domingo = 0, Lunes = 1 ... Sabado = 6 (System.DayOfWeek)
        [Required(ErrorMessage = "El dato es obligatorio")]
        public DayOfWeek DiaSemana { get; set; }

        [Required(ErrorMessage = "El dato es obligatorio")]
        public TimeOnly HoraDesde { get; set; }

        [Required(ErrorMessage = "El dato es obligatorio")]
        public TimeOnly HoraHasta { get; set; }
    }
}
