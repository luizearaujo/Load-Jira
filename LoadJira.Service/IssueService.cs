using System.Linq;
using LoadJira.Entities;
using LoadJira.Infra.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoadJira.Infra.Service;
using Serilog;
using System;

namespace LoadJira.Domain
{
    public class IssueService : BaseService
    {
        private readonly TypeRepository _typeRepository;
        private readonly PersonRepository _personRepository;
        private readonly StatusRepository _statusRepository;
        private readonly DetailRepository _detailRepository;
        private readonly ProjectRepository _projectRepository;
        private readonly IssueRepository _issueRepository;

        private readonly JiraWebApiService _jiraWebApiService;

        public IssueService(ILogger log,
                            IssueRepository issueRepository,
                            DetailRepository detailRepository,
                            PersonRepository personRepository,
                            ProjectRepository projectRepository,
                            StatusRepository statusRepository,
                            TypeRepository typeRepository,
                            JiraWebApiService jiraWebApiService)
            : base(log, issueRepository, detailRepository)
        {
            _typeRepository = typeRepository;
            _personRepository = personRepository;
            _statusRepository = statusRepository;
            _projectRepository = projectRepository;
            _issueRepository = issueRepository;
            _jiraWebApiService = jiraWebApiService;
        }

        public async void Execute(bool loadNewDataFromApi, string endDate)
        {
            _log.Information("Iniciando execução do serviço de Issues.");

            try
            {
                _log.Information("Obtendo issues para processamento.");
                var issuesToProcess = await this.Load(loadNewDataFromApi, endDate);

                if (!issuesToProcess.Any())
                {
                    _log.Information("Nenhuma issue encontrada para processamento.");
                    return;
                }

                _log.Information($"Encontradas {issuesToProcess.Count()} issues para processamento.");

                foreach (var issueToProcess in issuesToProcess)
                {
                    await ProcessSingleIssue(issueToProcess);
                }
            }
            catch (System.Exception ex)
            {
                _log.Error(ex, "Erro geral na execução do serviço de Issues.");
            }

            _log.Information("Execução do serviço de Issues finalizada.");
        }

        private async Task ProcessSingleIssue(Issue issueToProcess)
        {
            _log.Information($"Processando issue: {issueToProcess.Key}");
            try
            {
                // 1. Obter issue do Jira
                _log.Debug($"Obtendo detalhes da issue {issueToProcess.Key} da API do Jira.");
                var issue = await _jiraWebApiService.GetIssueAsync(issueToProcess.Key);
                if (issue == null)
                {
                    _log.Warning($"Issue {issueToProcess.Key} não encontrada na API do Jira. Pulando processamento.");
                    return;
                }

                // 2. Salvar entidades relacionadas
                bool allRelatedEntitiesSaved = true;

                allRelatedEntitiesSaved &= LogAndSave(_typeRepository, issue.Type, $"Tipo {issue.Type.Name}", issueToProcess.Key);
                allRelatedEntitiesSaved &= LogAndSave(_projectRepository, issue.Project, $"Projeto {issue.Project.Name}", issueToProcess.Key);
                allRelatedEntitiesSaved &= LogAndSave(_statusRepository, issue.Status, $"Status {issue.Status.Name}", issueToProcess.Key);
                allRelatedEntitiesSaved &= LogAndSave(_personRepository, issue.Reporter, $"Reportador {issue.Reporter.Name}", issueToProcess.Key);

                // 3. Obter e salvar detalhes da issue
                _log.Debug($"Obtendo detalhes adicionais para a issue {issueToProcess.Key} da API do Jira.");
                var details = await _jiraWebApiService.GetDetailsAsync(issueToProcess.Key);

                if (details != null && details.Any())
                {
                    foreach (var detail in details)
                    {
                        _log.Debug($"Processando detalhe {detail.Id} da issue {issueToProcess.Key}.");
                        bool detailAuthorSaved = LogAndSave(_personRepository, detail.Author, $"Autor do detalhe {detail.Id}", issueToProcess.Key);
                        bool detailSaved = _detailRepository.Save(detail, issueToProcess.Key);
                        if (!detailSaved)
                        {
                            _log.Error($"Falha ao salvar o detalhe {detail.Id} da issue {issueToProcess.Key}.");
                            allRelatedEntitiesSaved = false;
                        }
                        else
                        {
                            _log.Debug($"Detalhe {detail.Id} da issue {issueToProcess.Key} salvo com sucesso.");
                        }
                    }
                }
                else
                {
                    _log.Information($"Nenhum detalhe encontrado para a issue {issueToProcess.Key}.");
                }

                // 4. Atualizar issue se todas as entidades relacionadas foram salvas
                if (allRelatedEntitiesSaved)
                {
                    var issueUpdated = _issueRepository.Update(issue);
                    if (issueUpdated)
                    {
                        _log.Information($"Issue {issueToProcess.Key} atualizada com sucesso no repositório.");
                    }
                    else
                    {
                        _log.Error($"Falha ao atualizar a issue {issueToProcess.Key} no repositório.");
                    }
                }
                else
                {
                    _log.Warning($"Issue {issueToProcess.Key} não foi totalmente atualizada devido a falhas no salvamento de entidades relacionadas.");
                }
            }
            catch (System.Exception ex)
            {
                _log.Error(ex, $"Erro ao processar a issue {issueToProcess.Key}.");
            }
        }

        private bool LogAndSave<T>(BaseRepository<T> repository, T entity, string entityName, string issueKey)
            where T : class
        {
            if (entity == null)
            {
                _log.Warning($"Entidade {entityName} para a issue {issueKey} é nula. Não será salva.");
                return false;
            }

            bool saved = repository.Save(entity);
            if (saved)
            {
                _log.Debug($"{entityName} para a issue {issueKey} salvo com sucesso.");
            }
            else
            {
                _log.Error($"Falha ao salvar {entityName} para a issue {issueKey}.");
            }
            return saved;
        }

        private async Task<IEnumerable<Issue>> Load(bool loadNewDataFromApi, string endDate = null)
        {
            if (loadNewDataFromApi)
            {
                _log.Information($"Carregando novas chaves de issues da API do Jira até a data: {endDate ?? "atual"}.");
                var issuesKey = await _jiraWebApiService.GetKeysAsync(endDate);
                if (issuesKey != null && issuesKey.Any())
                {
                    _log.Information($"Encontradas {issuesKey.Count()} novas chaves de issues na API. Tentando salvar no repositório.");
                    var issuesSaved = _issueRepository.Save(issuesKey);
                    if (!issuesSaved)
                    {
                        _log.Error("Algumas chaves de issues não foram salvas no repositório.");
                        // Decidir se deve lançar exceção ou continuar com as que foram salvas
                        // Por enquanto, vamos continuar, mas logar o erro.
                    }
                    else
                    {
                        _log.Information("Todas as novas chaves de issues salvas com sucesso.");
                    }
                }
                else
                {
                    _log.Information("Nenhuma nova chave de issue encontrada na API do Jira.");
                }
            }

            _log.Information("Obtendo issues não processadas do repositório local.");
            return _issueRepository.Get().Where(x => !x.Processed).ToList();
        }
    }
}

