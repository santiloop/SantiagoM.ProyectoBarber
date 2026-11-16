using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 10 - CU01 Iniciar sesion: lo que se manda a POST api/auth/login.
    public class LoginDTO
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contrasena { get; set; } = string.Empty;
    }

    // Paso 10 - Respuesta del login: el token (JWT) y los datos para la pantalla.
    public class LoginRespuestaDTO
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
        public bool DebeCambiarContrasena { get; set; }
    }

    // Paso 10 - Cambiar la propia contrasena (PUT api/auth/contrasena).
    public class CambiarContrasenaDTO
    {
        [Required(ErrorMessage = "Escribí la contraseña actual")]
        public string Actual { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribí la contraseña nueva")]
        [MinLength(6, ErrorMessage = "La contraseña tiene que tener al menos 6 caracteres.")]
        public string Nueva { get; set; } = string.Empty;
    }
}
