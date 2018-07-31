using AltaBO;
using System;
using System.Web;
using System.Web.Security;

namespace AuthorizationApp.Services
{
    public static class HttpAuthorizationHelper
    {
        public static User LogIn(this IAltaUserRepository repository, string login, string pass)
        {
            return repository.GetUser(login, pass);
        }

        public static bool AddUserInCookie(this HttpResponse responce, User user, bool isPersistent)
        {
            if (user != null)
            {
                var ticket = new FormsAuthenticationTicket(1, user.Login, DateTime.Now, DateTime.Now.AddDays(7), isPersistent, Guid.NewGuid().ToString(), FormsAuthentication.FormsCookiePath);
                var encryptedTiket = FormsAuthentication.Encrypt(ticket);

                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTiket)
                {
                    HttpOnly = false,
                    Value = encryptedTiket,
                    Expires = DateTime.Now.Add(FormsAuthentication.Timeout),
                    Secure = FormsAuthentication.RequireSSL,
                    Path = FormsAuthentication.FormsCookiePath,
                    Domain = FormsAuthentication.CookieDomain
                };

                responce.Cookies.Add(cookie);
                return true;
            }
            return false;
        }

        public static string GetUserNameFromCookie(this HttpContext context, HttpRequest request)
        {
            var cookie = request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null)
            {
                try
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(cookie.Value);
                    return authTicket != null ? authTicket.Name : string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public static void LogOut(this HttpContext context)
        {
            FormsAuthentication.SignOut();
            context.User = null;
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}
