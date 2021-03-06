SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  Table [dbo].[TrackerEvents]    Script Date: 03/12/2011 17:44:19 ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TrackerEvents]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TrackerEvents](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[LogonDateTime] [datetime] NOT NULL,
	[Username] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[ComputerName] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[LogoffDateTime] [datetime] NULL,
	[domainname] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[ip] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[logonserver] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[os] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
 CONSTRAINT [PK_TrackerEvents_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebTrackerEvents]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WebTrackerEvents](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Username] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[ComputerName] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[EventType] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[IP] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[Browser] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[OS] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[Details] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
 CONSTRAINT [PK_WebTrackerEvents_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tickets]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Tickets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[AssignedTo] [nvarchar](max) NULL,
	[ShowTo] [nvarchar](max) NULL,
	[ReadBy] [nvarchar](max) NULL,
	[Priority] [nvarchar](max) NULL,
	[Faq] [bit] NOT NULL,
	[HideAssignedTo] [bit] NOT NULL,
	[Archive] [nvarchar](max) NULL,
 CONSTRAINT [PK_Tickets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

ALTER TABLE [dbo].[Tickets] ADD  CONSTRAINT [DF_Tickets_Faq]  DEFAULT ((0)) FOR [Faq]
GO

ALTER TABLE [dbo].[Tickets] ADD  CONSTRAINT [DF_Tickets_Archived]  DEFAULT ((0)) FOR [Archive]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Note]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Note](
	[TicketId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Username] [nvarchar](max) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Hide] [bit] NOT NULL,
 CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Tickets] FOREIGN KEY([TicketId])
REFERENCES [dbo].[Tickets] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Tickets]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NoteFiles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NoteFiles](
	[NoteId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Data] [varbinary](max) NOT NULL,
	[ContentType] [nvarchar](max) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_NoteFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[NoteFiles]  WITH CHECK ADD  CONSTRAINT [FK_NoteFiles_Note] FOREIGN KEY([NoteId])
REFERENCES [dbo].[Note] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NoteFiles] CHECK CONSTRAINT [FK_NoteFiles_Note]
GO