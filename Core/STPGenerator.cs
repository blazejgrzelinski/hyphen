using Hyphen.Data;
using Hyphen.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hyphen.Core
{
    class STPGenerator
    {
        public STPGenerator(Type type)
        {
            string mainTableName=type.Name;
            bool classAttr = GetClassAttribute(type, ref mainTableName);
            if (mainTableName == "")
            {
                mainTableName = type.Name;
            }

            if (classAttr)
            {
                IEnumerable<MemberInfo> members = type.GetProperties().ToArray();
                StringBuilder proc = new StringBuilder();


               StringBuilder spParams = new StringBuilder();
               StringBuilder selectFields = new StringBuilder();
               StringBuilder whereClause = new StringBuilder();
               StringBuilder joinClause = new StringBuilder();
                
                foreach (MemberInfo m in members)
                {
                    string name = m.Name;
                    if (AttributeReader.GetDbBinderIgnore(m as PropertyInfo).IgnoranceFlag != Attibutes.IgnoranceFlag.AlwaysIgnore)
                    {
                        ReferenceTable refTable = AttributeReader.GetReferenceTable(m);

                        string paramType = AttributeReader.GetSqlType(m).Type.ToString();

                        //add the varchar size when varchar
                        if (paramType.ToLower() == "varchar")
                        {

                            paramType = String.Format("Varchar({0})", AttributeReader.GetSqlType(m).Size);
                        }

                        if (m != members.Last())
                        {
                            spParams.AppendLine(String.Format("@{0} {1}=NULL,", name, paramType));

                            if (refTable ==null)
                            {
                                selectFields.AppendLine(String.Format("''{0}.[{1}],'' + CHAR(13) + ", mainTableName, name));
                            }
                            else
                            {
                                selectFields.AppendLine(String.Format("''{0}.[{1}] as {2},'' + CHAR(13) + ", refTable.ReferenceTableName, refTable.ColumnName, m.Name));
                            }

         
                        }
                        else
                        {
                            spParams.AppendLine(String.Format("@{0} {1}=NULL", name, paramType));
                            if (refTable == null)
                            {
                                selectFields.AppendLine(String.Format("''{0}.[{1}]'' + CHAR(13) ", mainTableName, name));
                            }
                            else
                            {
                                selectFields.AppendLine(String.Format("''{0}.[{1}] as {2}'' + CHAR(13) ", refTable.ReferenceTableName, refTable.ColumnName, m.Name));
                            }
                        }

                        whereClause.AppendLine(String.Format("IF(@{0} IS NOT NULL)", name));
                        whereClause.AppendLine("BEGIN");
                        whereClause.AppendLine("IF(@Where = '''') SET @Where = ''WHERE ''");
                        whereClause.AppendLine("ELSE SET @Where = @Where + ''AND ''");
                        whereClause.AppendLine("SET @Where = @Where + ''"+mainTableName+".["+name+"] ='''''' + CONVERT(VARCHAR, @"+name+") + '''''' '' + CHAR(13)");
                        whereClause.AppendLine("END");
                        whereClause.AppendLine("");

                        
                        if ( refTable!= null)
                        {
                            joinClause.Append(" + ");
                            joinClause.AppendLine("''INNER JOIN " + refTable.ReferenceTableName + " on " + mainTableName + "." + refTable.Key + "=" + refTable.ReferenceTableName + "." + refTable.KeyRef + " '' + CHAR(13) ");
                        }
                         

                    }
                }


                proc.Append(@"exec sp_executesql N'CREATE PROCEDURE [dbo].["+ type.Name +"Load] ");
                proc.AppendLine(spParams.ToString());    
                proc.AppendLine(@" AS BEGIN DECLARE @Select VARCHAR(1000),@From VARCHAR(500),@Where VARCHAR(1000)  SET @Select = ''SELECT ''+");

                proc.AppendLine(selectFields.ToString());

                proc.AppendLine(@"SET @From = ''FROM dbo.["+mainTableName+"]  '' + CHAR(13) ");

                proc.AppendLine(joinClause.ToString());

	            proc.AppendLine(@"
                SET @Where = ''''");

                proc.AppendLine(whereClause.ToString());

                proc.AppendLine(@"EXEC(@Select + @From + @Where)
	            PRINT ''D| Full Query '' + CHAR(13) + @Select + @From + @Where end'");


                string finalSql = proc.ToString();
                using (ConnectionManager sqlConnection = new ConnectionManager())
                {

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConnection.Connection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = finalSql;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException sx)
                    {
                        cmd.CommandText = finalSql.Replace("CREATE", "ALTER");
                        cmd.ExecuteNonQuery();
                    }


                    sqlConnection.Dispose();
                }
                
          ;

       


            }
        }

        private bool GetClassAttribute(Type type, ref string name)
        {
            Procedurable[] result = (Procedurable[])type.GetCustomAttributes(typeof(Procedurable), false);
            bool generate = false;
            if (result.ToArray().Length > 0)
            {
                generate = result[0].Generate;
                name = result[0].TypeName;
            }
            return generate;
        }

    }
}
