using System;
using ProyectoBarber.BD.Datos.Entity;
using System.Collections.Generic;
using System.Text;



namespace ProyectoBarber.Repositorio
{
    // Contrato del repositorio de Cliente (CU07).
    public interface IClienteRepositorio : IRepositorio<Cliente>
    {
        // CU07 - Extension 6b: evita telefono repetido. idExcluir = el cliente que se esta editando (0 en un alta).
        Task<bool> ExisteTelefono(string telefono, int idExcluir);

        // Paso 11 - Portal del cliente: busca al cliente por su telefono.
        Task<Cliente?> SelectByTelefono(string telefono);
    }
}
