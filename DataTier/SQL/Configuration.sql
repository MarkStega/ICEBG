
/******************************************************************************
Generated file - Created on 5/23/2022; Do not edit!
******************************************************************************/
use [ICEBG]
go

/******************************************************************************
Generated file - Created on 5/23/2022; Do not edit!
******************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Configuration_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[Configuration_Insert]
GO

CREATE PROCEDURE [dbo].[Configuration_Insert]
(
	@Id nchar(24),
	@Configuration nvarchar(max)
)

AS

SET NOCOUNT ON

INSERT INTO [Configuration]
(
	[Id],
	[Configuration]
)
VALUES
(
	@Id,
	@Configuration
)

GO

/******************************************************************************
Generated file - Created on 5/23/2022; Do not edit!
******************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Configuration_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[Configuration_Update]
GO

CREATE PROCEDURE [dbo].[Configuration_Update]
(
	@Id nchar(24),
	@Configuration nvarchar(max)
)

AS

SET NOCOUNT ON

UPDATE [Configuration]
SET [Configuration] = @Configuration
WHERE [Id] = @Id
GO

/******************************************************************************
Generated file - Created on 5/23/2022; Do not edit!
******************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Configuration_Upsert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[Configuration_Upsert]
GO

CREATE PROCEDURE [dbo].[Configuration_Upsert]
(
	@Id nchar(24),
	@Configuration nvarchar(max)
)

AS

SET NOCOUNT ON

SET TRANSACTION ISOLATION LEVEL READ COMMITTED

BEGIN TRANSACTION

UPDATE [Configuration]  WITH (UPDLOCK, HOLDLOCK)
SET [Configuration] = @Configuration
WHERE [Id] = @Id
IF(@@ROWCOUNT = 0)
BEGIN
	INSERT INTO [Configuration]
	(
		[Id],
		[Configuration]
	)
	VALUES
	(
		@Id,
		@Configuration
	)

END

COMMIT

GO

/******************************************************************************
Generated file - Created on 5/23/2022; Do not edit!
******************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Configuration_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[Configuration_Delete]
GO

CREATE PROCEDURE [dbo].[Configuration_Delete]
(
	@Id nchar(24)
)

AS

SET NOCOUNT ON

DELETE FROM [Configuration]
WHERE [Id] = @Id
GO

/******************************************************************************
Generated file - Created on 5/23/2022; Do not edit!
******************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Configuration_Select]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[Configuration_Select]
GO

CREATE PROCEDURE [dbo].[Configuration_Select]
(
	@Id nchar(24)
)

AS

SET NOCOUNT ON

SELECT [Id],
	[Configuration]
FROM [Configuration]
WHERE [Id] = @Id
GO

/******************************************************************************
Generated file - Created on 5/23/2022; Do not edit!
******************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Configuration_SelectAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dbo].[Configuration_SelectAll]
GO

CREATE PROCEDURE [dbo].[Configuration_SelectAll]

AS

SET NOCOUNT ON

SELECT [Id],
	[Configuration]
FROM [Configuration]
GO
