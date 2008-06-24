if exists (select * from sysobjects where id = object_id(N'fx.fnConvertVarbinaryToVarcharHex') and xtype in (N'FN', N'if', N'TF'))
	drop function fx.fnConvertVarbinaryToVarcharHex
GO

/*
<summary>
	Returns hexadecimal representation of binary data, using chars [0-0a-f]
</summary>

<description>
	Function has two 'parts':

	PART ONE: takes large VarbinaryValue chunks (greater than four bytes) 
	and splits them into half, calling the function recursively with 
	each half until the chunks are only four bytes long

	PART TWO: notices the VarbinaryValue is four bytes or less, and 
	starts actually processing these four byte chunks. It does this
	by splitting the least-significant (rightmost) byte into two 
	hexadecimal characters and recursively calling the function
	with the more significant bytes until none remain (four recursive
	calls in total).
</descrption>

<author>
	Craig Dunn
	Based on ufn_VarbinaryToVarcharHex by Clay Beatty.
</author>

<remarks>
	Clay Beatty's original function was written for Sql Server 2000.
	Sql Server 2005 introduces the varbinary(max) datatype which this 
	function now uses.

	References
	----------
	1) MSDN: Using Large-Value Data Types
	http://msdn2.microsoft.com/en-us/library/ms178158.aspx

	2) Clay's "original" Script, Save, Export SQL 2000 Database Diagrams
	http://www.thescripts.com/forum/thread81534.html or
	http://groups-beta.google.com/group/comp.databases.ms-sqlserver/browse_frm/thread/ca9a9229d06a56f9?dq=&hl=en&lr=&ie=UTF-8&oe=UTF-8&prev=/groups%3Fdq%3D%26num%3D25%26hl%3Den%26lr%3D%26ie%3DUTF-8%26oe%3DUTF-8%26group%3Dcomp.databases.ms-sqlserver%26start%3D25
</remarks>

<param name="VarbinaryValue">binary data to be converted to Hexadecimal </param>

<returns>Hexadecimal representation of binary data, using chars [0-0a-f]</returns>
*/

create function fx.fnConvertVarbinaryToVarcharHex
(
	@VarbinaryValue	varbinary(max)
)
returns varchar(max) AS
	begin

	declare @NumberOfBytes 	int

	set @NumberOfBytes = datalength(@VarbinaryValue)
	-- PART ONE --
	if (@NumberOfBytes > 4)
	begin
		declare @FirstHalfNumberOfBytes int
		declare @SecondHalfNumberOfBytes int
		set @FirstHalfNumberOfBytes  = @NumberOfBytes/2
		set @SecondHalfNumberOfBytes = @NumberOfBytes - @FirstHalfNumberOfBytes
		-- Call this function recursively with the two parts of the input split in half
		return fx.fnConvertVarbinaryToVarcharHex(cast(substring(@VarbinaryValue, 1					        , @FirstHalfNumberOfBytes)  as varbinary(max)))
			 + fx.fnConvertVarbinaryToVarcharHex(cast(substring(@VarbinaryValue, @FirstHalfNumberOfBytes+1 , @SecondHalfNumberOfBytes) as varbinary(max)))
	end
	
	if (@NumberOfBytes = 0)
	begin
		return ''	-- No bytes found, therefore no 'hex string' is returned
	end
	
	-- PART TWO --
	declare @LowByte 		int
	declare @HighByte 		int
	-- @NumberOfBytes <= 4 (four or less characters/8 hex digits were input)
	--						 eg. 88887777 66665555 44443333 22221111
	-- We'll process ONLY the right-most (least-significant) Byte, which consists
	-- of eight bits, or two hexadecimal values (eg. 22221111 --> XY) 
	-- where XY are two hex digits [0-f]

	-- 1. Carve off the rightmost four bits/single hex digit (ie 1111)
	--    BINARY AND 15 will result in a number with maxvalue of 15
	set @LowByte = CAST(@VarbinaryValue as int) & 15
	-- Now determine which ASCII char value
	set @LowByte = case 
	when (@LowByte < 10)		-- 9 or less, convert to digits [0-9]
		then (48 + @LowByte)	-- 48 ASCII = 0 ... 57 ASCII = 9
		else (87 + @LowByte)	-- else 10-15, convert to chars [a-f]
	end							-- (87+10)97 ASCII = a ... (87+15_102 ASCII = f

	-- 2. Carve off the rightmost eight bits/single hex digit (ie 22221111)
	--    Divide by 16 does a shift-left (now processing 2222)
	set @HighByte = CAST(@VarbinaryValue as int) & 255
	set @HighByte = (@HighByte / 16)
	-- Again determine which ASCII char value	
	set @HighByte = case 
	when (@HighByte < 10)		-- 9 or less, convert to digits [0-9]
		then (48 + @HighByte)	-- 48 ASCII = 0 ... 57 ASCII = 9
		else (87 + @HighByte)	-- else 10-15, convert to chars [a-f]
	end							-- (87+10)97 ASCII = a ... (87+15)102 ASCII = f
	
	-- 3. Trim the byte (two hex values) from the right (least significant) input Binary
	--    in preparation for further parsing
	set @VarbinaryValue = substring(@VarbinaryValue, 1, (@NumberOfBytes-1))

	-- 4. Recursively call this method on the remaining Binary data, concatenating the two 
	--    hexadecimal 'values' we just decoded as their ASCII character representation
	--    ie. we pass 88887777 66665555 44443333 back to this function, adding XY to the result string
	return fx.fnConvertVarbinaryToVarcharHex(@VarbinaryValue) + char(@HighByte) + char(@LowByte)
end
GO