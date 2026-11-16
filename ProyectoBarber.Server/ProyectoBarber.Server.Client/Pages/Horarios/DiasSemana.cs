namespace ProyectoBarber.Server.Client.Pages.Horarios
{
    // Paso 7 - Dias de la semana para los combos de horarios.
    // El numero es el mismo que usa el servidor (DayOfWeek): 0 = Domingo ... 6 = Sabado.
    // Se muestran de Lunes a Domingo.
    public static class DiasSemana
    {
        public record Dia(int Numero, string Nombre);

        public static readonly List<Dia> Lista = new()
        {
            new(1, "Lunes"),
            new(2, "Martes"),
            new(3, "Miércoles"),
            new(4, "Jueves"),
            new(5, "Viernes"),
            new(6, "Sábado"),
            new(0, "Domingo"),
        };

        // Para ordenar las franjas de Lunes (1) a Domingo (7).
        public static int Orden(int diaSemana) => diaSemana == 0 ? 7 : diaSemana;
    }
}
