if exists (select * from sysobjects where id = object_id(N'tools.fnCheckLeapYear') and xtype in (N'FN', N'IF', N'TF'))
drop function tools.fnCheckLeapYear
go

/*
This function returns 1 if year of input date is leap or 0 in other case
Author: Alex M. Zakharov
Example: 
print tools.fnCheckLeapYear(getDate())
*/

CREATE FUNCTION tools.fnCheckLeapYear(@dt datetime)
RETURNS bit AS
BEGIN
  declare @y int
  select @y = year(@dt)
  return case when (@y % 4 = 0) and ((@y % 100 <> 0) or (@y % 400 = 0)) then 1 else 0 end
END
GO

