using AltaBO;
using AltaMySqlDB.service;
using AuthorizationApp;
using ServerApp.App_Start;
using System.Web;
using System.Web.Http;
using Microsoft.Practices.Unity;
using AltaArchiveApp;

namespace ServerApp.Controllers
{
    public class BaseApiController : ApiController
    {
        private IDataManager dataManager;
        protected IDataManager DataManager
        {
            get
            {
                if (dataManager == null && CurrentUser != null)
                {
                    if (HttpContext.Current.User.IsInRole(WebApiApplication.SUB_SYSTEM_PYADA_TRADING))
                    {
                        dataManager = UnityConfig.GetConfiguredContainer().Resolve<IDataManager>(UnityConfig.SUB_SYSTEM_PAYDA_TRADE);
                    } else {
                        dataManager = UnityConfig.GetConfiguredContainer().Resolve<IDataManager>(UnityConfig.BROKER_BASE);
                    }
                }
                return dataManager;
            }
        }

        private ArchiveManager archiveManager;
        public ArchiveManager ArchiveManager { get {
                if (archiveManager == null && CurrentUser != null)
                {
                    if (HttpContext.Current.User.IsInRole(WebApiApplication.SUB_SYSTEM_PYADA_TRADING))
                    {
                        archiveManager = UnityConfig.GetConfiguredContainer().Resolve<ArchiveManager>(UnityConfig.SUB_SYSTEM_PAYDA_TRADE);
                    }
                    else
                    {
                        archiveManager = UnityConfig.GetConfiguredContainer().Resolve<ArchiveManager>(UnityConfig.BROKER_BASE);
                    }
                }
                return archiveManager;
            } }

        private User currentUser;
        protected User CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
                    {
                        currentUser = ((AltaUserIdentity)HttpContext.Current.User.Identity).User;
                    }
                }
                return currentUser;
            }
        }
    }
}