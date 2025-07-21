namespace LoadJira.Infra.Repository.command
{
    public static class IssueCommand
    {
        public static string GetCommand => @"select * from dbo.[Issue] where [Key] = @key";

        public static string GetAllCommand => @"select * from dbo.[Issue]";

        public static string InsertCommand => @"insert into dbo.[Issue]([Key], [Processed]) values(@key, @processed)";

        public static string UpdateCommand => @"UPDATE [dbo].[Issue] SET [Processed] = @Processed ,[Summary] = @Summary ,[StatusId] = @StatusId ,[ProjectKey] = @ProjectKey
                                                      ,[ReporterId] = @ReporterId ,[TypeId] = @TypeId ,[Description] = @Description ,[Created] = @Created
                                                WHERE [Key] = @Key";

        public static string UpdateStoryPointsCommand => @"UPDATE [dbo].[Issue] SET [FirstStoryPoint] = @FirstStoryPoint,[LastStoryPoint] = @LastStoryPoint,[StoryPointProcessed] = 1 WHERE [Key] = @Key";

        public static string UpdateFirstStoryPointCommand => @"UPDATE [dbo].[Issue] SET [FirstStoryPoint] = @FirstStoryPoint,[StoryPointProcessed] = 1 WHERE [Key] = @Key";

        public static string UpdateLastStoryPointCommand => @"UPDATE [dbo].[Issue] SET [LastStoryPoint] = @LastStoryPoint,[StoryPointProcessed] = 1 WHERE [Key] = @Key";

        public static string UpdatLastStoryPointWithoutValueCommand => @"UPDATE [dbo].[Issue] SET [StoryPointProcessed] = 1 WHERE [Key] = @Key";

        public static string UpdateTimeCommand => @"UPDATE [dbo].[Issue] SET [LeadTime] = @LeadTime,[CycleTime] = @CycleTime,[TimeProcessed] = @TimeProcessed WHERE [Key] = @Key";
    }
}
