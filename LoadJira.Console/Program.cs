using LoadJira.Domain;
using Serilog;
using LoadJira.Infra.Repository;
using LoadJira.Infra.Service;
using System;

namespace LoadJira.Console
{
    class Program
    {
        private static ILogger _log;

        private static string command;

        private static bool loadNewDataFromApi;
        private static string endDate;

        static void Main(string[] args)
        {
            _log = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();

            ParseArgs(args);

            if (string.IsNullOrEmpty(command))
            {
                _log.Error("Comando não especificado. Use: LoadJira.Console.exe <command> [loadNewDataFromApi] [endDate]");
                DisplayUsage();
                return;
            }

            _log.Information($"Iniciando execução do comando: {command}");

            try
            {
                // Initialize Repositories and Services with Dependency Injection
                var issueRepository = new IssueRepository(_log);
                var detailRepository = new DetailRepository(_log);
                var personRepository = new PersonRepository(_log);
                var projectRepository = new ProjectRepository(_log);
                var statusRepository = new StatusRepository(_log);
                var typeRepository = new TypeRepository(_log);
                var jiraWebApiService = new JiraWebApiService(_log);

                switch (command.ToLowerInvariant())
                {
                    case "issue":
                        var issueService = new IssueService(_log, issueRepository, detailRepository, personRepository, projectRepository, statusRepository, typeRepository, jiraWebApiService);
                        issueService.Execute(loadNewDataFromApi, endDate);
                        break;
                    case "sp":
                        var storyPointService = new StoryPointService(_log, issueRepository, detailRepository);
                        storyPointService.Execute();
                        break;
                    case "time":
                        var timeService = new TimeService(_log, issueRepository, detailRepository);
                        timeService.Execute();
                        break;
                    default:
                        _log.Error($"Comando inválido: {command}.");
                        DisplayUsage();
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, "Ocorreu um erro fatal durante a execução.");
            }

            _log.Information($"Execução do comando {command} finalizada.");
        }

        static void ParseArgs(string[] args)
        {
            loadNewDataFromApi = false;
            endDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (args.Length > 0)
            {
                command = args[0];

                if (args.Length >= 2)
                {
                    if (args[1].Equals("1"))
                    {
                        loadNewDataFromApi = true;
                    }
                }

                if (args.Length >= 3)
                {
                    endDate = args[2];
                }
            }
            else
            {
                DisplayUsage();
            }
        }

        static void DisplayUsage()
        {
            System.Console.WriteLine($@"Uso: LoadJira.Console.exe <command> [loadNewDataFromApi] [endDate]
command : <issue|sp|time>
loadNewDataFromApi : <0|1> (opcional, padrão: 0)
endDate : <yyyy-MM-dd> (opcional, padrão: {DateTime.Now.ToString("yyyy-MM-dd")})");
        }
    }
}

