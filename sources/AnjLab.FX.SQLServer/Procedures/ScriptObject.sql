if exists (select * from sysobjects where id = object_id(N'fx.ScriptObject') and xtype in (N'P'))
drop procedure fx.ScriptObject
go
/*
<summary>
	Generating DDL definistion for given object 
<summary>

<remarks>
	This alternative of sp_helptext. Generates full DDL definition for given objects. 
	Supported object types are
	- user view
	- stored procedure
	- function
	- trigger
	
	NOTE: the procedure DOES NOT support all possible DDL syntax constructions. 
	Look for restriction for scripting tables at fx.ScriptTable
</remarks>

<author>
	Alex Zakharov
	Copyright © AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>12\09\2009</date>
		
<example>
	exec fx.ScriptObject 'fx', 'ScriptObject'
</example>

<param name="Schema">Scripted object's schema</param>
<param name="Object">Scripted object's name</param>
<param name="PrintResult">Flag to determine should resulted script be printed to output</param>
<param name="Result">DDL statement</param>
*/

create procedure fx.ScriptObject(
	@Schema sysname = 'fx', 
	@Object sysname = 'ScriptObject',
	@PrintResult bit = 1,
	@Result nvarchar(max) = null output
) as
begin

	declare 
		@ObjectID int, 
		@ObjectText nvarchar(max), 
		@ObjectType sysname, 
		@Encrypted bit,
		@TextToPrint nvarchar(max)
	
	set @Result = space(0)

	select top 1 @ObjectID = o.object_id, @ObjectType = o.type  
	from sys.objects o
	inner join sys.schemas s on s.schema_id = o.schema_id
	where  o.name = @Object and s.name = @Schema

	if @ObjectID is null
	begin
		raiserror('Specified object is not found.', 16, 1)
		return -1
	end

	if @ObjectType not in ('P', 'V', 'FN', 'TF', 'TR')
	begin
		raiserror('Specified object has unsupported type. ', 16, 1)	
		print 'Supported types are view, stored procedure, function, trigger.'
		return -1
	end

	select top 1 @ObjectText = [text], @Encrypted  = encrypted
	from syscomments where id = @ObjectID

	if @ObjectText is null and @Encrypted is null
	begin
		raiserror('Definition of specified object is not found.', 16, 1)	
		return -1
	end

	if @Encrypted = 1
	begin
		raiserror('Definition of specified object is encrypted.', 16, 1)	
		print 'NOTE. You can use fx.ScriptEncryptedObject to get definition of encrypted object'
		return -1
	end

	set @Result = @Result + N'if exists(' + nchar(10)
	set @Result = @Result + N'	select * ' + nchar(10)
	set @Result = @Result + N'	from sys.objects o' + nchar(10)
	set @Result = @Result + N'	inner join sys.schemas s on s.schema_id = o.schema_id' + nchar(10)
	set @Result = @Result + N'	where o.name = ''' + @Object + N''' and s.name = ''' + @Schema + N''' and o.type = ''' + @ObjectType + N''')' + nchar(10)
	set @Result = @Result + N'drop ' + 
		case @ObjectType 
			when 'P' then N'procedure'
			when 'V' then N'view'
			when 'FN' then N'function'
			when 'TF' then N'function'
			when 'TR' then N'trigger'
		end +
		N' [' + @Schema + N'].[' + @Object + N']' + nchar(10)
	set @Result = @Result + N'go' + nchar(10) + nchar(10)
	set @Result = @Result + @ObjectText + nchar(10)
	set @Result = @Result + N'go'+ nchar(10)

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