using System;

namespace FirebaseWebGL
{
    public interface IFirebaseFunctions : IFirebaseModule
    {
        void Initialize(Action<FirebaseCallback<bool>> firebaseCallback);
        void ConnectFunctionsEmulator(string host, int port);
        IFirebaseFunctionsHttpsCallable HttpsCallable(string name, FirebaseFunctionsHttpsCallableOptions options);
        IFirebaseFunctionsHttpsCallable HttpsCallableFromURL(string url, FirebaseFunctionsHttpsCallableOptions options);
    }
}
