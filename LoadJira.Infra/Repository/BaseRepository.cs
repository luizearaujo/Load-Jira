using System.Data.SqlClient;
using Serilog;
using System;
using System.Collections.Generic;

namespace LoadJira.Infra.Repository
{
    public abstract class BaseRepository<T>
    {
        protected readonly string _connectionString;
        protected readonly ILogger _log;

        public BaseRepository(ILogger log)
        {
            _log = log.ForContext(GetType());
            _connectionString = Config.Config.SqlServerConn; // Considerar injetar isso também
        }

        protected SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                _log.Debug("Conexão com o banco de dados aberta com sucesso.");
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Erro ao abrir conexão com o banco de dados.");
                throw; // Re-throw to propagate the error
            }
            return connection;
        }

        public abstract bool Save(T entity);
        public abstract bool Save(IList<T> entities);

        // Adicionar métodos para Get, Update, Delete se necessário, seguindo o mesmo padrão
    }
}


