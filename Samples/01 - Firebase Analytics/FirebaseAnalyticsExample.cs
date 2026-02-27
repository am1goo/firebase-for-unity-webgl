using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseAnalyticsExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseAnalytics _analytics;

        private string _analyticsClientId;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _analytics = app.Analytics;
            if (_analytics == null)
            {
                Debug.LogError($"Start: {nameof(IFirebaseAnalytics)} is not injected");
                yield break;
            }

            bool? initialized = null;
            _analytics.Initialize((callback) =>
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

            _analytics.SetUserId("123");
            _analytics.SetUserProperties(new Dictionary<string, string>()
            {
                { "username", "test_player" },
            });
            _analytics.SetAnalyticsCollectionEnabled(true);
            _analytics.SetDefaultEventParameters(new Dictionary<string, string>()
            {
                { "platform", Application.platform.ToString() },
                { "version", Application.version },
                { "unity_version", Application.unityVersion },
            });
            _analytics.SetConsent(new Dictionary<FirebaseAnalyticsConsentType, FirebaseAnalyticsConsentValue>
            {
                { FirebaseAnalyticsConsentType.AdPersonalization, FirebaseAnalyticsConsentValue.Granted },
                { FirebaseAnalyticsConsentType.AdStorage, FirebaseAnalyticsConsentValue.Denied },
                { FirebaseAnalyticsConsentType.AdUserData, FirebaseAnalyticsConsentValue.Granted },
                { FirebaseAnalyticsConsentType.AnalyticsStorage, FirebaseAnalyticsConsentValue.Denied },
                { FirebaseAnalyticsConsentType.FunctionalityStorage,FirebaseAnalyticsConsentValue.Granted },
                { FirebaseAnalyticsConsentType.PersonalizationStorage, FirebaseAnalyticsConsentValue.Denied },
                { FirebaseAnalyticsConsentType.SecurityStorage, FirebaseAnalyticsConsentValue.Granted },
            });
            _analytics.LogEvent("test_event_no_params");
            _analytics.LogEvent("test_event_1_params", new Dictionary<string, object> { { "param1", "value1" } });
            _analytics.LogEvent("test_event_2_params", new Dictionary<string, object> { { "param1", "value1" }, { "param2", "value2" } });
            _analytics.LogEvent("test_event_2_params", new Dictionary<string, object> { { "param1", "value1" }, { "param2", "value2" }, { "param3", 3 } });
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_analytics == null)
                return;

            GUILayout.Label("Analytics:");
            GUILayout.Label($"- initialized: {_analytics.isInitialized}");
            if (_analytics.isInitialized)
            {
                GUILayout.Label($"- clientId: {_analyticsClientId}");
                if (GUILayout.Button("Get Client Id"))
                {
                    _analytics.GetGoogleAnalyticsClientId((callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"GetGoogleAnalyticsClientId: {callback.error}");
                            return;
                        }
                        _analyticsClientId = callback.result;
                    });
                }
            }
        }
#endif
    }
}
