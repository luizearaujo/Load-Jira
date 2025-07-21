using Dapper;
using LoadJira.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using Serilog;
using System;

namespace LoadJira.Infra.Repository
{
    public class TypeRepository : BaseRepository<LoadJira.Entities.Type>
    {
        public TypeRepository(ILogger log) : base(log) { }

        public override bool Save(LoadJira.Entities.Type type)
        {
            _log.Information($"Tentando salvar tipo {type.Id} - {type.Name}.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    var typeInDatabase = connection.QueryFirstOrDefault<LoadJira.Entities.Type>(command.TypeCommand.GetCommand, new { id = type.Id });

                    if (typeInDatabase == null)
                    {
                        return Insert(connection, type);
                    }
                    else
                    {
                        return Update(connection, type);
                    }
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao salvar tipo {type.Id} - {type.Name}.");
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao salvar tipo {type.Id} - {type.Name}.");
                    return false;
                }
            }
        }

        public override bool Save(IList<LoadJira.Entities.Type> entities)
        {
            _log.Warning("Este método Save(IList<Type> entities) não é implementado para TypeRepository. Use Save(Type type) em um loop.");
            return false;
        }

        private LoadJira.Entities.Type Get(SqlConnection connection, int id)
        {
            _log.Debug($"Buscando tipo {id} no banco de dados.");
            try
            {
                return connection.QueryFirstOrDefault<LoadJira.Entities.Type>(command.TypeCommand.GetCommand, new { id });
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao buscar tipo {id}.");
                return null;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao buscar tipo {id}.");
                return null;
            }
        }

        private bool Insert(SqlConnection connection, LoadJira.Entities.Type type)
        {
            _log.Information($"Inserindo tipo {type.Id} - {type.Name}.");
            try
            {
                var data = connection.Execute(command.TypeCommand.InsertCommand, type);
                if (data == 1)
                {
                    _log.Debug($"Tipo {type.Id} inserido com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar inserir o tipo {type.Id}.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao inserir tipo {type.Id} - {type.Name}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao inserir tipo {type.Id} - {type.Name}.");
                return false;
            }
        }

        private bool Update(SqlConnection connection, LoadJira.Entities.Type type)
        {
            _log.Information($"Atualizando tipo {type.Id} - {type.Name}.");
            try
            {
                var data = connection.Execute(command.TypeCommand.UpdateCommand, type);
                if (data == 1)
                {
                    _log.Debug($"Tipo {type.Id} atualizado com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar atualizar o tipo {type.Id}. Pode não existir ou dados são os mesmos.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao atualizar tipo {type.Id} - {type.Name}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao atualizar tipo {type.Id} - {type.Name}.");
                return false;
            }
        }
    }
}

