if exists (select * from sysobjects where id = object_id(N'fx.fnCheckLeapYear') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.fnCheckLeapYear
go

/*
<summary>
	Returns 1 if year of input date is leap or 0 in other case
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<example>
	print fx.fnCheckLeapYear(getDate())
</example>
*/

create function fx.fnCheckLeapYear(@dt datetime)
returns bit as
begin
  set nocount on
  declare @y int
  select @y = year(@dt)
  return case when (@y % 4 = 0) and ((@y % 100 <> 0) or (@y % 400 = 0)) then 1 else 0 end
end
GO

