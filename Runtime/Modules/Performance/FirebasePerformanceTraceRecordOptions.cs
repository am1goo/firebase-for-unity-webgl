using System;
using System.Collections.Generic;

[Serializable]
public sealed class FirebasePerformanceTraceRecordOptions
{
    public Dictionary<string, int> metrics { get; set; }
    public Dictionary<string, string> attributes { get; set; }
}
