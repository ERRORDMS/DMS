using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Database
{
	public class Queries
	{


		public static string CreateTables = @"
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'CategoryUserRel'))
BEGIN

CREATE TABLE [dbo].[CategoryUserRel](
	[RelAutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[CatAutoKey] [bigint] NULL,
	[UserID] [nvarchar](max) NULL,
 CONSTRAINT [PK_CategoryUserRel] PRIMARY KEY CLUSTERED 
(
	[RelAutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'UserDB'))

BEGIN

CREATE TABLE [dbo].[UserDB](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](max) NULL,
	[DBName] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserDatabases] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'UserStorage'))

BEGIN

CREATE TABLE [dbo].[UserStorage](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](max) NULL,
	[UsedStorage] [float] NULL,
	[Storage] [float] NULL,
 CONSTRAINT [PK_UserStorage] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'RolePermissions'))

BEGIN

CREATE TABLE [dbo].[RolePermissions](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[PermissionID] [bigint] NULL,
	[RoleID] [bigint] NULL,
 CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Roles'))

BEGIN

CREATE TABLE [dbo].[Roles](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END


IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Permissions'))

BEGIN

CREATE TABLE [dbo].[Permissions](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


insert into Permissions (Name) values ('Can Add Root Category')
insert into Permissions (Name) values ('Can Add Contacts')
insert into Permissions (Name) values ('Can Edit Contacts')
insert into Permissions (Name) values ('Can Add Search Keys')
insert into Permissions (Name) values ('Can Edit Search Keys')
insert into Permissions (Name) values ('Administrator')

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ContactsCategory'))

BEGIN

CREATE TABLE [dbo].[ContactsCategory](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(MAX) NULL,
 CONSTRAINT [PK_ContactsCategory] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Emails'))

BEGIN

CREATE TABLE [dbo].[Emails](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
    [From] NVARCHAR(MAX) NULL,
	[To] NVARCHAR(MAX) NULL,
	[Content] NVARCHAR(MAX) NULL,
	[Subject] NVARCHAR(MAX) NULL,
 CONSTRAINT [PK_Emails] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END


IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'UserCategories'))

BEGIN

CREATE TABLE [dbo].[UserCategories](
	[CatID] [bigint] NULL,
	[UserID] [nvarchar](max) NULL,
	[Fathers] [nvarchar](max) NULL,
	[CanView] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanAdd] [bit] NULL,
	[CanDownload] [bit] NULL,
	[CanDelete] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'DocumentVersions'))

BEGIN


CREATE TABLE [dbo].[DocumentVersions](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[Version] [bigint] NULL,
	[InfoAutoKey] [bigint] NULL,
	[LineAutoKey] [bigint] NULL,
    [FileName] NVARCHAR(MAX) NULL,
 CONSTRAINT [PK_DocumentVersions] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'EnterpriseCodes'))

BEGIN

CREATE TABLE [dbo].[EnterpriseCodes](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
 CONSTRAINT [PK_EnterpriseCodes] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'TrustedIPs'))

BEGIN

CREATE TABLE [dbo].[TrustedIPs](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](max) NULL,
	[IP] [nvarchar](max) NULL,
 CONSTRAINT [PK_TrustedIPs] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'UserRoles'))

BEGIN

CREATE TABLE [dbo].[UserRoles](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](max) NULL,
	[RoleID] [bigint] NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'RoleCategories'))

BEGIN

CREATE TABLE [dbo].[RoleCategories](
	[CatID] [bigint] NULL,
	[Fathers] [nvarchar](max) NULL,
	[CanView] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanDelete] [bit] NULL,
	[RoleID] [bigint] NULL,
	[CanAdd] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'UserPermissions'))

BEGIN
CREATE TABLE [dbo].[UserPermissions](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[PermissionID] [bigint] NULL,
	[UserID] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserPermissions] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END


		";

		public static string UpdateTables = CreateTables + @"

IF COL_LENGTH('dbo.Contacts', 'DMSUserID') IS NULL
BEGIN
  ALTER TABLE [Contacts]
    ADD [DMSUserID] NVARCHAR(MAX) NULL
END


IF COL_LENGTH('dbo.DocumentLine', 'Name') IS NULL
BEGIN
  ALTER TABLE [DocumentLine]
    ADD [Name] NVARCHAR(MAX) NULL
END

IF COL_LENGTH('dbo.SearchKeys', 'DMSUserID') IS NULL
BEGIN
  ALTER TABLE [SearchKeys]
    ADD [DMSUserID] NVARCHAR(MAX) NULL
END


IF COL_LENGTH('dbo.Users', 'AccountType') IS NULL
BEGIN
  ALTER TABLE [Users]
    ADD [AccountType] NVARCHAR(MAX) NULL
END


IF COL_LENGTH('dbo.Users', 'EnterpriseCode') IS NULL
BEGIN
  ALTER TABLE [Users]
    ADD [EnterpriseCode] NVARCHAR(MAX) NULL
END

IF COL_LENGTH('dbo.Users', 'Activated') IS NULL
BEGIN
  ALTER TABLE [Users]
    ADD [Activated] bit NULL
END

IF COL_LENGTH('dbo.Users', 'Phone') IS NULL
BEGIN
  ALTER TABLE [Users]
    ADD [Phone] NVARCHAR(MAX) NULL
END


IF COL_LENGTH('dbo.Users', '2FA') IS NULL
BEGIN
  ALTER TABLE [Users]
    ADD [2FA] bit NULL
END

IF COL_LENGTH('dbo.UserCategories', 'CanDownload') IS NULL
BEGIN
  ALTER TABLE [UserCategories]
    ADD [CanDownload] bit NULL
END

IF COL_LENGTH('dbo.RoleCategories', 'CanDownload') IS NULL
BEGIN
  ALTER TABLE [RoleCategories]
    ADD [CanDownload] bit NULL
END


IF COL_LENGTH('dbo.Contacts', 'CateriesID') IS NOT NULL
BEGIN
EXEC sp_rename 'Contacts.CateriesID', 'CategoriesID', 'COLUMN';

END

IF COL_LENGTH('dbo.DocumentLine', 'FileSize') IS NULL
BEGIN
  ALTER TABLE [DocumentLine]
    ADD [FileSize] bigint NULL
  Update DocumentLine Set FILESIZE  = 0 WHERE FileSize IS NULL

END



";
		/*
		public static string CreateEnterpriseTable = @"

USE [DBNAME]

CREATE TABLE [dbo].[UID](
	[CatID] [bigint] NULL,
	[Fathers] [nvarchar](max) NULL,
	[CanView] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanAdd] [bit] NULL,
	[CanDelete] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
";*/

		public static string ConfigureDBQuery = @"
ALTER DATABASE [DBNAME] SET COMPATIBILITY_LEVEL = 100


IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DBNAME].[dbo].[sp_fulltext_database] @action = 'enable'
end


ALTER DATABASE [DBNAME] SET ANSI_NULL_DEFAULT OFF 


ALTER DATABASE [DBNAME] SET ANSI_NULLS OFF 


ALTER DATABASE [DBNAME] SET ANSI_PADDING OFF 


ALTER DATABASE [DBNAME] SET ANSI_WARNINGS OFF 


ALTER DATABASE [DBNAME] SET ARITHABORT OFF 


ALTER DATABASE [DBNAME] SET AUTO_CLOSE OFF 


ALTER DATABASE [DBNAME] SET AUTO_SHRINK OFF 


ALTER DATABASE [DBNAME] SET AUTO_UPDATE_STATISTICS ON 


ALTER DATABASE [DBNAME] SET CURSOR_CLOSE_ON_COMMIT OFF 


ALTER DATABASE [DBNAME] SET CURSOR_DEFAULT  GLOBAL 


ALTER DATABASE [DBNAME] SET CONCAT_NULL_YIELDS_NULL OFF 


ALTER DATABASE [DBNAME] SET NUMERIC_ROUNDABORT OFF 


ALTER DATABASE [DBNAME] SET QUOTED_IDENTIFIER OFF 


ALTER DATABASE [DBNAME] SET RECURSIVE_TRIGGERS OFF 


ALTER DATABASE [DBNAME] SET  DISABLE_BROKER 


ALTER DATABASE [DBNAME] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 


ALTER DATABASE [DBNAME] SET DATE_CORRELATION_OPTIMIZATION OFF 


ALTER DATABASE [DBNAME] SET TRUSTWORTHY OFF 


ALTER DATABASE [DBNAME] SET ALLOW_SNAPSHOT_ISOLATION OFF 


ALTER DATABASE [DBNAME] SET PARAMETERIZATION SIMPLE 


ALTER DATABASE [DBNAME] SET READ_COMMITTED_SNAPSHOT OFF 


ALTER DATABASE [DBNAME] SET HONOR_BROKER_PRIORITY OFF 


ALTER DATABASE [DBNAME] SET RECOVERY SIMPLE 


ALTER DATABASE [DBNAME] SET  MULTI_USER 


ALTER DATABASE [DBNAME] SET PAGE_VERIFY CHECKSUM  


ALTER DATABASE [DBNAME] SET DB_CHAINING OFF 


ALTER DATABASE [DBNAME] SET FILESTREAM( NON_TRANSACTED_ACCESS = FULL, DIRECTORY_NAME = N'DBNAME' ) 


ALTER DATABASE [DBNAME] SET TARGET_RECOVERY_TIME = 0 SECONDS 


ALTER DATABASE [DBNAME] SET DELAYED_DURABILITY = DISABLED 


ALTER DATABASE [DBNAME] SET QUERY_STORE = OFF


USE [DBNAME]


ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = ON;


ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;


ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;


ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;


ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;


ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;


ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;


ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;


ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;


ALTER DATABASE [DBNAME] SET  READ_WRITE 

";


		public static string AddTablesQuery = @"
USE [DBNAME]


CREATE TABLE [dbo].[UserCategories](
	[CatID] [bigint] NULL,
	[UserID] [nvarchar](max) NULL,
	[Fathers] [nvarchar](max) NULL,
	[CanView] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanAdd] [bit] NULL,
	[CanDownload] [bit] NULL,
	[CanDelete] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

/****** Object:  Table [dbo].[CategoryUserRel]    Script Date: 8/10/2020 8:53:43 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[CategoryUserRel](
	[RelAutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[CatAutoKey] [bigint] NULL,
	[UserID] [nvarchar](max) NULL,
 CONSTRAINT [PK_CategoryUserRel] PRIMARY KEY CLUSTERED 
(
	[RelAutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

/****** Object:  Table [dbo].[Contacts]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Contacts](
	[ID] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[BusinessPhone] [nvarchar](max) NULL,
	[BusinessStreet] [nvarchar](max) NULL,
	[ProfessionID] [nvarchar](50) NULL,
	[MobilePhone] [nvarchar](50) NULL,
	[Birthday] [smalldatetime] NULL,
	[BusinessPhone2] [nvarchar](50) NULL,
	[MobilePhone2] [nvarchar](50) NULL,
	[BusinessFax] [nvarchar](50) NULL,
	[HomePhone] [nvarchar](50) NULL,
	[GenderID] [nvarchar](50) NULL,
	[GovernmentID] [nvarchar](50) NULL,
	[CategoriesID] [nvarchar](50) NULL,
	[BusinessCityID] [nvarchar](50) NULL,
	[Notes] [nvarchar](max) NULL,
	[CatID2] [nvarchar](50) NULL,
	[Percent] [float] NULL,
	[ContactPerson] [nvarchar](max) NULL,
	[UserDefinedN1] [float] NULL,
	[UserDefinedN2] [float] NULL,
	[UserDefinedN3] [float] NULL,
	[UserDefinedN4] [float] NULL,
	[UserDefinedN5] [float] NULL,
	[UserDefinedT1] [nvarchar](300) NULL,
	[UserDefinedT2] [nvarchar](300) NULL,
	[UserDefinedT3] [nvarchar](300) NULL,
	[UserDefinedT4] [nvarchar](300) NULL,
	[UserDefinedT5] [nvarchar](300) NULL,
	[UserDefinedB1] [bit] NULL,
	[UserDefinedB2] [bit] NULL,
	[UserDefinedB3] [bit] NULL,
	[UserDefinedB4] [bit] NULL,
	[UserDefinedB5] [bit] NULL,
	[Email1] [nvarchar](100) NULL,
	[Email2] [nvarchar](100) NULL,
	[WebSite] [nvarchar](100) NULL,
	[ContactPerson1] [nvarchar](150) NULL,
	[ContactPerson2] [nvarchar](150) NULL,
	[ContactPerson3] [nvarchar](150) NULL,
	[ContactPerson4] [nvarchar](150) NULL,
	[PriceLevel] [nvarchar](50) NULL,
	[MaxDebt] [float] NULL,
	[VatID] [nvarchar](50) NULL,
	[HourRate] [float] NULL,
	[FingerPrintID] [nvarchar](50) NULL,
	[Photo] [image] NULL,
	[IsStopped] [bit] NULL,
	[Archive] [nvarchar](50) NULL,
	[Nationality] [nvarchar](150) NULL,
	[MaritalStatusID] [nvarchar](50) NULL,
	[Weight] [float] NULL,
	[Height] [float] NULL,
	[HRID] [nvarchar](50) NULL,
	[IsHideBallance] [bit] NULL,
	[DateTimeAdded] [smalldatetime] NULL,
	[UserID] [nvarchar](50) NULL,
	[MotherNatID] [nvarchar](50) NULL,
	[OrderNo] [bigint] NULL,
	[Name2] [nvarchar](200) NULL,
	[Name3] [nvarchar](200) NULL,
	[Name4] [nvarchar](200) NULL,
	[VATNo] [nvarchar](50) NULL,
	[HomeAdress] [nvarchar](200) NULL,
	[JobDescr] [nvarchar](200) NULL,
	[TradeType] [nvarchar](50) NULL,
	[VatFileTypeID] [nvarchar](50) NULL,
	[VatFileStatusID] [nvarchar](50) NULL,
	[JobStartDate] [smalldatetime] NULL,
	[VATFileStartDate] [smalldatetime] NULL,
	[RankID] [nvarchar](50) NULL,
	[MaxChecksDebt] [float] NULL,
	[VatPeriodID] [nvarchar](50) NULL,
	[IsVIP] [bit] NULL,
	[TurnID] [nvarchar](50) NULL,
	[IsReferal] [bit] NULL,
	[InsuranceID] [nvarchar](50) NULL,
	[InsuranceCompanyID] [nvarchar](50) NULL,
	[Percent2] [float] NULL,
	[AutoKey] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[DMSUserID] [nvarchar](max) NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

/****** Object:  Table [dbo].[DocumentCatRel]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[DocumentCatRel](
	[CatAutoKey] [bigint] NULL,
	[DocumentAutoKey] [bigint] NULL,
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[DocumentCatTree]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[DocumentCatTree](
	[ID] [nvarchar](50) NULL,
	[Name] [nvarchar](300) NULL,
	[Father] [nvarchar](50) NULL,
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[FatherAutoKey] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[DocumentContactsRel]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[DocumentContactsRel](
	[ContactAutoKey] [bigint] NULL,
	[DocumentAutoKey] [bigint] NULL,
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[DocumentLine]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[DocumentLine](
	[InfoAutoKey] [bigint] NULL,
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[Ext] [nvarchar](10) NULL,
	[Pages] [bigint] NULL,
	[LDeleteNote] [nvarchar](300) NULL,
	[LIsDeleted] [bit] NULL,
	[LUserDeleteit] [nvarchar](50) NULL,
	[LDateTimeDeleted] [smalldatetime] NULL,
	[Name] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

/****** Object:  Table [dbo].[DocumentSearchKeysRel]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[DocumentSearchKeysRel](
	[SearchAutoKey] [bigint] NULL,
	[DocumentAutoKey] [bigint] NULL,
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[DocumentsInfo]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[DocumentsInfo](
	[AddedByUserID] [nvarchar](50) NULL,
	[DateTimeAdded] [smalldatetime] NULL,
	[CreatedByUserID] [nvarchar](50) NULL,
	[DateTimeCreated] [smalldatetime] NULL,
	[CategoryID] [nvarchar](50) NULL,
	[SubCategoryID] [nvarchar](50) NULL,
	[CategoryID2] [nvarchar](50) NULL,
	[ManualFileNo] [nvarchar](50) NULL,
	[IsApproved] [bit] NULL,
	[Approvedby] [nvarchar](50) NULL,
	[DateTimeApproval] [smalldatetime] NULL,
	[SubjectID] [nvarchar](50) NULL,
	[DocTypeID] [nvarchar](50) NULL,
	[Sender] [nvarchar](50) NULL,
	[Reciepient] [nvarchar](50) NULL,
	[Note] [nvarchar](300) NULL,
	[BarCode] [nvarchar](300) NULL,
	[TransNo] [bigint] NULL,
	[TransBookID] [nvarchar](50) NULL,
	[TransTypeID] [nvarchar](50) NULL,
	[IsDeleted] [bit] NULL,
	[UserDeleteit] [nvarchar](50) NULL,
	[DateTimeDeleted] [smalldatetime] NULL,
	[InfoAutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[DeleteNote] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[InfoAutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[Images]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ARITHABORT ON

CREATE TABLE [dbo].[Images] AS FILETABLE ON [PRIMARY] FILESTREAM_ON [FS]
WITH
(
FILETABLE_DIRECTORY = N'DBNAME', FILETABLE_COLLATE_FILENAME = Arabic_CI_AI
)

/****** Object:  Table [dbo].[SearchKeys]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[SearchKeys](
	[ID] [nvarchar](50) NULL,
	[Name] [nvarchar](300) NULL,
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[DMSUserID] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

/****** Object:  Table [dbo].[Users]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Users](
	[Name] [nvarchar](250) NULL,
	[Pass] [image] NULL,
	[ID] [nvarchar](50) NULL,
	[UserGroupNo] [nvarchar](50) NULL,
	[StartupFormNo] [nvarchar](50) NULL,
	[BookID] [nvarchar](50) NULL,
	[IsSuperVisor] [image] NULL,
	[AllBooks] [image] NULL,
	[StoreID] [nvarchar](50) NULL,
	[CostCenterID] [nvarchar](50) NULL,
	[CashID] [nvarchar](50) NULL,
	[ClinicID] [nvarchar](50) NULL,
	[AllowBackup] [bit] NULL,
	[IsStopped] [bit] NULL,
	[HasExpaDateWarn] [bit] NULL,
	[HideSalesMan] [bit] NULL,
	[HideAtReception] [bit] NULL,
	[CanAddToArchive] [bit] NULL,
	[CanDeleteFromArchive] [bit] NULL,
	[SectionID] [nvarchar](50) NULL,
	[HasMinQtyWarn] [bit] NULL,
	[ReportID] [nvarchar](50) NULL,
	[ContactCategoryID] [nvarchar](50) NULL,
	[ShowReportToolbar] [bit] NULL,
	[HasChecksWarn] [bit] NULL,
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[LocationID] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

/****** Object:  Table [dbo].[UserStorage]    Script Date: 8/10/2020 8:53:44 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[UserStorage](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](max) NULL,
	[UsedStorage] [float] NULL,
	[Storage] [float] NULL,
 CONSTRAINT [PK_UserStorage] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[DocumentCatRel]  WITH CHECK ADD  CONSTRAINT [FK_DocumentCatRel_DocumentsInfo] FOREIGN KEY([DocumentAutoKey])
REFERENCES [dbo].[DocumentsInfo] ([InfoAutoKey])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[DocumentCatRel] CHECK CONSTRAINT [FK_DocumentCatRel_DocumentsInfo]

ALTER TABLE [dbo].[DocumentContactsRel]  WITH CHECK ADD  CONSTRAINT [FK_DocumentContactsRel_DocumentsInfo] FOREIGN KEY([DocumentAutoKey])
REFERENCES [dbo].[DocumentsInfo] ([InfoAutoKey])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[DocumentContactsRel] CHECK CONSTRAINT [FK_DocumentContactsRel_DocumentsInfo]

ALTER TABLE [dbo].[DocumentLine]  WITH CHECK ADD  CONSTRAINT [FK_DocumentLine_DocumentsInfo] FOREIGN KEY([InfoAutoKey])
REFERENCES [dbo].[DocumentsInfo] ([InfoAutoKey])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[DocumentLine] CHECK CONSTRAINT [FK_DocumentLine_DocumentsInfo]

ALTER TABLE [dbo].[DocumentSearchKeysRel]  WITH CHECK ADD  CONSTRAINT [FK_DocumentSearchKeysRel_DocumentsInfo] FOREIGN KEY([DocumentAutoKey])
REFERENCES [dbo].[DocumentsInfo] ([InfoAutoKey])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [dbo].[DocumentSearchKeysRel] CHECK CONSTRAINT [FK_DocumentSearchKeysRel_DocumentsInfo]

EXEC sys.sp_addextendedproperty @name=N'AllowZeroLength', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'Attributes', @value=N'2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'Collatinrder', @value=N'1033' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'ColumnHidden', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'ColumnOrder', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'ColumnWidth', @value=N'-1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'DataUpdatable', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'GUID', @value=N'弌ᣅߔ䌡⧥몿㗎' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'109' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'MS_IMEMode', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'MS_IMESentMode', @value=N'3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'Name', @value=N'Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'OrdinalPosition', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'Required', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'Size', @value=N'250' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'SourceField', @value=N'Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'SourceTable', @value=N'Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'Type', @value=N'10' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'UnicodeCompression', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'

EXEC sys.sp_addextendedproperty @name=N'AllowZeroLength', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'Attributes', @value=N'2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'Collatinrder', @value=N'1033' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'ColumnHidden', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'ColumnOrder', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'ColumnWidth', @value=N'-1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'DataUpdatable', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'GUID', @value=N'ઘ놯乄䖈聜㟢弧' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'Name', @value=N'Pass' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'OrdinalPosition', @value=N'1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'Required', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'Size', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'SourceField', @value=N'Pass' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'SourceTable', @value=N'Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'Type', @value=N'11' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Pass'

EXEC sys.sp_addextendedproperty @name=N'AllowZeroLength', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'Attributes', @value=N'2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'Collatinrder', @value=N'1033' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'ColumnHidden', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'ColumnOrder', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'ColumnWidth', @value=N'-1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'DataUpdatable', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'GUID', @value=N'⼼༱䘂邃풡薔쉬' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'109' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'MS_IMEMode', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'MS_IMESentMode', @value=N'3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'Name', @value=N'ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'OrdinalPosition', @value=N'2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'Required', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'Size', @value=N'50' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'SourceField', @value=N'ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'SourceTable', @value=N'Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'Type', @value=N'10' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'UnicodeCompression', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'ID'

EXEC sys.sp_addextendedproperty @name=N'AllowZeroLength', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'Attributes', @value=N'2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'Collatinrder', @value=N'1033' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'ColumnHidden', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'ColumnOrder', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'ColumnWidth', @value=N'-1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'DataUpdatable', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'GUID', @value=N'䬠቗罌䀰䆮쐬꿟⊢' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_BoundColumn', @value=N'1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ColumnCount', @value=N'1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ColumnHeads', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ColumnWidths', @value=N'1440' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'111' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_IMEMode', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_IMESentMode', @value=N'3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_LimitToList', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ListRows', @value=N'8' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ListWidth', @value=N'1440twip' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_RowSource', @value=N'SELECT UserGroups.GroupNo
FROM UserGroups' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'MS_RowSourceType', @value=N'Table/View/StoredProc' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'Name', @value=N'UserGroupNo' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'OrdinalPosition', @value=N'3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'Required', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'Size', @value=N'50' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'SourceField', @value=N'UserGroupNo' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'SourceTable', @value=N'Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'Type', @value=N'10' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'UnicodeCompression', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'UserGroupNo'

EXEC sys.sp_addextendedproperty @name=N'AllowZeroLength', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'Attributes', @value=N'2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'Collatinrder', @value=N'1033' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'ColumnHidden', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'ColumnOrder', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'ColumnWidth', @value=N'-1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'DataUpdatable', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'GUID', @value=N'ᢺ蛏࿛䴔䒕倲䱗懯' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_BoundColumn', @value=N'1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ColumnCount', @value=N'1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ColumnHeads', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ColumnWidths', @value=N'1440' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'111' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_IMEMode', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_IMESentMode', @value=N'3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_LimitToList', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ListRows', @value=N'8' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_ListWidth', @value=N'1440twip' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_RowSource', @value=N'SELECT UserStartupForms.FormNo
FROM UserStartupForms' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'MS_RowSourceType', @value=N'Table/View/StoredProc' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'Name', @value=N'StartupFormNo' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'OrdinalPosition', @value=N'4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'Required', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'Size', @value=N'50' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'SourceField', @value=N'StartupFormNo' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'SourceTable', @value=N'Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'Type', @value=N'10' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'UnicodeCompression', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'StartupFormNo'

EXEC sys.sp_addextendedproperty @name=N'AllowZeroLength', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'Attributes', @value=N'2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'Collatinrder', @value=N'1033' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'ColumnHidden', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'ColumnOrder', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'ColumnWidth', @value=N'-1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'DataUpdatable', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'109' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'MS_IMEMode', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'MS_IMESentMode', @value=N'3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'Name', @value=N'BookID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'OrdinalPosition', @value=N'5' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'Required', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'Size', @value=N'50' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'SourceField', @value=N'BookID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'SourceTable', @value=N'Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'Type', @value=N'10' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'UnicodeCompression', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'BookID'

EXEC sys.sp_addextendedproperty @name=N'Attributes', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

EXEC sys.sp_addextendedproperty @name=N'DateCreated', @value=N'28/04/2002 08:36 PM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

EXEC sys.sp_addextendedproperty @name=N'LastUpdated', @value=N'04/08/2006 09:37 AM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

EXEC sys.sp_addextendedproperty @name=N'MS_DefaultView', @value=N'2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

EXEC sys.sp_addextendedproperty @name=N'MS_OrderByOn', @value=N'False' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

EXEC sys.sp_addextendedproperty @name=N'MS_Orientation', @value=N'0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

EXEC sys.sp_addextendedproperty @name=N'Name', @value=N'Users' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

EXEC sys.sp_addextendedproperty @name=N'RecordCount', @value=N'4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

EXEC sys.sp_addextendedproperty @name=N'Updatable', @value=N'True' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users'

INSERT INTO Images (name,is_directory,is_archive) VALUES ('Images', 1, 0) 

" + Queries.UpdateTables;
	}
}
