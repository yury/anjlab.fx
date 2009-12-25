if exists (select * from sysobjects where id = object_id(N'fx.GetAbbreviation') and xtype in (N'FN', N'if', N'TF'))
	drop function fx.GetAbbreviation
go

/*
<summary>
	Returns abbreviation of input string. The function ignores symbols from 
	set of symbols to exclude and removes double spaces.
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
</author>

<param name="StringValue">Initial string</param>

<returns>Abbreviation string</returns>

<example>
	print fx.GetAbbreviation(N'H. Gandall .2 (ss) hsgdhs ')
	result 'HG2SH'
</example>
*/

create function fx.GetAbbreviation(@String nvarchar(max))
returns nvarchar(max) as
begin

	declare 
		@Abbreviation nvarchar(max), 
		@ExcludeSymbols nvarchar(255),
		@Index int

	set @ExcludeSymbols = N'[]().,-_+/\><*!?@#$%^&{}'

	while len(@ExcludeSymbols) > 0
	begin
		set @String = replace(@String, substring(@ExcludeSymbols, 1, 1), space(0))
		set @ExcludeSymbols = substring(@ExcludeSymbols, 2, len(@ExcludeSymbols) - 1)
	end

	set @String = fx.RemoveDuplicateSymbols(@String, space(1))
	set @Abbreviation = space(0)

	set @Index = charindex(space(1), @String)
	while @Index > 0
	begin
		set @Abbreviation = @Abbreviation + upper(substring(@String, 1, 1))
		set @String = substring(@String, @Index + 1, len(@String) - 1)
		set @Index = charindex(space(1), @String)
	end
	set @Abbreviation = @Abbreviation + upper(substring(@String, 1, 1))

	return @Abbreviation

end
go