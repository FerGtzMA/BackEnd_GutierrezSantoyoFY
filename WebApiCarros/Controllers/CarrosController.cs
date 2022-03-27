using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCarros.Entidades;
using WebApiCarros.Filtros;
using WebApiCarros.Services;

namespace WebApiCarros.Controllers
{
    [ApiController]
    [Route("api/carros")]//ruta del controlador  //[Route("api/[controller]")]

    //---------------------             INYECCIÓN DE DEPENNECIAS            ------------------------------          INICIO
    public class CarrosController: ControllerBase
    {
        private readonly AplicationDbContext dbContext;
        private readonly IService service;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;
        private readonly ILogger<CarrosController> logger;
        private readonly EscribirEnArchivo hacerArchivos;

        public CarrosController(AplicationDbContext context, /*Se puede agregar ServiceA o Services B
                                                              Esto es para poder usar Ambos servicios
                                                               por eso se agrega toda la interfaz completa */IService service,
            ServiceTransient serviceTransient, ServiceScoped serviceScoped,
            ServiceSingleton serviceSingleton, ILogger<CarrosController> logger, EscribirEnArchivo hacerArchivos)
        {
            this.dbContext = context;
            this.service = service;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
            this.logger = logger;
            this.hacerArchivos = hacerArchivos;
        }
        //---------------------             INYECCIÓN DE DEPENNECIAS            ------------------------------          FIN

        //______________________________________________________________________________________________________________________    
        //private readonly AplicationDbContext dbContext;
        //public CarrosController(AplicationDbContext context)
        //{
        //this.dbContext = context;
        //}
        //______________________________________________________________________________________________________________________

        
        [HttpGet("GUID")]
        [ResponseCache(Duration = 10)]      //se define el tiempo en segundos y es lo que va a durar el metodo
                                            //es reutilizable
        //[Authorize]                       //sirve para que solo ese método tenga la autorización ante los demás
        //La siguiente linea usa la clase FiltroDeAccion que esta en la carpeta Filtros para una prueba
        [ServiceFilter(typeof(FiltroDeAccion))]
        //Este metodo no se hace con async, task, etc. porque no se trabaja con BD
        //Solo retorna un Ok()
        public ActionResult ObtenerGuid()
        {
            return Ok(new
            {
                //Los Transient Siempre va a cambiar al mostrarlo en el Swagger, independiente de las veces ejecutables
                CarrosControllerTransient = serviceTransient.guid,
                ServiceA_Transient = service.GetTransient(),
                //Los Scoped van a cambiar pero son los mismo entre ellos
                CarrosControllerScoped = serviceScoped.guid,
                ServiceA_Scoped = service.GetScoped(),
                //El Singleton Nunca va a cambiar al mostrarlo en el Swagger, independiente de las veces ejecutables
                CarrosControllerSingleton = serviceSingleton.guid,
                ServiceA_Singleton = service.GetSingleton()
            });
        }


        [HttpGet]   //api/carros
        [HttpGet("listado")]    //api/carros/listado

        //si le ponemos un "/" estamos sobreecribiendo la ruta del controlador      |EXAMEN
        //[HttpGet("/listado")]   //  /listado
        //[ResponseCache(Duration = 10)]
        //[Authorize]
        [ServiceFilter(typeof(FiltroDeAccion))]
        public async Task<ActionResult<List<Carro>>> Get() // o GetAll()
        {
            //*
            //Los Logs es para guardar información de nuestra aplicación si pueden llegar a fallar, 
            //como diferentes tipos de errores.
            //Niveles de logs: 
            // Critical
            // Error
            // Warning
            // Information
            // Debug: te muestra paso a paso que va haciendo sentencias de sql especificas
            // Trace
            // *//

            //Mandamos una excepcion al momento de mandar el listado de Carros.
            //Al momento de ejecutar se pinta o aparece el error y todo, para que se vea cuál es el error.
            //Y solo te llega a ti el error, el logger por eso sirven, para información o errores que esolo 
            //el desarrollador puede ver
            throw new NotImplementedException();
            logger.LogInformation("Se obtiene el listado de carros");
            logger.LogWarning("Mensaje de prueba warning");
            service.EjecutarJob();
            return await dbContext.Carros.Include(x => x.clases).ToListAsync();
        }

        //no puede haber 2 EndPoins Get iguales
        //metodo que trae el primero de la base de datos
        //lo que está en el parentesis del get es una regla de concatenación a nuestra ruta principal
        [HttpGet("primero")]        //api/carros/primero?
        public async Task<ActionResult<Carro>> primerCarro([FromHeader] int valor, [FromQuery] string carro, [FromQuery] int carroId)
        {
            return await dbContext.Carros.FirstOrDefaultAsync();
        }

        [HttpGet("segundo")]    //api/carros/segundo
        public ActionResult<Carro> segundoCarro()
        {
            return new Carro() { Nombre = "FER" };
        }

        //estamos definiendo una variable, puede serd e cualquier tipo
        //podemos definir varias variables
        //el signo de interrogación (?) es para que el parametro no sea obligatorio | [HttpGet("{id:int}/{param?}")]
        //el signo de igualdad es que le estamos asignando un valor por defecto     | [HttpGet("{id:int}/{param=Versa}")]
        [HttpGet("{id:int}/{param=Versa}")]
        public ActionResult<Carro> Get(int id, string param)
        {
            var carro =  dbContext.Carros.FirstOrDefault(x => x.Id == id);

            if(carro == null)
            {
                return NotFound();
            }

            return carro;

        }

        //recibe un string y el anterior un entero
        //Task es una promesa, como una llamada a base de datos
        //Task solo es como un void, no retornamos nada
        //Task<object> retornamos un objeto tipo object, o algún tipo de datos
        [HttpGet("{nombre}")]
        public async Task<ActionResult<Carro>> Get([FromRoute] string nombre)
        //[FromRoute] significa que el nombre me lo van a enviar desde la ruta
        {
            //el await hace hasta que finalice el proceso
            var carro = await dbContext.Carros.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (carro == null)
            {
                logger.LogError("No se encontro el Carro");
                return NotFound();
            }

            hacerArchivos.sobrescribir(nombre, "registroConsultado.txt");

            return carro;

        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Carro carro)
        {
            //Ejemplo para validar desde el controlador con la BD con ayuda del dbContext
            var existeCarroMismoNombre = await dbContext.Carros.AnyAsync(x => x.Nombre == carro.Nombre);

            if (existeCarroMismoNombre)
            {
                return BadRequest("Ya existe un auto con el mismo nombre");
            }
            dbContext.Add(carro);
            await dbContext.SaveChangesAsync();
            return Ok();
        }


        [HttpPut("{id:int}")] //        api/carros/1
        public async Task<ActionResult> Put(Carro carro, int id)
        {
            var exist = await dbContext.Carros.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            if (carro.Id != id)
            {
                return BadRequest("El id del carro no coincide con el establecido en la url.");
            }
            hacerArchivos.sobrescribir(carro.ToString(), "nuevosRegistros.txt");
            dbContext.Update(carro);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Carros.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            dbContext.Remove(new Carro()
            {
                Id = id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}