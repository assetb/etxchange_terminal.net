using AltaMySqlDB.model;
using AltaMySqlDB.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaTradingSystemApp.Services
{
    public static class DataManagerService
    {
        private static IDataManager brokerDbManager = new EntityContext();


        public static IDataManager TradingInstance()
        {
            return brokerDbManager;
        }
    }
}
