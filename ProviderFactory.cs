using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Globalization;
using AppVisum.Sys.EventTypes;

namespace AppVisum.Sys
{
    /// <summary>
    /// Handles the dependency-injection and provides means to set current providers and instanitiate them.
    /// </summary>
    public class ProviderFactory
    {

        #region Private variables
        private List<ProviderType> types;
        private List<Provider> providers;
        private Dictionary<ProviderType, Provider> selected;
        #endregion

        #region Events

        /// <summary>
        /// The event TypeRegistered is fired whenever a new ProviderType is registered at the ProviderFactory.
        /// </summary>
        public event EventHandler<ProviderTypeEventArgs> TypeRegistered;

        /// <summary>
        /// The event ProviderRegistered is fired whenever a new Provider is registered at the ProviderFactory.
        /// </summary>
        public event EventHandler<ProviderEventArgs> ProviderRegistered;

        /// <summary>
        /// The event ProviderSelected is fired whenever a Provider is selected for a ProviderType.
        /// </summary>
        public event EventHandler<ProviderEventArgs> ProviderSelected;

        /// <summary>
        /// The event ProviderInstanceCreated is fired whenever a new instance of a Provider is created.
        /// </summary>
        public event EventHandler<ProviderEventArgs> ProviderInstanceCreated;

        #endregion

        /// <summary>
        /// The default constructor for the ProviderFactory.
        /// Takes no arguments.
        /// </summary>
        public ProviderFactory()
        {
            types = new List<ProviderType>();
            providers = new List<Provider>();
            selected = new Dictionary<ProviderType, Provider>();
        }

        /// <summary>
        /// Registers a type to be used with the ProviderFactory.
        /// In order for the factory to be able to handle a Provider
        /// it's type must first be registered.
        /// </summary>
        /// <param name="type">The type to be registered.</param>
        /// <exception cref="System.ArgumentNullException">An NullArgumentException is thrown if type is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException is thrown if the provided provider is
        /// not an interface, allready registered, name duplicate or hasn't got the
        /// ProviderTypeAttribute.
        /// </exception>
        /// <example>
        /// Register a ProviderType-interface called Car
        /// <code>
        /// ProviderFactory pf = new ProviderFactory();
        /// pf.RegisterType(typeof(Car));
        /// </code>
        /// </example>
        public void RegisterType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsInterface)
                throw new ArgumentException("The provided type was not an Interface.", "type");

            if (types.Any(t => t.Type == type))
                throw new ArgumentException("The provided type is allready registered.", "type");

            MemberInfo memberinfo = type;

            object[] attributes = memberinfo.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is ProviderTypeAttribute)
                {
                    ProviderTypeAttribute attr = attribute as ProviderTypeAttribute;
                    if (types.Any(t => t.Name == attr.Name))
                        throw new ArgumentException("A provider type with that name is already registered.", "type");
                    ProviderType providerType = new ProviderType(type, attr);
                    types.Add(providerType);

                    if (TypeRegistered != null)
                        TypeRegistered(this, new ProviderTypeEventArgs(providerType));

                    return;
                }
            }

            throw new ArgumentException("The type provided was not a ProviderClass, the ProviderTypeAttribute was lacking.", "type");
        }

        /// <summary>
        /// Get every registered ProviderType.
        /// </summary>
        /// <returns>The registered ProviderTypes.</returns>
        /// <example>
        /// Get every ProviderType registered at a ProviderFactory.
        /// <code>
        /// ProviderFactory pf = new ProviderFactory();
        /// pf.RegisterType(typeof(Car));
        /// ProviderType[] types = pf.GetRegisteredProviderTypes();
        /// </code>
        /// </example>
        public ProviderType[] GetRegisteredProviderTypes()
        {
            return types.ToArray();
        }

        /// <summary>
        /// Registers a provider to be used by the ProviderFactory.
        /// Any given provider that is registered can be returned
        /// from the Instance&lt;T&gt; method.
        /// </summary>
        /// <param name="provider">The providertype.</param>
        /// <param name="instance" optional="true">An instance of the provider. If the provider doesn't contain any parameterless constructors instance is required.</param>
        /// <exception cref="System.ArgumentNullException">An ArgumentNullException is thrown if provider is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException is thrown if the provider is already added,
        /// if it's an interface, if it doesn't implement any of the registered
        /// types, if it doesn't have a parameterless constructor or if the instance
        /// provided is not an instance of the provider provided.
        /// </exception>
        /// <example>
        /// Register a new provider to be used in ProviderFactory:
        /// <code>
        /// ProviderFactory pf = new ProviderFactory();
        /// pf.RegisterType(typeof(Car));
        /// pf.Register(typeof(Audi));
        /// </code>
        /// </example>
        public void Register(Type provider, ProviderBase instance = null)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (providers.Any(t => t.Type == provider))
                throw new ArgumentException("The current provider is alreaddy added.", "provider");

            if (provider.IsInterface)
                throw new ArgumentException("The provided provider is an Interface.", "provider");

            if (!provider.IsSubclassOf(typeof(ProviderBase)))
                throw new ArgumentException("The provided provider is not a subclass of ProviderBase.", "provider");

            var ifaces = provider.GetInterfaces().Where(i => types.Any(t => t.Type == i)).ToArray();

            if (ifaces.Length == 0)
                throw new ArgumentException("The provided provider does not have a registered type.", "provider");

            if (instance == null)
            {
                ConstructorInfo ctr = provider.GetSpecificConstructor(typeof(ProviderFactory));
                if (ctr == null || !ctr.IsPublic)
                    throw new ArgumentException("The provided provider doesn't have an empty constructor, or a constructor"
                        + "demanding only one ProviderFactory and instance is set to null.");
            }
            else
            {
                Type type = instance.GetType();
                if (!type.Equals(provider) && !type.IsSubclassOf(provider))
                    throw new ArgumentException("The instance provided did not match the type of the provider.", "instance");
            }

            Provider p = new Provider(provider, instance, this);

            if (providers.Any(pr => pr.Name.Trim().ToLower() == p.Name.Trim().ToLower()))
                throw new ArgumentException("A provided with that name is alreaddy registered.", "provider");

            providers.Add(p);

            if (ProviderRegistered != null)
                ProviderRegistered(this, new ProviderEventArgs(p, types.Where(t => t.Type == ifaces[0]).First()));
        }

        /// <summary>
        /// Get every availible Provider for the current ProviderType T.
        /// If onlyUsable is set to true only the Providers which return
        /// CanUse = true are returned.
        /// </summary>
        /// <typeparam name="T">The ProviderType</typeparam>
        /// <param name="onlyUsable">A value determing whether or not only usable Providers should be returned. Defaults to true.</param>
        /// <returns>
        /// If onlyUsable is set to false, it returns every Provider for ProviderType T.
        /// Else, it returns every Provider for ProviderType T where canUse is true.</returns>
        /// <example>
        /// Get every Provider of Car where canUse is true.
        /// <code>
        /// ProviderFactory pf = new ProviderFactory();
        /// pf.RegisterType(typeof(Car));
        /// pf.Register(typeof(Audi));
        /// Provider[] prvs = pf.GetRigisteredProviders&lt;Car&gt;(true);
        /// </code>
        /// </example>
        public Provider[] GetRegisteredProviders<T>(bool onlyUsable = true)
        {
            Type t = typeof(T);

            ProviderType providerType = types.Where(tp => tp.Type == t).FirstOrDefault();

            var ret = providers.Where(p => p.IsOfProviderType(providerType));

            if (onlyUsable)
                ret = ret.Where(p => p.CanUse);

            return ret.ToArray();
        }

        /// <summary>
        /// Sets the current provider of any given ProviderType.
        /// This causes Instance&lt;T&gt; to return an instance
        /// of a provider with the name of the providername parameter.
        /// </summary>
        /// <typeparam name="T">The ProviderType</typeparam>
        /// <param name="providername">The name of the provider to be set</param>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException is thrown if T doesn't match any ProviderType
        /// registered or if no providers with the name of `providername` is found.
        /// </exception>
        /// <example>
        /// Set current Car to Audi.
        /// <code>
        /// ProviderFactory pf = new ProviderFactory();
        /// pf.RegisterType(typeof(Car));
        /// pf.Register(typeof(Audi));
        /// pf.SetCurrent&lt;Car&gt;("Audi");
        /// </code>
        /// </example>
        public void SetCurrent<T>(String providername)
        {
            Type t = typeof(T);

            ProviderType providerType = types.Where(tp => tp.Type == t).FirstOrDefault();

            if (providerType == null)
                throw new ArgumentException("No providertype of that type is registered.", "T");

            Provider selected = null;

            foreach (Provider provider in providers)
            {
                if (provider.Name.Trim().ToLower() == providername.ToLower().Trim())
                {
                    selected = provider;
                    break;
                }
            }

            if (selected == null)
                throw new ArgumentException("No providers with that name found.", "providername");

            this.selected[providerType] = selected;

            if (ProviderSelected != null)
                ProviderSelected(this, new ProviderEventArgs(selected, providerType));
        }

        /// <summary>
        /// Instansiate a provider of type T if one dosn't exist and returns it.
        /// </summary>
        /// <typeparam name="T">The ProviderType.</typeparam>
        /// <returns>The provider typed as T.</returns>
        /// <exception cref="System.ArgumentException">An ArgumentException is thrown if T dosn't match any registered ProviderTypes.</exception>
        /// <exception cref="System.Exception">An Exception is thrown if no providers that can be used is found for that particular ProviderType.</exception>
        /// <example>
        /// Get a Car.
        /// <code>
        /// ProviderFactory pf = new ProviderFactory();
        /// pf.RegisterType(typeof(Car));
        /// pf.Register(typeof(Audi));
        /// Car prv = pf.Instance&lt;Car&gt;();
        /// </code>
        /// </example>
        public T Instance<T>()
        {
            Type t = typeof(T);

            ProviderType providerType = types.Where(tp => tp.Type == t).FirstOrDefault();

            if (providerType == null)
                throw new ArgumentException("No providertype of that type is registered.", "T");

            Provider current = selected.Keys.Contains(providerType) ? selected[providerType] : providers.Where(p => p.IsOfProviderType(providerType) && p.CanUse).FirstOrDefault();
            if(current == null)
                throw new Exception("No providers that can be used was found for that type.");

            return Instance<T>(current);
        }

        /// <summary>
        /// Instansiate a provider of type T if one dosn't exist and returns it.
        /// </summary>
        /// <typeparam name="T">The ProviderType.</typeparam>
        /// <param name="providername">The name of the provider to select.</param>
        /// <returns>The first provider typed as T with the name of "providername".</returns>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException is thrown if T dosn't match any registered ProviderTypes
        /// or if no provider is found with the name of "providername".
        /// </exception>
        /// <example>
        /// Get a Car with the name "Audi".
        /// <code>
        /// ProviderFactory pf = new ProviderFactory();
        /// pf.RegisterType(typeof(Car));
        /// pf.Register(typeof(Audi));
        /// pf.Register(typeof(BMW));
        /// Car prv = pf.Instance&lt;Car&gt;("Audi");
        /// </code>
        /// </example>
        public T Instance<T>(String providername)
        {
            Type t = typeof(T);

            ProviderType providerType = types.Where(tp => tp.Type == t).FirstOrDefault();

            if (providerType == null)
                throw new ArgumentException("No providertype of that type is registered.", "T");

            Provider prov = providers.Where(p => p.Name.Trim().ToLower() == providername.Trim().ToLower()).FirstOrDefault();

            if (prov == null)
                throw new ArgumentException("No providers with that name is registered.", "providername");

            return Instance<T>(prov);
        }

        /// <summary>
        /// Instansiate an provider of type T if one dosn't exist and returns it.
        /// </summary>
        /// <typeparam name="T">The ProviderType.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>The provider typed as T.</returns>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException is thrown if T dosn't match any registered ProviderTypes,
        /// if the type provided doesn't implement T or if type isn't registered.
        /// </exception>
        /// <exception cref="System.Exception">An Exception is thrown if an attempt to instanitate a new T failed.</exception>
        /// <exception cref="System.ArgumentNullException">An ArgumentNullException is thrown if provider is null.</exception>
        /// <example>
        /// Get a provider typed as Car from audi 
        /// <code>
        /// ProviderFactory pf = new ProviderFactory();
        /// pf.RegisterType(typeof(Car));
        /// pf.Regsiter(typeof(Audi));
        /// Provider audi = pf.GetRegisteredProviders&lt;Car&gt;()[0];
        /// Car prv = Instance%lt;Car&gt;(audi);
        /// </code>
        /// </example>
        public T Instance<T>(Provider provider)
        {
            Type t = typeof(T);

            ProviderType providerType = types.Where(tp => tp.Type == t).FirstOrDefault();

            if (provider == null)
                throw new ArgumentNullException("Provider can't be null.", "provider");

            if (!provider.CanUse)
                throw new Exception("Provided provider can't be used.");

            if (providerType == null)
                throw new ArgumentException("No providertype of that type is registered.", "T");

            if (!provider.IsOfProviderType(providerType))
                throw new ArgumentException("Provided type does not inherit T.", "type");

            if (!providers.Contains(provider))
                throw new ArgumentException("Provided provider is not registered.", "type");

            Object instance = provider.Instance;

            if (instance is T)
                return (T)instance;

            throw new Exception("Faild to initialize instanitate type.");
        }

        /// <summary>
        /// The CreateInstance method takes a provider and instanisiate it.
        /// </summary>
        /// <param name="provider">The provider to instansiate.</param>
        /// <returns>An instance of the provider cast to a ProviderBase.</returns>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException in thrown if the Type provided dosen't extend ProviderBase,
        /// or if the Type provided dosen't have a valid constructor.
        ///</exception>
        internal ProviderBase CreateInstance(Provider provider)
        {
            if (!provider.Type.IsSubclassOf(typeof(ProviderBase)))
                throw new ArgumentException("Supplied type did not inherit ProviderBase.", "provider");

            ConstructorInfo ctr = provider.Type.GetSpecificConstructor(typeof(ProviderFactory));

            if (ctr == null)
                throw new ArgumentException("The type provided dosn't have a valid constructor.");

            Object[] parameters = (from param in ctr.GetParameters()
                                   select (!param.IsOptional ? this : param.DefaultValue)).ToArray();

            return ctr.Invoke(parameters) as ProviderBase;
        }
    }
}
