using System;

namespace FirebaseWebGL
{
    public interface IFirebaseAuth : IFirebaseModule
    {
        IFirebaseAuthLoggedUser loggerUser { get; }

        public delegate void OnLoggerUserChangedDelegate(IFirebaseAuthLoggedUser currentUser);
        OnLoggerUserChangedDelegate onLoggedUserChanged { get; set; }

        string languageCode { get; }
        string tenantId { get; }
        bool isRecaptchaInitialized { get; }

        void Initialize(Action<FirebaseCallback<bool>> firebaseCallback);
        void ConnectAuthEmulator(string url, FirebaseAuthEmulatorOptions options);
        void ApplyActionCode(string oobCode, Action<FirebaseCallback<bool>> firebaseCallback);
        void CheckActionCode(string oobCode, Action<FirebaseCallback<FirebaseAuthActionCodeInfo>> firebaseCallback);
        void ConfirmPasswordReset(string oobCode, string newPassword, Action<FirebaseCallback<bool>> firebaseCallback);
        void CreateUserWithEmailAndPassword(string email, string password, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void FetchSignInMethodsForEmail(string email, Action<FirebaseCallback<string[]>> firebaseCallback);
        void InitializeRecaptchaConfig(Action<FirebaseCallback<bool>> firebaseCallback);
        bool IsSignInWithEmailLink(string emailLink);
        void BeforeAuthStateChanged(Func<FirebaseAuthUser, bool> onBeforeAuthStateChanged);
        void OnAuthStateChanged(Action<FirebaseAuthUser> onAuthStateChanged);
        void OnIdTokenChanged(Action<FirebaseAuthUser> onIdTokenChanged);
        void RevokeAccessToken(string token, Action<FirebaseCallback<bool>> firebaseCallback);
        void SendPasswordResetEmail(string email, FirebaseAuthActionCodeSettings actionCodeSettings, Action<FirebaseCallback<bool>> firebaseCallback);
        void SendSignInLinkToEmail(string email, FirebaseAuthActionCodeSettings actionCodeSettings, Action<FirebaseCallback<bool>> firebaseCallback);
        void SignInAnonymously(Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void SignInWithCredential(FirebaseAuthCredential credential, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void SignInWithCustomToken(string customToken, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void SignInWithEmailAndPassword(string email, string password, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void SignInWithEmailLink(string email, string emailLink, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback);
        void SignOut(Action<FirebaseCallback<bool>> firebaseCallback);
        void UpdateCurrentUser(FirebaseAuthUser user, Action<FirebaseCallback<bool>> firebaseCallback);
        bool UseDeviceLanguage();
        void ValidatePassword(string password, Action<FirebaseCallback<FirebaseAuthPasswordValidationStatus>> firebaseCallback);
        void VerifyPasswordResetCode(string code, Action<FirebaseCallback<string>> firebaseCallback);
        FirebaseAuthActionCodeURL ParseActionCodeURL(string link);
    }
}
