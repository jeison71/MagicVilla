using MagicVilla_API.Modelos;

using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }
        public DbSet<Villa> Villas { get; set; }
        public DbSet<NumeroVilla> NumeroVillas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa() { 
                    Id = 1,
                    Nombre= "Villa Real",
                    ImagenUrl= "",
                    Ocupantes = 5,
                    MetrosCuadrados = 50,
                    Tarifa = 200,
                    Amenidad = "",
                    FechaActualizacion = DateTime.Now,
                    FechaCreacion = DateTime.Now,
                    Detalle = "",
                },
                new Villa()
                {
                    Id = 2,
                    Nombre = "Villa Kiara",
                    ImagenUrl = "",
                    Ocupantes = 6,
                    MetrosCuadrados = 60,
                    Tarifa = 500,
                    Amenidad = "",
                    FechaActualizacion = DateTime.Now,
                    FechaCreacion = DateTime.Now,
                    Detalle = "",
                }
            );
        }
    }
}
