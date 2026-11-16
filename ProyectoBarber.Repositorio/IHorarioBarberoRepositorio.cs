using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;

namespace ProyectoBarber.Repositorio
{
    // Contrato del repositorio de horarios de los barberos (CU08).
    public interface IHorarioBarberoRepositorio : IRepositorio<HorarioBarbero>
    {
        // Todas las franjas de un barbero, ordenadas por dia y hora.
        Task<List<HorarioBarbero>> SelectPorBarbero(int barberoId);

        // Las franjas de TODOS los barberos en un dia (con el Barbero cargado, para mostrar el nombre).
        Task<List<HorarioBarbero>> SelectPorDia(DayOfWeek dia);
    }
}
