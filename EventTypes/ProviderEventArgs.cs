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
        public Type Provider { get; private set; }

        /// <summary>
        /// The Instance readonly property is the instance
        /// of the provider or null if it hasn't been instanitiazed
        /// yet.
        /// </summary>
        public Object Instance { get; private set; }

        internal ProviderEventArgs(Type provider, Object instance, Type providerType, String providerTypeName)
            : base(providerType, providerTypeName)
        {
            Provider = provider;
            Instance = instance;
        }
    }
}
