using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyphen.Attibutes
{
    /// <summary>
    /// Specifies ignorance flag for property used by <see cref="IDbObjectBinder"/> objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DbBinderIgnoreAttribute : Attribute
    {
        #region Fields
        /// <summary>
        /// Internal field resposible for IgnoreWhenNull flag.
        /// </summary>
        protected bool _IgnoreWhenNull = false;
        /// <summary>
        /// Internal field resposible for <see cref="IgnoranceFlag"/> flags.
        /// </summary>
        protected IgnoranceFlag _IgnoranceFlag = IgnoranceFlag.AlwaysIgnore;
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether to ignore element if it's value equals null.
        /// </summary>
        /// <value><c>true</c> if ignor element if it's value equals null; otherwise, <c>false</c>.</value>
        public bool IgnoreWhenNull
        {
            get { return _IgnoreWhenNull; }
            set { _IgnoreWhenNull = value; }
        }
        /// <summary>
        /// Gets or sets the ignorance flag for element.
        /// </summary>
        /// <value>The ignorance flag.</value>
        public IgnoranceFlag IgnoranceFlag
        {
            get { return _IgnoranceFlag; }
            set { _IgnoranceFlag = value; }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DbBinderIgnoreAttribute"/> class.
        /// </summary>
        public DbBinderIgnoreAttribute() { }
        #endregion
    }
}
