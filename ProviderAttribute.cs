using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class ProviderTypeAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string name;

        // This is a positional argument
        public ProviderTypeAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        // This is a named argument
        public int Description { get; set; }
    }
}
