using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Telefono { get; set; }
    }
}
