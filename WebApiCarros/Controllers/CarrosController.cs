using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCarros.Entidades;

namespace WebApiCarros.Controllers
{
    [ApiController]
    [Route("api/carros")]//ruta del controlador  //[Route("api/[controller]")]
    public class CarrosController: ControllerBase
    {
        private readonly AplicationDbContext dbContext;
        public CarrosController(AplicationDbContext context)
        {
            this.dbContext = context;
        }

        [HttpGet]   //api/carros
        [HttpGet("listado")]    //api/carros/listado

        //si le ponemos un "/" estamos sobreecribiendo la ruta del controlador      |EXAMEN
        [HttpGet("/listado")]   //  /listado
        public async Task<ActionResult<List<Carro>>> Get() // o GetAll()
        {
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
                return NotFound();
            }

            return carro;

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Carro carro)
        {
            dbContext.Add(carro);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] //        api/carros/1
        public async Task<ActionResult> Put(Carro carro, int id)
        {
            if (carro.Id != id)
            {
                return BadRequest("El id del carro no coincide con el establecido en la url.");
            }
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
