using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseInstallations : IFirebaseInstallations
    {
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseInstallations_initialize();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseInstallations_deleteInstallations(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseInstallations_getId(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseInstallations_getToken(bool forceRefresh, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseInstallations_onIdChange(int instanceId, FirebaseJsonCallbackDelegate callback);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<bool>>> _onBoolCallbacks = new Dictionary<int, Action<FirebaseCallback<bool>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onIdChangeCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }
        public Action<string> onIdChanged { get; set; }

        private readonly int _instanceId;

        internal FirebaseInstallations()
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

            _isInitialized = FirebaseWebGL_FirebaseInstallations_initialize();
            onInitialized?.Invoke(_isInitialized);
            firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(_isInitialized));
        }

        public void DeleteInstallations(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseInstallations_deleteInstallations(requestId, OnBoolCallback);
        }

        public void GetId(Action<FirebaseCallback<string>> firebaseCallback)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseInstallations_getId(requestId, OnStringCallback);
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

            FirebaseWebGL_FirebaseInstallations_getToken(forceRefresh, requestId, OnStringCallback);
        }

        public void OnIdChange(Action<string> onIdChange)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            this.onIdChanged = onIdChanged;

            _onIdChangeCallbacks[_instanceId] = (callback) =>
            {
                if (callback.success)
                {
                    this.onIdChanged?.Invoke(callback.result);
                }
            };

            FirebaseWebGL_FirebaseInstallations_onIdChange(_instanceId, OnIdChangeCallback);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnBoolCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onBoolCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnStringCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onStringCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnIdChangeCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onIdChangeCallbacks, json, doNotRemoveCallback: true);
        }
    }
}
