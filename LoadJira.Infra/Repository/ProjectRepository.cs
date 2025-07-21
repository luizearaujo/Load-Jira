using Dapper;
using LoadJira.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using Serilog;
using System;

namespace LoadJira.Infra.Repository
{
    public class ProjectRepository : BaseRepository<Project>
    {
        public ProjectRepository(ILogger log) : base(log) { }

        public override bool Save(Project project)
        {
            _log.Information($"Tentando salvar projeto {project.Key}.");
            using (var connection = GetOpenConnection())
            {
                try
                {
                    var projectInDatabase = connection.QueryFirstOrDefault<Project>(command.ProjectCommand.GetCommand, new { key = project.Key });

                    if (projectInDatabase == null)
                    {
                        return Insert(connection, project);
                    }
                    else
                    {
                        return Update(connection, project);
                    }
                }
                catch (SqlException sqlEx)
                {
                    _log.Error(sqlEx, $"Erro SQL ao salvar projeto {project.Key}.");
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Erro inesperado ao salvar projeto {project.Key}.");
                    return false;
                }
            }
        }

        public override bool Save(IList<Project> entities)
        {
            _log.Warning("Este método Save(IList<Project> entities) não é implementado para ProjectRepository. Use Save(Project project) em um loop.");
            return false;
        }

        private Project Get(SqlConnection connection, string key)
        {
            _log.Debug($"Buscando projeto {key} no banco de dados.");
            try
            {
                return connection.QueryFirstOrDefault<Project>(command.ProjectCommand.GetCommand, new { key });
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao buscar projeto {key}.");
                return null;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao buscar projeto {key}.");
                return null;
            }
        }

        private bool Insert(SqlConnection connection, Project project)
        {
            _log.Information($"Inserindo projeto {project.Key}.");
            try
            {
                var data = connection.Execute(command.ProjectCommand.InsertCommand, project);

                if (data == 1)
                {
                    _log.Debug($"Projeto {project.Key} inserido com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar inserir o projeto {project.Key}.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao inserir projeto {project.Key}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao inserir projeto {project.Key}.");
                return false;
            }
        }

        private bool Update(SqlConnection connection, Project project)
        {
            _log.Information($"Atualizando projeto {project.Key}.");
            try
            {
                var data = connection.Execute(command.ProjectCommand.UpdateCommand, project);
                if (data == 1)
                {
                    _log.Debug($"Projeto {project.Key} atualizado com sucesso.");
                    return true;
                }
                _log.Warning($"Nenhuma linha afetada ao tentar atualizar o projeto {project.Key}. Pode não existir ou dados são os mesmos.");
                return false;
            }
            catch (SqlException sqlEx)
            {
                _log.Error(sqlEx, $"Erro SQL ao atualizar projeto {project.Key}.");
                return false;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Erro inesperado ao atualizar projeto {project.Key}.");
                return false;
            }
        }
    }
}

