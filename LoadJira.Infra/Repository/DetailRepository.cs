using Dapper;
using LoadJira.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using Serilog;
using System;

namespace LoadJira.Infra.Repository
{
    public class DetailRepository : BaseRepository<Detail>
    {
        public DetailRepository(ILogger log) : base(log) { }

        public override bool Save(Detail entity)
        {
            _log.Warning("Este método Save(Detail entity) não deve ser chamado diretamente para DetailRepository. Use Save(Detail detail, string issueKey).");
            return false; // Or throw an exception if this scenario is truly invalid
        }

        public bool Save(Detail detail, string issueKey)
        {
            _log.Information($"Tentando salvar detalhe {detail.Id} para a issue {issueKey}.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    var detailInDatabase = connection.QueryFirstOrDefault<Detail>(command.DetailCommand.GetCommand, new { id = detail.Id });

                    if (detailInDatabase == null)
                    {
                        return Insert(connection, detail, issueKey);
                    }
                    else
                    {
                        return Update(connection, detail, issueKey);
                    }
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao salvar detalhe {detail.Id} para a issue {issueKey}.");
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao salvar detalhe {detail.Id} para a issue {issueKey}.");
                    return false;
                }
            }
        }

        public override bool Save(IList<Detail> entities)
        {
            _log.Warning("Este método Save(IList<Detail> entities) não é implementado para DetailRepository. Use Save(Detail detail, string issueKey) em um loop.");
            return false;
        }

        private Detail Get(SqlConnection connection, int id)
        {
            _log.Debug($"Buscando detalhe {id} no banco de dados.");
            try
            {
                return connection.QueryFirstOrDefault<Detail>(command.DetailCommand.GetCommand, new { id });
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao buscar detalhe {id}.");
                return null;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao buscar detalhe {id}.");
                return null;
            }
        }

        private bool Insert(SqlConnection connection, Detail detail, string issueKey)
        {
            _log.Information($"Inserindo detalhe {detail.Id} para a issue {issueKey}.");
            try
            {
                var data = connection.Execute(command.DetailCommand.InsertCommand,
                    new
                    {
                        detail.Id,
                        detail.Created,
                        detail.Type,
                        detail.From,
                        detail.To,
                        IssueKey = issueKey,
                        AuthorId = detail.Author.Id
                    });
                if (data == 1)
                {
                    _log.Debug($"Detalhe {detail.Id} inserido com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar inserir o detalhe {detail.Id}.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao inserir detalhe {detail.Id} para a issue {issueKey}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao inserir detalhe {detail.Id} para a issue {issueKey}.");
                return false;
            }
        }

        public IEnumerable<Detail> GetIssueDetails(string IssueKey)
        {
            _log.Debug($"Buscando detalhes para a issue {IssueKey} no banco de dados.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    return connection.Query<Detail>(command.DetailCommand.GetIssueDetailsCommand, new { IssueKey }).ToList();
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao buscar detalhes para a issue {IssueKey}.");
                    return new List<Detail>();
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao buscar detalhes para a issue {IssueKey}.");
                    return new List<Detail>();
                }
            }
        }

        private bool Update(SqlConnection connection, Detail detail, string issueKey)
        {
            _log.Information($"Atualizando detalhe {detail.Id} para a issue {issueKey}.");
            try
            {
                var data = connection.Execute(command.DetailCommand.UpdateCommand, new
                {
                    detail.Id,
                    detail.Created,
                    detail.Type,
                    detail.From,
                    detail.To,
                    IssueKey = issueKey,
                    AuthorId = detail.Author.Id
                });
                if (data == 1)
                {
                    _log.Debug($"Detalhe {detail.Id} atualizado com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar atualizar o detalhe {detail.Id}. Pode não existir ou dados são os mesmos.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao atualizar detalhe {detail.Id} para a issue {issueKey}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao atualizar detalhe {detail.Id} para a issue {issueKey}.");
                return false;
            }
        }
    }
}

