using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.BD.Datos.Entity
{
    // Paso 12 - CU13 Avisos: mensajes que ven el Dueno y la Recepcionista dentro del sistema
    // (campanita). Se crean solos cuando:
    //   - un cliente pide, reprograma o cancela un turno desde la web (portal),
    //   - se agrega, cambia o quita un dia no laborable,
    //   - se agrega, cambia o quita una franja del horario de un barbero.
    // "Leido" es por persona: Usuario.UltimoAvisoLeidoId. Se guardan 2 meses.
    public class Aviso : EntityBase
    {
        public DateTime FechaHora { get; set; }

        // "Turno", "DiaNoLaborable" u "HorarioBarbero" (para el icono y el filtro).
        [MaxLength(20, ErrorMessage = "Máxima longitud 20 caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(400, ErrorMessage = "Máxima longitud 400 caracteres.")]
        public string Mensaje { get; set; } = string.Empty;

        // Dia al que se refiere (para el boton "Ver en la agenda"). Puede no tener.
        public DateOnly? FechaReferencia { get; set; }

        // Quien hizo el cambio (null = un cliente desde la web). A esa persona
        // el aviso no le suma al contador de no leidos.
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
