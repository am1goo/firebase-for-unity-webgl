using System;

namespace FirebaseWebGL
{
    public interface IFirebaseStorage : IFirebaseModule
    {
        void Initialize(Action<FirebaseCallback<bool>> callback);
        void ConnectStorageEmulator(string host, int port, FirebaseStorageEmulatorMockTokenOptions options);
        IFirebaseStorageReference Ref(string url);
    }
}
