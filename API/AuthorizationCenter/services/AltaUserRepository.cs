using AltaMySqlDB.model;
using AuthorizationApp;

namespace AuthorizationCenter.services
{
    public class AltaUserRepository : EntityContext, IAltaUserRepository
    {
        public AltaUserRepository(string connectionString) : base(connectionString) {

        }
    }
}