using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppVisum.Sys.DefaultProviderTypes
{
    [ProviderType("BlogProvider")]
    public interface IBlogProvider
    {
        int GetPostCount();
    }
}
