if exists (select * from sysobjects where id = object_id(N'fx.fnGetTimeFromDateTime') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.fnGetTimeFromDateTime
go

/*
<summary>
	Returns time value as string from given datetime variable 
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
	The main idea was discussed on http://sql.ru
<author>

<example>
	print fx.fnGetTimeFromDateTime(getDate())
</example>
*/

create function fx.fnGetTimeFromDateTime(@dt datetime)
returns varchar(8) AS
begin
	return convert(varchar(8), @dt, 108)
end
GO

