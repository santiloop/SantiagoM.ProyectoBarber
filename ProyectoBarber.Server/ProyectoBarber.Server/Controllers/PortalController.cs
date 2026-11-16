using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Server.Controllers
{
    // Paso 5: 'Servicio' es la entidad de la BD (no el proyecto ProyectoBarber.Servicio).
    using Servicio = ProyectoBarber.BD.Datos.Entity.Servicio;

    // ==================================================================
    // PORTAL CONTROLLER  -  ruta base: api/portal   (paso 11, etapa 2)
    // CU12 - El CLIENTE pide su turno desde la web, sin contrasena:
    //   - Entra con su telefono (y su nombre la primera vez: queda registrado).
    //   - Ve los horarios libres con el mismo selector que la recepcionista.
    //   - Si el horario esta libre, el turno se crea AL INSTANTE (Pendiente),
    //     marcado "pedido por el cliente". Limite: 2 turnos pedidos por mes.
    // CU14 - El CLIENTE consulta sus turnos y puede reprogramar y cancelar los
    //     pendientes, sin aprobacion. Todo queda en el historial del turno.
    // Decision acordada: sin contrasena ni codigo (el telefono identifica al cliente).
    // Todos los endpoints son publicos ([AllowAnonymous]).
    // Paso 12 - Pedir, reprogramar y cancelar generan un aviso (campanita) para el
    // dueno y la recepcionista, aunque el turno sea para dentro de mucho tiempo.
    // ==================================================================
    [AllowAnonymous]
    [ApiController]
    [Route("api/portal")]
    public class PortalController : ControllerBase
    {
        public const int LimitePorMes = 2;

        private readonly IClienteRepositorio clienteRepositorio;
        private readonly ITurnoRepositorio turnoRepositorio;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IBarberoRepositorio barberoRepositorio;
        private readonly IEstadoTurnoRepositorio estadoTurnoRepositorio;
        private readonly IHorarioLocalRepositorio horarioLocalRepositorio;
        private readonly IDiaNoLaborableRepositorio diaNoLaborableRepositorio;
        private readonly IValidadorAgenda validadorAgenda;
        private readonly IHistorialEstadoTurnoRepositorio historialRepositorio;
        private readonly IAvisoRepositorio avisoRepositorio;   // Paso 12

        public PortalController(IClienteRepositorio clienteRepositorio,
                                ITurnoRepositorio turnoRepositorio,
                                IServicioRepositorio servicioRepositorio,
                                IBarberoRepositorio barberoRepositorio,
                                IEstadoTurnoRepositorio estadoTurnoRepositorio,
                                IHorarioLocalRepositorio horarioLocalRepositorio,
                                IDiaNoLaborableRepositorio diaNoLaborableRepositorio,
                                IValidadorAgenda validadorAgenda,
                                IHistorialEstadoTurnoRepositorio historialRepositorio,
                                IAvisoRepositorio avisoRepositorio)
        {
            this.avisoRepositorio = avisoRepositorio;
            this.clienteRepositorio = clienteRepositorio;
            this.turnoRepositorio = turnoRepositorio;
            this.servicioRepositorio = servicioRepositorio;
            this.barberoRepositorio = barberoRepositorio;
            this.estadoTurnoRepositorio = estadoTurnoRepositorio;
            this.horarioLocalRepositorio = horarioLocalRepositorio;
            this.diaNoLaborableRepositorio = diaNoLaborableRepositorio;
            this.validadorAgenda = validadorAgenda;
            this.historialRepositorio = historialRepositorio;
        }

        // ------------------------------------------------------------------
        // Ingresar / registrarse con el telefono.
        // POST api/portal/ingresar   body: { "telefono": "1123456789", "nombre": "Pedro" }
        // Si el telefono existe devuelve ese cliente (con todos sus turnos).
        // Si no existe y viene el nombre, lo da de alta. Si no viene el nombre, avisa.
        // ------------------------------------------------------------------
        [HttpPost("ingresar")]
        public async Task<ActionResult<PortalClienteDTO>> Ingresar(PortalIngresoDTO DTO)
        {
            var telefono = Normalizar(DTO.Telefono);
            if (telefono.Length < 6)
                return BadRequest("Escribí tu teléfono completo (solo números).");

            var cliente = await BuscarCliente(DTO.Telefono);
            if (cliente != null)
                return Ok(new PortalClienteDTO { Id = cliente.Id, Nombre = cliente.Nombre, Telefono = cliente.Telefono });

            if (string.IsNullOrWhiteSpace(DTO.Nombre))
                return NotFound("Ese teléfono no está registrado. Escribí también tu nombre para registrarte.");

            var nuevo = new Cliente { Nombre = DTO.Nombre.Trim(), Telefono = telefono };
            await clienteRepositorio.Insert(nuevo);
            return Ok(new PortalClienteDTO { Id = nuevo.Id, Nombre = nuevo.Nombre, Telefono = nuevo.Telefono, EsNuevo = true });
        }

        // ------------------------------------------------------------------
        // Mis turnos: GET api/portal/turnos?telefono=1123456789
        // ------------------------------------------------------------------
        [HttpGet("turnos")]
        public async Task<ActionResult<List<TurnoListadoDTO>>> MisTurnos(string telefono)
        {
            var cliente = await BuscarCliente(telefono);
            if (cliente is null)
                return NotFound("Ese teléfono no está registrado.");
            return Ok(await turnoRepositorio.TurnosDelCliente(cliente.Id));
        }

        // ------------------------------------------------------------------
        // Datos para elegir: servicios, barberos activos, horario del local,
        // dias no laborables y horarios libres (lo mismo que usa la recepcionista).
        // ------------------------------------------------------------------
        [HttpGet("servicios")]
        public async Task<ActionResult<List<ServicioDTO>>> Servicios()
        {
            var lista = await servicioRepositorio.Select();
            return Ok(lista.OrderBy(s => s.Nombre)
                           .Select(s => new ServicioDTO { Id = s.Id, Nombre = s.Nombre, Duracion = s.Duracion, Precio = s.Precio })
                           .ToList());
        }

        [HttpGet("barberos")]
        public async Task<ActionResult<List<BarberoDTO>>> Barberos()
        {
            var lista = await barberoRepositorio.Select();
            return Ok(lista.Where(b => b.Activo).OrderBy(b => b.Nombre)
                           .Select(b => new BarberoDTO { Id = b.Id, Nombre = b.Nombre, Activo = true })
                           .ToList());
        }

        [HttpGet("horariolocal")]
        public async Task<ActionResult<List<HorarioLocalDTO>>> HorarioLocal()
        {
            var lista = await horarioLocalRepositorio.Select();
            return Ok(lista.Select(h => new HorarioLocalDTO
            {
                Id = h.Id,
                DiaSemana = (int)h.DiaSemana,
                DiaNombre = ReglasHorario.NombreDia(h.DiaSemana),
                HoraDesde = h.HoraDesde,
                HoraHasta = h.HoraHasta
            }).ToList());
        }

        [HttpGet("dianolaborable")]
        public async Task<ActionResult<List<DiaNoLaborableDTO>>> DiasNoLaborables()
        {
            var lista = await diaNoLaborableRepositorio.SelectDesde(DateOnly.FromDateTime(DateTime.Today));
            return Ok(lista.Select(d => new DiaNoLaborableDTO { Id = d.Id, Fecha = d.Fecha, Motivo = d.Motivo }).ToList());
        }

        [HttpGet("disponibles")]
        public async Task<ActionResult<List<HorarioDisponibleDTO>>> Disponibles(DateOnly fecha, int servicioId, int? barberoId, int? excluirTurnoId)
        {
            var servicio = await servicioRepositorio.SelectById(servicioId);
            if (servicio is null)
                return BadRequest($"No existe el servicio con id: {servicioId}.");
            if (fecha < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("La fecha ya pasó.");

            return Ok(await validadorAgenda.Disponibles(fecha, servicio, barberoId, excluirTurnoId ?? 0));
        }

        // ------------------------------------------------------------------
        // Pedir turno: POST api/portal/turno
        // Si el horario sigue libre (mismas reglas que la recepcionista) el turno
        // queda creado al instante como Pendiente y marcado "pedido por el cliente".
        // ------------------------------------------------------------------
        [HttpPost("turno")]
        public async Task<ActionResult<int>> PedirTurno(PortalPedirTurnoDTO DTO)
        {
            var cliente = await BuscarCliente(DTO.Telefono);
            if (cliente is null)
                return NotFound("Ese teléfono no está registrado.");

            var pedidos = await turnoRepositorio.PedidosDelClienteEnMes(cliente.Id, DTO.FechaHora.Year, DTO.FechaHora.Month);
            if (pedidos >= LimitePorMes)
                return BadRequest($"Ya pediste {LimitePorMes} turnos para {NombreMes(DTO.FechaHora.Month)}. " +
                                  "Si necesitás otro, llamá a la barbería.");

            var servicio = await servicioRepositorio.SelectById(DTO.ServicioId);
            if (servicio is null)
                return BadRequest("El servicio elegido no existe.");
            var barbero = await barberoRepositorio.SelectById(DTO.BarberoId);
            if (barbero is null)
                return BadRequest("El barbero elegido no existe.");

            var error = await validadorAgenda.ValidarTurno(DTO.FechaHora, servicio, barbero, 0);
            if (error != null)
                return BadRequest(error + " Elegí otro horario.");

            var pendiente = await estadoTurnoRepositorio.SelectByCodigo(ReglasEstado.Pendiente);
            if (pendiente is null)
                return BadRequest("No está cargado el estado Pendiente.");

            // Paso 12 - "Nuevo cliente" si es su primer turno.
            var esPrimerTurno = (await turnoRepositorio.TurnosDelCliente(cliente.Id)).Count == 0;

            var turno = new Turno
            {
                ClienteId = cliente.Id,
                BarberoId = barbero.Id,
                ServicioId = servicio.Id,
                EstadoTurnoId = pendiente.Id,
                FechaHora = DTO.FechaHora,
                FechaHoraFinEstimada = DTO.FechaHora.AddMinutes(servicio.Duracion),
                FechaAlta = DateTime.Now,
                Observaciones = string.IsNullOrWhiteSpace(DTO.Observaciones) ? null : DTO.Observaciones.Trim(),
                Precio = servicio.Precio,
                PedidoPorCliente = true
            };
            await turnoRepositorio.Insert(turno);

            await avisoRepositorio.Crear("Turno",
                $"{(esPrimerTurno ? "Nuevo cliente: " : "")}{cliente.Nombre}. Solicitó turno para el {turno.FechaHora:dd/MM} a las {turno.FechaHora:HH:mm} hs " +
                $"({servicio.Nombre} con {barbero.Nombre}). Teléfono: {cliente.Telefono}.",
                DateOnly.FromDateTime(turno.FechaHora), null);   // Paso 12
            return Ok(turno.Id);
        }

        // ------------------------------------------------------------------
        // Reprogramar un turno propio: PUT api/portal/turno/{id}/reprogramar
        // Solo turnos Pendientes y a futuro. Mismo servicio; cambia dia, hora o barbero.
        // Queda en el historial: "Reprogramado por el cliente: del ... al ...".
        // ------------------------------------------------------------------
        [HttpPut("turno/{id:int}/reprogramar")]
        public async Task<ActionResult<bool>> Reprogramar(int id, PortalReprogramarDTO DTO)
        {
            var (turno, error) = await TurnoPropioPendiente(id, DTO.Telefono);
            if (error != null) return BadRequest(error);

            var servicio = await servicioRepositorio.SelectById(turno!.ServicioId);
            var barbero = await barberoRepositorio.SelectById(DTO.BarberoId);
            if (servicio is null || barbero is null)
                return BadRequest("El servicio o el barbero no existen.");

            var errorAgenda = await validadorAgenda.ValidarTurno(DTO.FechaHora, servicio, barbero, turno.Id);
            if (errorAgenda != null)
                return BadRequest(errorAgenda + " Elegí otro horario.");

            var barberoAnterior = await barberoRepositorio.SelectById(turno.BarberoId);
            var fechaAnterior = turno.FechaHora;   // Paso 12 - para el aviso
            var motivo = $"Reprogramado por el cliente: del {turno.FechaHora:dd/MM HH:mm} ({barberoAnterior?.Nombre})" +
                         $" al {DTO.FechaHora:dd/MM HH:mm} ({barbero.Nombre}).";

            if (turno.FechaHora != DTO.FechaHora)
            {
                turno.Confirmado = false;
                turno.FechaConfirmacion = null;
            }
            turno.BarberoId = barbero.Id;
            turno.FechaHora = DTO.FechaHora;
            turno.FechaHoraFinEstimada = DTO.FechaHora.AddMinutes(servicio.Duracion);
            await turnoRepositorio.Update(turno);

            await historialRepositorio.Insert(new HistorialEstadoTurno
            {
                TurnoId = turno.Id,
                EstadoAnteriorId = turno.EstadoTurnoId,
                EstadoNuevoId = turno.EstadoTurnoId,
                FechaHora = DateTime.Now,
                Motivo = motivo
            });

            var clienteR = await BuscarCliente(DTO.Telefono);
            await avisoRepositorio.Crear("Turno",
                $"{clienteR?.Nombre} cambió su turno del {fechaAnterior:dd/MM HH:mm} al {DTO.FechaHora:dd/MM} a las {DTO.FechaHora:HH:mm} hs " +
                $"(con {barbero.Nombre}). Teléfono: {clienteR?.Telefono}.",
                DateOnly.FromDateTime(DTO.FechaHora), null);   // Paso 12
            return Ok(true);
        }

        // ------------------------------------------------------------------
        // Cancelar un turno propio: PUT api/portal/turno/{id}/cancelar
        // Sin aprobacion. Solo turnos Pendientes y a futuro. Queda en el historial.
        // ------------------------------------------------------------------
        [HttpPut("turno/{id:int}/cancelar")]
        public async Task<ActionResult<bool>> Cancelar(int id, PortalCancelarDTO DTO)
        {
            var (turno, error) = await TurnoPropioPendiente(id, DTO.Telefono);
            if (error != null) return BadRequest(error);

            var cancelado = await estadoTurnoRepositorio.SelectByCodigo(ReglasEstado.Cancelado);
            if (cancelado is null)
                return BadRequest("No está cargado el estado Cancelado.");

            var estadoAnterior = turno!.EstadoTurnoId;
            turno.EstadoTurnoId = cancelado.Id;
            await turnoRepositorio.Update(turno);

            await historialRepositorio.Insert(new HistorialEstadoTurno
            {
                TurnoId = turno.Id,
                EstadoAnteriorId = estadoAnterior,
                EstadoNuevoId = cancelado.Id,
                FechaHora = DateTime.Now,
                Motivo = "Cancelado por el cliente desde la web."
            });

            var clienteC = await BuscarCliente(DTO.Telefono);
            await avisoRepositorio.Crear("Turno",
                $"{clienteC?.Nombre} canceló su turno del {turno.FechaHora:dd/MM} a las {turno.FechaHora:HH:mm} hs. Teléfono: {clienteC?.Telefono}.",
                DateOnly.FromDateTime(turno.FechaHora), null);   // Paso 12
            return Ok(true);
        }

        // ================= Metodos de apoyo =================

        // El turno tiene que ser del cliente de ese telefono, estar Pendiente y ser a futuro.
        private async Task<(Turno? turno, string? error)> TurnoPropioPendiente(int id, string telefono)
        {
            var cliente = await BuscarCliente(telefono);
            if (cliente is null)
                return (null, "Ese teléfono no está registrado.");

            var turno = await turnoRepositorio.SelectById(id);
            if (turno is null || turno.ClienteId != cliente.Id)
                return (null, "No encontramos ese turno entre los tuyos.");

            var pendiente = await estadoTurnoRepositorio.SelectByCodigo(ReglasEstado.Pendiente);
            if (pendiente is null || turno.EstadoTurnoId != pendiente.Id)
                return (null, "Solo se pueden cambiar turnos pendientes.");

            if (turno.FechaHora <= DateTime.Now)
                return (null, "Ese turno ya pasó.");

            return (turno, null);
        }

        // Busca primero el telefono tal cual lo escribio y despues solo con numeros.
        private async Task<Cliente?> BuscarCliente(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return null;
            return await clienteRepositorio.SelectByTelefono(telefono.Trim())
                ?? await clienteRepositorio.SelectByTelefono(Normalizar(telefono));
        }

        private static string Normalizar(string telefono) =>
            new string((telefono ?? "").Where(char.IsDigit).ToArray());

        private static string NombreMes(int m)
        {
            string[] meses = { "", "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio",
                               "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
            return meses[m];
        }
    }
}
