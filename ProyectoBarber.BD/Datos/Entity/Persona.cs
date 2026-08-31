using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace ProyectoBarber.BD.Datos.Entity
{
    public class Persona     
    {
        [Key]
        public string DNI { get; set; }
        public string apellido { get; set; }

        public string nombre { get; set; }



    }
}
