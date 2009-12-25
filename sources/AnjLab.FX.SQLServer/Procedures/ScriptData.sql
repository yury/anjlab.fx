if exists (select * from sysobjects where id = object_id(N'fx.ScriptData') and xtype in (N'P'))
drop procedure fx.ScriptData
go
/*
<summary>
	Generating INSERT statements from existing data. 
<summary>

<remarks>
	Generated INSERTS can be executed to regenerate the data at some other location.
	This procedure is also useful to create a database setup, where in you can 
	script your data along with your table definitions.

	This procedure may not work with tables with too many columns.
	Results can be unpredictable with huge text columns or SQL Server 2000's sql_variant data types
	Whenever possible, Use @ScriptColumnsList parameter to ommit column list in the INSERT statement, for better results
	IMPORTANT: This procedure is not tested with internation data (Extended characters or Unicode). If needed
	you might want to convert the datatypes of character variables in this procedure to their respective unicode counterparts
	like nchar and nvarchar

	ALSO NOTE THAT THIS PROCEDURE IS NOT UPDATED TO WORK WITH NEW DATA TYPES INTRODUCED IN SQL SERVER 2005 
</remarks>

<author>
	Based on code of
	Narayana Vyas Kondreddi 
	http://vyaskn.tripod.com
	vyaskn@hotmail.com

	Acknowledgements:
		Divya Kalra	 For beta testing
		Mark Charsley	 For reporting a problem with scripting uniqueidentifier columns with null values
		Artur Zeygman	 For helping me simplify a bit of code for handling non-dbo owned tables
		Joris Laperre    For reporting a regression bug in handling text/ntext columns

	Copyright © 2002 Narayana Vyas Kondreddi. All rights reserved.
</author>

<date>1/17/2001</date>
		
<example>
	Example 1:	To generate INSERT statements for table 'fx.Countries':
	exec fx.ScriptData 'fx', 'Countries'

	Example 2: 	To ommit the column list in the INSERT statement: (Column list is included by default)
	IMPORTANT: If you have too many columns, you are advised to ommit column list, as shown below,
	to avoid erroneous results
	exec fx.ScriptData 'fx', 'Countries', @ScriptColumnsList = 0

	Example 3:	To generate INSERT statements for 'TheCopy' table from 'fx.Countries' table:
	exec fx.ScriptData 'fx', 'Countries', 'TheCopy'

	Example 4:	To generate INSERT statements for 'fx.Countries' table for only those short name 
	which contain the word 'A%' in them:
	NOTE: Do not complicate the from or where clause here. It's assumed that you are good with T-SQL if you are using this parameter
	exec fx.ScriptData 'fx', 'Countries', @Conditions = " from fx.Countries where ShortNameEng like 'A%'"

	Example 5: 	To specify that you want to include TIMESTAMP column's data as well in the INSERT statement:
	(By default TIMESTAMP column's data is not scripted)
	exec fx.ScriptData 'fx', 'Countries', @IncludeTimestamp = 1

	Example 6:	To print the debug information:
  	exec fx.ScriptData 'fx', 'Countries', @DebugMode = 1

	Example 7: 	If you are not the owner of the table, use @Schema parameter to specify the owner name
	To use this option, you must have select permissions on that table
	exec fx.ScriptData 'fx', 'Countries'

	Example 8: 	To generate INSERT statements for the rest of the columns excluding images
	When using this otion, DO NOT set @ScriptColumnsList parameter to 0.
	exec fx.ScriptData 'fx', 'Countries', @OmitImages = 1

	Example 9: 	To generate INSERT statements excluding (ommiting) IDENTITY columns:
	(By default IDENTITY columns are included in the INSERT statement)
	exec fx.ScriptData 'fx', 'Countries', @OmitIdentity = 1

	Example 10: To generate INSERT statements for the TOP 10 rows in the table:
	exec fx.ScriptData 'fx', 'Countries', @Top = 10

	Example 11: 	To generate INSERT statements with only those columns you want:
	exec fx.ScriptData 'fx', 'Countries', @IncludedColumns = "'CountryCode','Alfa2','Alfa3'"

	Example 12: 	To generate INSERT statements by omitting certain columns:
	exec fx.ScriptData 'fx', 'Countries', @ExcludedColumns = "'CountryCode','Alfa2','Alfa3'"

	Example 13:	To avoid checking the foreign key constraints while loading data with INSERT statements:
	exec fx.ScriptData 'fx', 'Countries', @DisableConstraints = 1

	Example 14: 	To exclude computed columns from the INSERT statement:
	exec fx.ScriptData 'fx', 'Countries', @OmitComputed = 1
</example>

<param name="Schema">Scripted object's schema</param>
<param name="Object">The table/view for which the INSERT statements will be generated using the existing data</param>
<param name="TargetObject">Use this parameter to specify a different table name into which the data will be inserted</param>
<param name="ScriptColumnsList">Use this parameter to include/ommit column list in the generated INSERT statement</param>
<param name="Conditions">Use this parameter to filter the rows based on a filter condition (using where)</param>
<param name="IncludeTimestamp">Specify 1 for this parameter, if you want to include the TIMESTAMP/ROWVERSION column's data in the INSERT statement</param>
<param name="DebugMode">If @DebugMode is set to 1, the SQL statements constructed by this procedure will be printed for later examination</param>
<param name="OmitImages">Use this parameter to generate INSERT statements by omitting the 'image' columns</param>
<param name="OmitIdentity">Use this parameter to ommit the identity columns</param>
<param name="Top">Use this parameter to generate INSERT statements only for the TOP n rows</param>
<param name="IncludedColumns">List of columns to be included in the INSERT statement</param>
<param name="ExcludedColumns">List of columns to be excluded from the INSERT statement</param>
<param name="DisableConstraints">When 1, disables foreign key constraints and enables them after the INSERT statements</param>
<param name="OmitComputed">When 1, computed columns will not be included in the INSERT statement</param>
<param name="PrintResult">Flag to determine should resulted script be printed to output</param>
<param name="Result">SQL statement</param>
*/

create procedure fx.ScriptData
(
	@Schema             sysname       = N'fx',		
	@Object             sysname       = N'Countries',  		    
	@TargetObject       sysname       = null, 	
	@ScriptColumnsList  bit           = 1,		    
	@Conditions         nvarchar(max) = null, 		    
	@IncludeTimestamp   bit           = 0, 		    
	@DebugMode          bit           = 0,			
	@OmitImages         bit           = 0,			
	@OmitIdentity       bit           = 0,		
	@Top                int           = null,			
	@IncludedColumns    nvarchar(max) = null,	
	@ExcludedColumns    nvarchar(max) = null,	
	@DisableConstraints bit           = 0,		
	@OmitComputed       bit           = 0,
	@PrintResult        bit           = 1,
	@Result             nvarchar(max) = null output	
)
as
begin

	set nocount on

	declare	@ColumnID     int, 		
			@ColumnsList  nvarchar(max), 
			@ColumnName   sysname, 
			@StartInsert  nvarchar(max), 
			@Type         sysname, 
			@Records nvarchar(max),	--This is the string that will be finally executed to generate INSERT statements
			@Identity     sysname		    --Will contain the IDENTITY column's name in the table

	set @Identity     = space(0)
	set @ColumnID     = 0
	set @ColumnName   = space(0)
	set @ColumnsList  = space(0)
	set @Records = space(0)
	set @Result       = space(0)

	--Making sure user only uses either @IncludedColumns or @ExcludedColumns
	if ((@IncludedColumns is not null) and (@ExcludedColumns is not null))
		begin
			raiserror('Use either @IncludedColumns or @ExcludedColumns. Do not use both the parameters at once',16,1)
			return -1 --Failure. Reason: Both @IncludedColumns and @ExcludedColumns parameters are specified
		end

	--Making sure the @IncludedColumns and @ExcludedColumns parameters are receiving values in proper format
	if ((@IncludedColumns is not null) and (patindex('''%''',@IncludedColumns) = 0))
		begin
			raiserror('Invalid use of @IncludedColumns property',16,1)
			print 'Specify column names surrounded by single quotes and separated by commas'
			print 'Eg: exec fx.ScriptData titles, @IncludedColumns = "''title_id'',''title''"'
			return -1 --Failure. Reason: Invalid use of @IncludedColumns property
		end

	if ((@ExcludedColumns is not null) and (patindex('''%''',@ExcludedColumns) = 0))
		begin
			raiserror('Invalid use of @ExcludedColumns property',16,1)
			print 'Specify column names surrounded by single quotes and separated by commas'
			print 'Eg: exec fx.ScriptData titles, @ExcludedColumns = "''title_id'',''title''"'
			return -1 --Failure. Reason: Invalid use of @ExcludedColumns property
		end

	--Checking to see if the database name is specified along wih the table name
	--Your database context should be local to the table for which you want to generate INSERT statements
	--specifying the database name is not allowed
	if (parsename(@Object, 3)) is not null
		begin
			raiserror('Do not specify the database name. Be in the required database and just specify the table name.',16,1)
			return -1 --Failure. Reason: Database name is specified along with the table name, which is not allowed
		end

	--Checking for the existence of 'user table' or 'view'
	--This procedure is not written to work on system tables
	--To script the data in system tables, just create a view on the system tables and script the view instead
	if @Schema is null
		begin
			if ((object_id(@Object,'U') is null) and (object_id(@Object,'V') is null)) 
				begin
					raiserror('Specified table is not found.',16,1)
					print 'Make sure you have select permission on that table.'
					return -1 --Failure. Reason: There is no user table or view with this name
				end
		end
	else
		begin
			if not exists (
				select * from sys.tables t
				inner join sys.schemas s on s.schema_id = t.schema_id
				where t.name = @Object and s.name = @Schema
			)
				begin
					raiserror('Specified table is not found.',16,1)
					print 'Make sure you have select permission on that table or view.'
					return -1 --Failure. Reason: There is no user table or view with this name		
				end
		end

	set @StartInsert = N'insert into ' + '[' + ltrim(rtrim(@Schema)) + N'].' + N'[' + rtrim(coalesce(@TargetObject, @Object)) + N']' 		

	--Get the first column's ID
	select @ColumnID = min(c.column_id)
	from sys.columns c
	inner join sys.tables t on t.object_id = c.object_id
	inner join sys.schemas s on s.schema_id = t.schema_id
	where t.name = @Object and s.name = @Schema

	--Loop through all the columns of the table, to get the column names and their data types
	while @ColumnID is not null
	begin

		select @ColumnName = quotename(c.name), @Type = tp.name
		from sys.columns c
		inner join sys.types tp on tp.system_type_id = c.system_type_id
		inner join sys.tables t on t.object_id = c.object_id
		inner join sys.schemas s on s.schema_id = t.schema_id
		where t.name = @Object and s.name = @Schema and c.column_id = @ColumnID and tp.name != 'sysname'
	
		if @IncludedColumns is not null --Selecting only user specified columns
		begin
			if charindex( '''' + substring(@ColumnName, 2, len(@ColumnName)-2) + '''', @IncludedColumns) = 0 
			begin
				goto SKIP_LOOP
			end
		end

		if @ExcludedColumns is not null --Selecting only user specified columns
		begin
			if charindex( '''' + substring(@ColumnName, 2, len(@ColumnName) - 2) + '''', @ExcludedColumns) <> 0 
			begin
				goto SKIP_LOOP
			end
		end

		--Making sure to output set identity_insert ON/OFF in case the table has an IDENTITY column
		if (select columnproperty( object_id(quotename(coalesce(@Schema, user_name())) + '.' + @Object), 
			substring(@ColumnName,2, len(@ColumnName) - 2), 'IsIdentity')) = 1 
		begin
			if @OmitIdentity = 0 --Determing whether to include or exclude the IDENTITY column
				set @Identity = @ColumnName
			else
				goto SKIP_LOOP			
		end
		
		--Making sure whether to output computed columns or not
		if @OmitComputed = 1
		begin
			if (select columnproperty( object_id(quotename(coalesce(@Schema, user_name())) + '.' + @Object), 
				substring(@ColumnName,2, len(@ColumnName) - 2), 'IsComputed')) = 1 
			begin
				goto SKIP_LOOP					
			end
		end
		
		--Tables with columns of IMAGE data type are not supported for obvious reasons
		if(@Type in ('image'))
			begin
				if (@OmitImages = 0)
					begin
						raiserror('Tables with image columns are not supported.',16,1)
						print 'Use @OmitImages = 1 parameter to generate INSERTs for the rest of the columns.'
						print 'DO NOT ommit Column List in the INSERT statements. If you ommit column list using @ScriptColumnsList=0, the generated INSERTs will fail.'
						return -1 --Failure. Reason: There is a column with image data type
					end
				else
					begin
					goto SKIP_LOOP
					end
			end

		--Determining the data type of the column and depending on the data type, the values part of
		--the INSERT statement is generated. Care is taken to handle columns with null values. Also
		--making sure, not to lose any data from flot, real, money, smallmomey, datetime columns
		set @Records = @Records + 
		case 
			when @Type in ('nchar','nvarchar') 
				then N'coalesce(''N'''''' + replace(rtrim(' + @ColumnName + N'),'''''''','''''''''''')+'''''''',''null'')'
			when @Type in ('char','varchar') 
				then 
					N'coalesce('''''''' + replace(rtrim(' + @ColumnName + N'),'''''''','''''''''''')+'''''''',''null'')'
			when @Type in ('datetime','smalldatetime') 
				then 
					N'coalesce('''''''' + rtrim(convert(char,' + @ColumnName + N',109))+'''''''',''null'')'
			when @Type in ('uniqueidentifier') 
				then  
					N'coalesce('''''''' + replace(convert(char(255),rtrim(' + @ColumnName + N')),'''''''','''''''''''')+'''''''',''null'')'
			when @Type in ('text', 'ntext') 
				then  
					N'coalesce('''''''' + replace(convert(char(8000),' + @ColumnName + N'),'''''''','''''''''''')+'''''''',''null'')'					
			when @Type in ('binary', 'varbinary') 
				then  
					N'coalesce(rtrim(convert(char,' + 'convert(int,' + @ColumnName + N'))),''null'')'  
			when @Type in ('timestamp', 'rowversion') 
				then  
					case 
						when @IncludeTimestamp = 0 
							then 
								N'''default''' 
							else 
								N'coalesce(rtrim(convert(char,' + 'convert(int,' + @ColumnName + '))),''null'')'  
					end
			when @Type in ('float','real','money','smallmoney')
				then
					N'coalesce(ltrim(rtrim(' + 'convert(char, ' +  @ColumnName  + ',2)' + ')),''null'')' 
			else 
				N'coalesce(ltrim(rtrim(' + 'convert(char, ' +  @ColumnName  + ')' + ')),''null'')' 
		end   + N'+' +  N''',''' + N' + '

		--Generating the column list for the INSERT statement
		set @ColumnsList = @ColumnsList +  @ColumnName + ','	

		SKIP_LOOP: --The label used in goto

		select @ColumnID = min(c.column_id)
		from sys.columns c
		inner join sys.tables t on t.object_id = c.object_id
		inner join sys.schemas s on s.schema_id = t.schema_id
		where t.name = @Object and s.name = @Schema and c.column_id > @ColumnID

	--Loop ends here!
	end

	--To get rid of the extra characters that got concatenated during the last run through the loop
	set @ColumnsList  = left(@ColumnsList, len(@ColumnsList) - 1)
	set @Records = left(@Records, len(@Records) - 6)

	if ltrim(@ColumnsList) = space(0) 
		begin
			raiserror('No columns specified. There should at least be one column to generate the output', 16, 1)
			return -1 --Failure. Reason: Looks like all the columns are ommitted using the @ExcludedColumns parameter
		end

	--Forming the final string that will be executed, to output the INSERT statements

	set @Records = 
		N'select @Value = (select nchar(9) + [Value] + char(10) as ''data()'' from (select ' +  
		case when @Top is null OR @Top < 0 then N'' else N' top ' + ltrim(str(@Top)) + space(1) end + 
		N' [Value] = ''' + rtrim(@StartInsert) + '''+' + 
		case @ScriptColumnsList when 1 then N'''(' + rtrim(@ColumnsList) +  N'''+' + N''')''' else space(0) end+ 
		N' +'' values(''+ ' +  @Records  + N'+'')''' + N' ' + 
		coalesce(@Conditions, N' from [' + ltrim(rtrim(@Schema)) + N'].[' + rtrim(@Object) + N'] (nolock)') +
		N') as a for xml path(''''))'
		
	/* In result of this complicated code there should be generated string like the following:
	
	select @Value = (
		select char(9) + [Value] --+ nchar(10) as 'data()' 
		from (
			select  [Value] = 
				'insert into [fx].[Countries]'+'(
					[CountryID],
					[CountryCode],
					[ShortNameRus],
					[FullNameRus],
					[ShortNameEng],
					[Alfa2],
					[Alfa3]'+')' +
				' values('+ 
					coalesce(ltrim(rtrim(convert(char, [CountryID]))),'null')+',' + 
					coalesce('''' + replace(rtrim([CountryCode]),'''','''''')+'''','null')+',' + 
					coalesce('N''' + replace(rtrim([ShortNameRus]),'''','''''')+'''','null')+',' + 
					coalesce('N''' + replace(rtrim([FullNameRus]),'''','''''')+'''','null')+',' + 
					coalesce('N''' + replace(rtrim([ShortNameEng]),'''','''''')+'''','null')+',' + 
					coalesce('''' + replace(rtrim([Alfa2]),'''','''''')+'''','null')+',' + 
					coalesce('''' + replace(rtrim([Alfa3]),'''','''''')+'''','null')+
				')'  
			from [fx].[Countries] (nolock) ) as a for xml path('')
		)
	*/

	--Determining whether to ouput any debug information
	if @DebugMode =1
		begin
			print '/* DEBUG INFORMATION */'
			print 'Beginning of the INSERT statement:'
			print @StartInsert
			print ''
			print 'The column list:'
			print @ColumnsList
			print ''
			print 'The select statement executed to generate the INSERTs'
			print @Records
			print ''
			print '/* End OF DEBUG INFORMATION */'
			print ''
		end
			
	set @Result = @Result +  N'set nocount on' + nchar(10)

	--Determining whether to print identity_insert or not
	if (@Identity <> space(0))
			set @Result = @Result +  
				N'set identity_insert ' + quotename(coalesce(@Schema,user_name())) + N'.' + quotename(@Object) + N' on'  + nchar(10)

	if @DisableConstraints = 1 and (object_id(quotename(coalesce(@Schema,user_name())) + '.' + @Object, 'U') is not null)
	begin
		set @Result = @Result +  N'alter table ' + quotename(@Schema) + N'.' + quotename(coalesce(@TargetObject, @Object)) + N' nocheck constraint all'  + nchar(10)
		set @Result = @Result +  N'go'  + nchar(10)
	end


	--All the hard work pays off here!!! You'll get your INSERT statements, when the next line executes!
	declare @Parameters nvarchar(50)
	set @Parameters = N'@Value nvarchar(max) output'
	exec sp_executesql @Records, @Parameters, @Records output
	set @Result = @Result + @Records

	if @DisableConstraints = 1 and (object_id(quotename(coalesce(@Schema,user_name())) + '.' + @Object, 'U') is not null)
	begin
		set @Result = @Result + N'alter table ' + quotename(@Schema) + N'.' + quotename(coalesce(@TargetObject, @Object)) + N' check constraints all ' + nchar(10)
		set @Result = @Result + N'go' + nchar(10)
	end

	if (@Identity <> space(0))
		set @Result = @Result + N'set identity_insert ' + quotename(coalesce(@Schema,user_name())) + N'.' + quotename(@Object) + N' off' + nchar(10)

	set @Result = @Result + N'set nocount off' + nchar(10)
	

	if @PrintResult = 1
	begin
		set @Records = @Result

		while charindex(nchar(10), @Records) - 1 > 0
		begin
			print substring(@Records, 1, charindex(nchar(10), @Records) - 1)
			set @Records = substring(@Records, charindex(nchar(10),@Records) + 1, len(@Records) - (charindex(nchar(10),@Records)) )
		end	
		print @Records
	end
	
	return 0 --Success. We are done!
end

go

