using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace ProyectoBarber.BD.Datos.Entity
{
    public class Usuario : EntityBase
    {
        [Required]
        public string NombreUsuario { get; set; } = string.Empty;
        [Required]
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty; // "Recepcionista" o "Dueño"
        public string Nombre { get; set; } = string.Empty;
    }
}
