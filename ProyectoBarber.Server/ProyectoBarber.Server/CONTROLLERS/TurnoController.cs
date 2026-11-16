using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoBarber.Server.Controllers
{
    // Paso 5: el proyecto ProyectoBarber.Servicio (HttpService) crea el espacio de nombres
    // 'ProyectoBarber.Servicio', que choca con la entidad Servicio. Este alias aclara que
    // 'Servicio' es la entidad de la BD.
    using Servicio = ProyectoBarber.BD.Datos.Entity.Servicio;

    // ==================================================================
    // TURNO CONTROLLER  -  ruta base: api/turno
    // Entidad central del sistema. Cubre los casos de uso:
    //   CU02 Registrar turno         -> GET disponibles + POST
    //   CU03 Consultar agenda        -> GET listaturnos (filtros) + GET aconfirmar
    //   CU04 Reprogramar turno       -> GET {id} + PUT {id}
    //   Turno con sus relaciones     -> GET SelectByIdCompleto/{id} (paso 13, DTO completo)
    //   CU05 Cancelar turno          -> PUT {id}/estado (CAN)
    //   CU06 Atencion / ausente      -> PUT {id}/estado (ENA, ATE, AUS)
    //   Aviso 2 horas antes          -> GET aconfirmar + PUT {id}/confirmar
    //   Historial de estados         -> GET {id}/historial + PUT {id}/historial/{historialId} (paso 8)
    //   CU10 Reporte                 -> GET reporte?desde=...&hasta=... (paso 9)
    // Actor: Recepcionista (el Dueno tambien puede usarlo).
    // Las reglas de agenda (paso 3) estan en ValidadorAgenda.
    // ==================================================================
    // Paso 10 - Dueno y recepcionista; el reporte solo el dueno. Cada cambio de estado
    // y reprogramacion guarda en el historial el usuario que lo hizo.
    [Authorize]
    [ApiController]
    [Route("api/turno")]
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoRepositorio repositorio;
        private readonly IClienteRepositorio clienteRepositorio;
        private readonly IBarberoRepositorio barberoRepositorio;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IEstadoTurnoRepositorio estadoTurnoRepositorio;
        private readonly IValidadorAgenda validadorAgenda;
        private readonly IHistorialEstadoTurnoRepositorio historialRepositorio;

        public TurnoController(ITurnoRepositorio repositorio,
                               IClienteRepositorio clienteRepositorio,
                               IBarberoRepositorio barberoRepositorio,
                               IServicioRepositorio servicioRepositorio,
                               IEstadoTurnoRepositorio estadoTurnoRepositorio,
                               IValidadorAgenda validadorAgenda,
                               IHistorialEstadoTurnoRepositorio historialRepositorio)
        {
            this.repositorio = repositorio;
            this.clienteRepositorio = clienteRepositorio;
            this.barberoRepositorio = barberoRepositorio;
            this.servicioRepositorio = servicioRepositorio;
            this.estadoTurnoRepositorio = estadoTurnoRepositorio;
            this.validadorAgenda = validadorAgenda;
            this.historialRepositorio = historialRepositorio;
        }

        // ------------------------------------------------------------------
        // GET api/turno  -  Listado crudo (solo Id de cada relacion).
        // Uso tecnico / pruebas en Swagger. Para pantallas usar listaturnos (CU03).
        // Paso 1: si no hay registros devuelve una lista vacia.
        // ------------------------------------------------------------------
        [HttpGet] //api turno
        public async Task<ActionResult<List<Turno>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");

            return Ok(lista);
        }

        // ------------------------------------------------------------------
        // CU03 - Consultar agenda del dia
        // GET api/turno/listaturnos?fecha=2026-10-08&barberoId=1&estado=PEN
        // Devuelve los turnos con NOMBRES de cliente, barbero, servicio y estado
        // (TurnoListadoDTO), igual que 'listapais' de la catedra.
        // Paso 1: si no hay turnos devuelve una lista vacia (antes NotFound).
        // Paso 4: los tres filtros son opcionales; sin filtros devuelve todos.
        // ------------------------------------------------------------------
        [HttpGet("listaturnos")] //api turno    listaturnos
        public async Task<ActionResult<List<TurnoListadoDTO>>> ListaTurnos(DateOnly? fecha, int? barberoId, string? estado)
        {
            var lista = await repositorio.ListaTurnos(fecha, barberoId, estado?.ToUpper());
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");

            return Ok(lista);
        }

        // ------------------------------------------------------------------
        // CU02 - Registrar turno (pasos 4 a 7: horarios libres y barbero sugerido)
        // GET api/turno/disponibles?fecha=2026-10-08&servicioId=1&barberoId=1
        // Paso 8: excluirTurnoId (opcional) = turno que se esta reprogramando; su lugar cuenta como libre.
        // Devuelve los horarios libres de esa fecha para ese servicio, en saltos de
        // 5 minutos. barberoId es opcional (cliente con barbero preferido).
        // En cada horario: los barberos libres y el sugerido (el de menos turnos ese dia).
        // ------------------------------------------------------------------
        [HttpGet("disponibles")] //api/turno/disponibles
        public async Task<ActionResult<List<HorarioDisponibleDTO>>> Disponibles(DateOnly fecha, int servicioId, int? barberoId, int? excluirTurnoId)
        {
            var servicio = await servicioRepositorio.SelectById(servicioId);
            if (servicio is null)
                return BadRequest($"No existe el servicio con id: {servicioId}.");

            if (fecha < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("La fecha ya pasó.");

            if (barberoId != null && await barberoRepositorio.SelectById(barberoId.Value) is null)
                return BadRequest($"No existe el barbero con id: {barberoId}.");

            var lista = await validadorAgenda.Disponibles(fecha, servicio, barberoId, excluirTurnoId ?? 0);
            return Ok(lista);
        }

        // ------------------------------------------------------------------
        // CU04 - Reprogramar turno (paso 2: traer el turno a editar)
        // GET api/turno/{id}
        // Devuelve un turno por Id como TurnoDTO para cargar el formulario.
        // ------------------------------------------------------------------
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
            DTO.FechaHoraFinEstimada = entidad.FechaHoraFinEstimada;
            DTO.Observaciones = entidad.Observaciones;

            return Ok(DTO);
        }

        // ------------------------------------------------------------------
        // Paso 13 - Turno COMPLETO (como SelectByIdCompleto de la catedra)
        // GET api/turno/SelectByIdCompleto/{id}
        // Devuelve el turno con Cliente, Barbero, Servicio y Estado como DTO anidados.
        // ------------------------------------------------------------------
        [HttpGet("SelectByIdCompleto/{id:int}")] //api/turno/SelectByIdCompleto/5
        public async Task<ActionResult<TurnoCompletoDTO>> GetByIdCompleto(int id)
        {
            var DTO = await repositorio.SelectByIdCompleto(id);
            if (DTO is null)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(DTO);
        }

        // ------------------------------------------------------------------
        // CU02 - Registrar turno
        // POST api/turno  -  devuelve el Id creado.
        // Paso 3: valida cliente, servicio y barbero, y las reglas de agenda
        // (saltos de 5 min, a futuro, feriado, horario del local y del barbero,
        // sin superposicion). El turno se crea SIEMPRE en estado Pendiente.
        // ------------------------------------------------------------------
        [HttpPost] //api turno
        public async Task<ActionResult<int>> Post(TurnoDTO DTO)
        {
            var (servicio, barbero, error) = await CargarYValidar(DTO, 0);
            if (error != null)
                return BadRequest(error);

            var pendiente = await estadoTurnoRepositorio.SelectByCodigo("PEN");
            if (pendiente is null)
                return BadRequest("No está cargado el estado Pendiente.");

            Turno entidad = new Turno();
            entidad.ClienteId = DTO.ClienteId;
            entidad.BarberoId = barbero!.Id;
            entidad.ServicioId = servicio!.Id;
            entidad.EstadoTurnoId = pendiente.Id;
            entidad.FechaHora = DTO.FechaHora;
            entidad.FechaHoraFinEstimada = DTO.FechaHora.AddMinutes(servicio.Duracion);
            entidad.FechaAlta = DateTime.Now;
            entidad.Observaciones = DTO.Observaciones;
            entidad.Precio = servicio.Precio;   // Paso 9 - precio al momento de dar el turno

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        // ------------------------------------------------------------------
        // CU04 - Reprogramar turno
        // PUT api/turno/{id}
        // Modifica fecha/hora, barbero, servicio, cliente u observaciones.
        // El id de la ruta debe coincidir con el del DTO.
        // Paso 3: solo turnos Pendientes (extension 1a) y mismas reglas que el alta,
        // sin contar el propio turno como superposicion. El estado no se cambia aca.
        // ------------------------------------------------------------------
        [HttpPut("{id:int}")] //api/turno5
        public async Task<ActionResult<bool>> Put(int id, TurnoDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            var pendiente = await estadoTurnoRepositorio.SelectByCodigo("PEN");
            if (pendiente is null || entidad.EstadoTurnoId != pendiente.Id)
                return BadRequest("Solo se pueden reprogramar turnos pendientes.");

            var (servicio, barbero, error) = await CargarYValidar(DTO, id);
            if (error != null)
                return BadRequest(error);

            // Paso 8: si cambia fecha/hora, barbero o servicio queda en el historial
            // como "Reprogramado" (Pendiente -> Pendiente).
            string? reprogramacion = null;
            if (entidad.FechaHora != DTO.FechaHora || entidad.BarberoId != barbero!.Id || entidad.ServicioId != servicio!.Id)
            {
                var barberoAnterior = await barberoRepositorio.SelectById(entidad.BarberoId);
                var servicioAnterior = await servicioRepositorio.SelectById(entidad.ServicioId);
                reprogramacion = $"Reprogramado: del {entidad.FechaHora:dd/MM HH:mm} ({barberoAnterior?.Nombre}, {servicioAnterior?.Nombre})" +
                                 $" al {DTO.FechaHora:dd/MM HH:mm} ({barbero!.Nombre}, {servicio!.Nombre}).";
            }

            // Paso 4: si cambia la fecha u hora, la confirmacion del cliente ya no vale.
            if (entidad.FechaHora != DTO.FechaHora)
            {
                entidad.Confirmado = false;
                entidad.FechaConfirmacion = null;
            }

            // Paso 9 - si cambia el servicio, el turno toma el precio del servicio nuevo.
            if (entidad.ServicioId != servicio!.Id)
                entidad.Precio = servicio.Precio;

            entidad.ClienteId = DTO.ClienteId;
            entidad.BarberoId = barbero!.Id;
            entidad.ServicioId = servicio!.Id;
            entidad.FechaHora = DTO.FechaHora;
            entidad.FechaHoraFinEstimada = DTO.FechaHora.AddMinutes(servicio.Duracion);
            entidad.Observaciones = DTO.Observaciones;

            var resultado = await repositorio.Update(entidad);

            if (reprogramacion != null)
            {
                await historialRepositorio.Insert(new HistorialEstadoTurno
                {
                    TurnoId = entidad.Id,
                    EstadoAnteriorId = pendiente.Id,
                    EstadoNuevoId = pendiente.Id,
                    FechaHora = DateTime.Now,
                    Motivo = reprogramacion,
                    UsuarioId = UsuarioActualId()   // Paso 10
                });
            }

            return Ok(resultado);
        }

        // ------------------------------------------------------------------
        // CU05 y CU06 - Cambiar el estado de un turno
        // PUT api/turno/{id}/estado   body: { "codigo": "ENA", "motivo": null }
        //   ENA Iniciar atencion (se puede antes de hora)   ATE Finalizar atencion
        //   AUS Ausente (desde la hora del turno)           CAN Cancelar
        //   PEN Volver a Pendiente (correccion de un error, sin limite de tiempo)
        // Desde En atencion, cancelar o ausente exigen motivo.
        // Cada cambio queda en el historial. Patron de ActualizarEstado de UDX.
        // ------------------------------------------------------------------
        [HttpPut("{id:int}/estado")] //api/turno/5/estado
        public async Task<ActionResult<TurnoListadoDTO>> CambiarEstado(int id, CambioEstadoTurnoDTO DTO)
        {
            var turno = await repositorio.SelectById(id);
            if (turno is null)
                return NotFound($"No existe el registro con id: {id}.");

            var estados = await estadoTurnoRepositorio.Select();
            var actual = estados.First(e => e.Id == turno.EstadoTurnoId);
            var nuevo = estados.FirstOrDefault(e => e.Codigo == DTO.Codigo.Trim().ToUpper());
            if (nuevo is null)
                return BadRequest("Estado inválido. Valores permitidos: ENA, ATE, AUS, CAN, PEN.");

            var ahora = DateTime.Now;
            var error = ReglasEstado.Validar(actual.Codigo, nuevo.Codigo, turno, DTO.Motivo, ahora);
            if (error != null)
                return BadRequest(error);

            switch (nuevo.Codigo)
            {
                case ReglasEstado.EnAtencion:
                    turno.FechaHoraInicioReal = ahora;
                    break;
                case ReglasEstado.Atendido:
                    turno.FechaHoraFinReal = ahora;
                    break;
                case ReglasEstado.Pendiente:
                    // Correccion: si el turno es a futuro y otro turno ya ocupo ese horario, no se puede.
                    if (turno.FechaHora > ahora &&
                        await repositorio.HaySuperposicion(turno.BarberoId, turno.FechaHora, turno.FechaHoraFinEstimada, turno.Id))
                        return BadRequest("No se puede volver a Pendiente: ese horario ya lo ocupa otro turno del barbero. Reprogramalo o dá un turno nuevo.");
                    turno.FechaHoraInicioReal = null;
                    turno.FechaHoraFinReal = null;
                    break;
            }

            turno.EstadoTurnoId = nuevo.Id;
            await repositorio.Update(turno);

            await historialRepositorio.Insert(new HistorialEstadoTurno
            {
                TurnoId = turno.Id,
                EstadoAnteriorId = actual.Id,
                EstadoNuevoId = nuevo.Id,
                FechaHora = ahora,
                Motivo = string.IsNullOrWhiteSpace(DTO.Motivo) ? null : DTO.Motivo.Trim(),
                UsuarioId = UsuarioActualId()   // Paso 10
            });

            var listado = await repositorio.ListaTurnos(DateOnly.FromDateTime(turno.FechaHora), turno.BarberoId, null);
            return Ok(listado.First(t => t.Id == turno.Id));
        }

        // ------------------------------------------------------------------
        // Historial de estados de un turno
        // GET api/turno/{id}/historial
        // ------------------------------------------------------------------
        [HttpGet("{id:int}/historial")] //api/turno/5/historial
        public async Task<ActionResult<List<HistorialEstadoDTO>>> Historial(int id)
        {
            if (await repositorio.SelectById(id) is null)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(await historialRepositorio.SelectPorTurno(id));
        }

        // ------------------------------------------------------------------
        // Paso 8 - Corregir un movimiento del historial (opcion B acordada)
        // PUT api/turno/{id}/historial/{historialId}
        //   body: { "fechaHora": "2026-10-10T18:05:00", "motivo": "...", "codigoEstadoNuevo": "AUS" }
        // - Fecha y motivo: se pueden corregir en cualquier movimiento. La fecha no puede
        //   ser a futuro ni salirse del orden (entre el movimiento anterior y el siguiente).
        // - Estado: solo en el ULTIMO movimiento, y el turno pasa a ese estado.
        //   Se valida con las mismas reglas de ReglasEstado (desde el estado anterior).
        //   Las reprogramaciones (Pendiente -> Pendiente) no cambian de estado.
        // Devuelve el historial actualizado.
        // ------------------------------------------------------------------
        [HttpPut("{id:int}/historial/{historialId:int}")] //api/turno/5/historial/12
        public async Task<ActionResult<List<HistorialEstadoDTO>>> EditarHistorial(int id, int historialId, EditarHistorialDTO DTO)
        {
            var turno = await repositorio.SelectById(id);
            if (turno is null)
                return NotFound($"No existe el registro con id: {id}.");

            var movimientos = await historialRepositorio.EntidadesPorTurno(id);
            var posicion = movimientos.FindIndex(h => h.Id == historialId);
            if (posicion < 0)
                return NotFound($"El turno {id} no tiene el movimiento {historialId}.");

            var movimiento = movimientos[posicion];
            var esUltimo = posicion == movimientos.Count - 1;
            var ahora = DateTime.Now;

            // Fecha: no a futuro y respetando el orden de los movimientos.
            if (DTO.FechaHora > ahora)
                return BadRequest("La fecha del movimiento no puede ser a futuro.");
            if (posicion > 0 && DTO.FechaHora < movimientos[posicion - 1].FechaHora)
                return BadRequest($"La fecha no puede ser anterior al movimiento previo ({movimientos[posicion - 1].FechaHora:dd/MM/yyyy HH:mm}).");
            if (!esUltimo && DTO.FechaHora > movimientos[posicion + 1].FechaHora)
                return BadRequest($"La fecha no puede ser posterior al movimiento siguiente ({movimientos[posicion + 1].FechaHora:dd/MM/yyyy HH:mm}).");

            var motivo = string.IsNullOrWhiteSpace(DTO.Motivo) ? null : DTO.Motivo.Trim();

            var estados = await estadoTurnoRepositorio.Select();
            var anterior = estados.First(e => e.Id == movimiento.EstadoAnteriorId);
            var nuevo = estados.First(e => e.Id == movimiento.EstadoNuevoId);

            var codigoPedido = DTO.CodigoEstadoNuevo?.Trim().ToUpper();
            if (!string.IsNullOrEmpty(codigoPedido) && codigoPedido != nuevo.Codigo)
            {
                if (!esUltimo)
                    return BadRequest("Solo se puede cambiar el estado del último movimiento.");
                if (anterior.Id == nuevo.Id)
                    return BadRequest("Este movimiento es una reprogramación: solo se puede corregir la fecha y el motivo.");

                var pedido = estados.FirstOrDefault(e => e.Codigo == codigoPedido);
                if (pedido is null)
                    return BadRequest("Estado inválido. Valores permitidos: ENA, ATE, AUS, CAN, PEN.");

                var error = ReglasEstado.Validar(anterior.Codigo, pedido.Codigo, turno, motivo, ahora);
                if (error != null)
                    return BadRequest(error);

                if (pedido.Codigo == ReglasEstado.Pendiente && turno.FechaHora > ahora &&
                    await repositorio.HaySuperposicion(turno.BarberoId, turno.FechaHora, turno.FechaHoraFinEstimada, turno.Id))
                    return BadRequest("No se puede volver a Pendiente: ese horario ya lo ocupa otro turno del barbero.");

                nuevo = pedido;
            }
            else if (anterior.Codigo == ReglasEstado.EnAtencion &&
                     (nuevo.Codigo == ReglasEstado.Cancelado || nuevo.Codigo == ReglasEstado.Ausente) &&
                     motivo == null)
            {
                return BadRequest($"Un turno que pasó de En atención a {nuevo.Nombre} tiene que tener motivo.");
            }

            movimiento.FechaHora = DTO.FechaHora;
            movimiento.Motivo = motivo;
            movimiento.EstadoNuevoId = nuevo.Id;
            await historialRepositorio.Update(movimiento);

            // El ultimo movimiento manda: el turno queda en ese estado y con sus horas reales.
            if (esUltimo)
            {
                turno.EstadoTurnoId = nuevo.Id;
                AplicarHorasReales(turno, anterior.Codigo, nuevo.Codigo, movimiento.FechaHora);
                await repositorio.Update(turno);
            }

            return Ok(await historialRepositorio.SelectPorTurno(id));
        }

        // Paso 10 - Id del usuario logueado (sale del token). null si no se pudo leer.
        private int? UsuarioActualId()
        {
            var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(valor, out var id) ? id : null;
        }

        // Paso 8 - Ajusta inicio/fin real segun el estado al que paso el turno
        // en su ultimo movimiento (usado al corregir el historial).
        private static void AplicarHorasReales(Turno turno, string anterior, string nuevo, DateTime fecha)
        {
            switch (nuevo)
            {
                case ReglasEstado.EnAtencion:
                    turno.FechaHoraInicioReal = fecha;
                    turno.FechaHoraFinReal = null;
                    break;
                case ReglasEstado.Atendido:
                    turno.FechaHoraFinReal = fecha;
                    break;
                case ReglasEstado.Pendiente:
                    turno.FechaHoraInicioReal = null;
                    turno.FechaHoraFinReal = null;
                    break;
                default: // Ausente o Cancelado
                    if (anterior == ReglasEstado.Pendiente)
                        turno.FechaHoraInicioReal = null;
                    turno.FechaHoraFinReal = null;
                    break;
            }
        }

        // ------------------------------------------------------------------
        // CU10 - Reporte (paso 9)
        // GET api/turno/reporte?desde=2026-10-01&hasta=2026-10-31
        // Totales por estado, % de ausentismo, recaudacion estimada, semanas
        // (lunes a domingo), por barbero, por servicio, clientes que mas faltan
        // y el detalle de ventas y cortes. Actor: Dueno.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]   // Paso 10
        [HttpGet("reporte")] //api/turno/reporte
        public async Task<ActionResult<ReporteDTO>> Reporte(DateOnly desde, DateOnly hasta)
        {
            if (hasta < desde)
                return BadRequest("La fecha 'hasta' no puede ser anterior a 'desde'.");
            if (hasta.DayNumber - desde.DayNumber > 370)
                return BadRequest("El período no puede ser mayor a un año.");

            var turnos = await repositorio.TurnosDelPeriodo(desde, hasta);
            return Ok(ReglasReporte.Armar(turnos, desde, hasta));
        }

        // ------------------------------------------------------------------
        // Aviso 2 horas antes - turnos a confirmar
        // GET api/turno/aconfirmar
        // Turnos Pendientes, sin confirmar, que empiezan en las proximas 2 horas.
        // La agenda lo consulta cada tanto y los muestra arriba: la recepcionista
        // llama al cliente y marca Confirmado, o cancela el turno.
        // ------------------------------------------------------------------
        [HttpGet("aconfirmar")] //api/turno/aconfirmar
        public async Task<ActionResult<List<TurnoListadoDTO>>> AConfirmar()
        {
            return Ok(await repositorio.AConfirmar(DateTime.Now, ReglasHorario.HorasAviso));
        }

        // ------------------------------------------------------------------
        // Aviso 2 horas antes - el cliente confirmo que viene
        // PUT api/turno/{id}/confirmar
        // Solo turnos Pendientes.
        // ------------------------------------------------------------------
        [HttpPut("{id:int}/confirmar")] //api/turno/5/confirmar
        public async Task<ActionResult<bool>> Confirmar(int id)
        {
            var turno = await repositorio.SelectById(id);
            if (turno is null)
                return NotFound($"No existe el registro con id: {id}.");

            var pendiente = await estadoTurnoRepositorio.SelectByCodigo(ReglasEstado.Pendiente);
            if (pendiente is null || turno.EstadoTurnoId != pendiente.Id)
                return BadRequest("Solo se pueden confirmar turnos pendientes.");

            turno.Confirmado = true;
            turno.FechaConfirmacion = DateTime.Now;
            await repositorio.Update(turno);

            return Ok(true);
        }

        // ------------------------------------------------------------------
        // DELETE api/turno/{id}  -  Borra fisicamente el turno.
        // Solo para corregir cargas erroneas. Para CU05 (cancelar) se cambia
        // el estado a Cancelado, asi no se pierde el historial.
        // ------------------------------------------------------------------
        [HttpDelete("{id:int}")] //api/turno5
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var resultado = await repositorio.Delete(id);
            if (!resultado)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(true);
        }

        // ================== Metodos de apoyo ==================

        // Verifica que existan cliente, servicio y barbero, y aplica las reglas de agenda.
        private async Task<(Servicio? servicio, Barbero? barbero, string? error)> CargarYValidar(TurnoDTO DTO, int idTurnoExcluir)
        {
            if (await clienteRepositorio.SelectById(DTO.ClienteId) is null)
                return (null, null, $"No existe el cliente con id: {DTO.ClienteId}.");

            var servicio = await servicioRepositorio.SelectById(DTO.ServicioId);
            if (servicio is null)
                return (null, null, $"No existe el servicio con id: {DTO.ServicioId}.");

            var barbero = await barberoRepositorio.SelectById(DTO.BarberoId);
            if (barbero is null)
                return (null, null, $"No existe el barbero con id: {DTO.BarberoId}.");

            var error = await validadorAgenda.ValidarTurno(DTO.FechaHora, servicio, barbero, idTurnoExcluir);
            return (servicio, barbero, error);
        }
    }
}
