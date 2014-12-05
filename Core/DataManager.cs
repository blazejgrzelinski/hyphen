using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Hyphen.Core;
using Hyphen.Sql;
using System.Data.SqlClient;
using System.Data;
using Hyphen.Data;
using System.Diagnostics;
using System.Collections;
using Hyphen.Attibutes;

namespace Hyphen.Core
{
    public interface Filterable
    {
        void AddFilter(string name, object value);
    }
    public abstract class DataManager<T, U> : SqlManager, Filterable where U: new()
    {
        public DataManager()
        {
            STPGenerator stpg = new STPGenerator(this.GetType());
        }

        public T Persist(string query = null)
        {
            T retValue;
            using (ConnectionManager sqlConnection= new ConnectionManager())
            {
                Type t = this.GetType();
             
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection.Connection;
                if (query== null)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = String.Format("{0}Persist", t.Name);
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                }

             

                PropertyInfo[] pi = t.GetProperties();
                SqlParameterCollection sqlParams = cmd.Parameters;

                FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                foreach (PropertyInfo prop in pi)
                {

                    DbBinderIgnoreAttribute ignore = AttributeReader.GetDbBinderIgnore(prop);
                    DbTypeAttribute dba = AttributeReader.GetSqlDbType(t.GetProperty(prop.Name));

                    if (ignore != null)
                    {
                        if (ignore.IgnoranceFlag == IgnoranceFlag.AlwaysIgnore ||ignore.IgnoranceFlag == IgnoranceFlag.IgnoreUnbind)
                            continue;
                    }

                    if (dba != null)
                    {
                        SqlParameter sp = new SqlParameter();
                        sp.DbType = dba.Type;
                        sp.ParameterName = String.Format("@{0}", prop.Name);
                        sp.Value =t.GetProperty(prop.Name).GetValue(this, null);

                        //ignore parameters when the Ignore when null flag is set and parameter has null value
                        if (ignore != null)
                        {
                            if (ignore.IgnoreWhenNull == true)
                            {
                                if (sp.Value == null)
                                    continue;
                            }
                        }

                        sqlParams.Add(sp);
                    }
                }

                try
                {
                    retValue = (T)cmd.ExecuteScalar();
                }
                catch (Exception sqlEx)
                {
                    throw sqlEx;
                }
                finally
                {
                    sqlConnection.Dispose();
                }
            }

            return retValue;
        }

        public DataTable Load()
        {
            DataTable dt = new DataTable();
            Type t = this.GetType();
            using (ConnectionManager sqlConnection = new ConnectionManager())
            {
                
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection.Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = String.Format("{0}Load", t.Name);
    
                foreach (KeyValuePair<string, object> de in filters)
                {
                   // DbBinderIgnoreAttribute ignore = AttributeReader.GetDbBinderIgnore(prop);
                    SqlParameter sp = new SqlParameter(de.Key, de.Value);
                    FieldInfo fi = t.GetField(de.Key);

                    //if (ignore != null)
                    //{
                        //if (ignore.IgnoranceFlag == IgnoranceFlag.IgnoreBind)
                            cmd.Parameters.Add(sp);
                    //}
                }


                // Define the data adapter and fill the dataset 
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                sqlConnection.Dispose();
            }
       
            return dt;
        }

        public List<U> LoadObjects(string query= null)
        {
            PreObjectsLoad();
            List<U> objectList = new List<U>();

            Type t = this.GetType();
            using (ConnectionManager sqlConnection = new ConnectionManager())
            {

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection.Connection;
                if (query == null)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = String.Format("{0}Load", t.Name);
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                }

               
                foreach (KeyValuePair<string, object> de in filters)
                {
                        SqlParameter sp = new SqlParameter(de.Key, de.Value);
                        cmd.Parameters.Add(sp);
                }   
            
            
                SqlDataReader sqlDr = cmd.ExecuteReader();
                while (sqlDr.Read())
                { 
                    object classObject = Activator.CreateInstance(t);
                    PropertyInfo[] propertiesList = t.GetProperties();
                    foreach (PropertyInfo prop in propertiesList)
                    {
                        DbBinderIgnoreAttribute ignore = AttributeReader.GetDbBinderIgnore(prop);

                        
                        if (ignore != null)
                        {
                            if (ignore.IgnoranceFlag == IgnoranceFlag.IgnoreBind)
                                continue;

                            if (ignore.IgnoranceFlag == IgnoranceFlag.AlwaysIgnore)
                                continue;
                        }

                        object val = sqlDr[prop.Name];
                        if (val == DBNull.Value)
                            t.GetProperty(prop.Name).SetValue(classObject, null, null);
                        else
                            t.GetProperty(prop.Name).SetValue(classObject, sqlDr[prop.Name], null);


                    }

                    objectList.Add((U)classObject);
                    PostObjectsObjectLoad((U)classObject);
                }

                cmd.Dispose();
            }

            PostObjectsLoad();
            return objectList;
        }

        public U LoadSingleObject(string query=null)
        {
            U uObject = new U();

            Type t = this.GetType();
            using (ConnectionManager sqlConnection = new ConnectionManager())
            {

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection.Connection;
                if (query == null)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = String.Format("{0}Load", t.Name);
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                }

                foreach (KeyValuePair<string, object> de in filters)
                {
                    // Guid g = new Guid(de.Value.ToString());
                    SqlParameter sp = new SqlParameter(de.Key, de.Value);
                    cmd.Parameters.Add(sp);
                }

                SqlDataReader sqlDr = cmd.ExecuteReader();

                object classObject = Activator.CreateInstance(t);

                while (sqlDr.Read())
                {
                    PropertyInfo[] propertiesList = t.GetProperties();
                    foreach (PropertyInfo prop in propertiesList)
                    {
                        DbBinderIgnoreAttribute ignore = AttributeReader.GetDbBinderIgnore(prop);

                        if (ignore != null)
                        {
                            if (ignore.IgnoranceFlag == IgnoranceFlag.IgnoreBind || ignore.IgnoranceFlag==IgnoranceFlag.AlwaysIgnore)
                                continue;
                        }

                        //prevent throwing exception when binaries is set to null in db. DBNull cannot be converted to System.Byte[]
                        try
                        {
                            object val = sqlDr[prop.Name];
                            if (val == DBNull.Value)
                                t.GetProperty(prop.Name).SetValue(classObject, null, null);
                            else
                                t.GetProperty(prop.Name).SetValue(classObject, sqlDr[prop.Name], null);
                        }
                        catch (IndexOutOfRangeException error)
                        {
                            if (ignore == null)
                            {
                                t.GetProperty(prop.Name).SetValue(classObject, null, null);
                            }
                        }

    
                    }

                    uObject = (U)classObject;
                }

                cmd.Dispose();
            }

            return uObject;
        }

        public U LoadSingleObject(Object id, SqlDbType parameterType=SqlDbType.UniqueIdentifier)
        {
            U uObject = new U();

            Type t = this.GetType();
            using (ConnectionManager sqlConnection = new ConnectionManager())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection.Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = String.Format("{0}Load", t.Name);

                foreach (KeyValuePair<string, object> de in filters)
                {
                    // Guid g = new Guid(de.Value.ToString());
                    SqlParameter sp = new SqlParameter(de.Key, de.Value);
                    cmd.Parameters.Add(sp);
                }

                SqlParameter idPar = new SqlParameter();
                idPar.SqlDbType = parameterType;
                idPar.ParameterName = "Id";
                idPar.Value = id;
                cmd.Parameters.Add(idPar);

                SqlDataReader sqlDr = cmd.ExecuteReader();

                object classObject = Activator.CreateInstance(t);

                while (sqlDr.Read())
                {
                    PropertyInfo[] propertiesList = t.GetProperties();
                    foreach (PropertyInfo prop in propertiesList)
                    {
                        DbBinderIgnoreAttribute ignore = AttributeReader.GetDbBinderIgnore(prop);
                        if (ignore != null)
                        {
                            if (ignore.IgnoranceFlag != IgnoranceFlag.IgnoreBind && ignore.IgnoranceFlag!=IgnoranceFlag.AlwaysIgnore)
                            {
                                //ignore when the null value is in db
                                if (ignore.IgnoreWhenNull == true && sqlDr[prop.Name] == null)
                                    continue;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        //prevent throwing exception when binaries is set to null in db. DBNull cannot be converted to System.Byte[]
                        if (prop.PropertyType == Type.GetType("System.Byte[]"))
                        {
                            object val = sqlDr[prop.Name];
                            if(val==DBNull.Value)
                                t.GetProperty(prop.Name).SetValue(classObject, null, null);
                            else
                                t.GetProperty(prop.Name).SetValue(classObject, sqlDr[prop.Name], null);

                        }
                        else
                        {
                            if (sqlDr[prop.Name] != DBNull.Value)
                                t.GetProperty(prop.Name).SetValue(classObject, sqlDr[prop.Name], null);
                            else
                                t.GetProperty(prop.Name).SetValue(classObject, null, null);
                        }
                    }

                    uObject = (U)classObject;
                }

                cmd.Dispose();
            }

            return uObject;
        }

        public bool DeleteSingleObject(Object id, SqlDbType parameterType = SqlDbType.UniqueIdentifier)
        {
            Type t = this.GetType();
            int affectedRows = 0;
            using (ConnectionManager sqlConnection = new ConnectionManager())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection.Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = String.Format("{0}Delete", t.Name);

                SqlParameter idPar = new SqlParameter();
                idPar.SqlDbType = parameterType;
                idPar.ParameterName = "Id";
                idPar.Value = id;
                cmd.Parameters.Add(idPar);

                affectedRows= cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            if (affectedRows == 0)
                return false;
            else
                return true;
        }

        public virtual void PreObjectsLoad()
        {

        }
        public virtual void PostObjectsLoad()
        {

        }

        public virtual void PostObjectsObjectLoad(U paremeter)
        {

        }
     
 

        public Dictionary<string, object> filters = new Dictionary<string, object>();
        public void AddFilter(string filterType, object filterValue)
        {
            filters.Add(filterType, filterValue);
        }

        public object GetFilter(string filterType)
        {
            if (filters.ContainsKey(filterType))
            {
                return filters[filterType];
            }
            else
            {
                throw new Exception("Can not find filter");
            }
        }

    }
}
