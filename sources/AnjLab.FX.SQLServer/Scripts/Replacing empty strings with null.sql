/* 
<summary>
	Replaces empty values with null for all string columns in a database. 
	The script does not affect not nullable and computed columns.
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>27/1/2009</date>

*/

set nocount on

declare 
	@SQL nvarchar(max),
	@SchemaName sysname,
	@TableName sysname,
	@ColumnName sysname

declare Fields cursor for
	select 
		s.name as SchemaName, 
		t.name as TableName, 
		c.name as ColumnName 
	from sys.tables t
	inner join sys.columns c on c.object_id = t.object_id
	inner join sys.types st on st.system_type_id = c.system_type_id
	inner join sys.schemas s on s.schema_id = t.schema_id
	where st.[name] in (N'char', N'varchar', N'text', N'nchar', N'nvarchar', N'ntext')
		and c.is_nullable = 1 and c.is_computed = 0
	order by s.name, t.name, c.name

print N'Replacing empty values with null in string columns'

open Fields
fetch next from Fields into @SchemaName, @TableName, @ColumnName
while @@fetch_status = 0 
begin
	begin try 
		set @SQL = N'update ' + @SchemaName + N'.' + @TableName + N' set ' + @ColumnName + 
			N' = null where ltrim(rtrim('  + @ColumnName + N')) = space(0)'
		exec sp_executesql @SQL
		print ' * Processed ' + @SchemaName + N'.' + @TableName + N'.' + @ColumnName
	end try
	begin catch
		print N'   Error occured while processing ' + @SchemaName + N'.' + @TableName + N'.' + @ColumnName
	end catch
	fetch next from Fields into @SchemaName, @TableName, @ColumnName
end

close Fields
deallocate Fields

print 'Done'