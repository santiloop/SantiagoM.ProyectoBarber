using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoBarber.Server.Controllers
{
    [ApiController]
    [Route("api/turno")]
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoRepositorio repositorio;

        public TurnoController(ITurnoRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet] //api turno
        public async Task<ActionResult<List<Turno>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");
            else if (lista.Count == 0)
                return Ok("Lista sin registros.");

            return Ok(lista);
        }

        [HttpGet("listaturnos")] //api turno    listaturnos
        public async Task<ActionResult<List<TurnoListadoDTO>>> ListaTurnos()
        {
            var lista = await repositorio.ListaTurnos();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");
            else if (lista.Count == 0)
                return NotFound("Lista sin registros.");

            return Ok(lista);
        }

        [HttpGet("{id:int}")] //api turno5
        public async Task<ActionResult<TurnoDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            TurnoDTO DTO = new TurnoDTO();
            DTO.Id = entidad.Id;
            DTO.ClienteId = entidad.ClienteId;
            DTO.BarberoId = entidad.BarberoId;
            DTO.ServicioId = entidad.ServicioId;
            DTO.EstadoTurnoId = entidad.EstadoTurnoId;
            DTO.FechaHora = entidad.FechaHora;

            return Ok(DTO);
        }

        [HttpPost] //api turno
        public async Task<ActionResult<int>> Post(TurnoDTO DTO)
        {
            Turno entidad = new Turno();
            entidad.ClienteId = DTO.ClienteId;
            entidad.BarberoId = DTO.BarberoId;
            entidad.ServicioId = DTO.ServicioId;
            entidad.EstadoTurnoId = DTO.EstadoTurnoId;
            entidad.FechaHora = DTO.FechaHora;

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        [HttpPut("{id:int}")] //api/turno5
        public async Task<ActionResult<bool>> Put(int id, TurnoDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            var entidad = new Turno();
            entidad.Id = DTO.Id;
            entidad.ClienteId = DTO.ClienteId;
            entidad.BarberoId = DTO.BarberoId;
            entidad.ServicioId = DTO.ServicioId;
            entidad.EstadoTurnoId = DTO.EstadoTurnoId;
            entidad.FechaHora = DTO.FechaHora;

            var resultado = await repositorio.Update(entidad);

            return Ok(resultado);
        }

        [HttpDelete("{id:int}")] //api/turno5
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var resultado = await repositorio.Delete(id);
            if (!resultado)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(true);
        }
    }
}

