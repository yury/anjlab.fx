if exists (select * from sysobjects where id = object_id(N'fx.GetDateFromDateTime') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.GetDateFromDateTime
go

/*
<summary>
	Returns date value from given datetime variable 
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
	The main idea was discussed on http://sql.ru
<author>

<example>
	print fx.GetDateFromDateTime(getDate())
</example>
*/

create function fx.GetDateFromDateTime(@dt datetime)
returns datetime as
begin
	return convert(datetime, floor(convert(float, @dt)))
end
GO

