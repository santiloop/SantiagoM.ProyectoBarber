using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Server.Controllers
{
    // Paso 5: el proyecto ProyectoBarber.Servicio (HttpService) crea el espacio de nombres
    // 'ProyectoBarber.Servicio', que choca con la entidad Servicio. Este alias aclara que
    // 'Servicio' es la entidad de la BD.
    using Servicio = ProyectoBarber.BD.Datos.Entity.Servicio;

    // ==================================================================
    // SERVICIO CONTROLLER  -  ruta base: api/servicio
    // CU09 - Gestionar servicios (ABM: listar, alta, modificar, baja).
    // Actor: Dueno. CU02 usa la duracion del servicio para calcular
    // cuanto ocupa cada turno. Paso 3: tambien el precio (solo informativo).
    // ==================================================================
    // Paso 10 - Ver: dueno y recepcionista. Alta, modificacion y baja: solo el dueno.
    [Authorize]
    [ApiController]
    [Route("api/servicio")]
    public class ServicioController : ControllerBase
    {
        private readonly IServicioRepositorio repositorio;

        public ServicioController(IServicioRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        // ------------------------------------------------------------------
        // CU09 - Listar servicios
        // GET api/servicio
        // Devuelve ServicioDTO (sin la lista de turnos de cada servicio).
        // ------------------------------------------------------------------
        [HttpGet] //api/servicio
        public async Task<ActionResult<List<ServicioDTO>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");

            var listaDTO = lista
                            .Select(s => new ServicioDTO
                            {
                                Id = s.Id,
                                Nombre = s.Nombre,
                                Duracion = s.Duracion,
                                Precio = s.Precio
                            })
                            .ToList();

            return Ok(listaDTO);
        }

        // ------------------------------------------------------------------
        // CU09 - Consultar un servicio (para editarlo)
        // GET api/servicio/{id}
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")] //api/servicio/5
        public async Task<ActionResult<ServicioDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            ServicioDTO DTO = new ServicioDTO();
            DTO.Id = entidad.Id;
            DTO.Nombre = entidad.Nombre;
            DTO.Duracion = entidad.Duracion;
            DTO.Precio = entidad.Precio;

            return Ok(DTO);
        }

        // ------------------------------------------------------------------
        // CU09 - Alta de servicio
        // POST api/servicio  -  devuelve el Id creado.
        // Extension 6b: rechaza un nombre que ya existe.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpPost] //api/servicio
        public async Task<ActionResult<int>> Post(ServicioDTO DTO)
        {
            if (await repositorio.ExisteNombre(DTO.Nombre, 0))
                return BadRequest($"Ya existe un servicio llamado {DTO.Nombre}.");

            Servicio entidad = new Servicio();
            entidad.Nombre = DTO.Nombre;
            entidad.Duracion = DTO.Duracion;
            entidad.Precio = DTO.Precio;

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        // ------------------------------------------------------------------
        // CU09 - Modificar servicio
        // PUT api/servicio/{id}  -  el id de la ruta debe coincidir con el del DTO.
        // Extension 6b: rechaza un nombre que ya tiene otro servicio.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpPut("{id:int}")] //api/servicio/5
        public async Task<ActionResult<bool>> Put(int id, ServicioDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            if (await repositorio.ExisteNombre(DTO.Nombre, id))
                return BadRequest($"Ya existe otro servicio llamado {DTO.Nombre}.");

            var entidad = new Servicio();
            entidad.Id = DTO.Id;
            entidad.Nombre = DTO.Nombre;
            entidad.Duracion = DTO.Duracion;
            entidad.Precio = DTO.Precio;

            var resultado = await repositorio.Update(entidad);

            return Ok(resultado);
        }

        // ------------------------------------------------------------------
        // CU09 - Baja de servicio
        // DELETE api/servicio/{id}
        // Extension Eliminar-a: si el servicio tiene turnos, la base rechaza
        // el borrado (Restrict) y se informa el motivo.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpDelete("{id:int}")] //api/servicio/5
        public async Task<ActionResult<bool>> Delete(int id)
        {
            bool resultado;
            try
            {
                resultado = await repositorio.Delete(id);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se puede eliminar el servicio porque tiene turnos registrados.");
            }

            if (!resultado)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(true);
        }
    }
}
