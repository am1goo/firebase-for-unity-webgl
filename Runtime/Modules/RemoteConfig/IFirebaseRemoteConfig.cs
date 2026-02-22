using System;
using System.Collections.Generic;

namespace FirebaseWebGL
{
    public interface IFirebaseRemoteConfig : IFirebaseModule
    {
        void Initialize(Action<FirebaseCallback<bool>> firebaseCallback);
        void Activate(Action<FirebaseCallback<bool>> firebaseCallback);
        void EnsureInitialized(Action<FirebaseCallback<bool>> firebaseCallback);
        void FetchAndActivate(Action<FirebaseCallback<bool>> firebaseCallback);
        void FetchConfig(Action<FirebaseCallback<bool>> firebaseCallback);
        string[] GetKeys();
        bool GetBoolean(string key);
        int GetInteger(string key);
        string GetString(string key);
        void OnConfigUpdate(Action<string[]> onConfigUpdate);
        void SetCustomSignals(IReadOnlyDictionary<string, string> customSignals);
        void SetLogLevel(FirebaseRemoteConfigLogLevel logLevel);
    }
}
