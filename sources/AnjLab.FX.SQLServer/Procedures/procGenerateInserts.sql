if exists (select * from sysobjects where id = object_id(N'fx.procGenerateInserts') and xtype in (N'P'))
drop procedure fx.procGenerateInserts
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
	Whenever possible, Use @include_column_list parameter to ommit column list in the INSERT statement, for better results
	IMPORTANT: This procedure is not tested with internation data (Extended characters or Unicode). If needed
	you might want to convert the datatypes of character variables in this procedure to their respective unicode counterparts
	like nchar and nvarchar

	ALSO NOTE THAT THIS PROCEDURE IS NOT UPDATED TO WORK WITH NEW DATA TYPES INTRODUCED IN SQL SERVER 2005 / YUKON
</remarks>

<author>
	Narayana Vyas Kondreddi 
	http://vyaskn.tripod.com
	vyaskn@hotmail.com

	Acknowledgements:
		Divya Kalra	 For beta testing
		Mark Charsley	 For reporting a problem with scripting uniqueidentifier columns with NULL values
		Artur Zeygman	 For helping me simplify a bit of code for handling non-dbo owned tables
		Joris Laperre    For reporting a regression bug in handling text/ntext columns

	Copyright © 2002 Narayana Vyas Kondreddi. All rights reserved.
</author>

<date>1/17/2001</date>
		
<example>
	Example 1:	To generate INSERT statements for table 'titles':
	EXEC fx.procGenerateScripts 'titles'

	Example 2: 	To ommit the column list in the INSERT statement: (Column list is included by default)
	IMPORTANT: If you have too many columns, you are advised to ommit column list, as shown below,
	to avoid erroneous results
	EXEC fx.procGenerateScripts 'titles', @include_column_list = 0

	Example 3:	To generate INSERT statements for 'titlesCopy' table from 'titles' table:
	EXEC fx.procGenerateScripts 'titles', 'titlesCopy'

	Example 4:	To generate INSERT statements for 'titles' table for only those titles 
	which contain the word 'Computer' in them:
	NOTE: Do not complicate the from or where clause here. It's assumed that you are good with T-SQL if you are using this parameter
	EXEC fx.procGenerateScripts 'titles', @from = "from titles where title like '%Computer%'"

	Example 5: 	To specify that you want to include TIMESTAMP column's data as well in the INSERT statement:
	(By default TIMESTAMP column's data is not scripted)
	EXEC fx.procGenerateScripts 'titles', @include_timestamp = 1

	Example 6:	To print the debug information:
  	EXEC fx.procGenerateScripts 'titles', @debug_mode = 1

	Example 7: 	If you are not the owner of the table, use @owner parameter to specify the owner name
	To use this option, you must have select permissions on that table
	EXEC fx.procGenerateScripts Nickstable, @owner = 'Nick'

	Example 8: 	To generate INSERT statements for the rest of the columns excluding images
	When using this otion, DO NOT set @include_column_list parameter to 0.
	EXEC fx.procGenerateScripts imgtable, @ommit_images = 1

	Example 9: 	To generate INSERT statements excluding (ommiting) IDENTITY columns:
	(By default IDENTITY columns are included in the INSERT statement)
	EXEC fx.procGenerateScripts mytable, @ommit_identity = 1

	Example 10: 	To generate INSERT statements for the TOP 10 rows in the table:
	EXEC fx.procGenerateScripts mytable, @top = 10

	Example 11: 	To generate INSERT statements with only those columns you want:
	EXEC fx.procGenerateScripts titles, @cols_to_include = "'title','title_id','au_id'"

	Example 12: 	To generate INSERT statements by omitting certain columns:
	EXEC fx.procGenerateScripts titles, @cols_to_exclude = "'title','title_id','au_id'"

	Example 13:	To avoid checking the foreign key constraints while loading data with INSERT statements:
	EXEC fx.procGenerateScripts titles, @disable_constraints = 1

	Example 14: 	To exclude computed columns from the INSERT statement:
	EXEC fx.procGenerateScripts MyTable, @ommit_computed_cols = 1
</example>

<param name="table_name">The table/view for which the INSERT statements will be generated using the existing data</param>
<param name="target_table">Use this parameter to specify a different table name into which the data will be inserted</param>
<param name="include_column_list">Use this parameter to include/ommit column list in the generated INSERT statement</param>
<param name="from">Use this parameter to filter the rows based on a filter condition (using where)</param>
<param name="include_timestamp">Specify 1 for this parameter, if you want to include the TIMESTAMP/ROWVERSION column's data in the INSERT statement</param>
<param name="debug_mode">If @debug_mode is set to 1, the SQL statements constructed by this procedure will be printed for later examination</param>
<param name="owner">Use this parameter if you are not the owner of the table</param>
<param name="ommit_images">Use this parameter to generate INSERT statements by omitting the 'image' columns</param>
<param name="ommit_identity">Use this parameter to ommit the identity columns</param>
<param name="top">Use this parameter to generate INSERT statements only for the TOP n rows</param>
<param name="cols_to_include">List of columns to be included in the INSERT statement</param>
<param name="cols_to_exclude">List of columns to be excluded from the INSERT statement</param>
<param name="disable_constraints">When 1, disables foreign key constraints and enables them after the INSERT statements</param>
<param name="ommit_computed_cols">When 1, computed columns will not be included in the INSERT statement</param>
*/

create procedure fx.procGenerateInserts
(
	@table_name varchar(776),  		-- The table/view for which the INSERT statements will be generated using the existing data
	@target_table varchar(776) = NULL, 	-- Use this parameter to specify a different table name into which the data will be inserted
	@include_column_list bit = 1,		-- Use this parameter to include/ommit column list in the generated INSERT statement
	@from varchar(800) = NULL, 		-- Use this parameter to filter the rows based on a filter condition (using where)
	@include_timestamp bit = 0, 		-- Specify 1 for this parameter, if you want to include the TIMESTAMP/ROWVERSION column's data in the INSERT statement
	@debug_mode bit = 0,			-- If @debug_mode is set to 1, the SQL statements constructed by this procedure will be printed for later examination
	@owner varchar(64) = NULL,		-- Use this parameter if you are not the owner of the table
	@ommit_images bit = 0,			-- Use this parameter to generate INSERT statements by omitting the 'image' columns
	@ommit_identity bit = 0,		-- Use this parameter to ommit the identity columns
	@top int = NULL,			-- Use this parameter to generate INSERT statements only for the TOP n rows
	@cols_to_include varchar(8000) = NULL,	-- List of columns to be included in the INSERT statement
	@cols_to_exclude varchar(8000) = NULL,	-- List of columns to be excluded from the INSERT statement
	@disable_constraints bit = 0,		-- When 1, disables foreign key constraints and enables them after the INSERT statements
	@ommit_computed_cols bit = 0		-- When 1, computed columns will not be included in the INSERT statement
	
)
as
begin


--Making sure user only uses either @cols_to_include or @cols_to_exclude
if ((@cols_to_include is not null) and (@cols_to_exclude is not null))
	begin
		raiserror('Use either @cols_to_include or @cols_to_exclude. Do not use both the parameters at once',16,1)
		return -1 --Failure. Reason: Both @cols_to_include and @cols_to_exclude parameters are specified
	end

--Making sure the @cols_to_include and @cols_to_exclude parameters are receiving values in proper format
if ((@cols_to_include is not null) and (patindex('''%''',@cols_to_include) = 0))
	begin
		raiserror('Invalid use of @cols_to_include property',16,1)
		print 'Specify column names surrounded by single quotes and separated by commas'
		print 'Eg: EXEC fx.procGenerateScripts titles, @cols_to_include = "''title_id'',''title''"'
		return -1 --Failure. Reason: Invalid use of @cols_to_include property
	end

if ((@cols_to_exclude is not null) and (patindex('''%''',@cols_to_exclude) = 0))
	begin
		raiserror('Invalid use of @cols_to_exclude property',16,1)
		print 'Specify column names surrounded by single quotes and separated by commas'
		print 'Eg: EXEC fx.procGenerateScripts titles, @cols_to_exclude = "''title_id'',''title''"'
		return -1 --Failure. Reason: Invalid use of @cols_to_exclude property
	end


--Checking to see if the database name is specified along wih the table name
--Your database context should be local to the table for which you want to generate INSERT statements
--specifying the database name is not allowed
if (parsename(@table_name,3)) is not null
	begin
		raiserror('Do not specify the database name. Be in the required database and just specify the table name.',16,1)
		return -1 --Failure. Reason: Database name is specified along with the table name, which is not allowed
	end

--Checking for the existence of 'user table' or 'view'
--This procedure is not written to work on system tables
--To script the data in system tables, just create a view on the system tables and script the view instead

if @owner is null
	begin
		if ((object_id(@table_name,'U') is null) and (object_id(@table_name,'V') is null)) 
			begin
				raiserror('User table or view not found.',16,1)
				print 'You may see this error, if you are not the owner of this table or view. In that case use @owner parameter to specify the owner name.'
				print 'Make sure you have select permission on that table or view.'
				return -1 --Failure. Reason: There is no user table or view with this name
			end
	end
else
	begin
		if not exists (select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @table_name and (TABLE_TYPE = 'BASE TABLE' OR TABLE_TYPE = 'VIEW') and TABLE_SCHEMA = @owner)
			begin
				raiserror('User table or view not found.',16,1)
				print 'You may see this error, if you are not the owner of this table. In that case use @owner parameter to specify the owner name.'
				print 'Make sure you have select permission on that table or view.'
				return -1 --Failure. Reason: There is no user table or view with this name		
			end
	end

--Variable declarations
declare	@Column_ID int, 		
		@Column_List varchar(8000), 
		@Column_Name varchar(128), 
		@Start_Insert varchar(786), 
		@Data_Type varchar(128), 
		@Actual_Values varchar(8000),	--This is the string that will be finally executed to generate INSERT statements
		@IDN varchar(128)		--Will contain the IDENTITY column's name in the table

--Variable Initialization
set @IDN = ''
set @Column_ID = 0
set @Column_Name = ''
set @Column_List = ''
set @Actual_Values = ''

if @owner is null 
	begin
		set @Start_Insert = 'insert into ' + '[' + rtrim(coalesce(@target_table,@table_name)) + ']' 
	end
else
	begin
		set @Start_Insert = 'insert ' + '[' + ltrim(rtrim(@owner)) + '].' + '[' + rtrim(coalesce(@target_table,@table_name)) + ']' 		
	end


--To get the first column's ID

select	@Column_ID = min(ORDINAL_POSITION) 	
from	INFORMATION_SCHEMA.COLUMNS (nolock) 
where 	TABLE_NAME = @table_name and
(@owner is null OR TABLE_SCHEMA = @owner)



--Loop through all the columns of the table, to get the column names and their data types
while @Column_ID is not null
	begin
		select 	@Column_Name = quotename(COLUMN_NAME), @Data_Type = DATA_TYPE 
		from 	INFORMATION_SCHEMA.COLUMNS (nolock) 
		where 	ORDINAL_POSITION = @Column_ID and 
		TABLE_NAME = @table_name and
		(@owner is null OR TABLE_SCHEMA = @owner)



		if @cols_to_include is not null --Selecting only user specified columns
		begin
			if charindex( '''' + substring(@Column_Name,2,len(@Column_Name)-2) + '''',@cols_to_include) = 0 
			begin
				goto SKIP_LOOP
			end
		end

		if @cols_to_exclude is not null --Selecting only user specified columns
		begin
			if charindex( '''' + substring(@Column_Name,2,len(@Column_Name)-2) + '''',@cols_to_exclude) <> 0 
			begin
				goto SKIP_LOOP
			end
		end

		--Making sure to output set identity_insert ON/OFF in case the table has an IDENTITY column
		if (select columnproperty( object_id(quotename(coalesce(@owner,user_name())) + '.' + @table_name),substring(@Column_Name,2,len(@Column_Name) - 2),'IsIdentity')) = 1 
		begin
			if @ommit_identity = 0 --Determing whether to include or exclude the IDENTITY column
				set @IDN = @Column_Name
			else
				goto SKIP_LOOP			
		end
		
		--Making sure whether to output computed columns or not
		if @ommit_computed_cols = 1
		begin
			if (select columnproperty( object_id(quotename(coalesce(@owner,user_name())) + '.' + @table_name),substring(@Column_Name,2,len(@Column_Name) - 2),'IsComputed')) = 1 
			begin
				goto SKIP_LOOP					
			end
		end
		
		--Tables with columns of IMAGE data type are not supported for obvious reasons
		if(@Data_Type in ('image'))
			begin
				if (@ommit_images = 0)
					begin
						raiserror('Tables with image columns are not supported.',16,1)
						print 'Use @ommit_images = 1 parameter to generate INSERTs for the rest of the columns.'
						print 'DO NOT ommit Column List in the INSERT statements. If you ommit column list using @include_column_list=0, the generated INSERTs will fail.'
						return -1 --Failure. Reason: There is a column with image data type
					end
				else
					begin
					goto SKIP_LOOP
					end
			end

		--Determining the data type of the column and depending on the data type, the values part of
		--the INSERT statement is generated. Care is taken to handle columns with NULL values. Also
		--making sure, not to lose any data from flot, real, money, smallmomey, datetime columns
		set @Actual_Values = @Actual_Values  +
		case 
			when @Data_Type in ('nchar','nvarchar') 
				then 
					'coalesce(''N'''''' + replace(rtrim(' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'
			when @Data_Type in ('char','varchar') 
				then 
					'coalesce('''''''' + replace(rtrim(' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'
			when @Data_Type in ('datetime','smalldatetime') 
				then 
					'coalesce('''''''' + rtrim(convert(char,' + @Column_Name + ',109))+'''''''',''NULL'')'
			when @Data_Type in ('uniqueidentifier') 
				then  
					'coalesce('''''''' + replace(convert(char(255),rtrim(' + @Column_Name + ')),'''''''','''''''''''')+'''''''',''NULL'')'
			when @Data_Type in ('text','ntext') 
				then  
					'coalesce('''''''' + replace(convert(char(8000),' + @Column_Name + '),'''''''','''''''''''')+'''''''',''NULL'')'					
			when @Data_Type in ('binary','varbinary') 
				then  
					'coalesce(rtrim(convert(char,' + 'convert(int,' + @Column_Name + '))),''NULL'')'  
			when @Data_Type in ('timestamp','rowversion') 
				then  
					case 
						when @include_timestamp = 0 
							then 
								'''DEFAULT''' 
							else 
								'coalesce(rtrim(convert(char,' + 'convert(int,' + @Column_Name + '))),''NULL'')'  
					end
			when @Data_Type in ('float','real','money','smallmoney')
				then
					'coalesce(ltrim(rtrim(' + 'convert(char, ' +  @Column_Name  + ',2)' + ')),''NULL'')' 
			else 
				'coalesce(ltrim(rtrim(' + 'convert(char, ' +  @Column_Name  + ')' + ')),''NULL'')' 
		end   + '+' +  ''',''' + ' + '
		
		--Generating the column list for the INSERT statement
		set @Column_List = @Column_List +  @Column_Name + ','	

		SKIP_LOOP: --The label used in goto

		select 	@Column_ID = MIN(ORDINAL_POSITION) 
		from 	INFORMATION_SCHEMA.COLUMNS (NOLOCK) 
		where 	TABLE_NAME = @table_name and 
		ORDINAL_POSITION > @Column_ID and
		(@owner is null OR TABLE_SCHEMA = @owner)


	--Loop ends here!
	end

--To get rid of the extra characters that got concatenated during the last run through the loop
set @Column_List = LEFT(@Column_List,len(@Column_List) - 1)
set @Actual_Values = LEFT(@Actual_Values,len(@Actual_Values) - 6)

if ltrim(@Column_List) = '' 
	begin
		raiserror('No columns to select. There should at least be one column to generate the output',16,1)
		return -1 --Failure. Reason: Looks like all the columns are ommitted using the @cols_to_exclude parameter
	end

--Forming the final string that will be executed, to output the INSERT statements
if (@include_column_list <> 0)
	begin
		set @Actual_Values = 
			'select ' +  
			case when @top is null OR @top < 0 then '' else ' TOP ' + ltrim(STR(@top)) + ' ' end + 
			'''' + rtrim(@Start_Insert) + 
			' ''+' + '''(' + rtrim(@Column_List) +  '''+' + ''')''' + 
			' +''values(''+ ' +  @Actual_Values  + '+'')''' + ' ' + 
			coalesce(@from,' from ' + case when @owner is null then '' else '[' + ltrim(rtrim(@owner)) + '].' end + '[' + rtrim(@table_name) + ']' + '(NOLOCK)')
	end
else if (@include_column_list = 0)
	begin
		set @Actual_Values = 
			'select ' + 
			case when @top is null OR @top < 0 then '' else ' TOP ' + ltrim(STR(@top)) + ' ' end + 
			'''' + rtrim(@Start_Insert) + 
			' '' +''values(''+ ' +  @Actual_Values + '+'')''' + ' ' + 
			coalesce(@from,' from ' + case when @owner is null then '' else '[' + ltrim(rtrim(@owner)) + '].' end + '[' + rtrim(@table_name) + ']' + '(NOLOCK)')
	end	

--Determining whether to ouput any debug information
if @debug_mode =1
	begin
		print '/*****START OF DEBUG INFORMATION*****'
		print 'Beginning of the INSERT statement:'
		print @Start_Insert
		print ''
		print 'The column list:'
		print @Column_List
		print ''
		print 'The select statement executed to generate the INSERTs'
		print @Actual_Values
		print ''
		print '*****end OF DEBUG INFORMATION*****/'
		print ''
	end
		
print 'set nocount on'

--Determining whether to print identity_insert or not
if (@IDN <> '')
	begin
		print 'set identity_insert ' + quotename(coalesce(@owner,user_name())) + '.' + quotename(@table_name) + ' ON'
		print 'go'
		print ''
	end


if @disable_constraints = 1 and (object_id(quotename(coalesce(@owner,user_name())) + '.' + @table_name, 'U') is not null)
	begin
		if @owner is null
			begin
				select 	'alter table ' + quotename(coalesce(@target_table, @table_name)) + ' nocheck constraint all' AS '--Code to disable constraints temporarily'
			end
		else
			begin
				select 	'alter table ' + quotename(@owner) + '.' + quotename(coalesce(@target_table, @table_name)) + ' nocheck constraint all' AS '--Code to disable constraints temporarily'
			end

		print 'go'
	end

print ''
print 'print ''Inserting values into ' + '[' + rtrim(coalesce(@target_table,@table_name)) + ']' + ''''


--All the hard work pays off here!!! You'll get your INSERT statements, when the next line executes!
EXEC (@Actual_Values)

print 'print ''Done'''
print ''


if @disable_constraints = 1 and (object_id(quotename(coalesce(@owner,user_name())) + '.' + @table_name, 'U') is not null)
	begin
		if @owner is null
			begin
				select 	'alter table ' + quotename(coalesce(@target_table, @table_name)) + ' CHECK CONSTRAINT ALL'  AS '--Code to enable the previously disabled constraints'
			end
		else
			begin
				select 	'alter table ' + quotename(@owner) + '.' + quotename(coalesce(@target_table, @table_name)) + ' CHECK CONSTRAINT ALL' AS '--Code to enable the previously disabled constraints'
			end

		print 'go'
	end

print ''
if (@IDN <> '')
	begin
		print 'set identity_insert ' + quotename(coalesce(@owner,user_name())) + '.' + quotename(@table_name) + ' OFF'
		print 'go'
	end

print 'set nocount off'

return 0 --Success. We are done!
end

go






