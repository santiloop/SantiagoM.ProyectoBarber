using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 11 - PORTAL DEL CLIENTE (etapa 2): el cliente entra solo con su telefono
    // (y su nombre la primera vez), sin contrasena. Decision acordada: sin proteccion extra.

    // Ingresar / registrarse: POST api/portal/ingresar
    public class PortalIngresoDTO
    {
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [MaxLength(20, ErrorMessage = "Máxima longitud 20 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        // Solo hace falta la primera vez (si el telefono no esta registrado).
        [MaxLength(200, ErrorMessage = "Máxima longitud 200 caracteres.")]
        public string? Nombre { get; set; }
    }

    // Respuesta de ingresar: los datos del cliente y cuantos turnos le quedan este mes.
    public class PortalClienteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public bool EsNuevo { get; set; }
    }

    // Pedir turno: POST api/portal/turno (si el horario esta libre, el turno se crea al instante).
    public class PortalPedirTurnoDTO
    {
        [Required] public string Telefono { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "El servicio es obligatorio.")]
        public int ServicioId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "El barbero es obligatorio.")]
        public int BarberoId { get; set; }
        public DateTime FechaHora { get; set; }
        [MaxLength(200)] public string? Observaciones { get; set; }
    }

    // Reprogramar un turno propio: PUT api/portal/turno/{id}/reprogramar
    public class PortalReprogramarDTO
    {
        [Required] public string Telefono { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "El barbero es obligatorio.")]
        public int BarberoId { get; set; }
        public DateTime FechaHora { get; set; }
    }

    // Cancelar un turno propio: PUT api/portal/turno/{id}/cancelar
    public class PortalCancelarDTO
    {
        [Required] public string Telefono { get; set; } = string.Empty;
    }
}
