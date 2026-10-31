DROP DATABASE IF EXISTS 5to_Pizzeria;
CREATE DATABASE 5to_Pizzeria;
USE 5to_Pizzeria;


-- ==========================================
-- 1. SUCURSALES
-- ==========================================

CREATE TABLE Sucursal(
    idSucursal INT NOT NULL AUTO_INCREMENT,
    direccion VARCHAR(150) NOT NULL,
    telefono VARCHAR(20),
    activa BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT PK_Sucursal PRIMARY KEY (idSucursal)
);


-- ==========================================
-- 2. USUARIOS
-- ==========================================

CREATE TABLE Usuario(
    idUsuario INT NOT NULL AUTO_INCREMENT,
    usuario VARCHAR(50) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL,
    passwordHash VARCHAR(255) NOT NULL,
    telefono VARCHAR(20),
    direccion VARCHAR(150),

    CONSTRAINT PK_Usuario PRIMARY KEY (idUsuario),
    CONSTRAINT UQ_Usuario_Email UNIQUE (email)
);


-- ==========================================
-- 3. EMPLEADOS
-- ==========================================

CREATE TABLE Empleado(
    idEmpleado INT NOT NULL AUTO_INCREMENT,
    idUsuario INT NOT NULL,
    idSucursal INT NOT NULL,
    rol INT NOT NULL,

    CONSTRAINT PK_Empleado PRIMARY KEY (idEmpleado),

    CONSTRAINT UQ_Empleado_Usuario
        UNIQUE (idUsuario),

    CONSTRAINT FK_Empleado_Usuario
        FOREIGN KEY (idUsuario)
        REFERENCES Usuario(idUsuario),

    CONSTRAINT FK_Empleado_Sucursal
        FOREIGN KEY (idSucursal)
        REFERENCES Sucursal(idSucursal)
);


-- ==========================================
-- 4. PIZZAS
-- ==========================================

CREATE TABLE Pizza(
    idPizza INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(255),
    precio DECIMAL(10,2) NOT NULL,
    disponible BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT PK_Pizza PRIMARY KEY (idPizza)
);


-- ==========================================
-- 5. PROMOCIONES
-- ==========================================

CREATE TABLE Promocion(
    idPromocion INT NOT NULL AUTO_INCREMENT,
    idPizza INT NOT NULL,
    titulo VARCHAR(100) NOT NULL,
    tipo INT NOT NULL,                      -- 1 = Porcentaje, 2 = Monto fijo por pizza
    valor DECIMAL(10,2) NOT NULL,           -- 20 (%) o 1000 ($)
    cantidadMinima INT NOT NULL DEFAULT 1,  -- desde cuántas pizzas aplica
    fechaInicio DATETIME NOT NULL,
    fechaFin DATETIME NOT NULL,
    activa BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT PK_Promocion PRIMARY KEY (idPromocion),

    CONSTRAINT FK_Promocion_Pizza
        FOREIGN KEY (idPizza)
        REFERENCES Pizza(idPizza),

    CONSTRAINT CK_Promocion_Fechas
        CHECK (fechaFin > fechaInicio),

    CONSTRAINT CK_Promocion_CantMin
        CHECK (cantidadMinima >= 1),

    CONSTRAINT CK_Promocion_Valor CHECK (
        (tipo = 1 AND valor > 0 AND valor <= 100)
        OR (tipo = 2 AND valor > 0)
    )
);


-- ==========================================
-- 6. PEDIDOS
-- ==========================================

CREATE TABLE Pedido(
    idPedido INT NOT NULL AUTO_INCREMENT,
    idUsuario INT NOT NULL,
    idSucursal INT NOT NULL,
    idRepartidor INT NULL,
    fechaHora DATETIME NOT NULL,
    estado INT NOT NULL,
    tipoEntrega INT NOT NULL,
    direccionEntrega VARCHAR(150) NULL,
    total DECIMAL(10,2) NOT NULL,

    CONSTRAINT PK_Pedido PRIMARY KEY (idPedido),

    CONSTRAINT FK_Pedido_Usuario
        FOREIGN KEY (idUsuario)
        REFERENCES Usuario(idUsuario),

    CONSTRAINT FK_Pedido_Sucursal
        FOREIGN KEY (idSucursal)
        REFERENCES Sucursal(idSucursal),

    CONSTRAINT FK_Pedido_Repartidor
        FOREIGN KEY (idRepartidor)
        REFERENCES Empleado(idEmpleado)
);


-- ==========================================
-- 7. DETALLE DEL PEDIDO
-- ==========================================

CREATE TABLE DetallePedido(
    idPedido INT NOT NULL,
    idPizza INT NOT NULL,
    cantidad INT NOT NULL,
    precioUnitario DECIMAL(10,2) NOT NULL,       -- precio original
    descuento DECIMAL(10,2) NOT NULL DEFAULT 0,  -- descuento total de la línea
    idPromocion INT NULL,                        -- promo aplicada (historial)

    CONSTRAINT PK_DetallePedido
        PRIMARY KEY (idPedido, idPizza),

    CONSTRAINT FK_DetallePedido_Pedido
        FOREIGN KEY (idPedido)
        REFERENCES Pedido(idPedido),

    CONSTRAINT FK_DetallePedido_Pizza
        FOREIGN KEY (idPizza)
        REFERENCES Pizza(idPizza),

    CONSTRAINT FK_DetallePedido_Promocion
        FOREIGN KEY (idPromocion)
        REFERENCES Promocion(idPromocion)
);