using AltaBO;

namespace AuthorizationApp
{
    public interface IAltaUserRepository
    {
        User GetUser(string login);
        User GetUser(string login, string pass);
    }
}
