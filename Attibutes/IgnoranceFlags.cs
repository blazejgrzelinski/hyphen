using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyphen.Attibutes
{
    /// <summary>
    /// Instructs <see cref="IDbObjectBinder"/> how to bind properties to <see cref="System.Data.Common.DbCommand"/>.
    /// </summary>    
    public enum IgnoranceFlag
    {
        /// <summary>
        /// Property is never ignored. It is bound to <see cref="System.Data.Common.DbCommand"/> and unbound from 
        /// <see cref="System.Data.Common.DbDataReader"/>.
        /// </summary>
        DoNotIgnore,
        /// <summary>
        /// Property is ignored on bind. Whend unbinding object it is read from <see cref="System.Data.Common.DbDataReader"/>.
        /// </summary>
        IgnoreBind,
        /// <summary>
        /// Property is ignored on unbind. When binding object it is bound to <see cref="System.Data.Common.DbCommand"/>.
        /// </summary>
        IgnoreUnbind,
        /// <summary>
        /// Property is ignored by <see cref="IDbObjectBinder"/>.
        /// </summary>
        AlwaysIgnore
    }
}
