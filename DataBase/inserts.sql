USE bd_Pizzeria;

-- ==========================================
-- 1. CARGAR EL MENÚ DE PIZZAS
-- ==========================================
INSERT INTO Pizza (nombre, descripcion, precio, disponible) VALUES 
('Muzzarella Tradicional', 'Salsa de tomate, abundante muzzarella y aceitunas verdes', 7500.00, true),
('Fugazzeta Especial', 'Cebolla fileteada, muzzarella, provolone y un toque de orégano', 8800.00, true),
('Napolitana con Ajo', 'Salsa, muzzarella, rodajas de tomate fresco y provenzal', 9200.00, true),
('Calabresa Picante', 'Salsa de tomate, muzzarella y rodajas de longaniza calabresa', 9800.00, true),
('Cuatro Quesos', 'Muzzarella, provolone, roquefort y parmesano fundidos', 10500.00, true);

-- ==========================================
-- 2. CARGAR CLIENTES DE PRUEBA (Opcional pero útil)
-- ==========================================
INSERT INTO Cliente (nombre, telefono, direccion) VALUES 
('Misael', '1123456789', 'Av. Siempre Viva 742'),
('Andres', '1198765432', 'CABA');
