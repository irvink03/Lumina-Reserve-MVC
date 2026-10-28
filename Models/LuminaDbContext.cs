using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lumina_Reserve_MVC.Models;

public partial class LuminaDbContext : DbContext
{
    public LuminaDbContext()
    {
    }

    public LuminaDbContext(DbContextOptions<LuminaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<CitaCorte> CitaCortes { get; set; }

    public virtual DbSet<CodigosVerificacionBot> CodigosVerificacionBots { get; set; }

    public virtual DbSet<Corte> Cortes { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Gasto> Gastos { get; set; }

    public virtual DbSet<Notificacione> Notificaciones { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Resena> Resenas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sucursale> Sucursales { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=LuminaReserveDB;User Id=SA;Password=2811;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Citas__3214EC0730859C41");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Barbero).WithMany(p => p.Cita)
                .HasForeignKey(d => d.BarberoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Barbero");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Cita)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Cliente");

            entity.HasOne(d => d.Sucursal).WithMany(p => p.Cita)
                .HasForeignKey(d => d.SucursalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Sucursal");
        });

        modelBuilder.Entity<CitaCorte>(entity =>
        {
            entity.HasKey(e => new { e.CitaId, e.CorteId }).HasName("PK__CitaCort__F9E7DE57C99A21FD");

            entity.Property(e => e.PrecioCobrado).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cita).WithMany(p => p.CitaCortes)
                .HasForeignKey(d => d.CitaId)
                .HasConstraintName("FK_CitaCortes_Cita");

            entity.HasOne(d => d.Corte).WithMany(p => p.CitaCortes)
                .HasForeignKey(d => d.CorteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CitaCortes_Corte");
        });

        modelBuilder.Entity<CodigosVerificacionBot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CodigosV__3214EC07D9C282E9");

            entity.ToTable("CodigosVerificacionBot");

            entity.Property(e => e.Codigo).HasMaxLength(10);
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.FechaGeneracion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Usado).HasDefaultValue(false);

            entity.HasOne(d => d.Usuario).WithMany(p => p.CodigosVerificacionBots)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_CodigosVerificacionBot_Usuario");
        });

        modelBuilder.Entity<Corte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cortes__3214EC0712707CF8");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Categoria).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Empleado__3214EC074B91B95C");

            entity.HasIndex(e => e.UsuarioId, "UQ__Empleado__2B3DE7B947478299").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Especialidad).HasMaxLength(100);
            entity.Property(e => e.PorcentajeComision)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Sucursal).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.SucursalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Sucursal");

            entity.HasOne(d => d.Usuario).WithOne(p => p.Empleado)
                .HasForeignKey<Empleado>(d => d.UsuarioId)
                .HasConstraintName("FK_Empleados_Usuario");
        });

        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gastos__3214EC07F36CE562");

            entity.Property(e => e.Categoria).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.FechaGasto)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.RegistradoPor).WithMany(p => p.Gastos)
                .HasForeignKey(d => d.RegistradoPorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gastos_Usuario");

            entity.HasOne(d => d.Sucursal).WithMany(p => p.Gastos)
                .HasForeignKey(d => d.SucursalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gastos_Sucursal");
        });

        modelBuilder.Entity<Notificacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC075261A5A2");

            entity.Property(e => e.Enviado).HasDefaultValue(false);
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Tipo).HasMaxLength(50);

            entity.HasOne(d => d.Cita).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.CitaId)
                .HasConstraintName("FK_Notificaciones_Cita");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notificaciones_Usuario");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pagos__3214EC07D65B692B");

            entity.HasIndex(e => e.CitaId, "UQ__Pagos__F0E2D9D3992BDD93").IsUnique();

            entity.Property(e => e.DescuentoAplicado)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MetodoPago).HasMaxLength(50);
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cita).WithOne(p => p.Pago)
                .HasForeignKey<Pago>(d => d.CitaId)
                .HasConstraintName("FK_Pagos_Cita");
        });

        modelBuilder.Entity<Resena>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Resenas__3214EC07BA09E45C");

            entity.HasIndex(e => e.CitaId, "UQ__Resenas__F0E2D9D319BBCC52").IsUnique();

            entity.Property(e => e.Aprobada).HasDefaultValue(true);
            entity.Property(e => e.Comentario).HasMaxLength(500);
            entity.Property(e => e.FechaResena)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Barbero).WithMany(p => p.Resenas)
                .HasForeignKey(d => d.BarberoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resenas_Barbero");

            entity.HasOne(d => d.Cita).WithOne(p => p.Resena)
                .HasForeignKey<Resena>(d => d.CitaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resenas_Cita");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Resenas)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resenas_Cliente");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0774BDDEA5");

            entity.HasIndex(e => e.Nombre, "UQ__Roles__75E3EFCFF40A2645").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Sucursale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sucursal__3214EC07AAFB2BE6");

            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.HoraApertura).HasDefaultValue(new TimeOnly(10, 0, 0));
            entity.Property(e => e.HoraCierre).HasDefaultValue(new TimeOnly(20, 0, 0));
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC0737DFDB3D");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__60695A196CB84E24").IsUnique();

            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);

            entity.HasMany(d => d.Rols).WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RolId")
                        .HasConstraintName("FK_UsuarioRoles_Rol"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .HasConstraintName("FK_UsuarioRoles_Usuario"),
                    j =>
                    {
                        j.HasKey("UsuarioId", "RolId").HasName("PK__UsuarioR__24AFD797DD3CCBB9");
                        j.ToTable("UsuarioRoles");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
