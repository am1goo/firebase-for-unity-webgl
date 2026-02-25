using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseMessaging : IFirebaseMessaging
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseMessaging_initialize(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseMessaging_getToken(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseMessaging_deleteToken(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseMessaging_onMessage(int instanceId, FirebaseJsonCallbackDelegate callback);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<bool>>> _onBoolCallbacks = new Dictionary<int, Action<FirebaseCallback<bool>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onMessageReceivedCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();

        private bool _isInitializing = false;

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }
        public Action<string> onMessageReceived { get; set; }

        private FirebaseMessagingServiceWorker _serviceWorker;
        public IFirebaseMessagingServiceWorker ServiceWorker => _serviceWorker;

        private readonly int _instanceId;
        private string _token;

        public FirebaseMessaging(bool enableServiceWorker)
        {
            _instanceId = _requests.NextId();

            if (enableServiceWorker)
            {
                _serviceWorker = new FirebaseMessagingServiceWorker();
            }
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
            FirebaseWebGL_FirebaseMessaging_initialize(requestId, OnBoolCallback);
        }

        public void GetToken(Action<FirebaseCallback<string>> firebaseCallback)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            if (_token != null)
            {
                firebaseCallback?.Invoke(FirebaseCallback<string>.Success(_token));
                return;
            }

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                if (callback.success)
                {
                    _token = callback.result;
                }

                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseMessaging_getToken(requestId, OnStringCallback);
        }

        public void DeleteToken(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            if (_token == null)
            {
                firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(false));
                return;
            }

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                if (callback.success)
                {
                    if (callback.result)
                    {
                        _token = null;
                    }
                }

                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseMessaging_deleteToken(requestId, OnBoolCallback);
        }

        public void OnMessage(Action<string> onMessageReceived)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            this.onMessageReceived = onMessageReceived;

            _onMessageReceivedCallbacks[_instanceId] = (callback) =>
            {
                if (callback.success)
                {
                    this.onMessageReceived?.Invoke(callback.result);
                }
            };

            FirebaseWebGL_FirebaseMessaging_onMessage(_instanceId, OnMessageCallback);
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
        private static void OnMessageCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onMessageReceivedCallbacks, json, doNotRemoveCallback: true);
        }
    }
}
