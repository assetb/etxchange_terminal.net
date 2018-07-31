using AltaMySqlDB.model;
using AltaMySqlDB.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaArchiveApp.Services {
    public static class DataManagerService {
        private static IArchiveDataManager archiveDbManager = new ArchiveDbContext();
        private static IDataManager brokerDbManager = new EntityContext();


        public static IArchiveDataManager Instanse() {
            return archiveDbManager;
        }


        public static IDataManager TradingInstance() {
            return brokerDbManager;
        }
    }
}
