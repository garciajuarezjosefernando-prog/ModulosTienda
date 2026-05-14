
CREATE TABLE [dbo].[Usuarios](
	[usuario] [varchar](50) NOT NULL,
	[nombre] [varchar](300) NULL,
	[contrasena] [varchar](10) NULL,
	[correo] [varchar](50) NULL,
	[tipo] [varchar](2) NULL,
	[activo] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
-- Los tipos de ousuario segun lo hablado en con moi serian A
-- Admin AD
-- Master MA
-- Vendedor VE

