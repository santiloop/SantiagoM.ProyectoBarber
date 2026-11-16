using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;

namespace ProyectoBarber.Repositorio
{
    // Contrato del repositorio de dias no laborables (CU16).
    public interface IDiaNoLaborableRepositorio : IRepositorio<DiaNoLaborable>
    {
        // true si ya hay un dia no laborable con esa fecha. idExcluir = el que se esta editando (0 en un alta).
        Task<bool> ExisteFecha(DateOnly fecha, int idExcluir);

        // Dias no laborables desde una fecha en adelante, ordenados.
        Task<List<DiaNoLaborable>> SelectDesde(DateOnly desde);
    }
}
