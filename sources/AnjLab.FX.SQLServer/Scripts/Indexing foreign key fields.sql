/* 
<summary>
	This script creates non-clustered indexes for all fields which are used in foreign keys,
	if they are not indexed. Index name template is IX_<table name><column name>.
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>10/2/2008</date>
*/

SET NOCOUNT ON -- Hide (1 row affected) messages

DECLARE ForeignKeys CURSOR FOR
	select fk.name as fkname, so.name as tabname, c.name as colname
	from sys.foreign_keys fk 
	inner join sys.objects so on so.object_id = fk.parent_object_id
	inner join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
	inner join sys.columns c on c.object_id = fkc.parent_object_id and c.column_id = fkc.parent_column_id

DECLARE @fkname nvarchar(255), @tabname nvarchar(255), @colname nvarchar(255), @sql nvarchar(max)

OPEN ForeignKeys
FETCH NEXT FROM ForeignKeys INTO @fkname, @tabname, @colname
WHILE @@FETCH_STATUS = 0
BEGIN
	IF not exists (select * from sys.indexes where name = N'ix' + @tabname + @colname)
	BEGIN
		BEGIN TRY
			SET @sql = N'Create nonclustered index ix'+ @tabname + @colname + N' on ' + @tabname + N'(' + @colname + N' asc)'
			EXEC sp_executesql @sql
			print N'   - Index ' + N'ix' + @tabname + @colname + N' is created.'
		END TRY
		BEGIN CATCH
			print N'   - Error while creating index: ' + N'ix' + @tabname + @colname
		END CATCH
	END
	FETCH NEXT FROM ForeignKeys INTO @fkname, @tabname, @colname
END

CLOSE ForeignKeys
DEALLOCATE ForeignKeys

go