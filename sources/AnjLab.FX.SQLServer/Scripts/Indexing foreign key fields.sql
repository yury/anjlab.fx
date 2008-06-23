/* 
<summary>
	Creates non-clustered indexes for all fields which are used in foreign keys,
	if they are not indexed. Index name template is ix<table name><column name>.
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>10/2/2008</date>
*/

set nocount on -- Hide (1 row affected) messages

declare ForeignKeys cursor for
	select fk.name as fkname, so.name as tabname, c.name as colname
	from sys.foreign_keys fk 
	inner join sys.objects so on so.object_id = fk.parent_object_id
	inner join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
	inner join sys.columns c on c.object_id = fkc.parent_object_id and c.column_id = fkc.parent_column_id

declare @fkname nvarchar(255), @tabname nvarchar(255), @colname nvarchar(255), @sql nvarchar(max)

open ForeignKeys
fetch next from ForeignKeys into @fkname, @tabname, @colname
while @@fetch_status = 0
begin
	if not exists (select * from sys.indexes where name = N'ix' + @tabname + @colname)
	begin
		begin try
			set @sql = N'Create nonclustered index ix'+ @tabname + @colname + N' on ' + @tabname + N'(' + @colname + N' asc)'
			exec sp_executesql @sql
			print N'   - Index ' + N'ix' + @tabname + @colname + N' is created.'
		end try
		begin catch
			print N'   - Error while creating index: ' + N'ix' + @tabname + @colname
		end catch
	end
	fetch next from ForeignKeys into @fkname, @tabname, @colname
end

close ForeignKeys
deallocate ForeignKeys

go