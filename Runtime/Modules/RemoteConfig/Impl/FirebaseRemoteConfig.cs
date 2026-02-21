using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseRemoteConfig : IFirebaseRemoteConfig
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_initialize(int requestId, FirebaseCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_activate(int requestId, FirebaseCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_ensureInitialized(int requestId, FirebaseCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_fetchAndActivate(int requestId, FirebaseCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_fetchConfig(int requestId, FirebaseCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseRemoteConfig_getBoolean(string key);
        [DllImport("__Internal")]
        private static extern int FirebaseWebGL_FirebaseRemoteConfig_getInteger(string key);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseRemoteConfig_getString(string key);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_onConfigUpdate(int instanceId, FirebaseCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_setCustomSignals(string customSignalsAsJson);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_setLogLevel(int logLevel);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<bool>>> _onBoolCallbacks = new Dictionary<int, Action<FirebaseCallback<bool>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string[]>>> _onConfigUpdateCallbacks = new Dictionary<int, Action<FirebaseCallback<string[]>>>();

        private bool _isInitializing = false;

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }
        public Action<string[]> onConfigUpdate { get; set; }

        private readonly int _instanceId;

        public FirebaseRemoteConfig()
        {
            _instanceId = _requests.NextId();
        }

        public void Initialize(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (_isInitializing)
                return;

            if (isInitialized)
                return;

            if (Application.isEditor)
            {
                firebaseCallback?.Invoke(new FirebaseCallback<bool>(false));
                return;
            }

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                _isInitializing = false;
                if (callback.success)
                {
                    _isInitialized = callback.result;
                    onInitialized?.Invoke(_isInitialized);
                }

                firebaseCallback?.Invoke(callback);
            });
            FirebaseWebGL_FirebaseRemoteConfig_initialize(requestId, OnBoolCallback);
        }

        public void Activate(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });
            FirebaseWebGL_FirebaseRemoteConfig_activate(requestId, OnBoolCallback);
        }

        public void EnsureInitialized(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });
            FirebaseWebGL_FirebaseRemoteConfig_ensureInitialized(requestId, OnBoolCallback);
        }

        public void FetchAndActivate(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });
            FirebaseWebGL_FirebaseRemoteConfig_fetchAndActivate(requestId, OnBoolCallback);
        }

        public void FetchConfig(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });
            FirebaseWebGL_FirebaseRemoteConfig_fetchConfig(requestId, OnBoolCallback);
        }

        public bool GetBoolean(string key)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            return FirebaseWebGL_FirebaseRemoteConfig_getBoolean(key);
        }

        public int GetInteger(string key)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            return FirebaseWebGL_FirebaseRemoteConfig_getInteger(key);
        }

        public string GetString(string key)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            return FirebaseWebGL_FirebaseRemoteConfig_getString(key);
        }

        public void OnConfigUpdate(Action<string[]> onConfigUpdate)
        {
            this.onConfigUpdate = onConfigUpdate;

            _onConfigUpdateCallbacks[_instanceId] = (callback) =>
            {
                if (callback.success)
                {
                    this.onConfigUpdate?.Invoke(callback.result);
                }
            };

            FirebaseWebGL_FirebaseRemoteConfig_onConfigUpdate(_instanceId, OnConfigUpdateCallback);
        }

        public void SetCustomSignals(IReadOnlyDictionary<string, string> customSignals)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var customSignalsAsJson = JsonConvert.SerializeObject(customSignals);
            FirebaseWebGL_FirebaseRemoteConfig_setCustomSignals(customSignalsAsJson);
        }

        public void SetLogLevel(FirebaseRemoteConfigLogLevel logLevel)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            FirebaseWebGL_FirebaseRemoteConfig_setLogLevel((int)logLevel);
        }

        [MonoPInvokeCallback(typeof(FirebaseCallbackDelegate))]
        private static void OnBoolCallback(string json)
        {
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<bool>>(json);

            if (_onBoolCallbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                _onBoolCallbacks.Remove(firebaseCallback.requestId);
                try
                {
                    callback?.Invoke(firebaseCallback);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        [MonoPInvokeCallback(typeof(FirebaseCallbackDelegate))]
        private static void OnConfigUpdateCallback(string json)
        {
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<string[]>>(json);

            var instanceId = firebaseCallback.requestId;
            if (_onConfigUpdateCallbacks.TryGetValue(instanceId, out var callback))
            {
                try
                {
                    callback?.Invoke(firebaseCallback);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}
