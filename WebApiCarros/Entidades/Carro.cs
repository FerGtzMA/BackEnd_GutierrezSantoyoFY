using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiCarros.Validaciones;

namespace WebApiCarros.Entidades
{
    public class Carro: IValidatableObject
    {
        public int Id { get; set; }

        //Required haca que no puedas mandar un Post sin el atributo (nombre)
        [Required(ErrorMessage = "El campo {0} es requerido para el Carro.")]  //haciendo requerido mi atributo Nombre
        [StringLength(maximumLength:20, ErrorMessage = "El campo {0} tiene un valor de hasta 10 caracterer.")]
        //se comenta la linea de abajo porque se quiere comprobar si se funciona el IEnumerable, el metodo de abajo
        //[PrimeraLetraMayus]
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





        //---------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Para que se ejecuten debe de primero cumplirse con las reglas por Atributo Ejemplo: Range
            // Tomar a consideración que primero se ejecutaran las validaciones mappeadas en los atributos
            // y posteriormente las declaradas en la entidad
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    //La función que contiene cada return yield, debe devolver un objeto que
                    //implemente la interfaz IEnumerable<object>
                    yield return new ValidationResult("La primera letra debe ser MAYUSCULA",
                        new String[] { nameof(Nombre) });
                }

                //nameof signifique que me va a a marcar el error en esa validación y atributo
                // new String[] { nameof(Menor) });
            }
        }
    }
}
