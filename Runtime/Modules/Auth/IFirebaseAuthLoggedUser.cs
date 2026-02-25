using System;
using System.Collections.Generic;

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
        void LinkWithCredential(FirebaseAuthCredential credential, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void LinkInWithPopup(string providerId, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void LinkInWithPopup(string providerId, Dictionary<string, string> customParameters, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void ReauthenticateWithCredential(FirebaseAuthCredential credential, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void ReauthenticateInWithPopup(string providerId, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void ReauthenticateInWithPopup(string providerId, Dictionary<string, string> customParameters, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void SendEmailVerification(Action<FirebaseCallback<bool>> firebaseCallback);
        void SendEmailVerification(FirebaseAuthActionCodeSettings actionCodeSettings, Action<FirebaseCallback<bool>> firebaseCallback);
        void Unlink(string providerId, Action<FirebaseCallback<FirebaseAuthUser>> firebaseCallback);
        void UpdateEmail(string newEmail, Action<FirebaseCallback<bool>> firebaseCallback);
        void UpdatePassword(string newPassword, Action<FirebaseCallback<bool>> firebaseCallback);
        void UpdateProfile(string displayName, string photoURL, Action<FirebaseCallback<bool>> firebaseCallback);
        void VerifyBeforeUpdateEmail(string newEmail, Action<FirebaseCallback<bool>> firebaseCallback);
        void VerifyBeforeUpdateEmail(string newEmail, FirebaseAuthActionCodeSettings actionCodeSettings, Action<FirebaseCallback<bool>> firebaseCallback);
    }
}
