USE 5to_Pizzeria;


-- ==========================================
-- 1. SUCURSALES
-- ==========================================

INSERT INTO Sucursal
(direccion, telefono, activa)
VALUES
('Av. Corrientes 1234', '011-4567-8901', TRUE),
('Av. Cabildo 2450', '011-4567-8902', TRUE),
('Av. Rivadavia 5600', '011-4567-8903', TRUE);


-- ==========================================
-- 2. USUARIOS
-- ==========================================

INSERT INTO Usuario
(usuario, nombre, apellido, email, passwordHash, telefono, direccion)
VALUES

-- CLIENTES (ids 1 a 3)
('andres_torrico', 'Andrés', 'Torrico', 'andres@gmail.com', 'HASH_CLIENTE_1', '1198765432', 'CABA'),
('misael_gomez', 'Misael', 'Gómez', 'misael@gmail.com', 'HASH_CLIENTE_2', '1123456789', 'Av. Siempre Viva 742'),
('lucia_fernandez', 'Lucía', 'Fernández', 'lucia@gmail.com', 'HASH_CLIENTE_3', '1167891234', 'Av. Santa Fe 1500'),

-- ADMINISTRADOR (id 4)
('admin', 'Andrés', 'Torrico', 'admin@pizzeria.com', 'HASH_ADMIN', '1122334455', NULL),

-- REPARTIDORES (ids 5 y 6)
('juan_martinez', 'Juan', 'Martínez', 'juan.repartidor@pizzeria.com', 'HASH_REPARTIDOR_1', '1155553333', NULL),
('pedro_garcia', 'Pedro', 'García', 'pedro.repartidor@pizzeria.com', 'HASH_REPARTIDOR_2', '1155554444', NULL),

-- CLIENTE (id 7)
('tyson', 'Tyson', 'Tyson', 'tyson@gmail.com', 'lsE385FghIetMEBIBHOgc7coGnjNFiifNsfcTWQRyD4U3wdVw', '1100000000', NULL);


-- ==========================================
-- 3. EMPLEADOS
-- ==========================================

-- Roles:
-- 1 = Administrador
-- 2 = Repartidor

INSERT INTO Empleado
(idUsuario, idSucursal, rol)
VALUES
(4, 1, 1),   -- idEmpleado 1: Admin - Sucursal 1
(5, 1, 2),   -- idEmpleado 2: Juan (repartidor) - Sucursal 1
(6, 3, 2);   -- idEmpleado 3: Pedro (repartidor) - Sucursal 3


-- ==========================================
-- 4. PIZZAS
-- ==========================================

INSERT INTO Pizza
(nombre, descripcion, precio, disponible)
VALUES
('Muzzarella Tradicional', 'Salsa de tomate, abundante muzzarella y aceitunas verdes', 7500.00, TRUE),
('Fugazzeta Especial', 'Cebolla fileteada, muzzarella, provolone y un toque de orégano', 8800.00, TRUE),
('Napolitana con Ajo', 'Salsa, muzzarella, rodajas de tomate fresco y provenzal', 9200.00, TRUE),
('Calabresa Picante', 'Salsa de tomate, muzzarella y rodajas de longaniza calabresa', 9800.00, TRUE),
('Cuatro Quesos', 'Muzzarella, provolone, roquefort y parmesano fundidos', 10500.00, TRUE);


-- ==========================================
-- 5. PEDIDOS DE PRUEBA
-- ==========================================

-- Pedido 1: Andrés, sucursal Corrientes, delivery, repartidor Juan (idEmpleado = 2)
INSERT INTO Pedido
(idUsuario, idSucursal, idRepartidor, fechaHora,
 estado, tipoEntrega, direccionEntrega, total)
VALUES
(1, 1, 2, NOW(), 4, 2, 'CABA', 25500.00);

INSERT INTO DetallePedido
(idPedido, idPizza, cantidad, precioUnitario)
VALUES
(1, 1, 1, 7500.00),
(1, 3, 1, 9200.00),
(1, 2, 1, 8800.00);


-- Pedido 2: Misael, sucursal Cabildo, retiro en local, sin repartidor
INSERT INTO Pedido
(idUsuario, idSucursal, idRepartidor, fechaHora,
 estado, tipoEntrega, direccionEntrega, total)
VALUES
(2, 2, NULL, NOW(), 3, 1, NULL, 16300.00);

INSERT INTO DetallePedido
(idPedido, idPizza, cantidad, precioUnitario)
VALUES
(2, 1, 1, 7500.00),
(2, 2, 1, 8800.00);


-- Pedido 3: Lucía, sucursal Rivadavia, delivery, repartidor Pedro (idEmpleado = 3)
INSERT INTO Pedido
(idUsuario, idSucursal, idRepartidor, fechaHora,
 estado, tipoEntrega, direccionEntrega, total)
VALUES
(3, 3, 3, NOW(), 4, 2, 'Av. Santa Fe 1500', 17300.00);

INSERT INTO DetallePedido
(idPedido, idPizza, cantidad, precioUnitario)
VALUES
(3, 4, 1, 9800.00),
(3, 1, 1, 7500.00);