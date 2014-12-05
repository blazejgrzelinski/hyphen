using System;
 using System.Data;
 using System.Data.Odbc;
 using System.Data.SqlClient;
 using System.Data.OleDb;


namespace Hyphen.Core
 {
   public interface IDBManager
   {  
     string ConnectionString
     {
       get;
       set;
     }
  
     IDbConnection Connection
     {
       get;
     }
     IDbTransaction Transaction
     {
       get;
     }
  
     IDataReader DataReader
     {
       get;
     }
     IDbCommand Command
     {
       get;
     }
  
     IDbDataParameter[]Parameters
     {
       get;
     }
   }
 }