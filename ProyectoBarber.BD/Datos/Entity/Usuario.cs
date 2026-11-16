using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace ProyectoBarber.BD.Datos.Entity
{
    // Usuario: cuenta de acceso del Recepcionista o del Dueno.
    // CU01 (iniciar sesion) y CU11 (gestionar usuarios).
    // Paso 1: largo maximo en NombreUsuario para poder tener indice unico.
    // Paso 10: la contrasena se guarda ENCRIPTADA (hash), Rol = "Dueno" o "Recepcionista"
    //          (ver ProyectoBarber.Shared.DTO.Roles), Activo y cambio de contrasena obligatorio.
    public class Usuario : EntityBase
    {
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(50, ErrorMessage = "Máxima longitud 50 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        // Paso 10 - Hash de la contrasena (PasswordHasher de ASP.NET). Nunca el texto.
        [Required(ErrorMessage = "El dato es obligatorio")]
        public string Contrasena { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Máxima longitud 20 caracteres.")]
        public string Rol { get; set; } = string.Empty; // "Dueno" o "Recepcionista"

        [MaxLength(100, ErrorMessage = "Máxima longitud 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        // Paso 10 - Un usuario inactivo no puede iniciar sesion (es la forma de darlo de baja).
        public bool Activo { get; set; } = true;

        // Paso 10 - true cuando el dueno le pone una contrasena: al entrar la tiene que cambiar.
        public bool DebeCambiarContrasena { get; set; }

        // Paso 12 - Avisos: Id del ultimo aviso que vio este usuario (los mayores son "nuevos").
        public int UltimoAvisoLeidoId { get; set; }
    }
}
