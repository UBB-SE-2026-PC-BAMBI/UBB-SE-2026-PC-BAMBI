using Microsoft.Data.SqlClient;
namespace BankApp.Server.DataAccess
{
    public class AppDbContext : IDisposable
    {
        // Content deprecated: Should implement IDbContext interface and behave according to 
        // the UML diagram

        // TODO: implement
        public AppDbContext(string connectionString) { }
        public SqlConnection GetConnection() { throw new NotImplementedException(); }
        public SqlTransaction BeginTransaction() { throw new NotImplementedException(); }
        public void CommitTransaction() { throw new NotImplementedException(); }
        public void RollbackTransaction() { throw new NotImplementedException(); }
        public SqlTransaction? GetCurrentTransaction() { throw new NotImplementedException(); }
        public void Dispose() { }
    }
}