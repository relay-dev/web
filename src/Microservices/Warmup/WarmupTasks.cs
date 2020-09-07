using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microservices.Warmup
{
    public static class WarmupTasks
    {
        private static readonly ConcurrentDictionary<string, Type> WarmupTypes = new ConcurrentDictionary<string, Type>();

        public static void AddWarmupType(Type type)
        {
            WarmupTypes.TryAdd(type.Name, type);
        }

        public static List<Type> All => WarmupTypes.Values.ToList();
    }
}
