using AltaBO;
using AuthorizationApp.Specifics;
using System;
using System.Security.Principal;

namespace AuthorizationApp
{
    public class AltaUserIdentity : IIdentity
    {
        private string login;
        private User user;
        private IAltaUserRepository repository;

        public AltaUserIdentity(string login)
        {
            this.login = login;
            user = new User()
            {
                Login = this.login
            };
        }

        public AltaUserIdentity(string login, IAltaUserRepository repository)
        {
            this.login = login;
            this.repository = repository;
        }

        public User User
        {
            get
            {
                if (user == null || user.Login != login)
                {
                    if (!string.IsNullOrEmpty(login) && repository != null)
                        user = repository.GetUser(login);
                    else
                        user = null;
                }
                return user;
            }
        }

        public bool IsRoles(string roles)
        {
            if (User != null)
            {
                foreach (var role in roles.Split(';'))
                {
                    try
                    {
                        switch ((BasicRolesEnum)Enum.Parse(typeof(BasicRolesEnum), role, true))
                        {
                            case (BasicRolesEnum.Company):
                                return User.CompanyId > 0;
                            case (BasicRolesEnum.Customer):
                                return User.CustomerId > 0;
                            case (BasicRolesEnum.Supplier):
                                return User.SupplierId > 0;
                            case (BasicRolesEnum.Broker):
                                return User.BrokerId > 0;
                        }
                    }
                    catch (Exception)
                    {

                    }

                    if (User.Role.Equals(role))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string AuthenticationType
        {
            get
            {
                return typeof(User).ToString();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return User != null;
            }
        }

        public string Name
        {
            get
            {
                return User != null ? User.Login : "anonym";
            }
        }
    }
}
