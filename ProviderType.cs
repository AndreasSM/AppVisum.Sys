using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys
{
    public class ProviderType
    {
        private Type providerType;
        private ProviderTypeAttribute providerTypeAttribute;

        public ProviderType(Type pt, ProviderTypeAttribute pta)
        {
            providerType = pt;
            providerTypeAttribute = pta;
        }

        public Type Type
        {
            get { return providerType; }
        }

        public String Name
        {
            get { return providerTypeAttribute.Name; }
        }

        public String Description
        {
            get { return providerTypeAttribute.Description; }
        }
    }
}
