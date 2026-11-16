using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;

namespace ProyectoBarber.Repositorio
{
    // Paso 10 - CU01 Iniciar sesion y CU11 Gestionar usuarios.
    public interface IUsuarioRepositorio : IRepositorio<Usuario>
    {
        Task<Usuario?> SelectByNombreUsuario(string nombreUsuario);

        // true si otro usuario (distinto de idExcluir) ya usa ese nombre de usuario.
        Task<bool> ExisteNombreUsuario(string nombreUsuario, int idExcluir);

        // Cantidad de duenos activos sin contar idExcluir (para no quedar sin ningun dueno).
        Task<int> DuenosActivos(int idExcluir);
    }
}
