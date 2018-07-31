using AltaBO;
using AltaMySqlDB.service;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/profile")]
    public class ProfileController : BaseApiController
    {
        [HttpGet, Route("")]
        public User GetUser()
        {
            var user = DataManager.GetUser(CurrentUser.Id);

            if (user == null) return null;

            var company = DataManager.GetCompanyByUserId(user.Id);

            if (company != null)
            {
                user.CompanyId = company.id;
                user.CustomerId = DataManager.GetCustomerId(company.id);
                user.SupplierId = DataManager.GetSupplierId(company.id);
            }
            return user;
        }

    }
}