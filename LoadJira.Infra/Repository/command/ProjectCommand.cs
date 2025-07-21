namespace LoadJira.Infra.Repository.command
{
    public static class ProjectCommand
    {
        private static string _getCommand => @"SELECT [Key], [Name] FROM [dbo].[Project] WHERE [Key] = @key";

        private static string _insertCommand => @"INSERT INTO [dbo].[Project]([Key] ,[Name]) VALUES(@Key, @Name)";

        private static string _updateCommand => @"UPDATE [dbo].[Project] SET [Name] = @Name WHERE [Key] = @Key";

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
