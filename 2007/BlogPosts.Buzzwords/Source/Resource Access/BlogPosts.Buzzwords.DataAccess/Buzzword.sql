
IF NOT EXISTS (SELECT NAME FROM sys.objects WHERE TYPE = 'P' AND NAME = 'RethrowError')
BEGIN
	EXEC('CREATE PROCEDURE [dbo].RethrowError AS RETURN')
END
GO

ALTER PROCEDURE RethrowError AS
    /* Return if there is no error information to retrieve. */
    IF ERROR_NUMBER() IS NULL
        RETURN;

    DECLARE
        @ErrorMessage    NVARCHAR(4000),
        @ErrorNumber     INT,
        @ErrorSeverity   INT,
        @ErrorState      INT,
        @ErrorLine       INT,
        @ErrorProcedure  NVARCHAR(200); 

    /* Assign variables to error-handling functions that
       capture information for RAISERROR. */

    SELECT
        @ErrorNumber = ERROR_NUMBER(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE(),
        @ErrorLine = ERROR_LINE(),
        @ErrorProcedure = ISNULL(ERROR_PROCEDURE(), '-'); 

    /* Building the message string that will contain original
       error information. */

    SELECT @ErrorMessage = 
        N'Error %d, Level %d, State %d, Procedure %s, Line %d, ' + 
         'Message: '+ ERROR_MESSAGE(); 

    /* Raise an error: msg_str parameter of RAISERROR will contain
	   the original error information. */

    RAISERROR(@ErrorMessage, @ErrorSeverity, 1,
        @ErrorNumber,    /* parameter: original error number. */
        @ErrorSeverity,  /* parameter: original error severity. */
        @ErrorState,     /* parameter: original error state. */
        @ErrorProcedure, /* parameter: original error procedure name. */
        @ErrorLine       /* parameter: original error line number. */
        );

GO

----------------------------------------------------------------
-- [dbo].[buzzwords] Table
--
IF NOT EXISTS (SELECT NAME FROM sys.objects WHERE TYPE = 'P' AND NAME = 'Insertbuzzwords')
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[Insertbuzzwords] AS RETURN')
END

GO

ALTER PROCEDURE [dbo].[Insertbuzzwords]
    @id int OUT,
	@name nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY
    INSERT INTO [dbo].[buzzwords] ([name])
	VALUES (@name)
    SET @id = SCOPE_IDENTITY()
    END TRY

    BEGIN CATCH
		EXEC RethrowError;
	END CATCH
    
    SET NOCOUNT OFF
END    

GO

IF NOT EXISTS (SELECT NAME FROM sys.objects WHERE TYPE = 'P' AND NAME = 'Updatebuzzwords')
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[Updatebuzzwords] AS RETURN')
END

GO

ALTER PROCEDURE [dbo].[Updatebuzzwords]
    @id int,
	@name nvarchar(50)
AS
BEGIN

	--The [dbo].[buzzwords] table doesn't have a timestamp column. Optimistic concurrency logic cannot be generated
	SET NOCOUNT ON

	BEGIN TRY
	UPDATE [dbo].[buzzwords] 
	SET [name] = @name
	WHERE [id]=@id

	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR('Concurrent update error. Updated aborted.', 16, 2)
	END
    END TRY

    BEGIN CATCH
		EXEC RethrowError;
	END CATCH	

	SET NOCOUNT OFF
END

GO

IF NOT EXISTS (SELECT NAME FROM sys.objects WHERE TYPE = 'P' AND NAME = 'Deletebuzzwords')
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[Deletebuzzwords] AS RETURN')
END

GO

ALTER PROCEDURE [dbo].[Deletebuzzwords]
	 @id int
AS
BEGIN
	SET NOCOUNT ON
	
    DELETE FROM [dbo].[buzzwords]
	WHERE [id]=@id
    
    SET NOCOUNT OFF
END

GO

IF NOT EXISTS (SELECT NAME FROM sys.objects WHERE TYPE = 'P' AND NAME = 'GetAllFrombuzzwords')
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[GetAllFrombuzzwords] AS RETURN')
END

GO

ALTER PROCEDURE [dbo].[GetAllFrombuzzwords]    
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT
	[buzzwords].[id] AS 'id',
	[buzzwords].[name] AS 'name'
FROM [dbo].[buzzwords] [buzzwords]

	SET NOCOUNT OFF
END

GO

IF NOT EXISTS (SELECT NAME FROM sys.objects WHERE TYPE = 'P' AND NAME = 'GetbuzzwordsByid')
BEGIN
	EXEC('CREATE PROCEDURE [dbo].[GetbuzzwordsByid] AS RETURN')
END

GO

ALTER PROCEDURE [dbo].[GetbuzzwordsByid] 
	@id int
AS
BEGIN

	SET NOCOUNT ON
	
	SELECT
	[buzzwords].[id] AS 'id',
	[buzzwords].[name] AS 'name'
	FROM [dbo].[buzzwords] [buzzwords]
	WHERE [id]=@id

	SET NOCOUNT OFF
END

GO


