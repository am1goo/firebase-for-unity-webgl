using System;

namespace FirebaseWebGL
{
    public interface IFirebaseInstallations : IFirebaseModule
    {
        void Initialize(Action<FirebaseCallback<bool>> callback);
        void DeleteInstallations(Action<FirebaseCallback<bool>> callback);
        void GetId(Action<FirebaseCallback<string>> callback);
        void GetToken(bool forceRefresh, Action<FirebaseCallback<string>> callback);
        void OnIdChange(Action<string> callback);
    }
}
