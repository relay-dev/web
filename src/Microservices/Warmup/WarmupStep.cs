using System;

namespace Microservices.Warmup
{
    public class WarmupStep
    {
        public bool IsAsync { get; set; }

        public WarmupStep RunAfter { get; set; }

        public Type WarmupType { get; set; }
    }
}
