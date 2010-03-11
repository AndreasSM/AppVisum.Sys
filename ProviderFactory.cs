using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Globalization;

namespace AppVisum.Sys
{
    public class ProviderFactory
    {
        private Dictionary<Type, ProviderClass> types;
        private Dictionary<Type, Object> providers;
        private Dictionary<Type, Type> selected;

        /// <summary>
        /// The default constructor for the ProviderFactory.
        /// Takes no arguments.
        /// </summary>
        public ProviderFactory()
        {
            types = new Dictionary<Type, ProviderClass>();
            providers = new Dictionary<Type, Object>();
            selected = new Dictionary<Type, Type>();
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
        /// not an interface, allready registered or hasn't got the
        /// ProviderTypeAttribute.
        /// </exception>
        public void RegisterType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsInterface)
                throw new ArgumentException("The provided type was not an Interface.", "type");

            if (types.Keys.Contains(type))
                throw new ArgumentException("The provided type is allready registered.", "type");

            MemberInfo memberinfo = type;

            object[] attributes = memberinfo.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is ProviderTypeAttribute)
                {
                    ProviderTypeAttribute attr = attribute as ProviderTypeAttribute;
                    if (types.Values.Where(t => t.ProviderAttribute.Name == attr.Name).Count() > 0)
                        throw new ArgumentException("A provider type with that name is already registered.", "type");
                    ProviderClass pc = new ProviderClass(type, attr);
                    types.Add(type, pc);
                    return;
                }
            }

            throw new ArgumentException("The type provided was not a ProviderClass, the ProviderTypeAttribute was lacking.", "type");
        }

        /// <summary>
        /// Registers a provider to be used by the ProviderFactory.
        /// Any given provider that is registered can be returned
        /// from the Instance&lt;T&gt; method.
        /// </summary>
        /// <param name="provider">The providertype</param>
        /// <param name="instance" optional="true">An instance of the provider. If the provider doesn't contain any parameterless constructors instance is required.</param>
        /// <exception cref="System.ArgumentNullException">An ArgumentNullException is thrown if provider is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException is thrown if the provider is allready added,
        /// if it's an interface, if it doesn't implement any of the registered
        /// types, if it doesn't have a parameterless constructor or if the instance
        /// provided is not an instance of the provider provided.
        /// </exception>
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
                ConstructorInfo ctr = provider.GetParameterlessConstructor();
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
        public void SetCurrent<T>(String providername)
        {
            Type t = typeof(T);
            if (!types.Keys.Contains(t))
                throw new ArgumentException("No providertype of that type is registered.", "T");

            Type selected = null;

            foreach (Type provider in providers.Keys)
            {
                if (provider.Name.Trim().ToLower() == providername.ToLower().Trim())
                {
                    selected = provider;
                    break;
                }
            }

            if (selected == null)
                throw new ArgumentException("No providers with that name found.", "providername");

            this.selected[t] = selected;
        }

        /// <summary>
        /// Instansiate an provider of type T if one dosn't exist and returns it.
        /// </summary>
        /// <typeparam name="T">The ProviderType</typeparam>
        /// <returns>The provider typed as T</returns>
        /// <exception cref="System.ArgumentException">An ArgumentException is thrown if T dosn't match any registered ProviderTypes.</exception>
        /// <exception cref="System.Exception">An Exception is thrown if no providers is found for that particular ProviderType.</exception>
        public T Instance<T>()
        {
            Type t = typeof(T);
            if(!types.Keys.Contains(t))
                throw new ArgumentException("No providertype of that type is registered.", "T");

            Type current = selected.Keys.Contains(t) ? selected[t] : providers.Keys.Where(p => p.GetInterfaces().Contains(t)).FirstOrDefault();
            if(current == null)
                throw new Exception("No providers found for that type.");

            return Instance<T>(current);
        }

        /// <summary>
        /// Instansiate an provider of type T if one dosn't exist and returns it.
        /// </summary>
        /// <typeparam name="T">The ProviderType</typeparam>
        /// <param name="providername">The name of the provider to select</param>
        /// <returns>The provider typed as T</returns>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException is thrown if T dosn't match any registered ProviderTypes
        /// or if no provider is found with the name of `providername`.
        /// </exception>
        public T Instance<T>(String providername)
        {
            Type t = typeof(T);
            if (!types.Keys.Contains(t))
                throw new ArgumentException("No providertype of that type is registered.", "T");

            var provs = providers.Where(kvp => kvp.Key.Name.Split('.').Last().ToLower().Trim() == providername).FirstOrDefault();
            if (provs.Key == null)
                throw new ArgumentException("No providers with that name is registered.", "providername");

            return Instance<T>(provs.Key);
        }

        /// <summary>
        /// Instansiate an provider of type T if one dosn't exist and returns it.
        /// </summary>
        /// <typeparam name="T">The ProviderType</typeparam>
        /// <param name="type">The providers Type</param>
        /// <returns>The provider typed as T</returns>
        /// <exception cref="System.ArgumentException">
        /// An ArgumentException is thrown if T dosn't match any registered ProviderTypes,
        /// if the type provided doesn't implement T or if type isn't registered.
        /// </exception>
        /// <exception cref="System.Exception">An Exception is thrown if an attempt to instanitate a new T failed.</exception>
        public T Instance<T>(Type type)
        {
            Type t = typeof(T);
            if (!types.Keys.Contains(t))
                throw new ArgumentException("No providertype of that type is registered.", "T");

            if (!type.GetInterfaces().Contains(t))
                throw new ArgumentException("Provided type does not inherit T.", "type");

            if (!providers.Keys.Contains(type))
                throw new ArgumentException("Provided type is not registered.", "type");

            Object instance = providers[type];

            if (instance == null)
            {
                ConstructorInfo ctr = type.GetParameterlessConstructor();
                Object[] parameters = (from param in ctr.GetParameters()
                                       select param.DefaultValue).ToArray();

                instance = ctr.Invoke(parameters);
            }

            if (instance is T)
                return (T)instance;

            throw new Exception("Faild to initialize instanitate type.");
        }
    }
}
