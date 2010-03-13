using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys.EventTypes
{
    /// <summary>
    /// The ProviderRegisteredEventArgs type is used to provide EventArgs
    /// for whenever a Provider and a ProviderType is needed.
    /// </summary>
    public class ProviderEventArgs : ProviderTypeEventArgs
    {
        /// <summary>
        /// The Provider readonly property is the Type-object
        /// for the Provider.
        /// </summary>
        public Provider Provider { get; private set; }

        internal ProviderEventArgs(Provider provider, ProviderType providerType)
            : base(providerType)
        {
            Provider = provider;
        }
    }
}
