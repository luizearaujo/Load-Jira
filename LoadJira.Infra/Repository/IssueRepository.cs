using Dapper;
using LoadJira.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Serilog;
using System.Threading.Tasks;

namespace LoadJira.Infra.Repository
{
    public class IssueRepository : BaseRepository<Issue>
    {
        public IssueRepository(ILogger log) : base(log) { }

        public override bool Save(Issue entity)
        {
            _log.Warning("Este método Save(Issue entity) não deve ser chamado diretamente para IssueRepository. Use Save(IEnumerable<Issue> issues).");
            return false; // Or throw an exception if this scenario is truly invalid
        }

        public override bool Save(IList<Issue> issues)
        {
            _log.Information($"Tentando salvar {issues.Count} issues no banco de dados.");
            var savedCount = 0;
            using (var connection = GetOpenConnection())
            {
                foreach (var issueKey in issues)
                {
                    try
                    {
                        var issueFromDatabase = connection.QueryFirstOrDefault<Issue>(command.IssueCommand.GetCommand, new { key = issueKey.Key });

                        if (issueFromDatabase == null)
                        {
                            savedCount += connection.Execute(command.IssueCommand.InsertCommand, issueKey);
                            _log.Debug($"Issue {issueKey.Key} inserida com sucesso.");
                        }
                        else
                        {
                            _log.Debug($"Issue {issueKey.Key} já existe no banco de dados. Pulando inserção.");
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        _log.Error(sqlEx, $"Erro SQL ao salvar a issue {issueKey.Key}.");
                    }
                    catch (System.Exception ex)
                    {
                        _log.Error(ex, $"Erro inesperado ao salvar a issue {issueKey.Key}.");
                    }
                }
            }
            _log.Information($"Total de {savedCount} issues salvas/atualizadas com sucesso.");
            return savedCount == issues.Count; // Returns true if all issues were processed without error and inserted/updated
        }

        public Issue Get(string key)
        {
            _log.Debug($"Buscando issue {key} no banco de dados.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    return connection.QueryFirstOrDefault<Issue>(command.IssueCommand.GetCommand, new { key });
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao buscar a issue {key}.");
                    return null;
                }
                catch (System.Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao buscar a issue {key}.");
                    return null;
                }
            }
        }

        public bool Update(Issue issue)
        {
            _log.Information($"Atualizando issue {issue.Key} no banco de dados.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    var data = connection.Execute(command.IssueCommand.UpdateCommand,
                        new
                        {
                            Processed = issue.Processed,
                            Summary = issue.Summary,
                            StatusId = issue.Status.Id,
                            ProjectKey = issue.Project.Key,
                            ReporterId = issue.Reporter.Id,
                            TypeId = issue.Type.Id,
                            Description = issue.Description,
                            Created = issue.Created,
                            Key = issue.Key
                        });
                    if (data == 1)
                    {
                        _log.Debug($"Issue {issue.Key} atualizada com sucesso.");
                        return true;
                    }
                    _log.Warning($"Nenhuma linha afetada ao tentar atualizar a issue {issue.Key}. Pode não existir ou dados são os mesmos.");
                    return false;
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao atualizar a issue {issue.Key}.");
                    return false;
                }
                catch (System.Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao atualizar a issue {issue.Key}.");
                    return false;
                }
            }
        }

        public bool SaveTime(Issue issue)
        {
            _log.Information($"Salvando informações de tempo para a issue {issue.Key}.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    var data = connection.Execute(command.IssueCommand.UpdateTimeCommand,
                        new
                        {
                            issue.TimeProcessed,
                            issue.CycleTime,
                            issue.LeadTime,
                            issue.Key
                        });
                    if (data == 1)
                    {
                        _log.Debug($"Informações de tempo para a issue {issue.Key} salvas com sucesso.");
                        return true;
                    }
                    _log.Warning($"Nenhuma linha afetada ao tentar salvar informações de tempo para a issue {issue.Key}.");
                    return false;
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao salvar informações de tempo para a issue {issue.Key}.");
                    return false;
                }
                catch (System.Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao salvar informações de tempo para a issue {issue.Key}.");
                    return false;
                }
            }
        }

        public bool SaveStoryPoints(Issue issue)
        {
            _log.Information($"Salvando Story Points para a issue {issue.Key}.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    var data = 0;

                    if (issue.FirstStoryPoint.HasValue || issue.LastStoryPoint.HasValue)
                    {
                        if (issue.FirstStoryPoint.HasValue && issue.LastStoryPoint.HasValue)
                        {
                            var entityToUpdate = new { issue.Key, FirstStoryPoint = issue.FirstStoryPoint.Value, LastStoryPoint = issue.LastStoryPoint.Value };
                            data = connection.Execute(command.IssueCommand.UpdateStoryPointsCommand, entityToUpdate);
                            _log.Debug($"Story Points (First e Last) para a issue {issue.Key} atualizados.");
                        }
                        else if (issue.FirstStoryPoint.HasValue && !issue.LastStoryPoint.HasValue)
                        {
                            var entityToUpdate = new { issue.Key, FirstStoryPoint = issue.FirstStoryPoint.Value };
                            data = connection.Execute(command.IssueCommand.UpdateFirstStoryPointCommand, entityToUpdate);
                            _log.Debug($"First Story Point para a issue {issue.Key} atualizado.");
                        }
                        else if (!issue.FirstStoryPoint.HasValue && issue.LastStoryPoint.HasValue)
                        {
                            var entityToUpdate = new { issue.Key, LastStoryPoint = issue.LastStoryPoint.Value };
                            data = connection.Execute(command.IssueCommand.UpdateLastStoryPointCommand, entityToUpdate);
                            _log.Debug($"Last Story Point para a issue {issue.Key} atualizado.");
                        }

                        if (data == 1)
                        {
                            _log.Debug($"Story Points para a issue {issue.Key} salvos com sucesso.");
                            return true;
                        }
                        _log.Warning($"Nenhuma linha afetada ao tentar salvar Story Points para a issue {issue.Key}.");
                        return false;
                    }

                    // Caso ambos sejam nulos, atualiza para nulo no banco
                    var entityWithoutValueToUpdate = new { issue.Key };
                    data = connection.Execute(command.IssueCommand.UpdatLastStoryPointWithoutValueCommand, entityWithoutValueToUpdate);
                    if (data == 1)
                    {
                        _log.Debug($"Story Points para a issue {issue.Key} definidos como nulos com sucesso.");
                        return true;
                    }
                    _log.Warning($"Nenhuma linha afetada ao tentar definir Story Points como nulos para a issue {issue.Key}.");
                    return false;
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao salvar Story Points para a issue {issue.Key}.");
                    return false;
                }
                catch (System.Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao salvar Story Points para a issue {issue.Key}.");
                    return false;
                }
            }
        }

        public IEnumerable<Issue> Get()
        {
            _log.Debug("Buscando todas as issues no banco de dados.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    return connection.Query<Issue>(command.IssueCommand.GetAllCommand).ToList();
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, "Erro SQL ao buscar todas as issues.");
                    return new List<Issue>();
                }
                catch (System.Exception ex)
                {
                    _log.Error(ex, "Erro inesperado ao buscar todas as issues.");
                    return new List<Issue>();
                }
            }
        }
    }
}

