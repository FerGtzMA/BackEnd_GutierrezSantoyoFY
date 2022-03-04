using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCarros.Entidades;

namespace WebApiCarros.Controllers
{
    [ApiController]
    [Route("api/clases")] //ruta del controlador
    public class ClasesController : ControllerBase
    {
        private readonly AplicationDbContext dbContext;
        public ClasesController(AplicationDbContext context)
        {
            this.dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Clase>>> GetAll()
        {
            return await dbContext.Clases.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Clase>> GetById(int id)
        {
            return await dbContext.Clases.FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Clase>> Post(Clase clase)
        {
            var existeCarro = await dbContext.Carros.AnyAsync(x => x.Id == clase.Id);

            if (!existeCarro)
            {
                return BadRequest($"No existe el carro con el número de serie: { clase.Id}");
            }
            dbContext.Add(clase);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Clase clase, int id)
        {
            var exist = await dbContext.Clases.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("La clase especificada no extiste.");
            }

            if (clase.Id != id)
            {
                return BadRequest("El id de la clase no coincide con el establecido en la url.");
            }

            dbContext.Update(clase);
            await dbContext.SaveChangesAsync();
            return Ok();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Clases.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El recurso no fue encontrado.");
            }
            dbContext.Remove(new Clase { Id = id });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
