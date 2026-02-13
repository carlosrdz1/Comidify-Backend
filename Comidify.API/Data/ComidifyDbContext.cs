using Microsoft.EntityFrameworkCore;
using Comidify.API.Models;

namespace Comidify.API.Data;

public class ComidifyDbContext : DbContext
{
    public ComidifyDbContext(DbContextOptions<ComidifyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Comida> Comidas { get; set; }
    public DbSet<Ingrediente> Ingredientes { get; set; }
    public DbSet<ComidaIngrediente> ComidaIngredientes { get; set; }
    public DbSet<MenuSemanal> MenusSemanales { get; set; }
    public DbSet<MenuComida> MenuComidas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(e => e.Comidas)
                  .WithOne(e => e.Usuario)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Ingredientes)
                  .WithOne(e => e.Usuario)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.MenusSemanales)
                  .WithOne(e => e.Usuario)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Comida
        modelBuilder.Entity<Comida>(entity =>
        {
            entity.HasMany(e => e.Ingredientes)
                  .WithOne(e => e.Comida)
                  .HasForeignKey(e => e.ComidaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Ingrediente (sin cambios adicionales)

        // ComidaIngrediente (tabla intermedia)
        modelBuilder.Entity<ComidaIngrediente>(entity =>
        {
            entity.HasOne(e => e.Comida)
                  .WithMany(e => e.Ingredientes)
                  .HasForeignKey(e => e.ComidaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Ingrediente)
                  .WithMany(e => e.ComidaIngredientes)
                  .HasForeignKey(e => e.IngredienteId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // MenuSemanal
        modelBuilder.Entity<MenuSemanal>(entity =>
        {
            entity.HasMany(e => e.Comidas)
                  .WithOne(e => e.MenuSemanal)
                  .HasForeignKey(e => e.MenuSemanalId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // MenuComida (tabla intermedia)
        modelBuilder.Entity<MenuComida>(entity =>
        {
            entity.HasOne(e => e.MenuSemanal)
                  .WithMany(e => e.Comidas)
                  .HasForeignKey(e => e.MenuSemanalId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Comida)
                  .WithMany(e => e.MenuComidas)
                  .HasForeignKey(e => e.ComidaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}