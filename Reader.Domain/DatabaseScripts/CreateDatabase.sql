USE [master]
GO

/****** Object:  Database [contingencyreader]    Script Date: 5/30/2013 9:24:26 PM ******/
DROP DATABASE [contingencyreader]
GO

/****** Object:  Database [contingencyreader]    Script Date: 5/30/2013 9:24:26 PM ******/
CREATE DATABASE [contingencyreader]
GO

ALTER DATABASE [contingencyreader] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [contingencyreader].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO

ALTER DATABASE [contingencyreader] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [contingencyreader] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [contingencyreader] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [contingencyreader] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [contingencyreader] SET ARITHABORT OFF 
GO

ALTER DATABASE [contingencyreader] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [contingencyreader] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [contingencyreader] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [contingencyreader] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [contingencyreader] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [contingencyreader] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [contingencyreader] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [contingencyreader] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [contingencyreader] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [contingencyreader] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [contingencyreader] SET  DISABLE_BROKER 
GO

ALTER DATABASE [contingencyreader] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [contingencyreader] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [contingencyreader] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [contingencyreader] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO

ALTER DATABASE [contingencyreader] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [contingencyreader] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [contingencyreader] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [contingencyreader] SET RECOVERY FULL 
GO

ALTER DATABASE [contingencyreader] SET  MULTI_USER 
GO

ALTER DATABASE [contingencyreader] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [contingencyreader] SET DB_CHAINING OFF 
GO

ALTER DATABASE [contingencyreader] SET  READ_WRITE 
GO

USE [contingencyreader]
GO

/****** Object:  Table [dbo].[Feeds]    Script Date: 5/30/2013 9:24:11 PM ******/
DROP TABLE [dbo].[Feeds]
GO

/****** Object:  Table [dbo].[Feeds]    Script Date: 5/30/2013 9:24:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Feeds](
	[FeedID] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](MAX) NOT NULL,
	[URL] [nvarchar](MAX) NOT NULL,
	[BlogURL] NVARCHAR(MAX) NULL, 
    [Favicon] IMAGE NULL,	
	[LastRefresh] DATETIME NULL,
PRIMARY KEY CLUSTERED 
(
	[FeedID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

/****** Object:  Table [dbo].[Items]    Script Date: 5/30/2013 9:28:35 PM ******/
DROP TABLE [dbo].[Items]
GO

/****** Object:  Table [dbo].[Items]    Script Date: 5/30/2013 9:28:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Items](
	[ItemID] [int] IDENTITY(1,1) NOT NULL,
	[FeedID] [int] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[PublishDate] [datetime] NULL,
	[Content] [nvarchar](max) NULL,
	[Url] [nvarchar](MAX) NOT NULL,
	[IsRead] [bit] NOT NULL,
	[isStarred] [bit] NOT NULL,
	[FetchDate] DATETIME NOT NULL, 
PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

/****** Object:  Table [dbo].[Feeds]    Script Date: 1/16/2015 11:49:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Options](
	[OptionID] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](50) NULL,
	[Value] [nvarchar](max) NULL
PRIMARY KEY CLUSTERED 
(
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
