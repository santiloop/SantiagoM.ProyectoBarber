using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // CLIENTE CONTROLLER  -  ruta base: api/cliente
    // CU07 - Gestionar clientes (ABM: listar, alta, modificar, baja).
    // Actor: Recepcionista. Tambien se usa desde CU02 para el alta rapida
    // de un cliente nuevo al registrar un turno.
    // ==================================================================
    // Paso 10 - Dueno y recepcionista (hay que haber iniciado sesion).
    [Authorize]
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepositorio repositorio;

        public ClienteController(IClienteRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        // ------------------------------------------------------------------
        // CU07 - Listar clientes
        // GET api/cliente
        // Paso 1: si no hay registros devuelve una lista vacia.
        // ------------------------------------------------------------------
        [HttpGet] //api cliente
        public async Task<ActionResult<List<Cliente>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");

            return Ok(lista);
        }

        // ------------------------------------------------------------------
        // CU07 - Consultar un cliente (para editarlo)
        // GET api/cliente/{id}
        // ------------------------------------------------------------------
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

        // ------------------------------------------------------------------
        // CU07 - Alta de cliente (y alta rapida desde CU02)
        // POST api/cliente  -  devuelve el Id creado.
        // Extension 6b: rechaza un telefono que ya existe.
        // ------------------------------------------------------------------
        [HttpPost] //api cliente
        public async Task<ActionResult<int>> Post(ClienteDTO DTO)
        {
            if (await repositorio.ExisteTelefono(DTO.Telefono, 0))
                return BadRequest($"Ya existe un cliente con el teléfono {DTO.Telefono}.");

            Cliente entidad = new Cliente();
            entidad.Nombre = DTO.Nombre;
            entidad.Telefono = DTO.Telefono;

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        // ------------------------------------------------------------------
        // CU07 - Modificar cliente
        // PUT api/cliente/{id}  -  el id de la ruta debe coincidir con el del DTO.
        // Extension 6b: rechaza un telefono que ya tiene otro cliente.
        // ------------------------------------------------------------------
        [HttpPut("{id:int}")] //api/cliente5
        public async Task<ActionResult<bool>> Put(int id, ClienteDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            if (await repositorio.ExisteTelefono(DTO.Telefono, id))
                return BadRequest($"Ya existe otro cliente con el teléfono {DTO.Telefono}.");

            var entidad = new Cliente();
            entidad.Id = DTO.Id;
            entidad.Nombre = DTO.Nombre;
            entidad.Telefono = DTO.Telefono;

            var resultado = await repositorio.Update(entidad);

            return Ok(resultado);
        }

        // ------------------------------------------------------------------
        // CU07 - Baja de cliente
        // DELETE api/cliente/{id}
        // Extension Eliminar-a: si el cliente tiene turnos, la base rechaza
        // el borrado (Restrict) y se informa el motivo.
        // ------------------------------------------------------------------
        [HttpDelete("{id:int}")] //api/cliente5
        public async Task<ActionResult<bool>> Delete(int id)
        {
            bool resultado;
            try
            {
                resultado = await repositorio.Delete(id);
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se puede eliminar el cliente porque tiene turnos registrados.");
            }

            if (!resultado)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(true);
        }
    }
}
