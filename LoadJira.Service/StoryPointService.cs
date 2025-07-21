using LoadJira.Entities;
using LoadJira.Infra.Repository;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System;

namespace LoadJira.Domain
{
    public class StoryPointService : BaseService
    {
        public StoryPointService(ILogger log, IssueRepository issueRepository, DetailRepository detailRepository) 
            : base(log, issueRepository, detailRepository) { }

        public void Execute()
        {
            _log.Information("Iniciando execução do serviço de Story Points.");

            try
            {
                _log.Information("Obtendo issues não processadas para Story Points.");
                var issues = _issueRepository.Get().Where(x => !x.StoryPointProcessed);

                if (!issues.Any())
                {
                    _log.Information("Nenhuma issue encontrada para processamento de Story Points.");
                    return;
                }

                _log.Information($"Encontradas {issues.Count()} issues para processamento de Story Points.");

                foreach (var issue in issues)
                {
                    ProcessSingleIssueStoryPoints(issue);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Erro geral na execução do serviço de Story Points.");
            }

            _log.Information("Execução do serviço de Story Points finalizada.");
        }

        private void ProcessSingleIssueStoryPoints(Issue issue)
        {
            _log.Information($"Processando Story Points para issue: {issue.Key}");
            try
            {
                _log.Debug($"Obtendo detalhes da issue {issue.Key} para calcular Story Points.");
                var details = _detailRepository.GetIssueDetails(issue.Key)?.ToList();

                if (details == null || !details.Any())
                {
                    _log.Information($"Nenhum detalhe encontrado para a issue {issue.Key}. Não é possível calcular Story Points.");
                    // Optionally mark as processed if no details mean no SP changes ever
                    // issue.StoryPointProcessed = true; 
                    // _issueRepository.SaveStoryPoints(issue); // Need a method to just mark as processed
                    return;
                }

                _log.Debug($"Calculando primeiro e último Story Point para issue {issue.Key}.");
                var firstStoryPoint = FirstStoryPoint(details);
                var lastStoryPoint = LastStoryPoint(details);

                issue.FirstStoryPoint = firstStoryPoint;
                issue.LastStoryPoint = lastStoryPoint;
                issue.StoryPointProcessed = true; // Mark as processed after attempting calculation

                _log.Debug($"Atualizando Story Points ({issue.FirstStoryPoint} -> {issue.LastStoryPoint}) para issue {issue.Key} no repositório.");
                var issueUpdated = _issueRepository.SaveStoryPoints(issue);

                if (issueUpdated)
                {
                    _log.Information($"Story Points para issue {issue.Key} atualizados com sucesso.");
                }
                else
                {
                    _log.Error($"Falha ao atualizar Story Points para issue {issue.Key}.");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro ao processar Story Points para a issue {issue.Key}.");
            }
        }

        private int? FirstStoryPoint(IList<Detail> details)
        {
            var storyPointDetails = details.Where(x => x.Type?.Equals("Story Points", StringComparison.OrdinalIgnoreCase) == true).OrderBy(x => x.Created).ToList();

            if (!storyPointDetails.Any())
            {
                return null;
            }

            var firstChange = storyPointDetails.FirstOrDefault();
            if (firstChange != null && int.TryParse(firstChange.To, out int result))
            {
                return result;
            }

            return null;
        }

        private int? LastStoryPoint(IList<Detail> details)
        {
            var storyPointDetails = details.Where(x => x.Type?.Equals("Story Points", StringComparison.OrdinalIgnoreCase) == true).OrderBy(x => x.Created).ToList();

            if (!storyPointDetails.Any())
            {
                return null;
            }

            var lastChange = storyPointDetails.LastOrDefault();
            if (lastChange != null && int.TryParse(lastChange.To, out int result))
            {
                return result;
            }

            return null;
        }
    }
}

