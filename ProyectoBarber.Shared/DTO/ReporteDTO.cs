using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 9 - CU10 Reporte de un periodo (un mes o una semana).
    // Respuesta de GET api/turno/reporte?desde=2026-10-01&hasta=2026-10-31
    // La recaudacion es ESTIMADA: suma el precio guardado en cada turno Atendido.
    public class ReporteDTO
    {
        public DateOnly Desde { get; set; }
        public DateOnly Hasta { get; set; }

        // Totales del periodo.
        public ReporteResumenDTO Resumen { get; set; } = new();

        // Semanas de lunes a domingo, recortadas al periodo.
        public List<ReporteSemanaDTO> Semanas { get; set; } = new();

        public List<ReporteBarberoDTO> Barberos { get; set; } = new();
        public List<ReporteServicioDTO> Servicios { get; set; } = new();

        // Clientes con mas ausencias en el periodo (los 10 primeros).
        public List<ReporteClienteDTO> ClientesQueFaltan { get; set; } = new();

        // "Ventas y cortes": cada turno Atendido con lo que se cobro.
        public List<ReporteVentaDTO> Ventas { get; set; } = new();
    }

    public class ReporteResumenDTO
    {
        public int Total { get; set; }
        public int Pendientes { get; set; }
        public int EnAtencion { get; set; }
        public int Atendidos { get; set; }
        public int Ausentes { get; set; }
        public int Cancelados { get; set; }
        // Ausentes sobre (Atendidos + Ausentes), en porcentaje.
        public decimal PorcentajeAusentismo { get; set; }
        public decimal Recaudacion { get; set; }
    }

    public class ReporteSemanaDTO
    {
        public DateOnly Desde { get; set; }
        public DateOnly Hasta { get; set; }
        public ReporteResumenDTO Resumen { get; set; } = new();
    }

    public class ReporteBarberoDTO
    {
        public string Barbero { get; set; } = string.Empty;
        public int Atendidos { get; set; }
        public int Ausentes { get; set; }
        public int Cancelados { get; set; }
        // Suma de (fin real - inicio real) de los turnos atendidos.
        public int MinutosTrabajados { get; set; }
        public decimal Recaudacion { get; set; }
    }

    public class ReporteServicioDTO
    {
        public string Servicio { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Recaudacion { get; set; }
    }

    public class ReporteClienteDTO
    {
        public string Cliente { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int Ausencias { get; set; }
    }

    public class ReporteVentaDTO
    {
        public int TurnoId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Barbero { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime? FechaHoraInicioReal { get; set; }
        public DateTime? FechaHoraFinReal { get; set; }
    }
}
