USE [master]
GO
/****** Object:  Database [Borisov_test]    Script Date: 27.02.2018 15:26:18 ******/
CREATE DATABASE [Borisov_test]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Borisov_test', FILENAME = N'C:\DATA\Borisov_test.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Borisov_test_log', FILENAME = N'C:\DATA\Borisov_test_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Borisov_test] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Borisov_test].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
USE [Borisov_test]
GO
/****** Object:  Table [dbo].[Hashes]    Script Date: 27.02.2018 15:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hashes](
	[LogID] [numeric](18, 0) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[hash] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Hashes] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC,
	[FileName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Session]    Script Date: 27.02.2018 15:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Session](
	[Log_id] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Root] [nvarchar](255) NOT NULL,
	[Start] [datetime] NOT NULL,
 CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED 
(
	[Log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [master]
GO
ALTER DATABASE [Borisov_test] SET  READ_WRITE 
GO
