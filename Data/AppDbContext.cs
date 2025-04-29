using EcoSens_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoSens_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empleados> Empleados { get; set; }
        public DbSet<TipoUsuario> TipoUsuario { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<AreaDTO> AreaDTO { get; set; }
        public DbSet<Conjuntos> Conjuntos { get; set; }
        public DbSet<Contenedores> Contenedores { get; set; }
        public DbSet<TipoContenedor> TipoContenedor { get; set; }
        public DbSet<Notificaciones> Notificaciones { get; set; }

    }
}
