if exists (select * from sysobjects where id = object_id(N'fx.procCleanServerCashe') and xtype in (N'P'))
drop procedure fx.procCleanServerCashe
GO
/*
<summary>
	Clean up SQL Server system cashe
</summary>

<remarks>
	The procedure should be executed under administrator account
</remarks>

<author>
	Nick Zhebrun
	Copyright (c) AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
</author>

<example>
	exec fx.procCleanServerCashe
</example>

*/

create procedure fx.procCleanServerCashe as
begin

	checkpoint
    dbcc dropcleanbuffers
	dbcc freesystemcashe('All')

end