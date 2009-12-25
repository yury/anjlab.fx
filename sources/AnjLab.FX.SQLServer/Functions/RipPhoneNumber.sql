if exists (select * from sysobjects where id = object_id(N'fx.RipPhoneNumber') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.RipPhoneNumber
go

/*
<summary>
	Tries to rip phone number from free input string.
	The function supposes that phone number is subsequence of digits and some
	symbols.
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
</author>

<param name="StringValue">Source string</param>

<returns>Substring consists phone number</returns>

<example>
	print fx.RipPhoneNumber(N'Alex Zakharov *(768)-9876-098 7call after 6PM')
</example>
*/

create function fx.RipPhoneNumber(@String nvarchar(max)) 
returns nvarchar(max) as
begin

	declare 
		@Position int,
		@Char nchar(1),
		@Phone nvarchar(max),
		@Isnumber bit

	set @Phone = space(0)
	set @Position = 1

	while @Position <= len(@String)
	begin
		set @Char = substring(@String, @Position, 1)
		if @Char in (N'.', N'0', N'1', N'2', N'3', N'4', N'5', N'6', N'7', N'8', N'9', N'-', N'(', N')', N'x', space(1))
			set @Phone = @Phone + @Char
		else 
			if len(@Phone) > 1 break
			else set @Phone = space(0)
		set @Position = @Position + 1
	end

	return @Phone

end