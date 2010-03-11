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
    }
}
