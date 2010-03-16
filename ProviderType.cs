using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys
{
    /// <summary>
    /// Represents a registered ProviderType.
    /// </summary>
    public class ProviderType
    {
        private Type providerType;
        private ProviderTypeAttribute providerTypeAttribute;

        internal ProviderType(Type pt, ProviderTypeAttribute pta)
        {
            providerType = pt;
            providerTypeAttribute = pta;
        }

        /// <summary>
        /// The type of the underlaying ProviderType.
        /// </summary>
        public Type Type
        {
            get { return providerType; }
        }

        /// <summary>
        /// The name of the ProviderType.
        /// </summary>
        public String Name
        {
            get { return providerTypeAttribute.Name; }
        }

        /// <summary>
        /// The description of the ProviderType. Read-only. Defaults to "";
        /// </summary>
        public String Description
        {
            get { return providerTypeAttribute.Description; }
        }
    }
}
