using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Pool;

namespace FirebaseWebGL
{
    internal sealed class FirebaseAnalytics : IFirebaseAnalytics
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAnalytics_initialize(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAnalytics_getGoogleAnalyticsClientId(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAnalytics_setAnalyticsCollectionEnabled(bool enabled);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAnalytics_setUserId(string userId);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAnalytics_setUserProperties(string propertiesAsJson);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAnalytics_setDefaultEventParameters(string parametersAsJson);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAnalytics_setConsent(string consentAsJson);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAnalytics_logEvent(string eventName, string eventValuesAsJson);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();

        private static readonly Dictionary<int, Action<FirebaseCallback<bool>>> _onBoolCallbacks = new Dictionary<int, Action<FirebaseCallback<bool>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();
        
        private bool _isInitializing = false;

        private bool _isInitialized = false;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }

        private string _clientId = null;

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

            FirebaseWebGL_FirebaseAnalytics_initialize(requestId, OnBoolCallback);
        }

        public void GetGoogleAnalyticsClientId(Action<FirebaseCallback<string>> firebaseCallback)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            if (_clientId != null)
            {
                firebaseCallback?.Invoke(FirebaseCallback<string>.Success(_clientId));
                return;
            }

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                if (callback.success)
                {
                    _clientId = callback.result;
                }

                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAnalytics_getGoogleAnalyticsClientId(requestId, OnStringCallback);
        }

        public void SetAnalyticsCollectionEnabled(bool enabled)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            FirebaseWebGL_FirebaseAnalytics_setAnalyticsCollectionEnabled(enabled);
        }

        public void SetUserId(string userId)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            FirebaseWebGL_FirebaseAnalytics_setUserId(userId);
        }

        public void SetUserProperties(Dictionary<string, string> properties)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var propertiesAsJson = JsonConvert.SerializeObject(properties);
            FirebaseWebGL_FirebaseAnalytics_setUserProperties(propertiesAsJson);
        }

        public void SetDefaultEventParameters(Dictionary<string, string> parameters)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var parametersAsJson = JsonConvert.SerializeObject(parameters);
            FirebaseWebGL_FirebaseAnalytics_setDefaultEventParameters(parametersAsJson);
        }

        public void SetConsent(Dictionary<FirebaseAnalyticsConsentType, FirebaseAnalyticsConsentValue> consent)
        {
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            using (DictionaryPool<string, string>.Get(out var cache))
            {
                foreach (var kv in consent)
                {
                    var consentType = kv.Key;
                    var key = consentType switch
                    {
                        FirebaseAnalyticsConsentType.AdPersonalization => "ad_personalization",
                        FirebaseAnalyticsConsentType.AdStorage => "ad_storage",
                        FirebaseAnalyticsConsentType.AdUserData => "ad_user_data",
                        FirebaseAnalyticsConsentType.AnalyticsStorage => "analytics_storage",
                        FirebaseAnalyticsConsentType.FunctionalityStorage => "functionality_storage",
                        FirebaseAnalyticsConsentType.PersonalizationStorage => "personalization_storage",
                        FirebaseAnalyticsConsentType.SecurityStorage => "security_storage",
                        _ => throw new Exception($"unsupported consent type {consentType}")
                    };

                    var consentValue = kv.Value;
                    var value = consentValue switch
                    {
                        FirebaseAnalyticsConsentValue.Granted => "granted",
                        FirebaseAnalyticsConsentValue.Denied => "denied",
                        _ => throw new Exception($"unsupported consent value {consentValue}")
                    };
                    cache[key] = value;
                }

                var consentAsJson = JsonConvert.SerializeObject(cache);
                FirebaseWebGL_FirebaseAnalytics_setConsent(consentAsJson);
            }
        }

        public void LogEvent(string eventName)
        {
            LogEvent(eventName, eventParams: null);
        }

        public void LogEvent(string eventName, Dictionary<string, object> eventParams)
        { 
            if (!isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            string eventParamsAsJson = eventParams != null ? JsonConvert.SerializeObject(eventParams) : null;
            FirebaseWebGL_FirebaseAnalytics_logEvent(eventName, eventParamsAsJson);
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
    }
}
