using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys
{
    public class Provider : ProviderBase
    {
        Type provider;
        ProviderFactory factory;

        public Provider(Type provider, ProviderFactory factory)
        {
            this.provider = provider;
        }

        public override string Name
        {
            get
            {
                return Instance.Name;
            }
        }

        public override string Description
        {
            get
            {
                return Instance.Description;
            }
        }

        public ProviderBase Instance
        {

        }
    }
}
