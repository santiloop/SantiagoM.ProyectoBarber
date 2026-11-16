using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.BD.Datos.Entity
{
    // EstadoTurno: Pendiente, Atendido, Ausente o Cancelado.
    // Se cambia en CU05 (cancelar) y CU06 (atendido / ausente).
    // Paso 1: se agrega Codigo (como Estado.Codigo de la catedra) para que el codigo
    // reconozca cada estado sin depender del Id. Los 4 estados se cargan solos
    // en la migracion (HasData en AppDbContext) y no se editan desde la API.
    public class EstadoTurno : EntityBase
    {
        // PEN, ATE, AUS o CAN
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(3, ErrorMessage = "Máxima longitud 3 caracteres.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(50, ErrorMessage = "Máxima longitud 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        public List<Turno> Turnos { get; set; } = new();
    }
}
