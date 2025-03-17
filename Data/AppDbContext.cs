using EcoSens_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoSens_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoEmpleado> TipoEmpleado { get; set; }
        public DbSet<Area> AreaT { get; set; }
        public DbSet<Conjuntos> Conjuntos { get; set; }
        public DbSet<Contenedores> Contenedores { get; set; }
        public DbSet<TipoContenedor> TipoContenedors { get; set; }
    }
}
