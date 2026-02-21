using FirebaseWebGL;
using System;
using System.Collections.Generic;

public interface IFirebaseRemoteConfig : IFirebaseModule
{
    void Initialize(Action<FirebaseCallback<bool>> firebaseCallback);
    void Activate(Action<FirebaseCallback<bool>> firebaseCallback);
    void EnsureInitialized(Action<FirebaseCallback<bool>> firebaseCallback);
    void FetchAndActivate(Action<FirebaseCallback<bool>> firebaseCallback);
    void FetchConfig(Action<FirebaseCallback<bool>> firebaseCallback);
    bool GetBoolean(string key);
    int GetInteger(string key);
    string GetString(string key);
    void OnConfigUpdate(Action<string[]> onConfigUpdate);
    void SetCustomSignals(IReadOnlyDictionary<string, string> customSignals);
    void SetLogLevel(FirebaseRemoteConfigLogLevel logLevel);
}
