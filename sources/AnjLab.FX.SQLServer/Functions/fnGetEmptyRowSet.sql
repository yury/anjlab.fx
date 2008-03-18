if exists (select * from sysobjects where id = object_id(N'fx.fnGetEmptyRowSet') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.fnGetEmptyRowSet
go

/*
This funtion returns rowset with defined number of record
Author: Alex M. Zakharov
Date: 07\11\2007
Example:
select * from fx.fnGetEmptyRowSet(5)
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

