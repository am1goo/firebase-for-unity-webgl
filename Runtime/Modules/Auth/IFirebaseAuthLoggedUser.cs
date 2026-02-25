using System;

namespace FirebaseWebGL
{
    public interface IFirebaseAuthLoggedUser
    {
        string uid { get; }
        string displayName { get; }
        string email { get; }
        string phoneNumber { get; }
        string photoURL { get; }
        string providerId { get; }
        bool isAnonymous { get; }
        bool emailVerified { get; }

        void DeleteUser(Action<FirebaseCallback<bool>> firebaseCallback);
        void GetIdToken(bool forceRefresh, Action<FirebaseCallback<string>> firebaseCallback);
        void GetIdTokenResult(bool forceRefresh, Action<FirebaseCallback<FirebaseAuthIdTokenResult>> firebaseCallback);
        void Reload(Action<FirebaseCallback<bool>> firebaseCallback);
    }
}
