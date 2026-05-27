

CREATE TABLE [dbo].[Productos](
	[producto] [nvarchar](15) NOT NULL,
	[descripcion] [nvarchar](32) NOT NULL,
	[UM] [nvarchar](2) NOT NULL,
	[precio] [decimal](8, 3) NOT NULL,
	[maximoAlm] [smallint] NOT NULL,
	[minimoAlm] [smallint] NOT NULL,
	[porComision] [decimal](3,2) NOT NULL
 CONSTRAINT [PK_producto] PRIMARY KEY CLUSTERED 
(
	[producto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

----UM es la unidad de medida 
----KG Kilogramos
----PZ Pieza
----LT Litros
----BT Botella
