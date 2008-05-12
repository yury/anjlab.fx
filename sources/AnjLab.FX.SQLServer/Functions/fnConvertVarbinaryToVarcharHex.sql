if exists (select * from sysobjects where id = object_id(N'fx.fnConvertVarbinaryToVarcharHex') and xtype in (N'FN', N'IF', N'TF'))
	DROP FUNCTION fx.fnConvertVarbinaryToVarcharHex
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
	Sql Server 2005 introduces the VARBINARY(max) datatype which this 
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

CREATE FUNCTION fx.fnConvertVarbinaryToVarcharHex
(
	@VarbinaryValue	VARBINARY(max)
)
RETURNS VARCHAR(max) AS
	BEGIN
	DECLARE @NumberOfBytes 	INT

	SET @NumberOfBytes = DATALENGTH(@VarbinaryValue)
	-- PART ONE --
	IF (@NumberOfBytes > 4)
	BEGIN
		DECLARE @FirstHalfNumberOfBytes INT
		DECLARE @SecondHalfNumberOfBytes INT
		SET @FirstHalfNumberOfBytes  = @NumberOfBytes/2
		SET @SecondHalfNumberOfBytes = @NumberOfBytes - @FirstHalfNumberOfBytes
		-- Call this function recursively with the two parts of the input split in half
		RETURN fx.fnConvertVarbinaryToVarcharHex(CAST(SUBSTRING(@VarbinaryValue, 1					        , @FirstHalfNumberOfBytes)  AS VARBINARY(max)))
			 + fx.fnConvertVarbinaryToVarcharHex(CAST(SUBSTRING(@VarbinaryValue, @FirstHalfNumberOfBytes+1 , @SecondHalfNumberOfBytes) AS VARBINARY(max)))
	END
	
	IF (@NumberOfBytes = 0)
	BEGIN
		RETURN ''	-- No bytes found, therefore no 'hex string' is returned
	END
	
	-- PART TWO --
	DECLARE @LowByte 		INT
	DECLARE @HighByte 		INT
	-- @NumberOfBytes <= 4 (four or less characters/8 hex digits were input)
	--						 eg. 88887777 66665555 44443333 22221111
	-- We'll process ONLY the right-most (least-significant) Byte, which consists
	-- of eight bits, or two hexadecimal values (eg. 22221111 --> XY) 
	-- where XY are two hex digits [0-f]

	-- 1. Carve off the rightmost four bits/single hex digit (ie 1111)
	--    BINARY AND 15 will result in a number with maxvalue of 15
	SET @LowByte = CAST(@VarbinaryValue AS INT) & 15
	-- Now determine which ASCII char value
	SET @LowByte = CASE 
	WHEN (@LowByte < 10)		-- 9 or less, convert to digits [0-9]
		THEN (48 + @LowByte)	-- 48 ASCII = 0 ... 57 ASCII = 9
		ELSE (87 + @LowByte)	-- else 10-15, convert to chars [a-f]
	END							-- (87+10)97 ASCII = a ... (87+15_102 ASCII = f

	-- 2. Carve off the rightmost eight bits/single hex digit (ie 22221111)
	--    Divide by 16 does a shift-left (now processing 2222)
	SET @HighByte = CAST(@VarbinaryValue AS INT) & 255
	SET @HighByte = (@HighByte / 16)
	-- Again determine which ASCII char value	
	SET @HighByte = CASE 
	WHEN (@HighByte < 10)		-- 9 or less, convert to digits [0-9]
		THEN (48 + @HighByte)	-- 48 ASCII = 0 ... 57 ASCII = 9
		ELSE (87 + @HighByte)	-- else 10-15, convert to chars [a-f]
	END							-- (87+10)97 ASCII = a ... (87+15)102 ASCII = f
	
	-- 3. Trim the byte (two hex values) from the right (least significant) input Binary
	--    in preparation for further parsing
	SET @VarbinaryValue = SUBSTRING(@VarbinaryValue, 1, (@NumberOfBytes-1))

	-- 4. Recursively call this method on the remaining Binary data, concatenating the two 
	--    hexadecimal 'values' we just decoded as their ASCII character representation
	--    ie. we pass 88887777 66665555 44443333 back to this function, adding XY to the result string
	RETURN fx.fnConvertVarbinaryToVarcharHex(@VarbinaryValue) + CHAR(@HighByte) + CHAR(@LowByte)
END
GO