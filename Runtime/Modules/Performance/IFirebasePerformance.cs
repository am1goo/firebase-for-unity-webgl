using System;

namespace FirebaseWebGL
{
    public interface IFirebasePerformance : IFirebaseModule
    {
        void Initialize(Action<FirebaseCallback<bool>> callback);
        IFirebasePerformanceTrace Trace(string name);
    }
}