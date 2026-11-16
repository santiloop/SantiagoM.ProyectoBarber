using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // BARBERO CONTROLLER  -  ruta base: api/barbero
    // CU08 - Gestionar barberos (ABM: listar, alta, modificar, baja).
    // Actor: Dueno. CU02 usa este listado para asignar un barbero libre.
    // ==================================================================
    // Paso 10 - Ver: dueno y recepcionista. Alta, modificacion y baja: solo el dueno.
    [Authorize]
    [ApiController]
    [Route("api/barbero")]
    public class BarberoController : ControllerBase
    {
        private readonly IBarberoRepositorio repositorio;

        public BarberoController(IBarberoRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        // ------------------------------------------------------------------
        // CU08 - Listar barberos
        // GET api/barbero
        // Paso 1: si no hay registros devuelve una lista vacia (antes devolvia
        // un texto y la page de Blazor no lo podia leer como lista).
        // ------------------------------------------------------------------
        [HttpGet] //api barbero
        public async Task<ActionResult<List<Barbero>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");

            return Ok(lista);
        }

        // ------------------------------------------------------------------
        // CU08 - Consultar un barbero (para editarlo)
        // GET api/barbero/{id}
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")] //api barbero5
        public async Task<ActionResult<BarberoDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            BarberoDTO DTO = new BarberoDTO();
            DTO.Id = entidad.Id;
            DTO.DNI = entidad.DNI;
            DTO.Nombre = entidad.Nombre;
            DTO.Activo = entidad.Activo;
            DTO.Telefono = entidad.Telefono;   // Paso 10

            return Ok(DTO);
        }

        // ------------------------------------------------------------------
        // CU08 - Alta de barbero
        // POST api/barbero  -  devuelve el Id creado.
        // Extension 6b: rechaza un DNI que ya existe.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpPost] //api barbero
        public async Task<ActionResult<int>> Post(BarberoDTO DTO)
        {
            if (await repositorio.ExisteDNI(DTO.DNI, 0))
                return BadRequest($"Ya existe un barbero con el DNI {DTO.DNI}.");

            Barbero entidad = new Barbero();
            entidad.DNI = DTO.DNI;
            entidad.Nombre = DTO.Nombre;
            entidad.Activo = DTO.Activo;
            entidad.Telefono = string.IsNullOrWhiteSpace(DTO.Telefono) ? null : DTO.Telefono.Trim();   // Paso 10

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        // ------------------------------------------------------------------
        // CU08 - Modificar barbero (incluye activarlo / desactivarlo)
        // PUT api/barbero/{id}  -  el id de la ruta debe coincidir con el del DTO.
        // Extension 6b: rechaza un DNI que ya tiene otro barbero.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpPut("{id:int}")] //api/barbero5
        public async Task<ActionResult<bool>> Put(int id, BarberoDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            if (await repositorio.ExisteDNI(DTO.DNI, id))
                return BadRequest($"Ya existe otro barbero con el DNI {DTO.DNI}.");

            var entidad = new Barbero();
            entidad.Id = DTO.Id;
            entidad.DNI = DTO.DNI;
            entidad.Nombre = DTO.Nombre;
            entidad.Activo = DTO.Activo;
            entidad.Telefono = string.IsNullOrWhiteSpace(DTO.Telefono) ? null : DTO.Telefono.Trim();   // Paso 10

            var resultado = await repositorio.Update(entidad);

            return Ok(resultado);
        }

        // ------------------------------------------------------------------
        // CU08 - Baja de barbero
        // DELETE api/barbero/{id}
        // Extension Eliminar-a: si tiene turnos, la base rechaza el borrado
        // (Restrict) y se informa que conviene desactivarlo.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpDelete("{id:int}")] //api/barbero5
        public async Task<ActionResult<bool>> Delete(int id)
        {
            bool resultado;
            try
            {
                resultado = await repositorio.Delete(id);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se puede eliminar el barbero porque tiene turnos registrados. Desactivalo en su lugar.");
            }

            if (!resultado)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(true);
        }
    }
}
