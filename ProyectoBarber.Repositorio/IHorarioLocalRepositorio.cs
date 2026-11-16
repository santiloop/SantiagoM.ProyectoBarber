using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;

namespace ProyectoBarber.Repositorio
{
    // Contrato del repositorio del horario del local (CU15).
    public interface IHorarioLocalRepositorio : IRepositorio<HorarioLocal>
    {
        // Franjas del local para un dia de la semana, ordenadas por hora.
        Task<List<HorarioLocal>> SelectPorDia(DayOfWeek dia);
    }
}
