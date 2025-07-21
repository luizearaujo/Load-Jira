
namespace LoadJira.Infra.Repository.command
{
    public static class PersonCommand
    {
        private static string _getCommand => @"SELECT [Id] ,[Name] FROM [dbo].[Person] WHERE [Id] = @Id";

        private static string _insertCommand => @"INSERT INTO [dbo].[Person]([Id], [Name]) VALUES(@Id, @Name)";

        private static string _updateCommand => @"UPDATE [dbo].[Person] SET [Name] = @Name WHERE [Id] = @Id";

        public static string GetCommand
        {
            get { return _getCommand; }
        }

        public static string InsertCommand 
        { 
            get { return _insertCommand; } 
        }

        public static string UpdateCommand {
            get { return _updateCommand; } 
        }
    }
}
