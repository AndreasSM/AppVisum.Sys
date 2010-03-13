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
        public ProviderType ProviderType { get; private set; }

        internal ProviderTypeEventArgs(ProviderType providerType)
        {
            ProviderType = providerType;
        }
    }
}
