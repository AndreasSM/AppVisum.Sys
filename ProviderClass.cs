using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys
{
    class ProviderClass
    {
        private Type type;
        private ProviderTypeAttribute provAttr;

        public ProviderClass(Type type, ProviderTypeAttribute providerAttribute)
        {
            this.type = type;
            this.provAttr = providerAttribute;
        }

        public Type Type
        {
            get { return type; }
        }

        public ProviderTypeAttribute ProviderAttribute
        {
            get { return provAttr; }
        }
    }
}
