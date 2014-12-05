using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace Hyphen.Sql
{
    class ConnectionManager: IDisposable
    {
        SqlConnection conn; 

        public SqlConnection Connection
        {
            get { return conn; }
        }
        public ConnectionManager()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AppConnectionString"].ConnectionString);
            conn.Open();
        }


        #region IDisposable Members

        public void Dispose()
        {
            conn.Close();
        }

        #endregion
    }
}
