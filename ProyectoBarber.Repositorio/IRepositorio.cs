using System;
using System.Collections.Generic;
using System.Text;

using ProyectoBarber.BD.Datos.Entity;

namespace ProyectoBarber.Repositorio
{
    public interface IRepositorio<E> where E : class, IEntityBase
    {
        Task<List<E>> Select();
        Task<E?> SelectById(int id);
        Task<E> Insert(E entity);
        Task<bool> Update(E entity);
        Task<bool> Delete(int id);
        Task<bool> Existe(E entity);
    }
}