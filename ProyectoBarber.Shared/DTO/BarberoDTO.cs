using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    public class BarberoDTO
    {
        public int Id { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
