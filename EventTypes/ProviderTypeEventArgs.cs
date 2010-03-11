using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys.EventTypes
{
    /// <summary>
    /// The TypeRegisteredEventArgs type is used to provide EventArgs for whenever a
    /// ProviderType is needed.
    /// </summary>
    public class ProviderTypeEventArgs : EventArgs
    {
        /// <summary>
        /// The ProviderType readonly property is the ProviderType-interface that was
        /// registered at the ProviderFactory.
        /// </summary>
        public Type ProviderType { get; private set; }

        /// <summary>
        /// The ProviderTypeName readonly property is the name of the ProviderType
        /// that was registered at the ProviderFactory.
        /// </summary>
        public String ProviderTypeName { get; private set; }

        internal ProviderTypeEventArgs(Type provider, String name)
        {
            ProviderType = provider;
            ProviderTypeName = name;
        }
    }
}
