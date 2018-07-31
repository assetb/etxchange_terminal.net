using AltaArchiveApp;
using AltaMySqlDB.model;
using AltaMySqlDB.service;
using AuthorizationApp;
using AutoMapper;
using DocumentFormation;
using EtsApp;
using Microsoft.Practices.Unity;
using ServerApp.Services;
using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Unity.WebApi;

namespace ServerApp.App_Start
{
    public class UnityConfig
    {
        public const string BROKER_BASE = "BrokerBase";
        public const string SUB_SYSTEM_PAYDA_TRADE = "SybSystemPaydaTrade";
        public const string USER_REPOSITORY = "UserRepository";

        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        public static void Register(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityDependencyResolver(GetConfiguredContainer());
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            var etsUrl = WebConfigurationManager.AppSettings["EtsUrl"];
            var tempDirectory = HttpContext.Current.Server.MapPath("~/App_Data");

            container.RegisterType<OnlineBindService, OnlineBindService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(etsUrl));

            //  ets api old version
            var etsApi = new EtsApi(string.Format("{0}/{1}", tempDirectory, "Online_war.ini"));
            container.RegisterInstance<IEtsApi>(etsApi);
            
            container.RegisterType<IDataManager, EntityContext>(BROKER_BASE, new InjectionConstructor(BROKER_BASE));
            container.RegisterType<IDataManager, EntityContext>(SUB_SYSTEM_PAYDA_TRADE, new InjectionConstructor(SUB_SYSTEM_PAYDA_TRADE));
            container.RegisterType<IAltaUserRepository, AltaUserRepository>(BROKER_BASE, new InjectionConstructor(BROKER_BASE));
            container.RegisterType<IAltaUserRepository, AltaUserRepository>(SUB_SYSTEM_PAYDA_TRADE, new InjectionConstructor(SUB_SYSTEM_PAYDA_TRADE));
            //archive
            container.RegisterType<ArchiveManager, ArchiveManager>(BROKER_BASE,
                new InjectionFactory((c, t, s) => new ArchiveManager(
                 container.Resolve<IDataManager>(BROKER_BASE),
                WebConfigurationManager.AppSettings["ArchiveHost"],
                WebConfigurationManager.AppSettings["ArchiveUser"],
                WebConfigurationManager.AppSettings["ArchivePass"],
                WebConfigurationManager.AppSettings["ArchiveRootPath"]
                )));

            container.RegisterType<ArchiveManager, ArchiveManager>(SUB_SYSTEM_PAYDA_TRADE,
                new InjectionFactory((c, t, s) => new ArchiveManager(
                 container.Resolve<IDataManager>(SUB_SYSTEM_PAYDA_TRADE),
                WebConfigurationManager.AppSettings["ArchiveHost_PaydaTrade"],
                WebConfigurationManager.AppSettings["ArchiveUser_PaydaTrade"],
                WebConfigurationManager.AppSettings["ArchivePass_PaydaTrade"],
                WebConfigurationManager.AppSettings["ArchiveRootPath_PaydaTrade"]
                )));

            container.RegisterInstance<IMapper>("SupplierMapper", MappingConfig.CreateSupplierMapper());
        }


    }
}