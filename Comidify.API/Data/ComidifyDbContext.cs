using Microsoft.EntityFrameworkCore;
using Comidify.API.Models;

namespace Comidify.API.Data
{
    public class ComidifyDbContext : DbContext
    {
        public ComidifyDbContext(DbContextOptions<ComidifyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comida> Comidas { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<ComidaIngrediente> ComidaIngredientes { get; set; }
        public DbSet<MenuSemanal> MenusSemanales { get; set; }
        public DbSet<MenuComida> MenuComidas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Comida
            modelBuilder.Entity<Comida>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TipoComida).IsRequired();
                entity.HasIndex(e => e.Nombre);
            });

            // Configuración de Ingrediente
            modelBuilder.Entity<Ingrediente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Nombre);
            });

            // Configuración de ComidaIngrediente
            modelBuilder.Entity<ComidaIngrediente>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Comida)
                    .WithMany(c => c.ComidaIngredientes)
                    .HasForeignKey(e => e.ComidaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Ingrediente)
                    .WithMany(i => i.ComidaIngredientes)
                    .HasForeignKey(e => e.IngredienteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de MenuSemanal
            modelBuilder.Entity<MenuSemanal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            });

            // Configuración de MenuComida
            modelBuilder.Entity<MenuComida>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.MenuSemanal)
                    .WithMany(m => m.MenuComidas)
                    .HasForeignKey(e => e.MenuSemanalId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Comida)
                    .WithMany(c => c.MenuComidas)
                    .HasForeignKey(e => e.ComidaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.MenuSemanalId, e.DiaSemana, e.TipoComida })
                    .IsUnique();
            });
        }
    }
}