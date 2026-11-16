using System;
using ProyectoBarber.BD.Datos.Entity;
using System.Collections.Generic;
using System.Text;



namespace ProyectoBarber.Repositorio
{
    // Contrato del repositorio de Barbero (CU08).
    public interface IBarberoRepositorio : IRepositorio<Barbero>
    {
        // CU08 - Extension 6b: evita DNI repetido. idExcluir = el barbero que se esta editando (0 en un alta).
        Task<bool> ExisteDNI(string dni, int idExcluir);
    }
}
