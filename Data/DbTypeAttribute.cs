using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Hyphen.Data
{
    /// <summary>
    /// Specifies SQL type mapping for given element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited=true)]
    public class DbTypeAttribute : Attribute
    {
        #region Fields
        /// <summary>
        /// Internal field containing <see cref="System.Data.DbType"/>.
        /// </summary>
        protected DbType _Type;
        /// <summary>
        /// Internal field containing size of DbType.
        /// </summary>
        protected int? _Size = null;
        /// <summary>
        /// Internal field containing nullable flag.
        /// </summary>
        protected bool _Nullable = true;
        #endregion
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public virtual DbType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual int Size
        {
            get { return (int)_Size; }
            set { _Size = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsSizeSet
        {
            get { return _Size != null; }
        }
        /// <summary>
        /// Specifies w
        /// </summary>
        public virtual bool Nullable
        {
            get { return _Nullable; }
            set { _Nullable = value; }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public DbTypeAttribute() { 
        
        }
        public DbTypeAttribute(DbType type)
        {
            this.Type = type;
        }
        #endregion
    }


    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=false)]
    public class Procedurable : System.Attribute
    {
        private bool generate = false;
        private string mainTable = "";

        public Procedurable(bool generate, string mainTable="")
        {
            this.generate = generate;
            this.mainTable = mainTable;
        }

        public bool Generate
        {
            get
            {
                return generate;
            }
        }

        public string TypeName
        {
            get
            {
                return mainTable;
            }
        }
    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SqlDbTypeH : System.Attribute
    {
        private SqlDbType type = SqlDbType.Int;
        private Int32 size = 0;


        public SqlDbTypeH(SqlDbType type, Int32 size=0)
        {
            this.type = type;
            this.size = size;
        }

        public SqlDbType Type
        {
            get
            {
                return type;
            }
        }

        public Int32 Size
        {
            get
            {
                return size;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReferenceTable : System.Attribute
    {
        private string referenceTableName = "";
        private string keyRef = "Id";
        private string key = "";
        private string columnName = "";

        public ReferenceTable(string referenceTableName,string columnName, string keyRef="Id", string key=null)
        {
            this.referenceTableName = referenceTableName;
            this.keyRef = keyRef;
            if (key == null)
            {
                this.key = this.referenceTableName + "Id";
            }
            else
            {
                this.key = key;
            }
            this.columnName = columnName;
        }

        public string ReferenceTableName
        {
            get
            {
                return referenceTableName;
            }
        }

        public string KeyRef
        {
            get
            {
                return keyRef;
            }
        }

        public string Key
        {
            get
            {
                return key;
            }
        }

        public string ColumnName
        {
            get
            {
                return columnName;
            }
        }
    }
}