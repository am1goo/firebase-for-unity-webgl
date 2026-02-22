using System;
using System.Collections.Generic;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebasePerformanceTraceRecordOptions
    {
        public Dictionary<string, int> metrics { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }
}
