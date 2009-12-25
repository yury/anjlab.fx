if exists (select * from sysobjects where id = object_id(N'fx.CleanServerCashe') and xtype in (N'P'))
drop procedure fx.CleanServerCashe
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
	exec fx.CleanServerCashe
</example>

*/

create procedure fx.CleanServerCashe as
begin

	checkpoint
    dbcc dropcleanbuffers with no_infomsgs
	dbcc FREESYSTEMCACHE ('ALL') with no_infomsgs


end