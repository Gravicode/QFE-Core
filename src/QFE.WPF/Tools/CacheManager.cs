using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace QFE.WPF.Tools
{
    public class CacheManager<T>
    {
        public T this[string name]
        {
            get
            {  //Get the default MemoryCache to cache objects in memory
                ObjectCache cache = MemoryCache.Default;
                object data = cache.Get(name);
                if (data != null)
                {
                    return (T)data;
                }
                return default(T);
            }
            set
            {
                //Get the default MemoryCache to cache objects in memory
                ObjectCache cache = MemoryCache.Default;

                //Create a custom Timeout of 10 seconds
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.MaxValue;

                //Get data from the database and write them to the result

                //add the object to the cache
                cache.Add(name, value, policy);
            }
        }
    }
}
