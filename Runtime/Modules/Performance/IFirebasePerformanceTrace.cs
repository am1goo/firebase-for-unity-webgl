using System;
using System.Collections.Generic;

namespace FirebaseWebGL
{
    public interface IFirebasePerformanceTrace
    {
        string name { get; }
        bool isStarted { get; }
        bool isStopped { get; }

        void Start();
        void Stop();
        void PutAttribute(string attr, string name);
        void RemoteAttribute(string attr);
        string GetAttribute(string attr);
        IReadOnlyDictionary<string, string> GetAttributes();
        void PutMetric(string name, int num);
        int GetMetric(string name);
        void IncrementMetric(string name);
        void IncrementMetric(string name, int num);
        void Record(TimeSpan startTime, TimeSpan duration, FirebasePerformanceTraceRecordOptions options);
    }
}
