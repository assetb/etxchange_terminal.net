using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace AltaTransport
{
    public class MSSQLTransport
    {
        private SqlConnection sqlServer;


        public bool Connected()
        {
            if (sqlServer == null || sqlServer.State.HasFlag(ConnectionState.Open)) GetConnectedServer();
            if (sqlServer == null) {
                MessageBox.Show("No connection to Server. \nCheck setings.", "Connection", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            return true;
        }


        public void GetConnectedServer()
        {
            sqlServer = new SqlConnection {ConnectionString = @"Data Source=10.1.1.3;User ID=sa;Password=korund2014"};

            try {
                sqlServer.Open();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Connection", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }


        public DataSet Execute(string query)
        {
            if (Connected()) {
                var sqlData = new DataSet();
                var sqlAdapter = new SqlDataAdapter {SelectCommand = new SqlCommand(query, sqlServer)};
                try {
                    sqlAdapter.Fill(sqlData);
                    return sqlData;
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message + "\nClosing App.", "Connection", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }

            return null;
        }


        public void Close()
        {
            sqlServer.Close();
            sqlServer = null;
        }
    }
}
