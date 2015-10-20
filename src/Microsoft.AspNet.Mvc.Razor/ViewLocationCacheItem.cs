using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc.Razor
{
    public struct ViewLocationCacheItem
    {
        public ViewLocationCacheItem(Type type, string location)
        {
            Type = type;
            Location = location;
        }

        public string Location { get; }

        public Type Type { get; }

    }
}
