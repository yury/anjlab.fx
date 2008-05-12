if exists (select * from sysobjects where id = object_id(N'fx.fnGetEmptyRowSet') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.fnGetEmptyRowSet
go

/*
<summary>
	This funtion returns rowset with defined number of record
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>7\11\2007</date>

<example>
	select * from fx.fnGetEmptyRowSet(5)
</example>
*/

CREATE FUNCTION fx.fnGetEmptyRowSet(@Records int) 
RETURNS @Empty table(RecordId int) AS
BEGIN

WHILE @Records > 0 
BEGIN
	INSERT INTO @Empty VALUES(@Records)
	SET @Records = @Records - 1
END

RETURN

END

