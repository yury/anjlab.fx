if exists (select * from sysobjects where id = object_id(N'fx.ScriptTable') and xtype in (N'P'))
drop procedure fx.ScriptTable
go
/*
<summary>
	Generating DDL definistion for given table 
<summary>

<remarks>
	Generates full DDL definition for given table and related objects, including the following
	- checking existence before creating the table
	- columns definition including collation, nullable, identity and computing options
	- constraints definition including primary, foreign keys, check and default constraints
	- indexes definition
	
	NOTE: the procedure DOES NOT support all possible DDL syntax constructions. List of known
	unsuported constructions is given below:
	- properties ralated to physical structure (files, filegroups, partitions, size)
	- including option for indexes
	- fulltext indexes
</remarks>

<author>
	Alex Zakharov
	Copyright © AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>12\01\2009</date>
		
<example>
select * from sys.check_constraints c
inner join sys.tables t on t.object_id = c.parent_object_id

	exec fx.ScriptTable 'fx', 'Countries'
</example>

<param name="Schema">Scripted object's schema</param>
<param name="Table">Scripted table</param>
<param name="ScriptCollation">Flag to determine should be column collation be scripted</param>
<param name="PrintResult">Flag to determine should resulted script be printed to output</param>
<param name="Result">DDL statement</param>
*/

create procedure fx.ScriptTable(
	@Schema sysname = 'fx', 
	@Table sysname = 'Countries',
	@ScriptCollation bit = 0,	
	@PrintResult bit = 1,
	@Result nvarchar(max) = null output
) as
begin

	declare
		@Column nvarchar(255),
		@Type nvarchar(255),
		@Lenght nvarchar(255),
		@Nullable nvarchar(255),
		@Identity nvarchar(255),
		@TextToPrint nvarchar(max),
		@ID int

	set @Result = space(0)

	select top 1 @ID = t.object_id 
	from sys.tables t inner join sys.schemas s on s.schema_id = t.schema_id
	where s.name = @Schema and t.name = @Table

	if @ID is null
	begin
		raiserror('Specified table is not found.', 16, 1)
		return -1
	end	

	set @Result = @Result + N'if not exists(' + nchar(10)
	set @Result = @Result + N'	select * ' + nchar(10)
	set @Result = @Result + N'	from sys.tables t ' + nchar(10)
	set @Result = @Result + N'	inner join sys.schemas s on s.schema_id = t.schema_id' + nchar(10)
	set @Result = @Result + N'	where s.name = ''' + @Schema + N''' and t.name = ''' + @Table + N''')' + nchar(10)
	set @Result = @Result + N'begin' + nchar(10) + nchar(10)
	set @Result = @Result + N'	create table [' + @Schema + N'].[' + @Table + N']('

	-- columns definitions
	select @Result = @Result + cast((
		select nchar(10) + '		' + [Column] + ',' as 'data()'
		from (
			select [Column] = 
			-- column name
				'[' + c.name + '] ' + 
			-- data type (or definition for computed column)
				case
					when cc.name is not null then N'as ' + cc.[definition]
					when cc.name is null and t.name in ('char', 'varchar', 'nchar', 'nvarchar') and c.max_length = -1 then t.name + N'(max)'
					when cc.name is null and t.name in ('char', 'varchar', 'nchar', 'nvarchar') and c.max_length > -1 then t.name +N'(' + ltrim(str(c.max_length)) + N')'
					when cc.name is null and t.name in ('decimal') then t.name + '(' + ltrim(str(c.max_length)) + N', ' + ltrim(str(c.[precision])) + N')'
					else t.name
					end +
			-- collation
				case when c.collation_name is not null and @ScriptCollation = 1 
					then N' collate ' + c.collation_name 
					else space(0)
					end +
			-- nullable
				case c.is_nullable when 0 then ' not null' else space(0) end +
			-- identity
				case when i.[Name] is not null 
					then N' identity(' + ltrim(cast(i.seed_value as nvarchar(10))) + N', ' + ltrim(cast(i.increment_value as nvarchar(10))) + N')'
					else space(0)
					end +
			-- default
				case when d.object_id is not null 
					then N' constraint [' + d.name + N'] default ' + d.[definition]
					else space(0)
					end 
			from sys.columns c
			inner join sys.types t on t.system_type_id = c.system_type_id
			left join sys.identity_columns i on i.column_id = c.column_id and i.object_id = @ID
			left join sys.default_constraints d on d.object_id = c.default_object_id
			left join sys.computed_columns cc on cc.object_id = c.object_id and cc.column_id = c.column_id
			where c.object_id = @ID and t.name != 'sysname'
			--order by c.column_id
		) as a for xml path('')) as nvarchar(max))

	-- primary keys definitions		
	select @Result = @Result + isnull(nchar(10) + N'		constraint [' + [Name] + 
		N'] ' + [Type] + [Clustered] + N' (' + substring([Columns], 1, len([Columns]) - 1) + N'),', space(0))
	from (
		select 
			[Name] = k.name, 
			[Type] = case k.type when 'PK' then N'primary key ' else N'unique ' end,
			[Clustered] = case when i.[type] = 1 then N'clustered' else N'nonclustered' end,
			[Columns] = (
				select [Name] + N',' as N'data()' from (
					select [Name] = N'[' + c.name + N']'  + case ic.is_descending_key when 0 then N' asc' else N' desc' end
					from sys.index_columns ic 
					inner join sys.columns c on c.object_id = ic.object_id and c.column_id = ic.column_id
					where ic.object_id = @ID and ic.index_id = i.index_id
					) as a for xml path('')
				)
		from sys.key_constraints k
		inner join sys.indexes i on i.object_id = k.parent_object_id and k.unique_index_id = i.index_id
		where k.parent_object_id = @ID 
			
		) as a

	-- foreign keys definitions
	select @Result = @Result + isnull((
		select nchar(10) + N'		' + [Column] + N',' as N'data()'
		from (
			select [Column] = 
				N'constraint [' +  fk.name + 
				N'] foreign key([' + c.name + N']) references [' + rs.name + N'].[' + rt.name + N']([' + rc.name + N'])' +
				case fk.delete_referential_action 
					when 1 then N' on delete cascade'
					when 2 then N' on delete set null'
					when 3 then N' on delete set default'
					else space(0)
				end +
				case fk.update_referential_action 
					when 1 then N' on update cascade'
					when 2 then N' on update set null'
					when 3 then N' on update set default'
					else space(0)
				end +
				case when fk.is_not_for_replication = 1 then N' for replication' else space(0) end
			from sys.foreign_keys fk
			inner join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
			inner join sys.columns c on c.object_id = fkc.parent_object_id and c.column_id = fkc.parent_column_id
			inner join sys.tables rt on rt.object_id = fkc.referenced_object_id
			inner join sys.schemas rs on rs.schema_id = rt.schema_id
			inner join sys.columns rc on rc.object_id = fkc.referenced_object_id and rc.column_id = fkc.referenced_column_id
			where fk.parent_object_id = @ID
		) as a  for xml path('')), SPACE(0))
	
	-- check constraints
	select @Result = @Result + isnull((
		select nchar(10) + N'		' + [Column] + N',' as N'data()'
		from (
			select [Column] = N'constraint [' + cc.name + N'] ' +
				case when cc.is_not_for_replication = 1 then N'not for replication ' else space(0) end +  
					N'check' + cc.[definition]
			from sys.check_constraints cc
			where cc.parent_object_id = @ID
		) as a  for xml path('')), space(0))

	
	set @Result = substring(@Result, 1, len(@Result) - 1) + nchar(10) + N'	)' + nchar(10)
		
	-- indexes definitions		
	select @Result = @Result + isnull(nchar(10) + 
		N'	create ' + [Unique] + [Clustered] + N' index [' + [Name] + 
		N'] on [' + @Schema + N'].[' + @Table + N']('  + substring([Columns], 1, len([Columns]) - 1) + N')', space(0))
	from (
		select 
			[Name] = i.name, 
			[Clustered] = case when i.[type] = 1 then N'clustered' else N'nonclustered' end,
			[Unique] = case when i.is_unique = 1 then N' unique ' else space(0) end,
			[Columns] = (
				select [Name] + N',' as N'data()' from (
					select [Name] = N'[' + c.name + N']'  + case ic.is_descending_key when 0 then N' asc' else N' desc' end
					from sys.index_columns ic 
					inner join sys.columns c on c.object_id = ic.object_id and c.column_id = ic.column_id
					where ic.object_id = @ID and ic.index_id = i.index_id
					) as a for xml path('')
				)
		from sys.indexes i 
		where 
			i.object_id = @ID and 
			i.is_primary_key = 0
		) as a
		
	set @Result = @Result + nchar(10) + nchar(10) + N'end' + nchar(10) + N'go' + nchar(10)-- + nchar(10)

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

	

	