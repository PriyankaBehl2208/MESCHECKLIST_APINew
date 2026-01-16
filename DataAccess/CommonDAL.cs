using Microsoft.Extensions.Options;
using System.Data;
using Microsoft.Data.SqlClient;
namespace MESCHECKLIST.DataAccess
{
    public class CommonDAL : IDisposable
    {
        private readonly string _connectionString;
        private readonly string _EKLConnection;

        public CommonDAL(IOptions<DatabaseConfig> config)
        {

            var _configuration = new ConfigurationBuilder()
                                                   .AddJsonFile("appSettings.Development.json")
                                                   .Build();
            IConfigurationSection appSettings = _configuration.GetSection("ConnectionStrings");
            _connectionString = appSettings["DefaultConnection"];
            _EKLConnection = appSettings["EKLConnection"];

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException("Connection string is not initialized.");
            }
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private SqlConnection GetEKLConnection()
        {
            return new SqlConnection(_EKLConnection);
        }

        public async Task<DataSet> ExecuteStoredProcedureAsync_Dataset(string storedProcedure, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(storedProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddRange(parameters);
                }

                var dataSet = new DataSet();
                var dataAdapter = new SqlDataAdapter(command);

                await connection.OpenAsync();
                await Task.Run(() => dataAdapter.Fill(dataSet));

                return dataSet;
            }
        }
        public async Task<DataTable> ExecuteStoredProcedureAsync(string storedProcedure, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(storedProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddRange(parameters);
                }

                var dataTable = new DataTable();
                var dataAdapter = new SqlDataAdapter(command);

                await connection.OpenAsync();
                await Task.Run(() => dataAdapter.Fill(dataTable));

                return dataTable;
            }
        }

        public async Task<DataTable> ExecuteStoredProcedureAsyncEKL(string storedProcedure, SqlParameter[] parameters = null)
        {
            using (var connection = GetEKLConnection())
            using (var command = new SqlCommand(storedProcedure, connection))
            {
                command.CommandType = CommandType.Text;
                if (parameters != null)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddRange(parameters);
                }

                var dataTable = new DataTable();
                var dataAdapter = new SqlDataAdapter(command);

                await connection.OpenAsync();
                await Task.Run(() => dataAdapter.Fill(dataTable));

                return dataTable;
            }
        }

        public async Task<int> ExecuteNonQueryStoredProcedureAsync(string storedProcedure, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(storedProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<object> ExecuteScalarStoredProcedureAsync(string storedProcedure, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(storedProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddRange(parameters);
                }

                await connection.OpenAsync();
                return await command.ExecuteScalarAsync();
            }
        }

        // IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Nothing to dispose here since connections are handled in using statements.
        }
    }
}
