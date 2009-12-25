set nocount on

declare @Schemas table([Name] sysname, [Order] int)

insert into @Schemas values('Tools', 1)
insert into @Schemas values('Security', 2)
insert into @Schemas values('Common', 3)
insert into @Schemas values('Inventory', 4)
insert into @Schemas values('Dispatch', 5)
insert into @Schemas values('International', 6)

declare @Schema sysname, @Object sysname

declare Objects cursor for 
	select s.name, o.name 
	from sys.objects o 
	inner join sys.schemas s on s.schema_id = o.schema_id
	inner join @Schemas ss on ss.[Name] = s.name
	where o.type in ('TR')
	order by ss.[Order] , o.name
	
	
open Objects
fetch next from Objects into @Schema, @Object

while @@fetch_status = 0
begin
	--print @Object
	exec fx.ScriptObject @Schema, @Object, 1
	fetch next from Objects into @Schema, @Object
end

close Objects
deallocate Objects
	
	
	




	