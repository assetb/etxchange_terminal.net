using System;
using System.Collections;
using System.Collections.Generic;
using AltaBO;
using System.Windows;
using ETSApp.Services;

namespace EtsApp
{
    public class EtsApi : IEtsApi
    {
        private DSSERVERLib.Connection connection;
        private DSSERVERLib.Online tableQuotes;
        private List<PriceOffer> priceOffer = new List<PriceOffer>();
        private bool isConnected = false, isLogin = false, isNetIntegrity = false;
        private int isConnectionTable = 0;
        private string iniFile = null;
        private string message = "";


        public EtsApi(bool isTest = false)
        {
            iniFile = @"Online" + (isTest ? "_test" : "_war") + ".ini";
        }


        public EtsApi(string pathToIniFile)
        {
            iniFile = pathToIniFile;
        }


        public bool GetConnection()
        {
            if (connection == null || !isConnected) OpenConnection();

            return isConnected;
        }


        public bool IsConnected()
        {
            return isConnected;
        }


        public int IsConnectionTable()
        {
            return isConnectionTable;
        }


        public bool OpenConnection()
        {
            if (!string.IsNullOrEmpty(iniFile)) {
                if (connection == null) {
                    connection = new DSSERVERLib.Connection();
                    connection.Disconnected += Connection_Disconnected;
                    connection.NetIntegrityOk += Connection_NetIntegrityOk;
                    connection.NetIntegrityLost += Connection_NetIntegrityLost;
                    connection.LoginOk += Connection_LoginOk;
                    connection.LoginFailed += Connection_LoginFailed;
                }
                try {
                    if (!isConnected) {
                        isConnected = connection.Open(iniFile, "", "", "");
                    }
                } catch (Exception ex) {
                    isConnected = false;
                    message = ex.Message;
                }
            }
            return isConnected;
        }


        private void Connection_NetIntegrityOk(int IDConnect)
        {
            isNetIntegrity = true;
        }


        private void Connection_LoginOk(int IDConnect)
        {
            isLogin = true;
        }


        private void Connection_LoginFailed(int IDConnect)
        {
            isLogin = false;
            Close();
        }


        private void Connection_NetIntegrityLost(int IDConnect)
        {
            isNetIntegrity = false;
            Close();
        }


        private void Connection_Disconnected(int IDConnect)
        {
            Close();
        }


        public void CloseConnection()
        {
            connection.Close();
            isConnected = false;
            isLogin = false;
            isNetIntegrity = false;
        }


        public void CloseTable()
        {
            tableQuotes.Close(0);
            isConnectionTable = 0;
        }


        public void Close()
        {
            CloseTable();
            CloseConnection();
        }


        public int QuotesConnection()
        {
            if (tableQuotes == null) {
                tableQuotes = new DSSERVERLib.Online();
                tableQuotes.AddRow += TableQuotesAddRow;
                tableQuotes.Disconnected += TableQuotes_Disconnected;
                tableQuotes.Synchronized += TableQuotes_Synchronized;
            }

            try {
                if (isConnectionTable == 0 && isLogin == true && isNetIntegrity == true) {
                    isConnectionTable = tableQuotes.Open(DSSERVERLib.ConnectionType.RTSONL_DYNAMIC, "Quote", "issue_name, price, firm_name, moment", "id", null, null, DSSERVERLib.Sort.RTSONL_SORT_EMPTY);
                }
            } catch (Exception ex) {
                message = ex.Message;
            }

            return isConnectionTable;
        }


        private void TableQuotes_Synchronized(int IDConnect) { }


        private void EtsApi_Error(int IDConnect)
        {
            throw new NotImplementedException();
        }


        private void TableQuotes_Disconnected(int IDConnect)
        {
            priceOffer.Clear();
            Close();
        }


        void TableQuotesAddRow(int IDConnect, int IDRecord, object Fields)
        {
            IList collection = (IList)Fields;

            priceOffer.Add(new PriceOffer() {
                lotCode = collection[0].ToString(),
                lotPriceOffer = collection[1].ToString(),
                firmName = collection[2].ToString(),
                offerTime = DateTime.ParseExact(collection[3].ToString(), "ddMMyyyyHHmmss", null)
            });

            DBManager.CreateStockQuote(priceOffer[priceOffer.Count - 1]);
        }


        public List<PriceOffer> GetPriceOffers()
        {
            return new List<PriceOffer>(priceOffer);
        }


        public string GetMessage()
        {
            return message;
        }
    }
}