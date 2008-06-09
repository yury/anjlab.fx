if exists (select * from sysobjects where id = object_id(N'fx.fnGetDateFromDateTime') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.fnGetDateFromDateTime
go

/*
<summary>
	This function returns date value from given datetime variable 
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<example>
	print fx.fnGetDateFromDateTime(getDate())
</example>
*/

create function fx.fnGetDateFromDateTime(@dt datetime)
returns datetime AS
begin
	return convert(datetime, floor(convert(float, @dt)))
end
GO

