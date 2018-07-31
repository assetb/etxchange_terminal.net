using System.Security.Principal;

namespace AuthorizationApp
{
    public class AltaUserProvider : IPrincipal
    {
        private AltaUserIdentity userIdentity;

        public AltaUserProvider(string login) {
            userIdentity = new AltaUserIdentity(login);
        }

        public AltaUserProvider(string login, IAltaUserRepository repository)
        {
            userIdentity = new AltaUserIdentity(login, repository);
        }

        public IIdentity Identity
        {
            get
            {
                return userIdentity;
            }
        }

        public bool IsInRole(string role)
        {
            return userIdentity != null ? userIdentity.IsRoles(role) : false;

        }

        public override string ToString()
        {
            return userIdentity.Name;
        }
    }
}
