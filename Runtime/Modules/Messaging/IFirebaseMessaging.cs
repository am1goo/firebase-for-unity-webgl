using System;

namespace FirebaseWebGL
{
    public interface IFirebaseMessaging : IFirebaseModule
    {
        IFirebaseMessagingServiceWorker ServiceWorker { get; }

        void Initialize(Action<FirebaseCallback<bool>> callback);
        void GetToken(Action<FirebaseCallback<string>> callback);
        void DeleteToken(Action<FirebaseCallback<bool>> callback);

        void OnMessage(Action<string> onMessageReceived);
    }
}
