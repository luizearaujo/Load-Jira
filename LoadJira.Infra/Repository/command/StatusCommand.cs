namespace LoadJira.Infra.Repository.command
{
    public static class StatusCommand
    {
        private static string _getCommand => @"SELECT [Id],[Description] FROM [dbo].[Status] WHERE [Id] = @id";

        private static string _insertCommand => @"INSERT INTO [dbo].[Status]([Id] ,[Description]) VALUES(@Id,@Description)";

        private static string _updateCommand => @"UPDATE [dbo].[Status] SET [Description] = @Description WHERE [Id] = @Id";

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
    }
}
