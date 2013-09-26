using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;

namespace Sys.Developer.Tools.Bdl.Data
{
    public class Connection : IConnection
    {
        private const string masterConnString = "server=.;initial catalog=master;user id=sa;password=Passw0rd;";
        private static SqlConnection sqlConn = null;

        #region IConnection Members

        public bool Connect(string connectionString)
        {
            bool isConnected = false;
            if (String.IsNullOrEmpty(connectionString))
            {
                // We are using the local 
                connectionString = masterConnString;
            }

            try
            {
                using (sqlConn = new SqlConnection(connectionString))
                {
                    // Open the connection
                    sqlConn.Open();
                    isConnected = true;
                    Console.WriteLine(String.Format("Successfully connected to {0} on {1}.", sqlConn.Database, sqlConn.DataSource));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return isConnected;
        }

        public void Disconnect()
        {
            if (sqlConn != null)
            {
                try
                {
                    if (sqlConn.State != ConnectionState.Closed)
                    {
                        // Close the exception
                        sqlConn.Close();
                        Console.WriteLine(String.Format("Successfully disconnected from {0} on {1}.", sqlConn.Database, sqlConn.DataSource));
                        sqlConn = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        #endregion
    }
}
