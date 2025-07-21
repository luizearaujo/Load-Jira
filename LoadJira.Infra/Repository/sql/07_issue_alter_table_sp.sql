alter table [dbo].[issue]
add [FirstStoryPoint] numeric(3);

alter table [dbo].[issue]
add LastStoryPoint numeric(3);

alter table [dbo].[Issue]
add [StoryPointProcessed] numeric(1) default 0;