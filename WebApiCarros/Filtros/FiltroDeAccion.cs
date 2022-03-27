using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiCarros.Filtros
{
    public class FiltroDeAccion: IActionFilter
    {
        private readonly ILogger<FiltroDeAccion> log;

        public FiltroDeAccion(ILogger<FiltroDeAccion> log)
        {
            this.log = log;
        }

        //se ejecuta antes de que se procese la acción
        public void OnActionExecuting(ActionExecutingContext context)
        {
            log.LogInformation("Antes de ejecutar la accion");
        }
        //se ejecuta después de que se finalice la acción
        public void OnActionExecuted(ActionExecutedContext context)
        {
            log.LogInformation("Despues de ejecutar la accion");
        }
    }
}
