ALTER TABLE ModuloTienda.Productos
ALTER COLUMN porComision SMALLINT
ALTER TABLE ModuloTienda.Productos
ADD COLUMN activo BIT NOT NULL DEFAULT 1 AFTER porComision;