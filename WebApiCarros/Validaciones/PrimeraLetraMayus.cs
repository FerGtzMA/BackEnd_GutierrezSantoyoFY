//ESTA CLASE ES UNA FORMA DE VALIDAR ALGO QUE SE REQUIERA VALIDAR EN LOS ATRIBUTOS.

using System.ComponentModel.DataAnnotations;

namespace WebApiCarros.Validaciones
{
    //Hereda de ValidactionAtributes
    public class PrimeraLetraMayus: ValidationAttribute
    {
        //El metodo IsValed es requerido para recibir un objeto (atributo(nombre))
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //se verifica que value (atributo nombre) si es nulo éxito
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            //es una variable que combierte el objeto value (atributo Nombre) a String, la primera letra [0]
            var primeraLetra = value.ToString()[0].ToString();

            //Si la primera letra no es mayuscula entonces se retorna un mensaje
            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");

            }
            return ValidationResult.Success;
        }
    }
}
