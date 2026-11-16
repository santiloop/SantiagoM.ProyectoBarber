using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // ESTADO TURNO CONTROLLER  -  ruta base: api/estadoturno
    // SOLO LECTURA. Los 5 estados (Pendiente, En atencion, Atendido, Ausente y Cancelado)
    // se cargan con la migracion y la logica de CU05 y CU06 depende de ellos,
    // por eso no hay POST, PUT ni DELETE.
    // Se usa para los filtros y combos de la agenda (CU03).
    // ==================================================================
    // Paso 10 - Dueno y recepcionista (hay que haber iniciado sesion).
    [Authorize]
    [ApiController]
    [Route("api/estadoturno")]
    public class EstadoTurnoController : ControllerBase
    {
        private readonly IEstadoTurnoRepositorio repositorio;

        public EstadoTurnoController(IEstadoTurnoRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        // ------------------------------------------------------------------
        // CU03 - Listar los estados posibles de un turno
        // GET api/estadoturno
        // ------------------------------------------------------------------
        [HttpGet] //api/estadoturno
        public async Task<ActionResult<List<EstadoTurnoDTO>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");

            var listaDTO = lista
                            .Select(e => new EstadoTurnoDTO
                            {
                                Id = e.Id,
                                Codigo = e.Codigo,
                                Nombre = e.Nombre
                            })
                            .ToList();

            return Ok(listaDTO);
        }

        // ------------------------------------------------------------------
        // Consultar un estado por Id
        // GET api/estadoturno/{id}
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")] //api/estadoturno/1
        public async Task<ActionResult<EstadoTurnoDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            EstadoTurnoDTO DTO = new EstadoTurnoDTO();
            DTO.Id = entidad.Id;
            DTO.Codigo = entidad.Codigo;
            DTO.Nombre = entidad.Nombre;

            return Ok(DTO);
        }

        // ------------------------------------------------------------------
        // Consultar un estado por codigo (PEN, ENA, ATE, AUS, CAN)
        // GET api/estadoturno/{codigo}   (como GetByCodigo de la catedra)
        // Paso 13 - Devuelve el DTO "Resumen" que arma el repositorio.
        // ------------------------------------------------------------------
        [HttpGet("{codigo}")] //api/estadoturno/CAN
        public async Task<ActionResult<EstadoTurnoResumenDTO>> GetByCodigo(string codigo)
        {
            var DTO = await repositorio.SelectResumenByCodigo(codigo);
            if (DTO is null)
                return NotFound($"No existe el estado con código: {codigo}.");

            return Ok(DTO);
        }
    }
}
