using AltaBO;
using AltaMySqlDB.model;
using AltaMySqlDB.model.tables;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AltaMySqlDB.service
{
    public class DataManager
    {
        public EntityContext Context { get; }


        public DataManager()
        {
            Context = new EntityContext();
        }
        

        public Auction GetAuction(int id)
        {
            throw new NotImplementedException();
        }

        public List<Auction> GetCustomerAuctions(int customerId, DateTime? fromDate, DateTime? toDate, int? site)
        {
            throw new NotImplementedException();
        }
    }
}
