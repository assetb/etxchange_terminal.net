using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ETSApp {
    public class ETSExchangeAPI {
        #region Variables
        private static DSSERVERLib.Connection etsConnection;
        private static DSSERVERLib.Online tableQuotes;

        private static int quotesTableConId = 0;
        public static bool infinityRestart = false, isOnline = false;
        #endregion

        #region Functions
        public static void ConnectAndOpen() {
            Task conAndOpen = new Task(conAndOpenTask);
            conAndOpen.Start();
        }


        private static void conAndOpenTask() {
            while(infinityRestart) {
                if(GetConnection()) {
                    if(GetQuotesConnection() != 0) {
                        isOnline = true;
                        infinityRestart = false;
                    }
                }

                RestartConnection(10);
            }
        }


        public static bool GetConnection() {
            if(etsConnection == null) OpenConnection();

            return etsConnection == null ? false : true;
        }


        private static void OpenConnection() {
            etsConnection = new DSSERVERLib.Connection();
            etsConnection.Disconnected += EtsConnection_Disconnected;

            try {
                etsConnection.Open("Online_test.ini", "", "", "");
            } catch(Exception) { etsConnection = null; }
        }


        private static void EtsConnection_Disconnected(int IDConnect) {
            isOnline = false;
        }


        public static int GetQuotesConnection() {
            if(quotesTableConId == 0) {
                tableQuotes = new DSSERVERLib.Online();
                tableQuotes.AddRow += TableQuotes_AddRow;
                tableQuotes.Disconnected += TableQuotes_Disconnected;

                try {
                    quotesTableConId = tableQuotes.Open(DSSERVERLib.ConnectionType.RTSONL_DYNAMIC, "Quote", "issue_name, price, firm_name", "id", null, null, DSSERVERLib.Sort.RTSONL_SORT_EMPTY);
                } catch(Exception) { quotesTableConId = 0; }
            }

            return quotesTableConId;
        }


        private static void TableQuotes_Disconnected(int IDConnect) {
            isOnline = false;
        }


        public static void TableQuotes_AddRow(int IDConnect, int IDRecord, object Fields) {
            IList collection = (IList)Fields;
        }


        public static void RestartConnection(int seconds) {
            if(quotesTableConId != 0 && tableQuotes != null) tableQuotes.Close(quotesTableConId);

            if(etsConnection != null) etsConnection.Close();

            tableQuotes = null;
            etsConnection = null;

            // First variant of timing
            //Thread.Sleep(seconds * 1000);

            // Second variant
            DateTime curTime = DateTime.Now;
            DateTime newTime = curTime.AddSeconds(seconds);

            while(newTime <= DateTime.Now) { }
        }
        #endregion
    }
}