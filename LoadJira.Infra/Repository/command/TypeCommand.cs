
namespace LoadJira.Infra.Repository.command
{
    public static class TypeCommand
    {
        private static string _getCommand => @"SELECT * FROM [dbo].[Type] WHERE [id] = @id";

        private static string _insertCommand => @"INSERT INTO [dbo].[Type]([Id],[Description]) VALUES(@Id, @Description)";

        private static string _updateCommand => @"UPDATE [dbo].[Type] SET [Description] = @Description WHERE [Id] = @Id";

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
