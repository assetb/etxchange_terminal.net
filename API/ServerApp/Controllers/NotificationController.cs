using AltaBO;
using AltaMySqlDB.service;
using System.Collections.Generic;
using System.Web.Http;

namespace ServerApp.Controllers
{
    [RoutePrefix("api/notification"), Authorize]
    public class NotificationController : BaseApiController
    {
        [HttpGet, Route("supplier"), Authorize(Roles = "supplier")]
        public List<Notification> GetSuplierNotifications()
        {
            return DataManager.GetNotifications(supplierId: CurrentUser.SupplierId, belon: 3);
        }
    }
}
