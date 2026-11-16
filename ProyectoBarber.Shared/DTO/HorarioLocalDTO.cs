using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // DTO de una franja del horario del local. CU15.
    public class HorarioLocalDTO
    {
        public int Id { get; set; }

        // 0 = Domingo, 1 = Lunes ... 6 = Sabado
        [Range(0, 6, ErrorMessage = "El día tiene que ser un número de 0 (domingo) a 6 (sábado).")]
        public int DiaSemana { get; set; }

        // Solo lectura: el nombre del dia ("Lunes") para mostrar en pantalla.
        public string DiaNombre { get; set; } = string.Empty;

        // Formato "10:00:00"
        public TimeOnly HoraDesde { get; set; }
        public TimeOnly HoraHasta { get; set; }
    }
}
