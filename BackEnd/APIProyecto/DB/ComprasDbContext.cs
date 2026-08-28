using System;
using System.Collections.Generic;
using APIProyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace APIProyecto.DB;

public partial class ComprasDbContext : DbContext
{
    public ComprasDbContext(DbContextOptions<ComprasDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bitacora> Bitacoras { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Deposito> Depositos { get; set; }

    public virtual DbSet<DetalleCompra> DetalleCompras { get; set; }

    public virtual DbSet<DetalleOrdenesPago> DetalleOrdenesPagos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<MetodosPagoOrden> MetodosPagoOrdens { get; set; }

    public virtual DbSet<MovimientosCc> MovimientosCcs { get; set; }

    public virtual DbSet<NotasCreditoDebito> NotasCreditoDebitos { get; set; }

    public virtual DbSet<OrdenesPago> OrdenesPagos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductosProveedor> ProductosProveedors { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<RecuperacionPassword> RecuperacionPasswords { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StockUbicacion> StockUbicacions { get; set; }

    public virtual DbSet<Tarea> Tareas { get; set; }

    public virtual DbSet<Turno> Turnos { get; set; }

    public virtual DbSet<TurnosDuracion> TurnosDuracions { get; set; }

    public virtual DbSet<Ubicacione> Ubicaciones { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bitacora>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("bitacora_pkey");

            entity.ToTable("bitacora", "auditoria");

            entity.Property(e => e.IdLog).HasColumnName("id_log");
            entity.Property(e => e.Accion).HasColumnName("accion");
            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Modulo)
                .HasMaxLength(50)
                .HasColumnName("modulo");
            entity.Property(e => e.NombreEmpleado)
                .HasMaxLength(200)
                .HasColumnName("nombre_empleado");
            entity.Property(e => e.ValorAnterior)
                .HasColumnType("jsonb")
                .HasColumnName("valor_anterior");
            entity.Property(e => e.ValorNuevo)
                .HasColumnType("jsonb")
                .HasColumnName("valor_nuevo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Bitacoras)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_audit_usr");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("categorias_pkey");

            entity.ToTable("categorias", "logistica");

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("compras_pkey");

            entity.ToTable("compras", "finanzas");

            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.CondicionPago)
                .HasMaxLength(20)
                .HasColumnName("condicion_pago");
            entity.Property(e => e.FechaCompra).HasColumnName("fecha_compra");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_compra_prov");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_compra_usr");
        });

        modelBuilder.Entity<Deposito>(entity =>
        {
            entity.HasKey(e => e.IdDeposito).HasName("depositos_pkey");

            entity.ToTable("depositos", "logistica");

            entity.Property(e => e.IdDeposito).HasColumnName("id_deposito");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.HasKey(e => new { e.IdCompra, e.IdProducto }).HasName("pk_detalle_compra");

            entity.ToTable("detalle_compras", "finanzas");

            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(12, 2)
                .HasColumnName("precio_unitario");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detalle_compra_compra");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detalle_compra_prod");
        });

        modelBuilder.Entity<DetalleOrdenesPago>(entity =>
        {
            entity.HasKey(e => new { e.IdOrden, e.IdCompra }).HasName("pk_detalles_ordenes_pago");

            entity.ToTable("detalle_ordenes_pago", "finanzas");

            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.MontoAsignado)
                .HasPrecision(12, 2)
                .HasColumnName("monto_asignado");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.DetalleOrdenesPagos)
                .HasForeignKey(d => d.IdCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detalles_ordenes_pago_compra");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.DetalleOrdenesPagos)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_detalles_ordenes_pago_orden");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("empleados_pkey");

            entity.ToTable("empleados", "seguridad");

            entity.HasIndex(e => e.Dni, "empleados_dni_key").IsUnique();

            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Dni)
                .HasMaxLength(12)
                .HasColumnName("dni");
            entity.Property(e => e.FirmaDig).HasColumnName("firma_dig");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("facturas_pkey");

            entity.ToTable("facturas", "finanzas");

            entity.HasIndex(e => e.IdCompra, "facturas_id_compra_key").IsUnique();

            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.ArchivoAdjunto)
                .HasMaxLength(255)
                .HasColumnName("archivo_adjunto");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Impaga'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.Iva)
                .HasPrecision(5, 2)
                .HasColumnName("iva");
            entity.Property(e => e.Monto)
                .HasPrecision(12, 2)
                .HasColumnName("monto");
            entity.Property(e => e.NroComprobante)
                .HasMaxLength(20)
                .HasColumnName("nro_comprobante");
            entity.Property(e => e.TipoComprobante)
                .HasMaxLength(20)
                .HasColumnName("tipo_comprobante");

            entity.HasOne(d => d.IdCompraNavigation).WithOne(p => p.Factura)
                .HasForeignKey<Factura>(d => d.IdCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_facturas_comp");
        });

        modelBuilder.Entity<MetodosPagoOrden>(entity =>
        {
            entity.HasKey(e => e.IdMetodo).HasName("metodos_pago_orden_pkey");

            entity.ToTable("metodos_pago_orden", "finanzas");

            entity.Property(e => e.IdMetodo).HasColumnName("id_metodo");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.Monto)
                .HasPrecision(12, 2)
                .HasColumnName("monto");
            entity.Property(e => e.Referencia)
                .HasMaxLength(100)
                .HasColumnName("referencia");
            entity.Property(e => e.TipoMetodo)
                .HasMaxLength(30)
                .HasColumnName("tipo_metodo");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.MetodosPagoOrdens)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_metodos_pago_orden_op");
        });

        modelBuilder.Entity<MovimientosCc>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("movimientos_cc_pkey");

            entity.ToTable("movimientos_cc", "finanzas");

            entity.Property(e => e.IdMovimiento).HasColumnName("id_movimiento");
            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.IdNota).HasColumnName("id_nota");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.Monto)
                .HasPrecision(12, 2)
                .HasColumnName("monto");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(30)
                .HasColumnName("tipo_movimiento");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.MovimientosCcs)
                .HasForeignKey(d => d.IdCompra)
                .HasConstraintName("fk_movimientos_cc_compra");

            entity.HasOne(d => d.IdNotaNavigation).WithMany(p => p.MovimientosCcs)
                .HasForeignKey(d => d.IdNota)
                .HasConstraintName("fk_movimientos_cc_nota");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.MovimientosCcs)
                .HasForeignKey(d => d.IdOrden)
                .HasConstraintName("fk_movimientos_cc_orden");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.MovimientosCcs)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_movimientos_cc_prov");
        });

        modelBuilder.Entity<NotasCreditoDebito>(entity =>
        {
            entity.HasKey(e => e.IdNota).HasName("notas_credito_debito_pkey");

            entity.ToTable("notas_credito_debito", "finanzas");

            entity.Property(e => e.IdNota).HasColumnName("id_nota");
            entity.Property(e => e.ArchivoAdjunto)
                .HasMaxLength(255)
                .HasColumnName("archivo_adjunto");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.Monto)
                .HasPrecision(12, 2)
                .HasColumnName("monto");
            entity.Property(e => e.Motivo).HasColumnName("motivo");
            entity.Property(e => e.NroComprobante)
                .HasMaxLength(20)
                .HasColumnName("nro_comprobante");
            entity.Property(e => e.TipoNota)
                .HasMaxLength(10)
                .HasColumnName("tipo_nota");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.NotasCreditoDebitos)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_nota_prov");
        });

        modelBuilder.Entity<OrdenesPago>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("ordenes_pago_pkey");

            entity.ToTable("ordenes_pago", "finanzas");

            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.ArchivoPdf)
                .HasMaxLength(255)
                .HasColumnName("archivo_pdf");
            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha_emision");
            entity.Property(e => e.IdAutoriza).HasColumnName("id_autoriza");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.MontoTotal)
                .HasPrecision(12, 2)
                .HasColumnName("monto_total");

            entity.HasOne(d => d.IdAutorizaNavigation).WithMany(p => p.OrdenesPagoIdAutorizaNavigations)
                .HasForeignKey(d => d.IdAutoriza)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ordenes_pago_prov1");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.OrdenesPagos)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ordenes_pago_prov");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.OrdenesPagoIdUsuarioNavigations)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ordenes_pago_usr");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("permisos_pkey");

            entity.ToTable("permisos", "seguridad");

            entity.HasIndex(e => e.NombrePermiso, "permisos_nombre_permiso_key").IsUnique();

            entity.Property(e => e.IdPermiso).HasColumnName("id_permiso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.NombrePermiso)
                .HasMaxLength(100)
                .HasColumnName("nombre_permiso");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("productos_pkey");

            entity.ToTable("productos", "logistica");

            entity.HasIndex(e => e.CodigoBarras, "productos_codigo_barras_key").IsUnique();

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoBarras)
                .HasMaxLength(50)
                .HasColumnName("codigo_barras");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.StockMinimo)
                .HasDefaultValue(0)
                .HasColumnName("stock_minimo");

            entity.HasMany(d => d.IdCategoria).WithMany(p => p.IdProductos)
                .UsingEntity<Dictionary<string, object>>(
                    "ProdcutoCategorium",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_producto_categoria_cat"),
                    l => l.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_producto_categoria_prod"),
                    j =>
                    {
                        j.HasKey("IdProducto", "IdCategoria").HasName("pk_producto_categoria");
                        j.ToTable("prodcuto_categoria", "logistica");
                        j.IndexerProperty<int>("IdProducto").HasColumnName("id_producto");
                        j.IndexerProperty<int>("IdCategoria").HasColumnName("id_categoria");
                    });
        });

        modelBuilder.Entity<ProductosProveedor>(entity =>
        {
            entity.HasKey(e => new { e.IdProducto, e.IdProveedor }).HasName("pk_productos_proveedor");

            entity.ToTable("productos_proveedor", "logistica");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.PrecioCosto)
                .HasPrecision(12, 2)
                .HasColumnName("precio_costo");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductosProveedors)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productos_proveedor_prod");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.ProductosProveedors)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productos_proveedor_prov");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("proveedores_pkey");

            entity.ToTable("proveedores", "seguridad");

            entity.HasIndex(e => e.Alias, "proveedores_alias_key").IsUnique();

            entity.HasIndex(e => e.Cbu, "proveedores_cbu_key").IsUnique();

            entity.HasIndex(e => e.Cuit, "proveedores_cuit_key").IsUnique();

            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Alias)
                .HasMaxLength(50)
                .HasColumnName("alias");
            entity.Property(e => e.Cbu)
                .HasMaxLength(22)
                .HasColumnName("cbu");
            entity.Property(e => e.Cuit)
                .HasMaxLength(15)
                .HasColumnName("cuit");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(200)
                .HasColumnName("domicilio");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
            entity.Property(e => e.TipoSociedad)
                .HasMaxLength(6)
                .HasColumnName("tipo_sociedad");
        });

        modelBuilder.Entity<RecuperacionPassword>(entity =>
        {
            entity.HasKey(e => e.IdRecuperacion).HasName("recuperacion_passwords_pkey");

            entity.ToTable("recuperacion_passwords", "seguridad");

            entity.Property(e => e.IdRecuperacion).HasColumnName("id_recuperacion");
            entity.Property(e => e.CodigoToken)
                .HasMaxLength(6)
                .HasColumnName("codigo_token");
            entity.Property(e => e.FechaExpiracion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_expiracion");
            entity.Property(e => e.FechaGeneracion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_generacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Usado)
                .HasDefaultValue(false)
                .HasColumnName("usado");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.RecuperacionPasswords)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recuperacion_usu");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("roles_pkey");

            entity.ToTable("roles", "seguridad");

            entity.HasIndex(e => e.Nombre, "roles_nombre_key").IsUnique();

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.HasMany(d => d.IdPermisos).WithMany(p => p.IdRols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolesPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .HasConstraintName("fk_roles_permisos_perm"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("IdRol")
                        .HasConstraintName("fk_roles_permisos_rol"),
                    j =>
                    {
                        j.HasKey("IdRol", "IdPermiso").HasName("pk_roles_permisos");
                        j.ToTable("roles_permisos", "seguridad");
                        j.IndexerProperty<int>("IdRol").HasColumnName("id_rol");
                        j.IndexerProperty<int>("IdPermiso").HasColumnName("id_permiso");
                    });
        });

        modelBuilder.Entity<StockUbicacion>(entity =>
        {
            entity.HasKey(e => new { e.IdProducto, e.IdUbicacion }).HasName("pk_stock_ubicacion");

            entity.ToTable("stock_ubicacion", "logistica");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue(0)
                .HasColumnName("cantidad");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.StockUbicacions)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stock_ubicacion_prod");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.StockUbicacions)
                .HasForeignKey(d => d.IdUbicacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stock_ubicacion_ubic");
        });

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.IdTarea).HasName("tareas_pkey");

            entity.ToTable("tareas", "seguridad");

            entity.Property(e => e.IdTarea).HasColumnName("id_tarea");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Incompleta'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaLimite).HasColumnName("fecha_limite");
            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tarea_emp");
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.IdTurno).HasName("turnos_pkey");

            entity.ToTable("turnos", "seguridad");

            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.Turno1)
                .HasMaxLength(20)
                .HasColumnName("turno");
        });

        modelBuilder.Entity<TurnosDuracion>(entity =>
        {
            entity.HasKey(e => new { e.IdTurno, e.IdEmpleado }).HasName("pk_turnos_duracion");

            entity.ToTable("turnos_duracion", "seguridad");

            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.Fin).HasColumnName("fin");
            entity.Property(e => e.Inicio).HasColumnName("inicio");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.TurnosDuracions)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_turnos_duracion_emp");

            entity.HasOne(d => d.IdTurnoNavigation).WithMany(p => p.TurnosDuracions)
                .HasForeignKey(d => d.IdTurno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_turnos_duracion_tur");
        });

        modelBuilder.Entity<Ubicacione>(entity =>
        {
            entity.HasKey(e => e.IdUbicacion).HasName("ubicaciones_pkey");

            entity.ToTable("ubicaciones", "logistica");

            entity.Property(e => e.IdUbicacion).HasColumnName("id_ubicacion");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Estanteria)
                .HasMaxLength(50)
                .HasColumnName("estanteria");
            entity.Property(e => e.IdDeposito).HasColumnName("id_deposito");
            entity.Property(e => e.Sector)
                .HasMaxLength(50)
                .HasColumnName("sector");

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.Ubicaciones)
                .HasForeignKey(d => d.IdDeposito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ubicacion_depo");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("usuarios_pkey");

            entity.ToTable("usuarios", "seguridad");

            entity.HasIndex(e => e.Email, "usuarios_email_key").IsUnique();

            entity.Property(e => e.IdUsuario)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_usuario");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_rol");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_emp");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
