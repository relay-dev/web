using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microservices.Warmup
{
    public static class WarmupTasks
    {
        private static ConcurrentDictionary<string, Type> _warmupTypes = new ConcurrentDictionary<string, Type>();

        public static void AddWarmupType(Type type)
        {
            _warmupTypes.TryAdd(type.Name, type);
        }

        public static List<Type> All
        {
            get
            {
                return _warmupTypes.Values.ToList();
            }
        }
    }
}
