using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Respuesta de un cambio de horario o de un dia no laborable (CU15 y CU08).
    // Regla acordada: el cambio se guarda igual, y se devuelven los turnos pendientes
    // que quedaron afuera para que el recepcionista los reprograme o cancele.
    public class CambioHorarioDTO
    {
        // Id del registro creado o modificado.
        public int Id { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        // Turnos pendientes que quedaron fuera del horario (lista vacia si no hay).
        public List<TurnoListadoDTO> TurnosAfectados { get; set; } = new();
    }
}
