alter table [dbo].[issue]
add [LeadTime] numeric(10,2);

alter table [dbo].[issue]
add [CycleTime] numeric(10,2);

alter table [dbo].[Issue]
add [TimeProcessed] numeric(1) default 0;