if exists (select * from sysobjects where id = object_id(N'fx.CalculateYield') and xtype in (N'FN', N'IF', N'TF'))
drop function fx.CalculateYield
go

/*
<summary>
	Returns yield (in %%), based on time period (taking into account leap years) 
	and revenue.
<summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>09\27\2007</date>

<example>
	NOTE: table valued function fx.GetEmptyRowSet and scalar function fx.CheckLeapYear
	must be created before using.
	print fx.CalculateYield(getDate(), getDate()+1000, 100.00, 200.00)
	> 36.5191
</example>
*/

create function fx.CalculateYield(@StartDate datetime, @EndDate datetime, @PresentValue money, @FutureValue money) 
returns real as
begin

declare @Yield real

select @Yield = 
	100 * ((@FutureValue - @PresentValue ) / @PresentValue ) * 
	1/(cast(sum(Days * cast(LeapYear - 1 as bit)) as real)/365 + cast(sum(Days * LeapYear) as real)/366)
from (
	select 
		datediff(day, YearStartDate, YearEndDate) as Days,
		fx.CheckLeapYear(YearStartDate) as LeapYear
	from (
		select 
			YearStartDate = CASE Years WHEN year(@StartDate) THEN @StartDate ELSE '01/01/' + str(Years) END,
			YearEndDate   = CASE Years WHEN year(@EndDate)   THEN @EndDate	 ELSE '01/01/' + str(Years + 1) END
		from (
			select Year(@StartDate) + RecordId - 1 as Years
			from fx.GetEmptyRowSet(datediff(year, @StartDate, @EndDate) + 1)
		) as a
	) as b
) as c


return @Yield

end

GO
