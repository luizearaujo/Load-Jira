using Dapper;
using LoadJira.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using Serilog;
using System;

namespace LoadJira.Infra.Repository
{
    public class PersonRepository : BaseRepository<Person>
    {
        public PersonRepository(ILogger log) : base(log) { }

        public override bool Save(Person person)
        {
            _log.Information($"Tentando salvar pessoa {person.Id}.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    var personInDatabase = connection.QueryFirstOrDefault<Person>(command.PersonCommand.GetCommand, new { id = person.Id });

                    if (personInDatabase == null)
                    {
                        return Insert(connection, person);
                    }
                    else
                    {
                        return Update(connection, person);
                    }
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao salvar pessoa {person.Id}.");
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao salvar pessoa {person.Id}.");
                    return false;
                }
            }
        }

        public override bool Save(IList<Person> entities)
        {
            _log.Warning("Este método Save(IList<Person> entities) não é implementado para PersonRepository. Use Save(Person person) em um loop.");
            return false;
        }

        private Person Get(SqlConnection connection, string id)
        {
            _log.Debug($"Buscando pessoa {id} no banco de dados.");
            try
            {
                return connection.QueryFirstOrDefault<Person>(command.PersonCommand.GetCommand, new { id });
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao buscar pessoa {id}.");
                return null;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao buscar pessoa {id}.");
                return null;
            }
        }

        private bool Insert(SqlConnection connection, Person person)
        {
            _log.Information($"Inserindo pessoa {person.Id}.");
            try
            {
                var data = connection.Execute(command.PersonCommand.InsertCommand, person);
                if (data == 1)
                {
                    _log.Debug($"Pessoa {person.Id} inserida com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar inserir a pessoa {person.Id}.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao inserir pessoa {person.Id}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao inserir pessoa {person.Id}.");
                return false;
            }
        }

        private bool Update(SqlConnection connection, Person person)
        {
            _log.Information($"Atualizando pessoa {person.Id}.");
            try
            {
                var data = connection.Execute(command.PersonCommand.UpdateCommand, person);
                if (data == 1)
                {
                    _log.Debug($"Pessoa {person.Id} atualizada com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar atualizar a pessoa {person.Id}. Pode não existir ou dados são os mesmos.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao atualizar pessoa {person.Id}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao atualizar pessoa {person.Id}.");
                return false;
            }
        }
    }
}

