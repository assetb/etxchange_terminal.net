using AltaBO;
using AltaMySqlDB.service;
using AuthorizationApp;
using AuthorizationApp.Services;
using AuthorizationCenter.Models;
using System.Linq;
using System.Web.Mvc;

namespace AuthorizationCenter.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        IAltaUserRepository repository;
        IDataManager dataManager;

        public LoginController(IAltaUserRepository repository, IDataManager dataManager)
        {
            this.repository = repository;
            this.dataManager = dataManager;
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            return View(new FormLogin() { ReturnUrl = Request.Params["ReturnUrl"] });
        }

        private bool Registration(RegistrationForm form)
        {
            const int SUPPLIER_ROLE = 3;
            var company = dataManager.GetCompanies(bin: form.login).FirstOrDefault();

            if (company != null)
            {
                var person = new Person()
                {
                    name = (string.IsNullOrEmpty(form.personName) ? "Пользователь" : form.personName),
                    tel = form.tel
                };

                person.Id = dataManager.CreatePerson(person);
                if (person.Id > 0)
                {
                    if (dataManager.CreateUser(person.Id, form.login, form.pass, SUPPLIER_ROLE) > 0)
                    {
                        return dataManager.AddEmployee(company.id, person.Id, form.position);
                    }
                }
            }
            return false;
        }

        private bool NewSupplier(FormLogin form)
        {
            var company = dataManager.GetCompanies(bin: form.login).FirstOrDefault();
            if (company != null)
            {
                if (dataManager.GetUsersByCompany(company.id).Count == 0)
                {
                    if (dataManager.GetSupplierByCompanyId(company.id) != null)
                    {
                        return Registration(new RegistrationForm()
                        {
                            login = form.login,
                            pass = form.pass,
                            personName = "Пользователь",
                            position = "Сотрудник"
                        });
                    }
                }
            }

            return false;
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(FormLogin form)
        {
            var user = repository.LogIn(form.login, form.pass);

            if (user != null)
            {
                if (System.Web.HttpContext.Current.Response.AddUserInCookie(user, form.remembeMe))
                {
                    if (!string.IsNullOrEmpty(form.ReturnUrl))
                        return new RedirectResult(form.ReturnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            if (NewSupplier(form))
            {
                form.errorMessage = "Введите ещё раз логин и пароль.";
                form.login = null;
            }
            else
            {
                form.errorMessage = "Не верный логин и/или пароль";
            }

            form.pass = null;
            return View(form);
        }
    }
}