using LoadJira.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using LoadJira.Infra.Repository;

namespace LoadJira.Domain
{
    public class TimeService : BaseService
    {
        public TimeService(ILogger log, IssueRepository issueRepository, DetailRepository detailRepository)
            : base(log, issueRepository, detailRepository) { }

        public void Execute()
        {
            _log.Information("Iniciando execução do serviço de Tempo.");

            try
            {
                _log.Information("Obtendo issues não processadas para Tempo.");
                var issues = _issueRepository.Get().Where(x => !x.TimeProcessed);

                if (!issues.Any())
                {
                    _log.Information("Nenhuma issue encontrada para processamento de Tempo.");
                    return;
                }

                _log.Information($"Encontradas {issues.Count()} issues para processamento de Tempo.");

                foreach (var issue in issues)
                {
                    ProcessSingleIssueTime(issue);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Erro geral na execução do serviço de Tempo.");
            }

            _log.Information("Execução do serviço de Tempo finalizada.");
        }

        private void ProcessSingleIssueTime(Issue issue)
        {
            _log.Information($"Processando Tempo para issue: {issue.Key}");
            try
            {
                _log.Debug($"Obtendo detalhes da issue {issue.Key} para calcular Tempo.");
                var details = _detailRepository.GetIssueDetails(issue.Key)?.ToList();

                if (details == null || !details.Any())
                {
                    _log.Information($"Nenhum detalhe encontrado para a issue {issue.Key}. Não é possível calcular Tempo.");
                    // Optionally mark as processed if no details mean no time changes ever
                    // issue.TimeProcessed = true;
                    // _issueRepository.SaveTime(issue); // Need a method to just mark as processed
                    return;
                }

                _log.Debug($"Calculando Lead Time e Cycle Time para issue {issue.Key}.");
                double leadTime = GetLeadTime(issue, details);
                double cycleTime = GetCycleTime(issue, details);

                issue.LeadTime = leadTime;
                issue.CycleTime = cycleTime;
                issue.TimeProcessed = true; // Mark as processed after attempting calculation

                _log.Debug($"Atualizando dados de Tempo (Lead Time: {issue.LeadTime}, Cycle Time: {issue.CycleTime}) para issue {issue.Key} no repositório.");
                var issueUpdated = _issueRepository.SaveTime(issue);

                if (issueUpdated)
                {
                    _log.Information($"Dados de Tempo para issue {issue.Key} atualizados com sucesso.");
                }
                else
                {
                    _log.Error($"Falha ao atualizar dados de Tempo para issue {issue.Key}.");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro ao processar Tempo para a issue {issue.Key}.");
            }
        }

        public double GetCycleTime(Issue issue, IEnumerable<Detail> details)
        {
            _log.Debug($"Calculando Cycle Time para issue {issue.Key}.");
            try
            {
                var statusChanges = details.Where(x => x.Type?.Equals("status", StringComparison.OrdinalIgnoreCase) == true).OrderBy(x => x.Created).ToList();

                var doneStatusChange = statusChanges.LastOrDefault(x => x.To?.Equals("Done", StringComparison.OrdinalIgnoreCase) == true || x.To?.Equals("Concluído", StringComparison.OrdinalIgnoreCase) == true);

                if (doneStatusChange == null)
                {
                    _log.Information($"Issue {issue.Key} ainda não está em status 'Done' ou 'Concluído'. Cycle Time não pode ser calculado.");
                    return 0;
                }

                // Find the first status change to 'In Progress' or similar states
                var inProgressStatusChange = statusChanges.FirstOrDefault(x =>
                    x.To?.Equals("In Progress", StringComparison.OrdinalIgnoreCase) == true ||
                    x.To?.Equals("Analisar bug", StringComparison.OrdinalIgnoreCase) == true ||
                    x.To?.Equals("In Development", StringComparison.OrdinalIgnoreCase) == true ||
                    x.To?.Equals("Em desenvolvimento", StringComparison.OrdinalIgnoreCase) == true);

                if (inProgressStatusChange == null)
                {
                    _log.Warning($"Não foi encontrado um status inicial ('In Progress', etc.) para a issue {issue.Key}. Cycle Time não pode ser calculado.");
                    return 0; // Or throw an exception depending on desired behavior
                }

                var startDate = inProgressStatusChange.Created;
                var endDate = doneStatusChange.Created;

                var timeSpan = endDate - startDate;

                // Return in days, including fractional days
                return timeSpan.TotalDays;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro ao calcular Cycle Time para a issue {issue.Key}.");
                return 0; // Return 0 or re-throw on error
            }
        }

        public double GetLeadTime(Issue issue, IEnumerable<Detail> details)
        {
            _log.Debug($"Calculando Lead Time para issue {issue.Key}.");
            try
            {
                var statusChanges = details.Where(x => x.Type?.Equals("status", StringComparison.OrdinalIgnoreCase) == true).OrderBy(x => x.Created).ToList();

                var doneStatusChange = statusChanges.LastOrDefault(x => x.To?.Equals("Done", StringComparison.OrdinalIgnoreCase) == true || x.To?.Equals("Concluído", StringComparison.OrdinalIgnoreCase) == true);

                if (doneStatusChange == null)
                {
                    _log.Information($"Issue {issue.Key} ainda não está em status 'Done' ou 'Concluído'. Lead Time não pode ser calculado.");
                    return 0;
                }

                var endDate = doneStatusChange.Created;
                var startDate = issue.Created; // Lead Time is from creation date to done date

                var timeSpan = endDate - startDate;

                // Return in days, including fractional days
                return timeSpan.TotalDays;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro ao calcular Lead Time para a issue {issue.Key}.");
                return 0; // Return 0 or re-throw on error
            }
        }
    }
}

