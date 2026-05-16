USE [C:\DAT\BD\ANIBD.MDF]
GO

/****** Object:  Table [dbo].[Clientes]    Script Date: 15/05/2026 08:34:32 p. m. ******/
SET ANSI_NULLS ON
GO


CREATE TABLE [dbo].[Clientes](
	[idCliente] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](150) NULL,
	[RFC] [nvarchar](13) NOT NULL,
	[tipoCliente] [varchar](7) NULL,
	[telefono] [nvarchar](50) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[direccion] [nvarchar](250) NOT NULL,
	[ciudad] [nvarchar](100) NOT NULL,
	[estado] [nvarchar](100) NOT NULL,
	[cp] [nvarchar](10) NOT NULL,
	[pais] [nvarchar](100) NOT NULL,
	[fechaRegistro] [datetime] NULL,
	[activo] [bit] NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[idCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--ID CLIENTES ES IDENTIDAD SE VA A COLOCAR AUTOMATICAMENTE
--TIPOCLIENTE PUEDE SER PERSONA O EMPRESA 
