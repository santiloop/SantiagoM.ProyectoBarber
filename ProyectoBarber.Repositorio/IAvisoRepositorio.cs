using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;

namespace ProyectoBarber.Repositorio
{
    // Paso 12 - CU13 Avisos.
    public interface IAvisoRepositorio : IRepositorio<Aviso>
    {
        // Crea un aviso. usuarioId = quien hizo el cambio (null = cliente desde la web).
        Task Crear(string tipo, string mensaje, DateOnly? fechaReferencia, int? usuarioId);

        // Avisos desde una fecha, mas nuevos primero, con el usuario cargado.
        Task<List<Aviso>> Desde(DateTime desde);

        // Cantidad de avisos sin leer para un usuario (no cuenta los que genero el mismo).
        Task<int> NoLeidos(int usuarioId, int ultimoLeidoId, DateTime desde);

        // Id del aviso mas nuevo (0 si no hay).
        Task<int> UltimoId();

        // Borra los avisos mas viejos que la fecha (se guardan 2 meses).
        Task BorrarAnteriores(DateTime limite);
    }
}
