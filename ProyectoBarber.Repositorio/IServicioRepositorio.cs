using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;

namespace ProyectoBarber.Repositorio
{
    // Contrato del repositorio de Servicio (CU09).
    public interface IServicioRepositorio : IRepositorio<Servicio>
    {
        // CU09 - Extension 6b: evita nombre repetido. idExcluir = el servicio que se esta editando (0 en un alta).
        Task<bool> ExisteNombre(string nombre, int idExcluir);
    }
}
