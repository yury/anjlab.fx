if exists (select * from sysobjects where id = object_id(N'fx.SearchObjectByText') and xtype in (N'P'))
drop procedure fx.SearchObjectByText
GO
/*
<summary>
	Searches SQL Server programmability objects (stored procedures, functions, triggers, 
	views) by substring in object's body. 
</summary>

<remarks>
	Masks can be used for searching. * - any char, % - any chars.
</remarks>

<author>
	Alex Zakharov
	Copyright (c) AnjLab 2009, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
</author>

<example>
	exec fx.SearchObjectByText '%anjlab%'
</example>

<param name="Text">String to search</param>
*/

create procedure fx.SearchObjectByText(@Text nvarchar(max)) as
begin
	select so.xtype, so.name
	from dbo.sysobjects so
	where exists (
		select 1
		from syscomments sc1
		left join syscomments sc2 on sc1.id = sc2.id and sc1.colid + 1 = sc2.colid
		where sc1.id = so.id
			and (right(sc1.text, 2000) + left(isnull(sc2.text, ''), 2000) like @Text
			or sc1.text like @Text)
		 )
	order by so.xtype, so.name
end
go