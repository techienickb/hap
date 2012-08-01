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
/****** Object:  Table [dbo].[WebTrackerEvents]    Script Date: 03/12/2011 17:44:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebTrackerEvents]') AND type in (N'U'))
DROP TABLE [dbo].[WebTrackerEvents]
GO
/****** Object:  Table [dbo].[WebTrackerEvents]    Script Date: 03/12/2011 17:44:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
