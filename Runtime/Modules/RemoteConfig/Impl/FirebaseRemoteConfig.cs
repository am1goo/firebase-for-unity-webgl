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
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_initialize(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_activate(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_ensureInitialized(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_fetchAndActivate(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_fetchConfig(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseRemoteConfig_getKeys();
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseRemoteConfig_getBoolean(string key);
        [DllImport("__Internal")]
        private static extern int FirebaseWebGL_FirebaseRemoteConfig_getInteger(string key);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseRemoteConfig_getString(string key);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseRemoteConfig_onConfigUpdate(int instanceId, FirebaseJsonCallbackDelegate callback);
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
            {
                firebaseCallback?.Invoke(FirebaseCallback<bool>.Error(FirebaseCallbackErrors.InitializationIsAlreadyInProgress));
                return;
            }

            if (_isInitialized)
            {
                firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(_isInitialized));
                return;
            }

            if (Application.isEditor)
            {
                firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(false));
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

        public string[] GetKeys()
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var keysAsJson = FirebaseWebGL_FirebaseRemoteConfig_getKeys();
            return JsonConvert.DeserializeObject<string[]>(keysAsJson);
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
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

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

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnBoolCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onBoolCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnConfigUpdateCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onConfigUpdateCallbacks, json, doNotRemoveCallback: true);
        }
    }
}
