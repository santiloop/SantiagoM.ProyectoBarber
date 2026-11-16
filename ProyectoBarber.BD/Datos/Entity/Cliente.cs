using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace ProyectoBarber.BD.Datos.Entity
{
    // Cliente: persona que pide un turno. No tiene login. CU07 (ABM) y CU02.
    public class Cliente : EntityBase
    {
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(200, ErrorMessage = "Máxima longitud 200 caracteres.")]
        public string Nombre { get; set; }

        // Paso 1: el telefono pasa de int a texto. Un int no alcanza para un
        // celular con 549 y pierde los ceros iniciales. Es unico (indice en AppDbContext)
        // porque se usa para buscar al cliente.
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(20, ErrorMessage = "Máxima longitud 20 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        public List<Turno> Turnos { get; set; } = new();
    }
}
