using Microsoft.EntityFrameworkCore;
using WebApiCarros.Entidades;

namespace WebApiCarros
{
    public class AplicationDbContext: DbContext
    {
        public AplicationDbContext(DbContextOptions options): base(options)
        {

        }

        public DbSet<Carro> Carros { get; set; }
        public DbSet<Clase> Clases { get; set; }
    }
}
