using AltaMySqlDB.model;
using AuthorizationApp;

namespace ServerApp.Services
{
    public class AltaUserRepository : EntityContext, IAltaUserRepository
    {
        public AltaUserRepository(string connectionString) : base(connectionString) { }
    }
}