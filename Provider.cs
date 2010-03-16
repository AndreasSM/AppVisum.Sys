using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys
{
    /// <summary>
    /// Represents a registered Provider.
    /// </summary>
    public class Provider : ProviderBase
    {
        Type provider;
        ProviderFactory factory;
        ProviderBase instance;

        internal Provider(Type provider, ProviderBase instance, ProviderFactory factory)
        {
            this.provider = provider;
            this.instance = instance;
            this.factory = factory;
        }

        /// <summary>
        /// Determines whether the current Provider is a subset of the given ProviderType.
        /// </summary>
        /// <param name="pt">The ProviderType to check against.</param>
        /// <returns>True if the Provider implements the ProviderType, otherwise false.</returns>
        public bool IsOfProviderType(ProviderType pt)
        {
            return this.Type.GetInterfaces().Contains(pt.Type);
        }

        /// <summary>
        /// The name of the provider. Read-only.
        /// </summary>
        public override string Name
        {
            get
            {
                return Instance.Name;
            }
        }

        /// <summary>
        /// The description of the provider. Read-only. Defaults to "".
        /// </summary>
        public override string Description
        {
            get
            {
                return Instance.Description;
            }
        }

        /// <summary>
        /// Whether or not the current Provider can be used.
        /// </summary>
        public override bool CanUse
        {
            get
            {
                return Instance.CanUse;
            }
        }

        /// <summary>
        /// The underlaying Type of the current Provider.
        /// </summary>
        public Type Type
        {
            get
            {
                return provider;
            }
        }

        /// <summary>
        /// A instance of the current Provider.
        /// </summary>
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
