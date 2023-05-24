using Core.Common.Configuration;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Core.Common.Data {
    public class DataRepository<T> where T : class, new() {

        protected SqlConnection db;
        protected string _connectionName;
        

        public DataRepository(string connectionName) {
            _connectionName = connectionName;
        }

        public DataRepository() {

            _connectionName = "OutLineDb";
        }

        protected SqlConnection GetConnection(string connectionName = null) {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName ?? _connectionName].ConnectionString);
        }

        public IEnumerable<E> Query<E>(string query, object param = null, string connectionName = null) {
            using (var db = GetConnection(connectionName)) {
                return db.Query<E>(query, param, commandTimeout: 5000000);
            }
        }
       
        public int Execute(string sql, object param = null, string connectionName = null) {
            using (var db = GetConnection(connectionName)) {
                return db.Execute(sql, param, commandTimeout: 5000000);
            }
        }

        public E ExecuteScalar<E>(string sql, object param = null, string connectionName = null) {
            using (var db = GetConnection(connectionName)) {
                return db.ExecuteScalar<E>(sql, param, commandTimeout: 5000000);
            }
        }

      


    }
}
