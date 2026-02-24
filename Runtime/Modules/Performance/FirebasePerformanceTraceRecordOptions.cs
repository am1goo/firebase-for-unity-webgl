using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebasePerformanceTraceRecordOptions
    {
        [Preserve]
        public Dictionary<string, int> metrics { get; set; }
        [Preserve]
        public Dictionary<string, string> attributes { get; set; }
    }
}
