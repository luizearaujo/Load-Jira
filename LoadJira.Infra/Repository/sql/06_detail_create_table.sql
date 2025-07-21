use jira_database;


create table [Detail]
(
	[Id] numeric(20) primary key,
	[IssueKey] varchar(10) references [Issue]([Key]),
	[Created] datetime,
	[Type] varchar(100),
	[From] varchar(max),
	[To] varchar(max),
	[AuthorId] varchar(50) references [Person]([Id])
);