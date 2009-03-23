if exists (select * from sysobjects where id = object_id(N'fx.fnFormatPhoneNumber') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.fnFormatPhoneNumber
go
/*
<summary>
	Reformats input string as phone number using template (000) 000-0000
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
</author>

<param name="StringValue">Source string</param>

<returns>Formatted string consists phone number</returns>

<example>
	print fx.fnFormatPhoneNumber(replicate(N'0', 11))
</example>
*/

create function fx.fnFormatPhoneNumber(@String nvarchar(max)) 
returns nvarchar(max) as
begin

	declare @i int,
		@Extention nvarchar(max),
		@ExtentionStart int

	set @String = ltrim(rtrim(@String))
	set @String = replace(@String, space(1), space(0))
	set @String = replace(@String, N'(', space(0))
	set @String = replace(@String, N')', space(0))
	set @String = replace(@String, N'-', space(0))
	set @ExtentionStart = charindex('x', @String)
	if @ExtentionStart > 0 begin
		set @Extention = substring(@String, @ExtentionStart, len(@String) - @ExtentionStart)
		set @String = substring(@String, 1, @ExtentionStart - 1)
	end

	set @i = len(@String)

	if @i = 10
	begin
		set @String = space(10 - @i) + @String
		set @String = N'(' + substring(@String, 1, 3) + N') ' + 
			substring(@String, 4, 3) + N'-' + substring(@String, 7, 4)
	end 

	if @i = 7
		set @String = substring(@String, 1, 3) + N'-' + substring(@String, 4, 4)


	if @i > 10
	begin
		set @i = @i - 10
		set @String = substring(@String, 1, @i) + N' ' +
			N'(' + substring(@String, @i + 1, 3) + N')' + 
			substring(@String, @i + 4, 3) + N'-' + 
			substring(@String, @i + 7, 4)
	end


	if @ExtentionStart > 0
		set @String = @String + N' (' + @Extention + N')'

	return @String

end
go