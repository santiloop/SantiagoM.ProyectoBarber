using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 12 - Un aviso de la campanita (GET api/aviso?dias=7).
    public class AvisoDTO
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateOnly? FechaReferencia { get; set; }
        // Quien lo genero: nombre del usuario o "Cliente (web)".
        public string Autor { get; set; } = string.Empty;
        // true si este usuario todavia no lo habia visto.
        public bool Nuevo { get; set; }
    }
}
