using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;



namespace ProyectoBarber.BD.Datos.Entity
{
    // Servicio: corte, barba, etc. La duracion (minutos) define cuanto
    // ocupa el turno. CU09 (ABM) y CU02.
    public class Servicio : EntityBase
    {
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(100, ErrorMessage = "Máxima longitud 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        // Duracion ESTIMADA del servicio en minutos. La carga el Dueno y se usa para
        // reservar el horario y evitar turnos superpuestos (paso 3). La duracion real
        // queda registrada con Iniciar / Finalizar atencion (paso 4).
        [Required(ErrorMessage = "El dato es obligatorio")]
        public int Duracion { get; set; }

        // Paso 3 - Precio SOLO informativo (se muestra al dar el turno). No se cobra ni factura.
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public List<Turno> Turnos { get; set; } = new();
    }
}
