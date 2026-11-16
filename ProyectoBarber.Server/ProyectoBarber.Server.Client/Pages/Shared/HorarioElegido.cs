namespace ProyectoBarber.Server.Client.Pages.Shared
{
    // Paso 8 - Lo que devuelve el SelectorHorario cuando se elige un horario libre:
    // la hora de inicio, el fin estimado y el barbero elegido (por defecto el sugerido).
    public class HorarioElegido
    {
        public TimeOnly Hora { get; set; }
        public TimeOnly HoraFinEstimada { get; set; }
        public int BarberoId { get; set; }
        public string BarberoNombre { get; set; } = string.Empty;
        public bool EsSugerido { get; set; }
    }
}
