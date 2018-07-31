using AltaMySqlDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RegistrationUserService.service
{
    public static class AuthService
    {

        public static Boolean CheckLoginAndPass(string login, string pass) {
            var context = new EntityContext();
            return context.users.FirstOrDefault(u => (u.login == login && u.pass == pass)) != null;
        }
    }
}