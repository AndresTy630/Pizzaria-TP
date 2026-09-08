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
(nombre, apellido, email, passwordHash, telefono, direccion)
VALUES

-- CLIENTES
('Andrés', 'Torrico', 'andres@gmail.com', 'HASH_CLIENTE_1', '1198765432', 'CABA'),

('Misael', 'Gómez', 'misael@gmail.com', 'HASH_CLIENTE_2', '1123456789', 'Av. Siempre Viva 742'),

('Lucía', 'Fernández', 'lucia@gmail.com', 'HASH_CLIENTE_3', '1167891234', 'Av. Santa Fe 1500'),


-- ADMINISTRADOR
('Andrés', 'Torrico', 'admin@pizzeria.com', 'HASH_ADMIN', '1122334455', NULL),


-- CHEFS
('Carlos', 'Pérez', 'carlos.chef@pizzeria.com', 'HASH_CHEF_1', '1155551111', NULL),

('Sofía', 'Rodríguez', 'sofia.chef@pizzeria.com', 'HASH_CHEF_2', '1155552222', NULL),


-- REPARTIDORES
('Juan', 'Martínez', 'juan.repartidor@pizzeria.com', 'HASH_REPARTIDOR_1', '1155553333', NULL),

('Pedro', 'García', 'pedro.repartidor@pizzeria.com', 'HASH_REPARTIDOR_2', '1155554444', NULL);


-- ==========================================
-- 3. EMPLEADOS
-- ==========================================

-- Roles:
-- 1 = Administrador
-- 2 = Chef
-- 3 = Repartidor

INSERT INTO Empleado
(idUsuario, idSucursal, rol)
VALUES

-- Andrés - Administrador - Sucursal 1
(4, 1, 1),

-- Carlos - Chef - Sucursal 1
(5, 1, 2),

-- Sofía - Chef - Sucursal 2
(6, 2, 2),

-- Juan - Repartidor - Sucursal 1
(7, 1, 3),

-- Pedro - Repartidor - Sucursal 3
(8, 3, 3);


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

-- Pedido 1
-- Cliente: Andrés (idUsuario = 1)
-- Sucursal: Corrientes (idSucursal = 1)
-- Envío
-- Repartidor: Juan (idEmpleado = 4)

INSERT INTO Pedido
(idUsuario, idSucursal, idRepartidor, fechaHora,
 estado, tipoEntrega, direccionEntrega, total)
VALUES
(1, 1, 4, NOW(), 4, 2, 'CABA', 25500.00);


-- Detalle del Pedido 1
INSERT INTO DetallePedido
(idPedido, idPizza, cantidad, precioUnitario)
VALUES
(1, 1, 1, 7500.00),
(1, 3, 1, 9200.00),
(1, 2, 1, 8800.00);


-- Pedido 2
-- Cliente: Misael (idUsuario = 2)
-- Sucursal: Cabildo (idSucursal = 2)
-- Retiro en pizzería
-- Sin repartidor

INSERT INTO Pedido
(idUsuario, idSucursal, idRepartidor, fechaHora,
 estado, tipoEntrega, direccionEntrega, total)
VALUES
(2, 2, NULL, NOW(), 3, 1, NULL, 16300.00);


-- Detalle del Pedido 2
INSERT INTO DetallePedido
(idPedido, idPizza, cantidad, precioUnitario)
VALUES
(2, 1, 1, 7500.00),
(2, 2, 1, 8800.00);


-- Pedido 3
-- Cliente: Lucía (idUsuario = 3)
-- Sucursal: Rivadavia (idSucursal = 3)
-- Envío
-- Repartidor: Pedro (idEmpleado = 5)

INSERT INTO Pedido
(idUsuario, idSucursal, idRepartidor, fechaHora,
 estado, tipoEntrega, direccionEntrega, total)
VALUES
(3, 3, 5, NOW(), 4, 2, 'Av. Santa Fe 1500', 17300.00);


-- Detalle del Pedido 3
INSERT INTO DetallePedido
(idPedido, idPizza, cantidad, precioUnitario)
VALUES
(3, 4, 1, 9800.00),
(3, 1, 1, 7500.00);
