/* 
<summary>
	Generates stored procedures to delete records for each table taking into 
	accout table relationships.  Pocedure name template is proc<table name>Delete.
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>17/1/2009</date>

<param name="Action">What should be done with generated code: 0 print in output, 1 execute</param>
*/

set nocount on

-- input parameters
declare	@Action bit
set @Action = 0 -- Execute
	
declare 
	@SQL nvarchar(max),
	@TableID int, 
	@TableName nvarchar(255), 
	@RelatedTable nvarchar(255),
	@SchemaName nvarchar(255),
	@RelatedSchema nvarchar(255),
	@RelatedColumn nvarchar(255),
	@PrimaryKeyName nvarchar(255),
	@RelatedTableID int

declare @Relations table (
	TableID int,
	TableName nvarchar(255),
	SchemaName nvarchar(255),
	ColumnName nvarchar(255),
	Processed bit default 0
)

declare Tables cursor for
	select t.object_id, s.[Name] as SchemaName, t.[Name] as TableName 
	from sys.tables t
	inner join sys.schemas s on s.schema_id = t.schema_id
	order by t.name

print 'Generation strored procedures to delete records'

open Tables
fetch next from Tables into @TableID, @SchemaName, @TableName

while @@fetch_status = 0 
begin

	begin try 
		set @SQL = N'
			if exists (select * from sysobjects where id = object_id(N''' + 
			@SchemaName + N'.proc' + @TableName + N'Delete'') and xtype in (N''P''))
			drop procedure ' + @SchemaName + N'.' + @TableName + N'Delete'

		if @Action = 1 exec sp_executesql @SQL
		else print @SQL

		set @SQL = N'
			create procedure ' + @SchemaName + N'.proc' + @TableName + N'Delete(@Key int) as
			begin

				declare @CurrentKey int
				declare @Keys table ([Key] int, Processed bit default 0)
			'

		insert into @Relations(TableID, ColumnName, TableName, SchemaName)
			select t.object_id as TableID, c.name as ColumnName, t.Name as TableName, s.Name as SchemaName 
			from sys.foreign_keys fk
			inner join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
			inner join sys.columns c on c.object_id = fkc.parent_object_id 
				and c.column_id = fkc.parent_column_id
			inner join sys.tables t on t.object_id = fk.parent_object_id		
			inner join sys.schemas s on s.schema_id = t.schema_id
			where fk.referenced_object_id = @TableID

		while exists(select top 1 * from @Relations where Processed = 0)
		begin

			select top 1 
				@RelatedTableID = TableID,
				@RelatedSchema  = SchemaName,
				@RelatedTable   = TableName,
				@RelatedColumn  = ColumnName
			from @Relations where Processed = 0

			select @PrimaryKeyName = c.name
			from sys.indexes i 
			inner join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id		
			inner join sys.columns c on c.object_id = i.object_id and c.column_id = ic.column_id
			inner join sys.tables t on t.object_id = i.object_id
			where i.is_primary_key = 1  and i.object_id = @RelatedTableID

			set @SQL = @SQL + N'

				insert into @Keys([Key]) select [' + @PrimaryKeyName + N'] from ' + 
				@RelatedSchema + N'.' + @RelatedTable + N' where [' + @RelatedColumn + N'] = @Key
				while exists(select top 1 [Key] from @Keys where Processed = 0)
				begin
					select top 1 @CurrentKey = [Key] from @Keys where Processed = 0
					exec ' + @RelatedSchema + N'.' + @RelatedTable + N'Delete @CurrentKey
					update @Keys set Processed = 1 where [Key] = @CurrentKey
				end
				delete from @Keys
			'
			update @Relations set Processed = 1 
			where TableName = @RelatedTable and SchemaName = @RelatedSchema and 
				ColumnName = @RelatedColumn

		end

		delete from @Relations

		select @PrimaryKeyName = c.name
		from sys.indexes i 
		inner join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id		
		inner join sys.columns c on c.object_id = i.object_id and c.column_id = ic.column_id
		inner join sys.tables t on t.object_id = i.object_id
		where i.is_primay_key = 1  and i.object_id = @TableID

		set @SQL = @SQL + N'
				delete from ' + @SchemaName + N'.' + @TableName + N' where [' + @PrimaryKeyName + N'] = @Key
				return @@Error
			
			end'

		if @Action = 1 exec sp_executesql @SQL
		else print @SQL

		print ' * Generated procedure '@SchemaName + N'.proc' + @TableName + N'Delete'
	end try
	begin catch
		print ' * Error occured while creating procedure '@SchemaName + N'.proc' + @TableName + N'Delete'
	end catch

	fetch next from Tables into @TableID, @SchemaName, @TableName

end
close Tables
deallocate Tables

print 'Done'