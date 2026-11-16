using System;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 13 - DTO "Resumen" (como EstadoResumenDTO de la catedra).
    // Lo arma el REPOSITORIO con una proyeccion (Select) y no el controller.
    // GET api/estadoturno/{codigo}  ->  { "codigo": "PEN", "nombre": "Pendiente", "cantidadTurnos": 12 }
    public class EstadoTurnoResumenDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        // Cuantos turnos estan hoy en ese estado (sale de la navegacion EstadoTurno.Turnos).
        public int CantidadTurnos { get; set; }
    }
}
