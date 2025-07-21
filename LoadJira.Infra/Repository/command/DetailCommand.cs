
namespace LoadJira.Infra.Repository.command
{
    public static class DetailCommand
    {
        private static string _getCommand => @"SELECT [Id] ,[Created] ,[Type] ,[From] ,[To] ,[AuthorId] FROM [dbo].[Detail] WHERE [Id] = @id";

        private static string _insertCommand => @"INSERT INTO [dbo].[Detail] ([Id], [IssueKey] ,[Created] ,[Type] ,[From] ,[To] ,[AuthorId])
                                                    VALUES(@Id, @IssueKey, @Created, @Type, @From, @To, @AuthorId)";

        private static string _updateCommand => @"UPDATE [dbo].[Detail] SET [IssueKey] = @IssueKey ,[Created] = @Created ,[Type] = @Type ,[From] = @From ,
                                                    [To] = @To ,[AuthorId] = @AuthorId WHERE [Id] = @Id";

        private static string _getIssueDetailsCommand => @"SELECT [Id],[IssueKey],[Created],[Type],[From],[To],[AuthorId] FROM [dbo].[Detail] WHERE [IssueKey] = @IssueKey";
        public static string GetCommand
        {
            get { return _getCommand; }
        }

        public static string InsertCommand
        {
            get { return _insertCommand; }
        }

        public static string UpdateCommand
        {
            get { return _updateCommand; }
        }

        public static string GetIssueDetailsCommand
        {
            get { return _getIssueDetailsCommand; }
        }
    }
}
