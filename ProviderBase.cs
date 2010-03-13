using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys
{
    /// <summary>
    /// The base class for all providers. Every provider should extend this class.
    /// </summary>
    public abstract class ProviderBase
    {
        /// <summary>
        /// The unique Name of the Provider. No 2 providers should have the same name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The optinal description of the provider. If this is not overriden an empty string is returned.
        /// </summary>
        public virtual string Description
        {
            get { return ""; }
        }
    }
}
