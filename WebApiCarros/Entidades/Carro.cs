using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiCarros.Entidades
{
    public class Carro
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido para el Carro.")]  //haciendo requerido mi atributo Nombre
        [StringLength(maximumLength:10, ErrorMessage = "El campo {0} tiene un valor de hasta 10 caracterer.")]
        public string Nombre { get; set; }
        [Range(1951,2022, ErrorMessage = "El año no se encuentra dentro del rango.")]
        [NotMapped]    //sirve para tener atributos que no estan mapeados en la base de datos, no me lo guarda en la base de datos
        public int Anio { get; set; }

        [CreditCard]
        [NotMapped]
        public string Tarjeta { get; set; }

        [Url]
        [NotMapped]
        public string Url { get; set; }


        public List<Clase> clases { get; set; }
    }
}
