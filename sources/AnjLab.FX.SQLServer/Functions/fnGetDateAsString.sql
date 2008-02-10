if exists (select * from sysobjects where id = object_id(N'tools.fnGetDateAsString') and type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
drop function tools.fnGetDateAsString
GO
/*
This function returns date as custom string  
Author: Alex M. Zakharov
Date: 09\22\2007
Example:
print tools.fnGetDateAsString(getDate(), 1)
*/

CREATE FUNCTION tools.fnGetDateAsString (@Date datetime, @Mode tinyint) RETURNS nvarchar(100) AS

BEGIN

DECLARE @Result nvarchar(100)
DECLARE @Months table (Id tinyint, name nvarchar(20))

IF @Mode = 0 /* Полная дата*/ BEGIN

	INSERT INTO @Months VALUES(1, N'января')
	INSERT INTO @Months VALUES(2, N'февраля')
	INSERT INTO @Months VALUES(3, N'марта')
	INSERT INTO @Months VALUES(4, N'апреля')
	INSERT INTO @Months VALUES(5, N'мая')
	INSERT INTO @Months VALUES(6, N'июня')
	INSERT INTO @Months VALUES(7, N'июля')
	INSERT INTO @Months VALUES(8, N'августа')
	INSERT INTO @Months VALUES(9, N'сентября')
	INSERT INTO @Months VALUES(10, N'октября')
	INSERT INTO @Months VALUES(11, N'ноября')
	INSERT INTO @Months VALUES(12, N'декабря')

	SELECT @Result = cast(day(@Date) as nchar(2)) + N'  ' + name + N'  ' + cast(year(@Date) as nchar(4)) + N' г. ' 
		+ cast(datepart(hh, @Date) as nchar(2)) + N':' + cast(datepart(mi, @Date) as nchar(2))
	FROM @Months WHERE Id = month(@Date)

END

IF @Mode = 1 /* Полная дата без времени*/ BEGIN

	INSERT INTO @Months VALUES(1, N'января')
	INSERT INTO @Months VALUES(2, N'февраля')
	INSERT INTO @Months VALUES(3, N'марта')
	INSERT INTO @Months VALUES(4, N'апреля')
	INSERT INTO @Months VALUES(5, N'мая')
	INSERT INTO @Months VALUES(6, N'июня')
	INSERT INTO @Months VALUES(7, N'июля')
	INSERT INTO @Months VALUES(8, N'августа')
	INSERT INTO @Months VALUES(9, N'сентября')
	INSERT INTO @Months VALUES(10, N'октября')
	INSERT INTO @Months VALUES(11, N'ноября')
	INSERT INTO @Months VALUES(12, N'декабря')

	SELECT @Result = cast(day(@Date) as nchar(2)) + N'  ' + name + N'  ' + cast(year(@Date) as nchar(4)) + N' г. ' 
	FROM @Months WHERE Id = month(@Date)

END

IF @Mode = 2 /* Месяц в именительном падеже и год */
BEGIN

	INSERT INTO @Months VALUES(1, N'Январь')
	INSERT INTO @Months VALUES(2, N'Февраль')
	INSERT INTO @Months VALUES(3, N'Март')
	INSERT INTO @Months VALUES(4, N'Апрель')
	INSERT INTO @Months VALUES(5, N'Май')
	INSERT INTO @Months VALUES(6, N'Июнь')
	INSERT INTO @Months VALUES(7, N'Июль')
	INSERT INTO @Months VALUES(8, N'Август')
	INSERT INTO @Months VALUES(9, N'Сентябрь')
	INSERT INTO @Months VALUES(10, N'Октябрь')
	INSERT INTO @Months VALUES(11, N'Ноябрь')
	INSERT INTO @Months VALUES(12, N'Декабрь')

	SELECT @Result = name + N' ' + cast(year(@Date) as nchar(4)) FROM @Months WHERE Id = month(@Date)

END

RETURN @Result

END




