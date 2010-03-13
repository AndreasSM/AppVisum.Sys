using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AppVisum.Sys
{
    /// <summary>
    /// Provides extensions to Type. 
    /// </summary>
    static class TypeExtensions
    {
        /// <summary>
        /// Gets a construction from type which can be called without parameters.
        /// </summary>
        /// <param name="type">The Type in which to find constructors.</param>
        /// <returns>A constructor which can be called without parameters, or null if none exist.</returns>
        /// <example>
        /// Get a parameterless constructor for Car:
        /// <code>ConstructorInfo cts = typeof(Car).GetParameterlessConstructor();</code>
        /// </example>
        public static ConstructorInfo GetParameterlessConstructor(this Type type)
        {
            return (from c in type.GetConstructors()
                    where c.GetParameters().Where(p => !p.IsOptional).Count() == 0
                    select c).FirstOrDefault();
        }

        /// <summary>
        /// Gets the constructor from type which requires maximum one of
        /// given parameter "required" to be called, or one without any
        /// parameters if it exists.
        /// </summary>
        /// <param name="type">The Type in which to find constructors.</param>
        /// <param name="required">The Type you want the constructor to require maximum one of.</param>
        /// <returns>
        /// A constructor that don't require any parameters.
        /// If none exist, it returns a constructor that require exactly one of given parameter,
        /// or null if none such exist.
        /// </returns>
        /// <example>
        /// Get a constructor for Car which requires engine to be called:
        /// <code>ConstructorInfo cts = typeof(Car).GetSpecificConstructor(typeof(engine));</code>
        /// </example>
        public static ConstructorInfo GetSpecificConstructor(this Type type, Type required)
        {
            var ctrs = type.GetParameterlessConstructor();
            if (ctrs != null) return ctrs;
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                var reqParams = constructor.GetParameters().Where(p => !p.IsOptional);
                if (reqParams.Count() > 0 && reqParams.Any(p => p.ParameterType != required))
                    continue;

                if (reqParams.Where(p => p.ParameterType == required).Count() > 1)
                    continue;

                return constructor;
            }

            return null;
        }
    }
}
