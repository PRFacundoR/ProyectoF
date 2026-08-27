
CREATE SCHEMA IF NOT EXISTS seguridad;
CREATE SCHEMA IF NOT EXISTS logistica;        
CREATE SCHEMA IF NOT EXISTS compras_finanzas; 
CREATE SCHEMA IF NOT EXISTS auditoria;


CREATE TABLE seguridad.roles (
    id_rol SERIAL PRIMARY KEY,
    nombre VARCHAR(50) UNIQUE NOT NULL,
    descripcion TEXT
);

CREATE TABLE seguridad.empleados (
    id_empleado SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    dni INTEGER UNIQUE NOT NULL,
    turno VARCHAR(20) CHECK (turno IN ('Mañana', 'Tarde', 'Noche', 'Jornada Completa'))
);

CREATE TABLE seguridad.usuarios (
    id_usuario SERIAL PRIMARY KEY,
    id_rol INT NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL, 
    is_activo BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_user_emp FOREIGN KEY (id_usuario) REFERENCES seguridad.empleados(id_empleado),
    CONSTRAINT fk_user_rol FOREIGN KEY (id_rol) REFERENCES seguridad.roles(id_rol)
);

CREATE TABLE seguridad.recuperacion_passwords (
    id_recuperacion SERIAL PRIMARY KEY,
    id_usuario INT NOT NULL,
    codigo_token VARCHAR(6) NOT NULL, 
    fecha_generacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion TIMESTAMP NOT NULL, 
    is_usado BOOLEAN DEFAULT FALSE, 
    CONSTRAINT fk_recup_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario)
);

CREATE TABLE seguridad.tareas (
    id_tarea SERIAL PRIMARY KEY,
    id_empleado INT NOT NULL,
    descripcion TEXT NOT NULL,
    fecha_plazo DATE NOT NULL,
    estado VARCHAR(20) DEFAULT 'Incompleta' CHECK (estado IN ('Incompleta', 'Completada')),
    CONSTRAINT fk_tarea_emp FOREIGN KEY (id_empleado) REFERENCES seguridad.empleados(id_empleado)
);

CREATE TABLE seguridad.proveedores (
    id_proveedor SERIAL PRIMARY KEY,
    razon_social VARCHAR(150) NOT NULL,
    cuit VARCHAR(15) UNIQUE NOT NULL,
    domicilio VARCHAR(200),
    telefono VARCHAR(50),
    email VARCHAR(100),
    dias_plazo_pago INT DEFAULT 0 CHECK (dias_plazo_pago >= 0),
    is_activo BOOLEAN DEFAULT TRUE
);

CREATE TABLE seguridad.permisos (
    id_permiso SERIAL PRIMARY KEY,
    nombre_permiso VARCHAR(100) UNIQUE NOT NULL, 
    descripcion TEXT
);

CREATE TABLE seguridad.roles_permisos (
    id_rol INT NOT NULL,
    id_permiso INT NOT NULL,
    PRIMARY KEY (id_rol, id_permiso),
    CONSTRAINT fk_rp_rol FOREIGN KEY (id_rol) REFERENCES seguridad.roles(id_rol) ON DELETE CASCADE,
    CONSTRAINT fk_rp_permiso FOREIGN KEY (id_permiso) REFERENCES seguridad.permisos(id_permiso) ON DELETE CASCADE
);

INSERT INTO seguridad.permisos (nombre_permiso, descripcion) VALUES
('VerDashboard', 'Permite visualizar los gráficos y estadísticas'),
('RegistrarCompra', 'Permite ingresar nuevas facturas de compras'),
('GestionarUsuarios', 'Permite crear o modificar empleados y roles');

CREATE TABLE logistica.productos (
    id_producto SERIAL PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    codigo_barras VARCHAR(50) UNIQUE,
    stock_minimo INT DEFAULT 0 CHECK (stock_minimo >= 0),
    categorias VARCHAR(100)[],
    is_activo BOOLEAN DEFAULT TRUE
);

CREATE TABLE logistica.depositos (
    id_deposito SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    direccion VARCHAR(200),
    is_activo BOOLEAN DEFAULT TRUE
);

CREATE TABLE logistica.ubicaciones (
    id_ubicacion SERIAL PRIMARY KEY,
    id_deposito INT NOT NULL,
    sector VARCHAR(50) NOT NULL,
    estanteria VARCHAR(50) NOT NULL,
    is_activo BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_ubicacion_deposito FOREIGN KEY (id_deposito) REFERENCES logistica.depositos(id_deposito)
);

CREATE TABLE logistica.stock_ubicacion (
    id_producto INT NOT NULL,
    id_ubicacion INT NOT NULL,
    cantidad INT DEFAULT 0 CHECK (cantidad >= 0),
    PRIMARY KEY (id_producto, id_ubicacion),
    CONSTRAINT fk_stock_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
    CONSTRAINT fk_stock_ubic FOREIGN KEY (id_ubicacion) REFERENCES logistica.ubicaciones(id_ubicacion)
);



CREATE TABLE compras_finanzas.productos_proveedor (
    id_producto INT NOT NULL,
    id_proveedor INT NOT NULL,
    precio_costo DECIMAL(12,2) NOT NULL CHECK (precio_costo >= 0),
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_producto, id_proveedor),
    CONSTRAINT fk_pp_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
    CONSTRAINT fk_pp_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor)
);

CREATE TABLE compras_finanzas.compras (
    id_compra SERIAL PRIMARY KEY,
    id_proveedor INT NOT NULL,
    id_usuario INT NOT NULL, 
    tipo_comprobante VARCHAR(20) CHECK (tipo_comprobante IN ('Factura A', 'Factura B', 'Factura C')),
    nro_comprobante VARCHAR(20) NOT NULL,
    fecha_compra DATE NOT NULL,
    condicion_pago VARCHAR(20) CHECK (condicion_pago IN ('Contado', 'Cuenta Corriente')),
    plazo_dias INT DEFAULT 0, 
    fecha_vencimiento DATE,   
    archivo_adjunto VARCHAR(255),
    estado VARCHAR(20) DEFAULT 'Impaga' CHECK (estado IN ('Impaga', 'Pagada Parcial', 'Pagada')),
    monto_total DECIMAL(12,2) NOT NULL CHECK (monto_total >= 0),
    CONSTRAINT fk_compra_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor),
    CONSTRAINT fk_compra_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario)
);

CREATE TABLE compras_finanzas.detalle_compras (
    id_detalle SERIAL PRIMARY KEY,
    id_compra INT NOT NULL,
    id_producto INT NOT NULL,
    id_ubicacion_destino INT, 
    cantidad INT NOT NULL CHECK (cantidad > 0),
    precio_unitario DECIMAL(12,2) NOT NULL CHECK (precio_unitario >= 0),
    CONSTRAINT fk_det_compra FOREIGN KEY (id_compra) REFERENCES compras_finanzas.compras(id_compra),
    CONSTRAINT fk_det_prod FOREIGN KEY (id_producto) REFERENCES logistica.productos(id_producto),
    CONSTRAINT fk_det_ubic FOREIGN KEY (id_ubicacion_destino) REFERENCES logistica.ubicaciones(id_ubicacion)
);

CREATE TABLE compras_finanzas.notas_credito_debito (
    id_nota SERIAL PRIMARY KEY,
    id_proveedor INT NOT NULL,
    tipo_nota VARCHAR(10) CHECK (tipo_nota IN ('Credito', 'Debito')),
    nro_comprobante VARCHAR(20) NOT NULL,
    fecha DATE NOT NULL,
    motivo TEXT,
    monto DECIMAL(12,2) NOT NULL CHECK (monto > 0),
    archivo_adjunto VARCHAR(255),
    CONSTRAINT fk_nota_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor)
);

CREATE TABLE compras_finanzas.ordenes_pago (
    id_orden SERIAL PRIMARY KEY,
    id_proveedor INT NOT NULL,
    id_usuario INT NOT NULL,
    fecha_emision DATE NOT NULL DEFAULT CURRENT_DATE,
    monto_total DECIMAL(12,2) NOT NULL CHECK (monto_total > 0),
    archivo_pdf VARCHAR(255),
    CONSTRAINT fk_op_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor),
    CONSTRAINT fk_op_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario)
);

CREATE TABLE compras_finanzas.detalle_ordenes_pago (
    id_orden INT NOT NULL,
    id_compra INT NOT NULL, 
    monto_asignado DECIMAL(12,2) NOT NULL CHECK (monto_asignado > 0),
    PRIMARY KEY (id_orden, id_compra), 
    CONSTRAINT fk_dop_orden FOREIGN KEY (id_orden) REFERENCES compras_finanzas.ordenes_pago(id_orden),
    CONSTRAINT fk_dop_compra FOREIGN KEY (id_compra) REFERENCES compras_finanzas.compras(id_compra)
);

CREATE TABLE compras_finanzas.metodos_pago_orden (
    id_metodo SERIAL PRIMARY KEY,
    id_orden INT NOT NULL,
    tipo_metodo VARCHAR(30) CHECK (tipo_metodo IN ('Efectivo', 'Transferencia', 'Cheque', 'Saldo a Favor')),
    monto DECIMAL(12,2) NOT NULL CHECK (monto > 0),
    referencia VARCHAR(100), 
    CONSTRAINT fk_mpo_orden FOREIGN KEY (id_orden) REFERENCES compras_finanzas.ordenes_pago(id_orden)
);

CREATE TABLE compras_finanzas.movimientos_cc (
    id_movimiento SERIAL PRIMARY KEY,
    id_proveedor INT NOT NULL,
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    tipo_movimiento VARCHAR(30) CHECK (tipo_movimiento IN ('Saldo Inicial', 'Compra', 'Pago', 'Nota Credito', 'Nota Debito')),
    monto DECIMAL(12,2) NOT NULL, 
    id_compra INT NULL,
    id_orden INT NULL,
    id_nota INT NULL,

    CONSTRAINT fk_mcc_prov FOREIGN KEY (id_proveedor) REFERENCES seguridad.proveedores(id_proveedor),
    CONSTRAINT fk_mcc_compra FOREIGN KEY (id_compra) REFERENCES compras_finanzas.compras(id_compra),
    CONSTRAINT fk_mcc_orden FOREIGN KEY (id_orden) REFERENCES compras_finanzas.ordenes_pago(id_orden),
    CONSTRAINT fk_mcc_nota FOREIGN KEY (id_nota) REFERENCES compras_finanzas.notas_credito_debito(id_nota),

    CONSTRAINT chk_arco_exclusivo CHECK (
        (id_compra IS NOT NULL)::INT + 
        (id_orden IS NOT NULL)::INT + 
        (id_nota IS NOT NULL)::INT <= 1
    )
);



CREATE TABLE auditoria.bitacora (
    id_log SERIAL PRIMARY KEY,
    id_usuario INT, 
    nombre_empleado VARCHAR(200) NOT NULL, 
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modulo VARCHAR(50) NOT NULL,
    accion TEXT NOT NULL,         
    valor_anterior JSONB,          
    valor_nuevo JSONB,             
    CONSTRAINT fk_audit_usr FOREIGN KEY (id_usuario) REFERENCES seguridad.usuarios(id_usuario)
);








CREATE OR REPLACE FUNCTION compras_finanzas.fn_dashboard_totales(p_fecha_desde DATE, p_fecha_hasta DATE)
RETURNS TABLE (total_comprado DECIMAL, total_pagado DECIMAL, deuda_actual DECIMAL) AS $$
BEGIN
    RETURN QUERY 
    SELECT 
        COALESCE((SELECT SUM(monto_total) FROM compras_finanzas.compras WHERE fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta), 0) AS total_comprado,
        COALESCE((SELECT SUM(monto_total) FROM compras_finanzas.ordenes_pago WHERE fecha_emision BETWEEN p_fecha_desde AND p_fecha_hasta), 0) AS total_pagado,
        COALESCE((SELECT SUM(monto) FROM compras_finanzas.movimientos_cc), 0) AS deuda_actual;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION compras_finanzas.fn_ranking_productos(p_fecha_desde DATE, p_fecha_hasta DATE, p_limite INT)
RETURNS TABLE (id_producto INT, nombre VARCHAR, cantidad_comprada BIGINT) AS $$
BEGIN
    RETURN QUERY 
    SELECT p.id_producto, p.nombre, SUM(dc.cantidad) AS cantidad_comprada
    FROM compras_finanzas.detalle_compras dc
    JOIN compras_finanzas.compras c ON dc.id_compra = c.id_compra
    JOIN logistica.productos p ON dc.id_producto = p.id_producto
    WHERE c.fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta
    GROUP BY p.id_producto, p.nombre
    ORDER BY cantidad_comprada DESC
    LIMIT p_limite;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION compras_finanzas.fn_compras_por_tipo_factura(p_fecha_desde DATE, p_fecha_hasta DATE)
RETURNS TABLE (tipo_comprobante VARCHAR, cantidad BIGINT) AS $$
BEGIN
    RETURN QUERY 
    SELECT c.tipo_comprobante, COUNT(c.id_compra) AS cantidad
    FROM compras_finanzas.compras c
    WHERE c.fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta
    GROUP BY c.tipo_comprobante
    ORDER BY cantidad DESC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION compras_finanzas.fn_volumen_compras_mensual(p_fecha_desde DATE, p_fecha_hasta DATE)
RETURNS TABLE (mes_anio VARCHAR, total DECIMAL) AS $$
BEGIN
    RETURN QUERY 
    SELECT 
        TO_CHAR(c.fecha_compra, 'YYYY-MM')::VARCHAR AS mes_anio,
        SUM(c.monto_total) AS total
    FROM compras_finanzas.compras c
    WHERE c.fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta
    GROUP BY TO_CHAR(c.fecha_compra, 'YYYY-MM')
    ORDER BY mes_anio ASC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION compras_finanzas.fn_ranking_proveedores(p_fecha_desde DATE, p_fecha_hasta DATE, p_limite INT)
RETURNS TABLE (id_proveedor INT, razon_social VARCHAR, cantidad_operaciones BIGINT) AS $$
BEGIN
    RETURN QUERY 
    SELECT p.id_proveedor, p.razon_social, COUNT(c.id_compra) AS cantidad_operaciones
    FROM compras_finanzas.compras c
    JOIN seguridad.proveedores p ON c.id_proveedor = p.id_proveedor
    WHERE c.fecha_compra BETWEEN p_fecha_desde AND p_fecha_hasta
    GROUP BY p.id_proveedor, p.razon_social
    ORDER BY cantidad_operaciones DESC
    LIMIT p_limite;
END;
$$ LANGUAGE plpgsql;