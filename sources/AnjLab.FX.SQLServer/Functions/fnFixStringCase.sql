if exists (select * from sysobjects where id = object_id(N'tools.fnFixStringCase') and xtype in (N'FN', N'IF', N'TF'))
drop function tools.fnFixStringCase
go

/*
<summary>
	Transforms input string with the following template: first symbol and each first symbol
	after space char are in upper case, other symbols are in lower case. The function also
	makes all trim  for input string.
<summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>01\27\200</date>

<example>
	print tools.fnFixStringCase(N'SAN FRANCISCO')
	print tools.fnFixStringCase(N'   funcTiOn        ')
</example>
*/


create function tools.fnFixStringCase(@String nvarchar(max)) returns nvarchar(max) as
begin

	declare 
		@i int,
		@Processed   nvarchar(max)

	set @String = ltrim(rtrim(@String))
	set @Processed = space(0)

	while charindex(N' ', @String) > 0  begin
		set @i = charindex(N' ', @String)
		set @Processed = @Processed + upper(substring(@String, 1, 1)) + 
			lower(substring(@String, 2, @i - 1))
		set @String = substring(@String, @i + 1, len(@String) - @i)
	end

	set @Processed = @Processed + upper(substring(@String, 1, 1)) + lower(substring(@String, 2, len(@String)))

	if len(@Processed) < 4 set @Processed = lower(@String)
		
	return @Processed

end