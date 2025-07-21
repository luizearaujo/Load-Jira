use jira_database;

create table [Issue]
(
	[Key] varchar(10) primary key,
	[Processed] numeric(1),
	[Summary] varchar(500),
	[StatusId] numeric(10) references [Status]([Id]),
	[ProjectKey] varchar(10) references [Project]([Key]),
	[ReporterId] varchar(50) references [Person]([Id]),
	[TypeId] numeric(10) references [Type]([Id]),
	[Description] varchar(max),
	[Created] datetime
);