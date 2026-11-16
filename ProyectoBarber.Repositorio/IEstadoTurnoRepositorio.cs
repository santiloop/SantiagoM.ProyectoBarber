using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Contrato del repositorio de EstadoTurno (consulta para CU03, CU05 y CU06).
    // Los estados son fijos: el controller solo los lee.
    public interface IEstadoTurnoRepositorio : IRepositorio<EstadoTurno>
    {
        // Busca un estado por su codigo (PEN, ENA, ATE, AUS, CAN). Devuelve la entidad:
        // la usan los controllers para poner el estado a un turno.
        Task<EstadoTurno?> SelectByCodigo(string codigo);

        // Paso 13 - Como SelectByCodigo de la catedra: devuelve un DTO "Resumen"
        // armado en el repositorio (con la cantidad de turnos en ese estado).
        Task<EstadoTurnoResumenDTO?> SelectResumenByCodigo(string codigo);
    }
}
