using AltaMySqlDB.model;
using AltaMySqlDB.service;
using AuthorizationApp;
using AuthorizationApp.Services;
using AuthorizationCenter.Models;
using AuthorizationCenter.services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthorizationCenter.Controllers
{
    [RoutePrefix("api/login")]
    public class LoginApiController : ApiController
    {
        IAltaUserRepository repository;
        IDataManager dataManager;

        public LoginApiController(IAltaUserRepository repository, IDataManager dataManager)
        {
            this.repository = repository;
            this.dataManager = dataManager;
        }

        [HttpGet, Route("")]
        public String Login()
        {
            return "You need to login.";
        }

        [HttpGet, Route("broker/supplier/{companyId}")]
        public HttpResponseMessage LoginBroker(int companyId, string redirectUrl)
        {
            var users = dataManager.GetUsersByCompany(companyId);
            if (users.Count > 0)
            {
                var user = users.First();

                var formLogin = new FormLogin()
                {
                    login = user.Login,
                    pass = dataManager.GetPasswordUser(user.Id),
                    remembeMe = true,
                    ReturnUrl = redirectUrl
                };
                return Login(formLogin);
            }
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
        }

        [HttpPost, Route("")]
        public HttpResponseMessage Login(FormLogin form)
        {
            var user = repository.LogIn(form.login, form.pass);

            if (user != null)
            {
                if (System.Web.HttpContext.Current.Response.AddUserInCookie(user, form.remembeMe))
                {
                    if (!string.IsNullOrEmpty(form.ReturnUrl))
                    {
                        var responce = new HttpResponseMessage(HttpStatusCode.Redirect);
                        responce.Headers.Add("user", Convert.ToString(user.Id));
                        responce.Headers.Location = new Uri(form.ReturnUrl, UriKind.RelativeOrAbsolute);
                        return responce;
                    }
                    else
                        return new HttpResponseMessage(HttpStatusCode.OK);

                }
            }

            return new HttpResponseMessage(HttpStatusCode.Forbidden);
        }
    }
}
