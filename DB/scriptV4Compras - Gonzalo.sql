

CREATE SCHEMA IF NOT EXISTS seguridad;
CREATE SCHEMA IF NOT EXISTS logistica;        
CREATE SCHEMA IF NOT EXISTS finanzas; 
CREATE SCHEMA IF NOT EXISTS auditoria;

CREATE TABLE seguridad.turnos(id_turno SERIAL PRIMARY KEY,
                              turno VARCHAR(20) CHECK (turno IN ('Mañana', 'Tarde', 'Noche', 'Jornada Completa')) NOT NULL);

CREATE TABLE seguridad.empleados(id_empleado SERIAL PRIMARY KEY,
                                 nombre VARCHAR(100) NOT NULL,
                                 apellido VARCHAR(100) NOT NULL,
                                 dni VARCHAR(12) UNIQUE NOT NULL,
                                 firma_dig TEXT,
                                 activo BOOLEAN);

CREATE TABLE seguridad.turnos_duracion(id_turno INTEGER,
                                       id_empleado INTEGER,
                                       inicio DATE,
                                       fin DATE,
                                       CONSTRAINT pk_turnos_duracion PRIMARY KEY (id_turno, id_empleado),
                                       CONSTRAINT fk_turnos_duracion_tur FOREIGN KEY (id_turno) REFERENCES seguridad.turnos(id_turno),
                                       CONSTRAINT fk_turnos_duracion_emp FOREIGN KEY (id_empleado) REFERENCES seguridad.empleados(id_empleado));

CREATE TABLE seguridad.tareas(id_tarea SERIAL PRIMARY KEY,
                              id_empleado INTEGER NOT NULL,
                              descripcion TEXT NOT NULL,
                              fecha_limite DATE NOT NULL,
                              estado VARCHAR(20) DEFAULT 'Incompleta' CHECK (estado IN ('Incompleta', 'Completada')) NOT NULL,
                              CONSTRAINT fk_tarea_emp FOREIGN KEY (id_empleado) REFERENCES seguridad.empleados(id_empleado));

CREATE TABLE seguridad.permisos(id_permiso SERIAL PRIMARY KEY,
                                nombre_permiso VARCHAR(100) UNIQUE NOT NULL,
								descripcion VARCHAR(200) NOT NULL);

CREATE TABLE seguridad.roles(id_rol SERIAL PRIMARY KEY,
                             nombre VARCHAR(50) UNIQUE NOT NULL,
							 descripcion VARCHAR(200) NOT NULL);

CREATE TABLE seguridad.roles_permisos(id_rol INTEGER NOT NULL,
                                      id_permiso INTEGER NOT NULL,
									  CONSTRAINT pk_roles_permisos PRIMARY KEY (id_rol,id_permiso),
									  CONSTRAINT fk_roles_permisos_rol FOREIGN KEY (id_rol) REFERENCES seguridad.roles(id_rol) ON DELETE CASCADE,
									  CONSTRAINT fk_roles_permisos_perm FOREIGN KEY (id_permiso) REFERENCES seguridad.permisos(id_permiso) ON DELETE CASCADE);

CREATE TABLE seguridad.usuarios(id_usuario SERIAL PRIMARY KEY,
                                password_hash VARCHAR(255) NOT NULL,
                                email VARCHAR(150) UNIQUE NOT NULL,
                                id_rol INTEGER NOT NULL,
                                CONSTRAINT fk_usuario_emp FOREIGN KEY (id_usuario) REFERENCES seguridad.empleados(id_empleado),
                                CONSTRAINT fk_usuario_rol FOREIGN KEY (id_rol) REFERENCES seguridad.roles(id_rol));

CREATE TABLE seguridad.recuperacion_passwords(id_recuperacion SERIAL PRIMARY KEY,
                                              codigo_token VARCHAR(6) NOT NULL, 
                                              fecha_generacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                              fecha_expiracion TIMESTAMP NOT NULL, 
                                              usado BOOLEAN DEFAULT FALSE, 
                                              id_usuario INT NOT NULL,
                                              CONSTRAINT fk_recuperacion_usu FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario));

CREATE TABLE seguridad.proveedores(id_proveedor SERIAL PRIMARY KEY,
                                   nombre VARCHAR(150) NOT NULL,
                                   tipo_sociedad VARCHAR(6) NOT NULL, 
                                   cuit VARCHAR(15) UNIQUE NOT NULL,
                                   cbu VARCHAR(22) UNIQUE NOT NULL,
                                   alias VARCHAR(50) UNIQUE,
                                   domicilio VARCHAR(200) NOT NULL,
                                   telefono VARCHAR(50),
                                   email VARCHAR(100) NOT NULL,
                                   activo BOOLEAN DEFAULT TRUE);

CREATE TABLE logistica.productos(id_producto SERIAL PRIMARY KEY,
                                 nombre VARCHAR(150) NOT NULL,
                                 codigo_barras VARCHAR(50) UNIQUE,
                                 stock_minimo INT DEFAULT 0 CHECK (stock_minimo >= 0),
                                 activo BOOLEAN DEFAULT TRUE);

CREATE TABLE logistica.categorias(id_categoria SERIAL PRIMARY KEY,
                                  nombre VARCHAR(50));

CREATE TABLE logistica.prodcuto_categoria(id_producto INTEGER,
                                          id_categoria INTEGER,
                                          CONSTRAINT pk_producto_categoria PRIMARY KEY (id_producto, id_categoria),
                                          CONSTRAINT fk_producto_categoria_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
                                          CONSTRAINT fk_producto_categoria_cat FOREIGN KEY (id_categoria) REFERENCES logistica.categorias(id_categoria));

CREATE TABLE logistica.depositos(id_deposito SERIAL PRIMARY KEY,
                                 nombre VARCHAR(100) NOT NULL,
                                 direccion VARCHAR(200) NOT NULL,
                                 activo BOOLEAN DEFAULT TRUE);

CREATE TABLE logistica.ubicaciones(id_ubicacion SERIAL PRIMARY KEY,
                                   sector VARCHAR(50) NOT NULL,
                                   estanteria VARCHAR(50) NOT NULL,
                                   activo BOOLEAN DEFAULT TRUE,
                                   id_deposito INT NOT NULL,
                                   CONSTRAINT fk_ubicacion_depo FOREIGN KEY (id_deposito) REFERENCES logistica.depositos(id_deposito));

CREATE TABLE logistica.stock_ubicacion(id_producto INTEGER NOT NULL,
                                       id_ubicacion INTEGER NOT NULL,
                                       cantidad INTEGER DEFAULT 0 CHECK (cantidad >= 0),
                                       CONSTRAINT pk_stock_ubicacion PRIMARY KEY (id_producto, id_ubicacion),
                                       CONSTRAINT fk_stock_ubicacion_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
                                       CONSTRAINT fk_stock_ubicacion_ubic FOREIGN KEY (id_ubicacion) REFERENCES logistica.ubicaciones(id_ubicacion));

CREATE TABLE logistica.productos_proveedor(id_producto INTEGER NOT NULL,
                                           id_proveedor INTEGER NOT NULL,
                                           precio_costo DECIMAL(12,2) NOT NULL CHECK (precio_costo >= 0),
                                           fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                           CONSTRAINT pk_productos_proveedor PRIMARY KEY (id_producto, id_proveedor),
                                           CONSTRAINT fk_productos_proveedor_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
                                           CONSTRAINT fk_productos_proveedor_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor));

CREATE TABLE finanzas.compras(id_usuario INTEGER NOT NULL,
                              id_compra SERIAL PRIMARY KEY, 
                              id_proveedor INTEGER NOT NULL,
                              fecha_compra DATE NOT NULL,
                              condicion_pago VARCHAR(20) CHECK (condicion_pago IN ('Contado', 'Cuenta Corriente')),
                              CONSTRAINT fk_compra_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor),
                              CONSTRAINT fk_compra_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario));

CREATE TABLE finanzas.facturas(id_factura SERIAL PRIMARY KEY,
                               tipo_comprobante VARCHAR(20) CHECK (tipo_comprobante IN ('A', 'B', 'C')),
                               nro_comprobante VARCHAR(20) NOT NULL,
                               fecha_emision DATE,
                               fecha_vencimiento DATE NOT NULL,
                               monto DECIMAL(12,2) NOT NULL CHECK (monto >= 0),
                               iva DECIMAL(5,2),
                               estado VARCHAR(20) DEFAULT 'Impaga' CHECK (estado IN ('Impaga', 'Pagada Parcial', 'Pagada')),
                               archivo_adjunto VARCHAR(255),
							   id_compra INTEGER NOT NULL unique,
                               CONSTRAINT fk_facturas_comp FOREIGN KEY (id_compra) REFERENCES finanzas.compras(id_compra));


CREATE TABLE finanzas.detalle_compras(id_compra INTEGER NOT NULL,
                                      id_producto INTEGER NOT NULL,
                                      cantidad INTEGER NOT NULL CHECK (cantidad > 0),
                                      precio_unitario DECIMAL(12,2) NOT NULL CHECK (precio_unitario >= 0),
                                      CONSTRAINT pk_detalle_compra PRIMARY KEY (id_compra, id_producto),
                                      CONSTRAINT fk_detalle_compra_compra FOREIGN KEY (id_compra) REFERENCES finanzas.compras(id_compra),
                                      CONSTRAINT fk_detalle_compra_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto));

CREATE TABLE finanzas.notas_credito_debito(id_nota SERIAL PRIMARY KEY,
                                           id_proveedor INTEGER NOT NULL,
                                           tipo_nota VARCHAR(10) CHECK (tipo_nota IN ('Credito', 'Debito')),
                                           nro_comprobante VARCHAR(20) NOT NULL,
                                           fecha DATE NOT NULL,
                                           motivo TEXT,
                                           monto DECIMAL(12,2) NOT NULL CHECK (monto > 0),
                                           archivo_adjunto VARCHAR(255),
                                           CONSTRAINT fk_nota_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor));

CREATE TABLE finanzas.ordenes_pago(id_orden SERIAL PRIMARY KEY,
                                   fecha_emision DATE NOT NULL DEFAULT CURRENT_DATE,
                                   monto_total DECIMAL(12,2) NOT NULL CHECK (monto_total > 0),
                                   archivo_pdf VARCHAR(255),
                                   id_autoriza INTEGER NOT NULL,
                                   id_usuario INTEGER NOT NULL,
                                   id_proveedor INTEGER NOT NULL,
                                   CONSTRAINT fk_ordenes_pago_prov1 FOREIGN KEY (id_autoriza) REFERENCES seguridad.usuarios(id_usuario),
                                   CONSTRAINT fk_ordenes_pago_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario),
                                   CONSTRAINT fk_ordenes_pago_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor));

CREATE TABLE finanzas.detalle_ordenes_pago(id_orden INTEGER NOT NULL,
                                           id_compra INTEGER NOT NULL, 
                                           monto_asignado DECIMAL(12,2) NOT NULL CHECK (monto_asignado > 0),
                                           CONSTRAINT pk_detalles_ordenes_pago PRIMARY KEY (id_orden, id_compra),
                                           CONSTRAINT fk_detalles_ordenes_pago_orden FOREIGN KEY (id_orden) REFERENCES finanzas.ordenes_pago(id_orden),
                                           CONSTRAINT fk_detalles_ordenes_pago_compra FOREIGN KEY (id_compra) REFERENCES finanzas.compras(id_compra));

CREATE TABLE finanzas.metodos_pago_orden(id_metodo SERIAL PRIMARY KEY,
                                         tipo_metodo VARCHAR(30) CHECK (tipo_metodo IN ('Efectivo', 'Transferencia', 'Cheque', 'Saldo a Favor')),
                                         monto DECIMAL(12,2) NOT NULL CHECK (monto > 0),
                                         referencia VARCHAR(100), 
                                         id_orden INTEGER NOT NULL,
                                         CONSTRAINT fk_metodos_pago_orden_op FOREIGN KEY (id_orden) REFERENCES finanzas.ordenes_pago(id_orden));

CREATE TABLE finanzas.movimientos_cc(id_movimiento SERIAL PRIMARY KEY,
                                     fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                     tipo_movimiento VARCHAR(30) CHECK (tipo_movimiento IN ('Saldo Inicial', 'Compra', 'Pago', 'Nota Credito', 'Nota Debito')),
                                     monto DECIMAL(12,2) NOT NULL,

                                     id_compra INTEGER NULL,
                                     id_proveedor INTEGER NOT NULL,
                                     id_orden INTEGER NULL,
                                     id_nota INTEGER NULL,
                                     CONSTRAINT fk_movimientos_cc_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor),
                                     CONSTRAINT fk_movimientos_cc_compra FOREIGN KEY (id_compra) REFERENCES finanzas.compras(id_compra),
                                     CONSTRAINT fk_movimientos_cc_orden FOREIGN KEY (id_orden) REFERENCES finanzas.ordenes_pago(id_orden),
                                     CONSTRAINT fk_movimientos_cc_nota FOREIGN KEY (id_nota) REFERENCES finanzas.notas_credito_debito(id_nota),

                                     CONSTRAINT chk_arco_exclusivo CHECK((id_compra IS NOT NULL)::INT + 
                                                                         (id_orden IS NOT NULL)::INT + 
                                                                         (id_nota IS NOT NULL)::INT <= 1));


CREATE TABLE auditoria.bitacora(id_log SERIAL PRIMARY KEY,
                                id_usuario INTEGER, 
                                nombre_empleado VARCHAR(200) NOT NULL,
                                fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                modulo VARCHAR(50) NOT NULL,
                                accion TEXT NOT NULL,
                                valor_anterior JSONB,
                                valor_nuevo JSONB,
                                CONSTRAINT fk_audit_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario));





CREATE OR REPLACE FUNCTION finanzas.fn_dashboard_totales(p_fecha_desde DATE, p_fecha_hasta DATE)
RETURNS TABLE (total_comprado DECIMAL, total_pagado DECIMAL, deuda_actual DECIMAL) AS $$
BEGIN
    RETURN QUERY 
    SELECT 
        COALESCE((SELECT SUM(monto_total) FROM finanzas.compras WHERE fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta), 0) AS total_comprado,
        COALESCE((SELECT SUM(monto_total) FROM finanzas.ordenes_pago WHERE fecha_emision BETWEEN p_fecha_desde AND p_fecha_hasta), 0) AS total_pagado,
        COALESCE((SELECT SUM(monto) FROM finanzas.movimientos_cc), 0) AS deuda_actual;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION finanzas.fn_ranking_productos(p_fecha_desde DATE, p_fecha_hasta DATE, p_limite INT)
RETURNS TABLE (id_producto INT, nombre VARCHAR, cantidad_comprada BIGINT) AS $$
BEGIN
    RETURN QUERY 
    SELECT p.id_producto, p.nombre, SUM(dc.cantidad) AS cantidad_comprada
    FROM finanzas.detalle_compras dc
    JOIN finanzas.compras c ON dc.id_compra = c.id_compra
    JOIN logistica.productos p ON dc.id_producto = p.id_producto
    WHERE c.fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta
    GROUP BY p.id_producto, p.nombre
    ORDER BY cantidad_comprada DESC
    LIMIT p_limite;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION finanzas.fn_compras_por_tipo_factura(p_fecha_desde DATE, p_fecha_hasta DATE)
RETURNS TABLE (tipo_comprobante VARCHAR, cantidad BIGINT) AS $$
BEGIN
    RETURN QUERY 
    SELECT c.tipo_comprobante, COUNT(c.id_compra) AS cantidad
    FROM finanzas.compras c
    WHERE c.fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta
    GROUP BY c.tipo_comprobante
    ORDER BY cantidad DESC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION finanzas.fn_volumen_compras_mensual(p_fecha_desde DATE, p_fecha_hasta DATE)
RETURNS TABLE (mes_anio VARCHAR, total DECIMAL) AS $$
BEGIN
    RETURN QUERY 
    SELECT 
        TO_CHAR(c.fecha_compra, 'YYYY-MM')::VARCHAR AS mes_anio,
        SUM(c.monto_total) AS total
    FROM finanzas.compras c
    WHERE c.fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta
    GROUP BY TO_CHAR(c.fecha_compra, 'YYYY-MM')
    ORDER BY mes_anio ASC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION finanzas.fn_ranking_proveedores(p_fecha_desde DATE, p_fecha_hasta DATE, p_limite INT)
RETURNS TABLE (id_proveedor INT, razon_social VARCHAR, cantidad_operaciones BIGINT) AS $$
BEGIN
    RETURN QUERY 
    SELECT p.id_proveedor, p.razon_social, COUNT(c.id_compra) AS cantidad_operaciones
    FROM finanzas.compras c
    JOIN seguridad.proveedores p ON c.id_proveedor = p.id_proveedor
    WHERE c.fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta
    GROUP BY p.id_proveedor, p.razon_social
    ORDER BY cantidad_operaciones DESC
    LIMIT p_limite;
END;
$$ LANGUAGE plpgsql;