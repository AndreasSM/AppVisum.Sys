using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AppVisum.Sys
{
    static class TypeExtensions
    {
        public static ConstructorInfo GetParameterlessConstructor(this Type type)
        {
            return (from c in type.GetConstructors()
                    where c.GetParameters().Where(p => !p.IsOptional).Count() == 0
                    select c).FirstOrDefault();
        }

        public static ConstructorInfo GetSpecificConstructor(this Type type, Type required)
        {
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
