using System;

namespace FirebaseWebGL
{
    public interface IFirebaseMessagingServiceWorker : IFirebaseModule
    {
        void Initialize(Action<FirebaseCallback<bool>> callback);
        void ExperimentalSetDeliveryMetricsExportedToBigQueryEnabled(bool enabled);
    }
}
