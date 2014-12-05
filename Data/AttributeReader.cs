using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Hyphen.Attibutes;

namespace Hyphen.Data
{
    class AttributeReader
    {
        public static DbTypeAttribute GetSqlDbType(MemberInfo member)
        {
            object[] type = member.GetCustomAttributes(typeof(DbTypeAttribute), true);
            if (type.Length == 1)
            {
                return type[0] as DbTypeAttribute;
            }
            else { return null; }
        }

        /// <summary>
        /// Gets the db binder ignore attribute.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns><see cref="DbBinderIgnoreAttribute"/></returns>
        public static DbBinderIgnoreAttribute GetDbBinderIgnore(PropertyInfo property)
        {
            object[] name = property.GetCustomAttributes(
               typeof(DbBinderIgnoreAttribute), true);
            if (name.Length == 1)
            {
                return name[0] as DbBinderIgnoreAttribute;
            }
            else { return null; }
        }


        public static SqlDbTypeH GetSqlType(MemberInfo property)
        {
            object[] name = property.GetCustomAttributes(
               typeof(SqlDbTypeH), true);
            if (name.Length == 1)
            {
                return name[0] as SqlDbTypeH;
            }
            else { return null; }
        }

        public static ReferenceTable GetReferenceTable(MemberInfo property)
        {
            object[] name = property.GetCustomAttributes(
               typeof(ReferenceTable), true);
            if (name.Length == 1)
            {
                return name[0] as ReferenceTable;
            }
            else { return null; }
        }
    }

}
