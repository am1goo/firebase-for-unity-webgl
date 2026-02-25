using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseMessagingServiceWorker : IFirebaseMessagingServiceWorker
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseMessaging_ServiceWorker_initialize(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseMessaging_ServiceWorker_experimentalSetDeliveryMetricsExportedToBigQueryEnabled(bool enabled);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<bool>>> _onBoolCallbacks = new Dictionary<int, Action<FirebaseCallback<bool>>>();

        private bool _isInitializing = false;

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }
        public Action<string> onBackgroundMessageReceived { get; set; }

        private readonly int _instanceId;

        public FirebaseMessagingServiceWorker()
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
            FirebaseWebGL_FirebaseMessaging_ServiceWorker_initialize(requestId, OnBoolCallback);
        }

        public void ExperimentalSetDeliveryMetricsExportedToBigQueryEnabled(bool enabled)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            FirebaseWebGL_FirebaseMessaging_ServiceWorker_experimentalSetDeliveryMetricsExportedToBigQueryEnabled(enabled);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnBoolCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onBoolCallbacks, json);
        }
    }
}
