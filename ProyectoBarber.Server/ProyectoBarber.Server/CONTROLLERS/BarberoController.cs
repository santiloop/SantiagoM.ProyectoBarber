using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoBarber.Server.Controllers
{
    [ApiController]
    [Route("api/barbero")]
    public class BarberoController : ControllerBase
    {
        private readonly IBarberoRepositorio repositorio;

        public BarberoController(IBarberoRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet] //api barbero
        public async Task<ActionResult<List<barbero>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");
            else if (lista.Count == 0)
                return Ok("Lista sin registros.");

            return Ok(lista);
        }

        [HttpGet("{id:int}")] //api barbero5
        public async Task<ActionResult<BarberoDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            BarberoDTO DTO = new BarberoDTO();
            DTO.Id = entidad.Id;
            DTO.DNI = entidad.DNI;
            DTO.nombre = entidad.nombre;
            DTO.Estado = entidad.Estado;

            return Ok(DTO);
        }

        [HttpPost] //api barbero
        public async Task<ActionResult<int>> Post(BarberoDTO DTO)
        {
            barbero entidad = new barbero();
            entidad.DNI = DTO.DNI;
            entidad.nombre = DTO.nombre;
            entidad.Estado = DTO.Estado;

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        [HttpPut("{id:int}")] //api/barbero5
        public async Task<ActionResult<bool>> Put(int id, BarberoDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            var entidad = new barbero();
            entidad.Id = DTO.Id;
            entidad.DNI = DTO.DNI;
            entidad.nombre = DTO.nombre;
            entidad.Estado = DTO.Estado;

            var resultado = await repositorio.Update(entidad);

            return Ok(resultado);
        }

        [HttpDelete("{id:int}")] //api/barbero5
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var resultado = await repositorio.Delete(id);
            if (!resultado)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(true);
        }
    }
}
