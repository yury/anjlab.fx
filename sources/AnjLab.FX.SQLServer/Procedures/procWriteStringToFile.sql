if exists (select * from sysobjects where id = object_id(N'fx.procWriteStringToFile') and xtype in (N'P'))
drop procedure fx.procWriteStringToFile
go

/*
<summary>
	Writes given string to existing text file or creates new one.
</summary>

<remarks>
 Using OLE automation procedures mast be switched on. Use the following code to
 turn it on:

	sp_configure 'show advanced options', 1
	go
	reconfigure
	go
	sp_configure 'Ole Automation Procedures', 1
	go
	reconfigure
	go
</remarks>

<author>
	Phil Factor
	http://www.simple-talk.com/sql/t-sql-programming/reading-and-writing-files-in-sql-server-using-t-sql/
</author>

<example>
	exec fx.procWriteStringToFile N'This is an example', N'c:\', N'example.txt'
</example>

<param name="Text">String to search</param>
*/

create procedure fx.procWriteStringToFile(
	@String nvarchar(max), --8000 in SQL Server 2000
	@Path nvarchar(255),
	@Filename nvarchar(100)

)
as
begin
declare  
	@objFileSystem int,
    @objTextStream int,
	@objErrorObject int,
	@strErrorMessage Varchar(1000),
	@Command varchar(1000),
	@hr int,
	@fileAndPath varchar(80),
	@Source varchar(255),
	@Description Varchar(255),
	@Helpfile Varchar(255),
	@HelpID int


set nocount on

select @strErrorMessage='opening the File System Object'
execute @hr = sp_OACreate  'Scripting.FileSystemObject' , @objFileSystem out
select @FileAndPath=@path+'\'+@filename

if @hr=0 
begin

	select @objErrorObject=@objFileSystem , @strErrorMessage = 'creating file "' + @FileAndPath + '"'
	execute @hr = sp_OAMethod   @objFileSystem   , 'CreateTextFile'	, @objTextStream out, @FileAndPath,2, True
	Select @objErrorObject=@objTextStream, @strErrorMessage = 'writing to the file "' + @FileAndPath + '"'
	execute @hr = sp_OAMethod  @objTextStream, 'Write', Null, @String
	select @objErrorObject=@objTextStream, @strErrorMessage = 'closing the file "' + @FileAndPath + '"'
	execute @hr = sp_OAMethod  @objTextStream, 'Close'

end else begin

	execute sp_OAGetErrorInfo  @objErrorObject, 
		@source output,@Description output,@Helpfile output,@HelpID output
	select @strErrorMessage = 'Error whilst ' + coalesce(@strErrorMessage, 'unknown action')
			+ ', ' + coalesce(@Description,'')
	raiserror (@strErrorMessage,16,1)

end
execute  sp_OADestroy @objTextStream
execute sp_OADestroy @objTextStream

end