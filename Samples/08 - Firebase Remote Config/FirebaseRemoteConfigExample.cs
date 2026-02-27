using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseRemoteConfigExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseRemoteConfig _remoteConfig;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _remoteConfig = app.RemoteConfig;
            if (_remoteConfig == null)
            {
                Debug.LogError($"Start: {nameof(IFirebaseRemoteConfig)} is not injected");
                yield break;
            }

            bool? initialized = null;
            _remoteConfig.Initialize((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"Initialize: {callback.error}");
                    return;
                }
                initialized = callback.result;
            });
            yield return new WaitUntil(() => initialized != null);
            if (initialized.Value == false)
            {
                Debug.LogError("Initialize: not initialized");
                yield break;
            }

            _remoteConfig.SetCustomSignals(new Dictionary<string, string>
            {
                { "city", "London" },
                { "preferred_event_category", "cybersport" },
                { "nullable", null },
            });
            _remoteConfig.SetLogLevel(FirebaseRemoteConfigLogLevel.Debug);
            _remoteConfig.OnConfigUpdate(OnConfigUpdate);
            _remoteConfig.OnConfigUpdate(OnConfigUpdate);

            _remoteConfig.EnsureInitialized((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"EnsureInitialized: {callback.error}");
                    return;
                }
                Debug.Log($"EnsureInitialized: {callback.result}");
            });

            _remoteConfig.Activate((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"Activate: {callback.error}");
                    return;
                }
                Debug.Log($"Activate: {callback.result}");
            });

            _remoteConfig.FetchConfig((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"FetchConfig: {callback.error}");
                    return;
                }
                Debug.Log($"FetchConfig: {callback.result}");
            });

            _remoteConfig.FetchAndActivate((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"FetchAndActivate: {callback.error}");
                    return;
                }
                Debug.Log($"FetchAndActivate: {callback.result}");
            });

            var keys = _remoteConfig.GetKeys();
            Debug.Log($"GetKeys: {string.Join(", ", keys)}");

            var boolKey = "boolKey";
            Debug.Log($"GetBoolean: {boolKey}={_remoteConfig.GetBoolean(boolKey)}");

            var integerKey = "integerKey";
            Debug.Log($"GetInteger: {integerKey}={_remoteConfig.GetInteger(integerKey)}");

            var stringKey = "stringKey";
            Debug.Log($"GetString: {stringKey}={_remoteConfig.GetString(stringKey)}");

            void OnConfigUpdate(string[] updatedKeys)
            {
                Debug.Log($"OnConfigUpdate: {string.Join(", ", updatedKeys)}");
            }
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_remoteConfig == null)
                return;

            GUILayout.Label("RemoteConfig:");
            GUILayout.Label($"- initialized: {_remoteConfig.isInitialized}");
        }
#endif
    }
}
