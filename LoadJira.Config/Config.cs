namespace LoadJira.Config
{
    public static class Config
    {
        private static string _jiraUser => "user@domain.com";

        private static string _jiraToken => "Your Token";

        private static string _jiraUrl => "Jira Url";

        private static string _sqlServerConn => @"Connection String";

        public static string JiraUser { get { return _jiraUser; } }

        public static string JiraToken { get { return _jiraToken; } }

        public static string JiraUrl { get { return _jiraUrl; } }

        public static string SqlServerConn { get { return _sqlServerConn; } }

    }

}