using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseAppCheck : IFirebaseAppCheck
    {
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseAppCheck_initialize();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAppCheck_getLimitedUseToken(int requestId, FirebaseJsonCallbackDelegate callbackPtr);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAppCheck_getToken(bool forceRefresh, int requestId, FirebaseJsonCallbackDelegate callbackPtr);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAppCheck_onTokenChanged(int instanceId, FirebaseJsonCallbackDelegate callbackPtr);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAppCheck_setTokenAutoRefreshEnabled(bool isTokenAutoRefreshEnabled);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onTokenChangedCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }
        public Action<string> onTokenChanged { get; set; }

        private readonly int _instanceId;

        public FirebaseAppCheck()
        {
            _instanceId = _requests.NextId();
        }

        public void Initialize(Action<FirebaseCallback<bool>> firebaseCallback)
        {
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

            _isInitialized = FirebaseWebGL_FirebaseAppCheck_initialize();
            onInitialized?.Invoke(_isInitialized);
            firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(_isInitialized));
        }

        public void GetLimitedUseToken(Action<FirebaseCallback<string>> firebaseCallback)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAppCheck_getLimitedUseToken(requestId, OnStringCallback);
        }

        public void GetToken(bool forceRefresh, Action<FirebaseCallback<string>> firebaseCallback)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAppCheck_getToken(forceRefresh, requestId, OnStringCallback);
        }

        public void OnTokenChanged(Action<string> onTokenChanged)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            this.onTokenChanged = onTokenChanged;

            _onTokenChangedCallbacks[_instanceId] = (callback) =>
            {
                if (callback.success)
                {
                    this.onTokenChanged?.Invoke(callback.result);
                }
            };

            FirebaseWebGL_FirebaseAppCheck_onTokenChanged(_instanceId, OnTokenChangedCallback);
        }

        public void SetTokenAutoRefreshEnabled(bool isTokenAutoRefreshEnabled)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            FirebaseWebGL_FirebaseAppCheck_setTokenAutoRefreshEnabled(isTokenAutoRefreshEnabled);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnStringCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onStringCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnTokenChangedCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onTokenChangedCallbacks, json, doNotRemoveCallback: true);
        }
    }
}
