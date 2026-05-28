USE [MockSrvDb]
GO
/****** Object:  Table [dbo].[MockRequest]    Script Date: 2024-03-21 14:09:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MockRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HashKey]  AS (CONVERT([varchar](256),hashbytes('SHA2_256',((([RequestPath]+[RequestMethod])+isnull([RequestHeaders],''))+isnull([RequestQueryString],''))+isnull([RequestBody],'')),(2))) PERSISTED,
	[RequestPath] [nvarchar](max) NOT NULL,
	[RequestMethod] [nvarchar](max) NOT NULL,
	[RequestHeaders] [nvarchar](max) NULL,
	[RequestQueryString] [nvarchar](max) NULL,
	[RequestBody] [nvarchar](max) NULL,
	[ResponseBody] [nvarchar](max) NULL,
	[ResponseStatusCode] [int] NOT NULL,
	[ResponseContentType] [nvarchar](max) NULL,
	[ResponseHeaders] [nvarchar](max) NULL,
 CONSTRAINT [PK_MockRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[HashKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
