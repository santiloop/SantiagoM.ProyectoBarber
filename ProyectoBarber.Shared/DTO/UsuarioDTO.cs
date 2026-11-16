using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 10 - CU11 Gestionar usuarios (solo el Dueno).
    // La contrasena NUNCA viaja de vuelta al cliente; solo se manda al crear
    // un usuario o al ponerle una nueva (blanqueo).
    public class UsuarioDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [MaxLength(50, ErrorMessage = "Máxima longitud 50 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "Máxima longitud 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; } = Roles.Recepcionista;

        public bool Activo { get; set; } = true;

        // Solo al crear: contrasena inicial (el usuario la cambia al entrar).
        public string? Contrasena { get; set; }
    }

    // Paso 10 - Blanqueo: el dueno le pone una contrasena nueva a un usuario.
    public class NuevaContrasenaDTO
    {
        [Required(ErrorMessage = "Escribí la contraseña nueva")]
        [MinLength(6, ErrorMessage = "La contraseña tiene que tener al menos 6 caracteres.")]
        public string Nueva { get; set; } = string.Empty;
    }
}
