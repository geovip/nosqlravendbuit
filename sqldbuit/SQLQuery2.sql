select count(*) from [user]
create nonclustered index index_fullname on [user](fullname)

create index index_id on [user](id)
drop index PK__User__3214EC077F60ED59
 on [user]  WITH (ONLINE = ON, MAXDOP = 2);


delete [user]

declare @begin time
declare @end time

set @begin = convert(time, getdate(), 114)
select count(*) from [user] where fullname like '%1%'
set @end = convert(time, getdate(), 114)
print @begin
print @end

print datediff(ms,@begin,@end)

sp_help [user]