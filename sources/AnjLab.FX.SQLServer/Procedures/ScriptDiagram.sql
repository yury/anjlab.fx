if exists (select * from sysobjects where id = object_id(N'fx.ScriptDiagram') and xtype in (N'P'))
drop procedure fx.ScriptDiagram
GO
/*
<summary>
	Script SQL Server diagrams 
</summary>

<remarks>
	Helpful Articles
	
	1) Upload / Download to Sql 2005
	http://staceyw.spaces.live.com/blog/cns!F4A38E96E598161E!404.entry
	
	2) MSDN: Using Large-Value Data Types
	http://msdn2.microsoft.com/en-us/library/ms178158.aspx
	
	3) "original" Script, Save, Export SQL 2000 Database Diagrams
	http://www.thescripts.com/forum/thread81534.html
	http://groups-beta.google.com/group/comp.databases.ms-sqlserver/browse_frm/thread/ca9a9229d06a56f9?dq=&hl=en&lr=&ie=UTF-8&oe=UTF-8&prev=/groups%3Fdq%3D%26num%3D25%26hl%3Den%26lr%3D%26ie%3DUTF-8%26oe%3DUTF-8%26group%3Dcomp.databases.ms-sqlserver%26start%3D25
</remarks>

<author>
	Based on code of
	Craig Dunn
	inspired by usp_ScriptDatabaseDiagrams for Sql Server 2000 by Clay Beatty
</author>

<example>
	NOTE: Scalar-valued Function fx.ConvertVarbinaryTovarcharHex must exist before this script is run
	exec fx.ScriptDiagram 'dbo', 'General'
</example>

<param name="Schema">Object's schema name</param>
<param Name="Object">Name of the diagram</param>
<param name="PrintResult">Flag to determine should resulted script be printed to output</param>
<param name="Result">SQL statement</param>

*/

create procedure fx.ScriptDiagram
(
	@Schema  sysname = N'dbo',
	@Object  sysname = N'General',
	@PrintResult bit = 1,
	@Result nvarchar(max) = null output
)
as
begin

	set nocount on

	declare
		@DiagramID		int,
		@Index			int,
		@Size			int,
		@Chunk			int,
		@TextToPrint nvarchar(max)

	set @Result = space(0)
	-- set start Index, and Chunk 'constant' value
	set @Index = 1  -- 
	set @Chunk = 32	-- values that work: 2, 6
					-- values that fail: 15,16, 64

	-- Get PK DiagramID using the diagram's Name (which is what the user is familiar with)
	select 
		@DiagramID = diagram_id,	
		@Size = datalength([definition]) 
	from sysdiagrams 
	where [Name] = @Object

	if @DiagramID is null
	begin
		raiserror('Specified diagram is not found.', 16, 1)
		return -1
	end
	else -- Diagram exists
	begin
		-- Now with the DiagramID, do all the work
		set @Result = @Result + N'/*' + nchar(10)
		set @Result = @Result + N'<summary>' + nchar(10)
		set @Result = @Result + N'	Restoring diagram ''' + @Object + '''' + nchar(10)
		set @Result = @Result + N'</summary>' + nchar(10)
		set @Result = @Result + N'<remarks>' + nchar(10)
		set @Result = @Result + N'	Will attempt to create [sysdiagrams] table if it doesn''t already exist' + nchar(10)
		set @Result = @Result + N'</remarks>' + nchar(10)
		set @Result = @Result + N'<generated>' + left(convert(varchar(23), getDate(), 121), 16) + '</generated>' + nchar(10)
		set @Result = @Result + N'*/' + nchar(10)
		set @Result = @Result + N'if not exists (select * from sys.tables where name = ''sysdiagrams'')' + nchar(10)
		set @Result = @Result + N'begin' + nchar(10)
		set @Result = @Result + N'	-- creates the first time you add a diagram to a 2005/2008 database' + nchar(10)
		set @Result = @Result + N'		create table[dbo].[sysdiagrams](' + nchar(10)
		set @Result = @Result + N'			[Name] [sysName] not null,' + nchar(10)
		set @Result = @Result + N'			[principal_id] [int] not null,' + nchar(10)
		set @Result = @Result + N'			[DiagramID] [int] identity(1,1) not null,' + nchar(10)
		set @Result = @Result + N'			[version] [int] null,' + nchar(10)
		set @Result = @Result + N'			[definition] [varbinary](max) null,' + nchar(10)
		set @Result = @Result + N'			primary key clustered([DiagramID] asc)with (pad_Index  = off, ignore_dup_key = off)  ,' + nchar(10)
		set @Result = @Result + N'			constraint [UK_principal_Name] unique nonclustered ([principal_id] asc, [Name] asc)' + nchar(10)
		set @Result = @Result + N'		) ' + nchar(10)
		set @Result = @Result + N'		exec sys.sp_addextendedproperty @Object=N''microsoft_database_tools_support'', @value=1 , @level0type=N''SCHEMA'',@level0Name=N''dbo'', @level1type=N''TABLE'',@level1Name=N''sysdiagrams''' + nchar(10)
		set @Result = @Result + N'end' + nchar(10)
		set @Result = @Result + N'-- Target table will now exist, if it didn''t before' + nchar(10)
		set @Result = @Result + N'set nocount on' + nchar(10)
		set @Result = @Result + N'declare @newid int' + nchar(10)
		set @Result = @Result + N'	/*' + nchar(10)
		set @Result = @Result + N'		Output the insert that _creates_ the diagram record, with a non-null [definition],' + nchar(10)
		set @Result = @Result + N'		important because .WRITE *cannot* be called against a null value (in the WHILE loop)' + nchar(10)
		set @Result = @Result + N'		so we insert 0x so that .WRITE has something to append to...' + nchar(10)
		set @Result = @Result + N'	*/' + nchar(10)
		set @Result = @Result + N' ' + nchar(10)
		set @Result = @Result + N'begin try' + nchar(10)

		select top 1 @Result = @Result + N'insert into sysdiagrams ([Name], [principal_id], [version], [definition]) values (''' + 
			[Name] + N''', ' + cast (principal_id as nvarchar(100)) + N', ' + cast([version] as nvarchar(100)) + ', 0x)' + char(13) + char(10)
		from sysdiagrams where diagram_id = @DiagramID

		set @Result = @Result + N'		set @newid = scope_identity()' + nchar(10)
		set @Result = @Result + N'end try' + nchar(10)
		set @Result = @Result + N'begin catch' + nchar(10)
		set @Result = @Result + N'	print N''Error occured: ''  + Error_Message()' + nchar(10)
		set @Result = @Result + N'	return' + nchar(10)
		set @Result = @Result + N'end catch' + nchar(10)
		set @Result = @Result + N'begin try' + nchar(10)

		while @Index < @Size
		begin
			-- Output as many UPDATE statements as required to append all the diagram binary
			-- data, represented as hexadecimal strings
			select @Result = @Result + 
				 N'	update sysdiagrams set [definition] .Write ('
				+ N' 0x' + upper(fx.ConvertVarbinaryTovarcharHex (substring ([definition], @Index, @Chunk)))
				+ N', null, 0) where diagram_id = @newid -- Index:' + cast(@Index as nvarchar(100))
				+ nchar(10)
			from	sysdiagrams 
			where	diagram_id = @DiagramID

			set @Index = @Index + @Chunk
		end
		set @Result = @Result + N'end try' + nchar(10)
		set @Result = @Result + N'begin catch' + nchar(10)
		set @Result = @Result + N'	-- if we got here, the [definition] updates didn''t complete, so delete the diagram row' + nchar(10)
		set @Result = @Result + N'	-- (and hope it doesn''t fail!)' + nchar(10)
		set @Result = @Result + N'	delete from sysdiagrams where DiagramID = @newid' + nchar(10)
		set @Result = @Result + N'	print N''Error occured: '' + Error_Message() + '' ''' + nchar(10)
		set @Result = @Result + N'	return' + nchar(10)
		set @Result = @Result + N'end catch' + nchar(10)
	end

	if @PrintResult = 1 
	begin
		set @TextToPrint = @Result

		while charindex(nchar(10), @TextToPrint) - 1 > 0
		begin
			print substring(@TextToPrint, 1, charindex(nchar(10), @TextToPrint) - 1)
			set @TextToPrint = substring(@TextToPrint, charindex(nchar(10),@TextToPrint) + 1, len(@TextToPrint) - (charindex(nchar(10),@TextToPrint)) )
		end	
		print @TextToPrint
	end

	return 0
end