namespace FirebaseWebGL
{
    public interface IFirebaseApp : IFirebaseModule
    {
        IFirebaseAnalytics Analytics { get; }
        IFirebaseMessaging Messaging { get; }
        IFirebaseRemoteConfig RemoteConfig { get; }
        IFirebaseInstallations Installations { get; }
        IFirebasePerformance Performance { get; }
    }
}
