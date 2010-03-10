using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace AppVisum.Sys
{
    public class ProviderFactory
    {
        private Dictionary<Type, ProviderClass> types;
        private Dictionary<Type, Object> providers;

        /// <summary>
        /// The default constructor for the ProviderFactory.
        /// Takes no arguments.
        /// </summary>
        public ProviderFactory()
        {
            types = new Dictionary<Type, ProviderClass>();
            providers = new Dictionary<Type, Object>();
        }

        /// <summary>
        /// Registers a type to be used with the ProviderFactory.
        /// In order for the factory to be able to handle a Provider
        /// it's type must first be registered.
        /// </summary>
        /// <param name="type">The type to be registered.</param>
        public void RegisterType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsInterface)
                throw new ArgumentException("The provided type was not an Interface.", "type");

            if (types.Keys.Contains(type))
                throw new ArgumentException("The provided type is already registered.", "type");

            MemberInfo memberinfo = type;

            object[] attributes = memberinfo.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is ProviderTypeAttribute)
                {
                    ProviderTypeAttribute attr = attribute as ProviderTypeAttribute;
                    ProviderClass pc = new ProviderClass(type, attr);
                    types.Add(type, pc);
                    return;
                }
            }

            throw new ArgumentException("The type provided was not a ProviderClass, the ProviderTypeAttribute was lacking.", "type");
        }

        public void Register(Type provider, Object instance = null)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (providers.Keys.Contains(provider))
                throw new ArgumentException("The current provider is alreaddy added.", "provider");

            if (provider.IsInterface)
                throw new ArgumentException("The provided provider is an Interface.", "provider");

            var ifaces = provider.GetInterfaces().Where(i => types.Keys.Contains(i)).ToArray();

            if (ifaces.Length == 0)
                throw new ArgumentException("The provided provider does not have a registered type.", "provider");

            if (instance == null)
            {
                var ctrs = from c in provider.GetConstructors()
                           where c.GetParameters().Where(p => !p.IsOptional).Count() == 0
                           select c;
                ConstructorInfo ctr = ctrs.FirstOrDefault();
                if (ctr == null || ctr.IsPrivate)
                    throw new ArgumentException("The provided provider doesn't have an empty constructor and instance is set to null.");
            }
            else
            {
                Type type = instance.GetType();
                if (!type.Equals(provider) && !type.IsSubclassOf(provider))
                    throw new ArgumentException("The instance provided did not match the type of the provider.", "instance");
            }

            providers.Add(provider, instance);
        }
    }
}
