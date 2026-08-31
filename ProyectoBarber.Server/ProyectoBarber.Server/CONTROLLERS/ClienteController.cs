using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoBarber.Server.Controllers
{
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepositorio repositorio;

        public ClienteController(IClienteRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet] //api cliente
        public async Task<ActionResult<List<Cliente>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");
            else if (lista.Count == 0)
                return Ok("Lista sin registros.");

            return Ok(lista);
        }

        [HttpGet("{id:int}")] //api cliente5
        public async Task<ActionResult<ClienteDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            ClienteDTO DTO = new ClienteDTO();
            DTO.Id = entidad.Id;
            DTO.Nombre = entidad.Nombre;
            DTO.Telefono = entidad.Telefono;

            return Ok(DTO);
        }

        [HttpPost] //api cliente
        public async Task<ActionResult<int>> Post(ClienteDTO DTO)
        {
            Cliente entidad = new Cliente();
            entidad.Nombre = DTO.Nombre;
            entidad.Telefono = DTO.Telefono;

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        [HttpPut("{id:int}")] //api/cliente5
        public async Task<ActionResult<bool>> Put(int id, ClienteDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            var entidad = new Cliente();
            entidad.Id = DTO.Id;
            entidad.Nombre = DTO.Nombre;
            entidad.Telefono = DTO.Telefono;

            var resultado = await repositorio.Update(entidad);

            return Ok(resultado);
        }

        [HttpDelete("{id:int}")] //api/cliente5
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var resultado = await repositorio.Delete(id);
            if (!resultado)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(true);
        }
    }
}
