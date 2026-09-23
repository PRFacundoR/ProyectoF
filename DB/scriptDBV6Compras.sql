
-- ==============================================================================
-- 1. Esquemas
CREATE SCHEMA IF NOT EXISTS seguridad;
CREATE SCHEMA IF NOT EXISTS logistica;        
CREATE SCHEMA IF NOT EXISTS finanzas; 
CREATE SCHEMA IF NOT EXISTS auditoria;
-- ==============================================================================

-- ==============================================================================
-- 2. Seguridad (Empleados, Usuarios, Proveedores, Tareas, Roles, Permisos, Turnos y Contraseñas) 
 -- 10 tablas (contando intermedias)

-- Empleados, Turnos y Tareas
CREATE TABLE seguridad.turnos(id_turno SERIAL PRIMARY KEY,
                              turno VARCHAR(20) CHECK (turno IN ('Mañana', 'Tarde', 'Noche', 'Jornada Completa')) NOT NULL);

CREATE TABLE seguridad.empleados(id_empleado SERIAL PRIMARY KEY,
                                 nombre VARCHAR(100) NOT NULL,
                                 apellido VARCHAR(100) NOT NULL,
                                 dni VARCHAR(12) UNIQUE NOT NULL,
                                 firma_dig TEXT,
                                 activo BOOLEAN NOT NULL DEFAULT TRUE);

CREATE TABLE seguridad.turnos_duracion(id_turno INTEGER,
                                       id_empleado INTEGER,
                                       inicio DATE,
                                       fin DATE,
                                       CONSTRAINT pk_turnos_duracion PRIMARY KEY (id_turno, id_empleado, inicio),
                                       CONSTRAINT fk_turnos_duracion_tur FOREIGN KEY (id_turno) REFERENCES seguridad.turnos(id_turno),
                                       CONSTRAINT fk_turnos_duracion_emp FOREIGN KEY (id_empleado) REFERENCES seguridad.empleados(id_empleado));

CREATE TABLE seguridad.tareas(id_tarea SERIAL PRIMARY KEY,
                              descripcion TEXT NOT NULL,
                              fecha_limite DATE NOT NULL,
                              estado VARCHAR(20) DEFAULT 'Incompleta' CHECK (estado IN ('Incompleta', 'Completada')) NOT NULL,
                              id_empleado INTEGER NOT NULL,
                              CONSTRAINT fk_tarea_emp FOREIGN KEY (id_empleado) REFERENCES seguridad.empleados(id_empleado));

-- Roles y Permisos
CREATE TABLE seguridad.permisos(id_permiso SERIAL PRIMARY KEY,
                                nombre_permiso VARCHAR(100) UNIQUE NOT NULL,
								descripcion VARCHAR(200) NOT NULL);

CREATE TABLE seguridad.roles(id_rol SERIAL PRIMARY KEY,
                             nombre VARCHAR(50) UNIQUE NOT NULL,
							 descripcion VARCHAR(200) NOT NULL);

CREATE TABLE seguridad.roles_permisos(id_rol INTEGER,
                                      id_permiso INTEGER,
									  CONSTRAINT pk_roles_permisos PRIMARY KEY (id_rol, id_permiso),
									  CONSTRAINT fk_roles_permisos_rol FOREIGN KEY (id_rol) REFERENCES seguridad.roles(id_rol) ON DELETE CASCADE,
									  CONSTRAINT fk_roles_permisos_perm FOREIGN KEY (id_permiso) REFERENCES seguridad.permisos(id_permiso) ON DELETE CASCADE);

-- Datos de acceso (1 a 1 con empleado)
CREATE TABLE seguridad.usuarios(id_usuario SERIAL PRIMARY KEY,
                                password_hash VARCHAR(255) NOT NULL,
                                email VARCHAR(150) UNIQUE NOT NULL,
                                id_rol INTEGER NOT NULL,
                                CONSTRAINT fk_usuario_emp FOREIGN KEY (id_usuario) REFERENCES seguridad.empleados(id_empleado),
                                CONSTRAINT fk_usuario_rol FOREIGN KEY (id_rol) REFERENCES seguridad.roles(id_rol));

-- Gestión de Tokens para Recuperar Contraseña
CREATE TABLE seguridad.recuperacion_passwords(id_recuperacion SERIAL PRIMARY KEY,
                                              codigo_token VARCHAR(6) NOT NULL, 
                                              fecha_generacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                              fecha_expiracion TIMESTAMP NOT NULL, 
                                              usado BOOLEAN DEFAULT FALSE NOT NULL, 
                                              id_usuario INTEGER NOT NULL,
                                              CONSTRAINT fk_recuperacion_usu FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario));

-- Proveedores
CREATE TABLE seguridad.proveedores(id_proveedor SERIAL PRIMARY KEY,
                                   nombre VARCHAR(150) NOT NULL,
                                   tipo_sociedad VARCHAR(6) NOT NULL,  -- esto me pareció mejor que solo guardar la razon social, se puede cambiar
                                   cuit VARCHAR(15) UNIQUE NOT NULL,
                                   cbu VARCHAR(22) UNIQUE NOT NULL,
                                   alias VARCHAR(50) UNIQUE,
                                   domicilio VARCHAR(200) NOT NULL,
                                   telefono VARCHAR(50),
                                   email VARCHAR(100) NOT NULL,
                                   activo BOOLEAN DEFAULT TRUE NOT NULL);
-- ==============================================================================


-- ==============================================================================
-- 3. Logística (Depositos y Productos)
 -- 7 tablas (contando intermedias)

CREATE TABLE logistica.productos(id_producto SERIAL PRIMARY KEY,
                                 nombre VARCHAR(150) NOT NULL,
                                 codigo_barras VARCHAR(50) UNIQUE,
                                 stock_minimo INT DEFAULT 0 CHECK (stock_minimo >= 0) NOT NULL,
                                 activo BOOLEAN DEFAULT TRUE NOT NULL);

CREATE TABLE logistica.categorias(id_categoria SERIAL PRIMARY KEY,
                                  nombre VARCHAR(50) NOT NULL);

CREATE TABLE logistica.clasificaciones(id_producto INTEGER,
                                       id_categoria INTEGER,
                                       CONSTRAINT pk_producto_categoria PRIMARY KEY (id_producto, id_categoria),
                                       CONSTRAINT fk_producto_categoria_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
                                       CONSTRAINT fk_producto_categoria_cat FOREIGN KEY (id_categoria) REFERENCES logistica.categorias(id_categoria));

CREATE TABLE logistica.depositos(id_deposito SERIAL PRIMARY KEY,
                                 nombre VARCHAR(100),
                                 direccion VARCHAR(200) NOT NULL,
                                 activo BOOLEAN DEFAULT TRUE NOT NULL);

CREATE TABLE logistica.ubicaciones(id_ubicacion SERIAL PRIMARY KEY,
                                   sector VARCHAR(50) NOT NULL,
                                   estanteria SMALLINT NOT NULL,
                                   activo BOOLEAN DEFAULT TRUE NOT NULL,
                                   id_deposito INT NOT NULL,
                                   CONSTRAINT fk_ubicacion_depo FOREIGN KEY (id_deposito) REFERENCES logistica.depositos(id_deposito));

CREATE TABLE logistica.stock(id_ubicacion INTEGER,
                             id_producto INTEGER,
                             cantidad INTEGER DEFAULT 0 CHECK (cantidad >= 0) NOT NULL,
                             CONSTRAINT pk_stock PRIMARY KEY (id_producto, id_ubicacion),
                             CONSTRAINT fk_stock_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
                             CONSTRAINT fk_stock_ubic FOREIGN KEY (id_ubicacion) REFERENCES logistica.ubicaciones(id_ubicacion));

CREATE TABLE logistica.catalogo(c_barras_proveedor VARCHAR(50) UNIQUE NOT NULL,
                                id_proveedor INTEGER NOT NULL,
                                id_producto INTEGER NOT NULL,
                                precio_costo DECIMAL(12,2) CHECK (precio_costo >= 0) NOT NULL,
                                fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                CONSTRAINT pk_catalogo PRIMARY KEY (id_producto, id_proveedor),
                                CONSTRAINT fk_catalogo_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
                                CONSTRAINT fk_catalogo_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor));
-- ==============================================================================

-- seguir desde aqui

-- ==============================================================================
-- 4. Finanzas (Compras, Facturas, Ordenes de Pago y Notas de Credito/Debito)
 -- 12 tablas (contando intermedias)

CREATE TABLE finanzas.facturas(id_factura BIGSERIAL PRIMARY KEY,
                               tipo_comprobante VARCHAR(10) CHECK (tipo_comprobante IN ('Factura A', 'Factura B', 'Factura C')) NOT NULL,
                               nro_comprobante VARCHAR(20) NOT NULL,
                               fecha_emision DATE,
                               fecha_vencimiento DATE NOT NULL,
                               monto DECIMAL(12,2) CHECK (monto >= 0) NOT NULL,
                               iva DECIMAL(5,2),
                               condicion_pago VARCHAR(20) CHECK (condicion_pago IN ('Contado', 'Cuenta Corriente')) NOT NULL,
                               estado VARCHAR(20) DEFAULT 'Impaga' CHECK (estado IN ('Impaga', 'Pagada Parcialmente', 'Pagada', 'Anulada')),
                               archivo_adjunto VARCHAR(255));

-- igual a compras_realizadas
CREATE TABLE finanzas.compras_pendientes(id_usuario INTEGER NOT NULL,
                                         id_compra SERIAL PRIMARY KEY,
                                         id_factura BIGINT,
                                         fecha_pedido DATE NOT NULL,
                                         fecha_compra DATE NOT NULL,
                                         fecha_entrega DATE,
										 incompleta BOOL DEFAULT TRUE NOT NULL,
                                         CONSTRAINT fk_compra_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario),
                                         CONSTRAINT fk_compras_fact FOREIGN KEY (id_factura) REFERENCES finanzas.facturas(id_factura));
CREATE TABLE finanzas.pedidos(id_usuario INTEGER NOT NULL,
                              id_pedido SERIAL PRIMARY KEY,
                              fecha_pedido DATE NOT NULL,
                              estado VARCHAR(25) DEFAULT 'Pendiente' CHECK (estado IN ('Pendiente', 'Rechazado')) NOT NULL,
                              CONSTRAINT fk_compra_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario));

CREATE TABLE finanzas.detalle_compra_p(id_compra INTEGER NOT NULL,
                                       c_barras_proveedor VARCHAR(50) NOT NULL,
                                       cantidad INTEGER CHECK (cantidad > 0) NOT NULL,
                                       precio_unitario DECIMAL(12,2) CHECK (precio_unitario > 0) NOT NULL,
                                       CONSTRAINT pk_detalle_compra_p PRIMARY KEY (id_compra, c_barras_proveedor),
                                       CONSTRAINT fk_detalle_compra_pend FOREIGN KEY (id_compra) REFERENCES finanzas.compras_pendientes(id_compra) ON DELETE CASCADE,
                                       CONSTRAINT fk_detalle_compra_p_cat FOREIGN KEY (c_barras_proveedor) REFERENCES logistica.catalogo(c_barras_proveedor));
									   
CREATE TABLE finanzas.compras_realizadas(id_usuario INTEGER NOT NULL,
                                         id_compra SERIAL PRIMARY KEY,
                                         id_factura BIGINT NOT NULL,
										  id_pedido INTEGER NOT NULL,
                                         fecha_pedido DATE NOT NULL,
                                         fecha_compra DATE NOT NULL,
                                         fecha_entrega DATE,
										 CONSTRAINT fk_pedido FOREIGN KEY (id_pedido) REFERENCES finanzas.pedidos(id_pedido),
                                         CONSTRAINT fk_compra_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario),
                                         CONSTRAINT fk_compras_fact FOREIGN KEY (id_factura) REFERENCES finanzas.facturas(id_factura));



CREATE TABLE finanzas.detalle_compra_r(id_compra INTEGER NOT NULL,
                                       c_barras_proveedor VARCHAR(50) NOT NULL,
                                       cantidad INTEGER CHECK (cantidad > 0) NOT NULL,
                                       precio_unitario DECIMAL(12,2) CHECK (precio_unitario > 0) NOT NULL,
                                       CONSTRAINT pk_detalle_compra_r PRIMARY KEY (id_compra, c_barras_proveedor),
                                       CONSTRAINT fk_detalle_compra_real FOREIGN KEY (id_compra) REFERENCES finanzas.compras_realizadas(id_compra) ON DELETE CASCADE,
                                       CONSTRAINT fk_detalle_compra_r_cat FOREIGN KEY (c_barras_proveedor) REFERENCES logistica.catalogo(c_barras_proveedor));

CREATE TABLE finanzas.items_pedidos(id_pedido INTEGER NOT NULL,
                                    c_barras_proveedor VARCHAR(50) NOT NULL,
                                    cantidad INTEGER CHECK (cantidad > 0) NOT NULL,
                                    precio_unitario DECIMAL(12,2) CHECK (precio_unitario > 0) NOT NULL,
                                    CONSTRAINT pk_items_pedidos PRIMARY KEY (id_pedido, c_barras_proveedor),
                                    CONSTRAINT fk_items_pedidos_ped FOREIGN KEY (id_pedido) REFERENCES finanzas.pedidos(id_pedido) ON DELETE CASCADE,
                                    CONSTRAINT fk_items_pedidos_cat FOREIGN KEY (c_barras_proveedor) REFERENCES logistica.catalogo(c_barras_proveedor));

CREATE TABLE finanzas.notas_credito_debito(id_nota BIGSERIAL PRIMARY KEY,
                                           tipo_nota VARCHAR(10) CHECK (tipo_nota IN ('Credito', 'Debito')),
                                           nro_comprobante VARCHAR(20) NOT NULL,
                                           fecha DATE NOT NULL,
                                           motivo TEXT NOT NULL,
                                           monto DECIMAL(12,2) NOT NULL CHECK (monto > 0),
                                           archivo_adjunto VARCHAR(255),
                                           id_factura BIGINT NOT NULL,
                                           CONSTRAINT uq_notas_unicas UNIQUE (id_factura, tipo_nota),
                                           CONSTRAINT fk_nota_fact FOREIGN KEY (id_factura) REFERENCES finanzas.facturas(id_factura));

CREATE TABLE finanzas.ordenes_pago(id_orden BIGSERIAL PRIMARY KEY,
                                   fecha_emision DATE NOT NULL DEFAULT CURRENT_DATE,
                                   monto_total DECIMAL(12,2) NOT NULL CHECK (monto_total > 0),
                                   archivo_pdf VARCHAR(255),
                                   id_autoriza INTEGER NOT NULL,
                                   id_emisor INTEGER NOT NULL,
                                   id_proveedor INTEGER NOT NULL,
                                   CONSTRAINT fk_ordenes_pago_aut FOREIGN KEY (id_autoriza) REFERENCES seguridad.usuarios(id_usuario),
                                   CONSTRAINT fk_ordenes_pago_emi FOREIGN KEY (id_emisor) REFERENCES seguridad.usuarios(id_usuario),
                                   CONSTRAINT fk_ordenes_pago_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor));

CREATE TABLE finanzas.metodos_pago_orden(id_metodo SERIAL PRIMARY KEY,
                                         tipo_metodo VARCHAR(30) CHECK (tipo_metodo IN ('Efectivo', 'Transferencia', 'Cheque', 'Saldo a Favor')),
                                         monto DECIMAL(12,2) NOT NULL CHECK (monto > 0),
                                         referencia VARCHAR(100), 
                                         id_orden BIGINT NOT NULL,
                                         CONSTRAINT fk_metodos_pago_orden_op FOREIGN KEY (id_orden) REFERENCES finanzas.ordenes_pago(id_orden));

CREATE TABLE finanzas.detalle_ordenes_pago(id_orden BIGINT NOT NULL,
                                           id_factura BIGINT NOT NULL, 
                                           monto_asignado DECIMAL(12,2) NOT NULL CHECK (monto_asignado > 0),
                                           CONSTRAINT pk_detalles_ordenes_pago PRIMARY KEY (id_orden, id_factura),
                                           CONSTRAINT fk_detalles_ordenes_pago_ord FOREIGN KEY (id_orden) REFERENCES finanzas.ordenes_pago(id_orden),
                                           CONSTRAINT fk_detalles_ordenes_pago_fact FOREIGN KEY (id_factura) REFERENCES finanzas.facturas(id_factura));

-- Cuentas Corrientes (+ logica para el historial)
CREATE TABLE finanzas.movimientos_cc(id_movimiento BIGSERIAL PRIMARY KEY,
                                     fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                     tipo_movimiento VARCHAR(30) CHECK (tipo_movimiento IN ('Saldo Inicial', 'Compra', 'Pago', 'Nota Credito', 'Nota Debito')),
                                     monto DECIMAL(12,2) NOT NULL,

                                     id_factura BIGINT NULL,
                                     id_nota BIGINT NULL,
                                     id_orden BIGINT NULL,
                                     id_proveedor INTEGER NOT NULL,
                                     CONSTRAINT fk_movimientos_cc_fact FOREIGN KEY (id_factura) REFERENCES finanzas.facturas(id_factura),
                                     CONSTRAINT fk_movimientos_cc_nota FOREIGN KEY (id_nota) REFERENCES finanzas.notas_credito_debito(id_nota),
                                     CONSTRAINT fk_movimientos_cc_ord FOREIGN KEY (id_orden) REFERENCES finanzas.ordenes_pago(id_orden),
                                     CONSTRAINT fk_movimientos_cc_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor),

                                     -- Controla que cada movimiento se tome una sola vez
                                     CONSTRAINT chk_arco_exclusivo CHECK((id_factura IS NOT NULL)::INT + 
                                                                         (id_orden IS NOT NULL)::INT + 
                                                                         (id_nota IS NOT NULL)::INT <= 1));

-- ==============================================================================

-- ==============================================================================
-- 5. Auditoría
 -- x Tablas (armarlas con el tiempo)

CREATE TABLE auditoria.bitacora(id_log SERIAL PRIMARY KEY,
                                id_usuario INTEGER, 
                                nombre_empleado VARCHAR(200) NOT NULL,
                                fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                modulo VARCHAR(50) NOT NULL,
                                accion TEXT NOT NULL,
                                valor_anterior JSONB,
                                valor_nuevo JSONB,
                                CONSTRAINT fk_audit_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario));

-- ==============================================================================
