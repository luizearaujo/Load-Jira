using Dapper;
using LoadJira.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using Serilog;
using System;

namespace LoadJira.Infra.Repository
{
    public class StatusRepository : BaseRepository<Status>
    {
        public StatusRepository(ILogger log) : base(log) { }

        public override bool Save(Status status)
        {
            _log.Information($"Tentando salvar status {status.Id} - {status.Name}.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    var statusInDatabase = connection.QueryFirstOrDefault<Status>(command.StatusCommand.GetCommand, new { id = status.Id });

                    if (statusInDatabase == null)
                    {
                        return Insert(connection, status);
                    }
                    else
                    {
                        return Update(connection, status);
                    }
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao salvar status {status.Id} - {status.Name}.");
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao salvar status {status.Id} - {status.Name}.");
                    return false;
                }
            }
        }

        public override bool Save(IList<Status> entities)
        {
            _log.Warning("Este método Save(IList<Status> entities) não é implementado para StatusRepository. Use Save(Status status) em um loop.");
            return false;
        }

        private Status Get(SqlConnection connection, int id)
        {
            _log.Debug($"Buscando status {id} no banco de dados.");
            try
            {
                return connection.QueryFirstOrDefault<Status>(command.StatusCommand.GetCommand, new { id });
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao buscar status {id}.");
                return null;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao buscar status {id}.");
                return null;
            }
        }

        private bool Insert(SqlConnection connection, Status status)
        {
            _log.Information($"Inserindo status {status.Id} - {status.Name}.");
            try
            {
                var data = connection.Execute(command.StatusCommand.InsertCommand, status);
                if (data == 1)
                {
                    _log.Debug($"Status {status.Id} inserido com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar inserir o status {status.Id}.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao inserir status {status.Id} - {status.Name}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao inserir status {status.Id} - {status.Name}.");
                return false;
            }
        }

        private bool Update(SqlConnection connection, Status status)
        {
            _log.Information($"Atualizando status {status.Id} - {status.Name}.");
            try
            {
                var data = connection.Execute(command.StatusCommand.UpdateCommand, status);
                if (data == 1)
                {
                    _log.Debug($"Status {status.Id} atualizado com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar atualizar o status {status.Id}. Pode não existir ou dados são os mesmos.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao atualizar status {status.Id} - {status.Name}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao atualizar status {status.Id} - {status.Name}.");
                return false;
            }
        }
    }
}

