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
        ProviderBase instance;

        public Provider(Type provider, ProviderBase instance, ProviderFactory factory)
        {
            this.provider = provider;
            this.instance = instance;
            this.factory = factory;
        }

        public bool IsOfProviderType(ProviderType pt)
        {
            return this.Type.GetInterfaces().Contains(pt.Type);
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

        public override bool CanUse
        {
            get
            {
                return Instance.CanUse;
            }
        }

        public Type Type
        {
            get
            {
                return provider;
            }
        }

        public ProviderBase Instance
        {
            get
            {
                if (instance == null)
                    instance = factory.CreateInstance(this);

                return instance;
            }
        }


    }
}
