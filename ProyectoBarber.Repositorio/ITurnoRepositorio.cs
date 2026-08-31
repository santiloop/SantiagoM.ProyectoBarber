using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    public interface ITurnoRepositorio : IRepositorio<Turno>
    {
        Task<List<TurnoListadoDTO>> ListaTurnos();
    }
}