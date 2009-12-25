if exists (select * from sysobjects where id = object_id(N'fx.RemoveDuplicateSymbols') and xtype in (N'FN', N'if', N'TF'))
	drop function fx.RemoveDuplicateSymbols
GO

/*
<summary>
	Returns input string with replaced sequences of several the same given symbols
	with one symbol
</summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
	The main idea was discussed on http://sql.ru
</author>

<param name="StringValue">Initial string</param>
<param name="SymbolValue">Symbol, whose sequences will be tuncated</param>

<returns>Truncated string</returns>

<example>
print fx.RemoveDuplicateSymbols(N'       b    c d     e     ', N' ')
result ' b c d e '
</example>
*/

create function fx.RemoveDuplicateSymbols
(
	@StringValue nvarchar(max),
	@SymbolValue nchar(1) = N' '
)
returns nvarchar(max) AS
begin

	return 
		replace(
			replace(
				replace(@StringValue, @SymbolValue + @SymbolValue, @SymbolValue + nchar(1))
				, nchar(1) + @SymbolValue, N'')
			, nchar(1), N'')

end
GO