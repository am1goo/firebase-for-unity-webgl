namespace FirebaseWebGL
{
    public interface IFirebaseApp : IFirebaseModule
    {
        IFirebaseAuth Auth { get; }
        IFirebaseAnalytics Analytics { get; }
        IFirebaseAppCheck AppCheck { get; }
        IFirebaseFunctions Functions { get; }
        IFirebaseMessaging Messaging { get; }
        IFirebaseRemoteConfig RemoteConfig { get; }
        IFirebaseInstallations Installations { get; }
        IFirebasePerformance Performance { get; }
        IFirebaseStorage Storage { get; }
    }
}
