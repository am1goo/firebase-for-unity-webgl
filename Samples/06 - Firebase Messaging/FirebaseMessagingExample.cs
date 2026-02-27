using System.Collections;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseMessagingExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseMessaging _messaging;

        private string _messagingToken;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _messaging = app.Messaging;
            if (_messaging == null)
            {
                Debug.LogError($"Start: {nameof(IFirebaseMessaging)} is not injected");
                yield break;
            }

            bool? initialized = null;
            _messaging.Initialize((callback) =>
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

            _messaging.OnMessage(OnMessageReceived);
            _messaging.OnMessage(OnMessageReceived);

            if (_messaging.ServiceWorker != null)
            {
                bool? swInitialized = null;
                _messaging.ServiceWorker.Initialize((callback) =>
                {
                    if (callback.success == false)
                    {
                        Debug.LogError($"Initialize: {callback.error}");
                        return;
                    }
                    swInitialized = callback.result;
                });
                yield return new WaitUntil(() => swInitialized != null);
                if (swInitialized.Value == false)
                {
                    Debug.LogError("Initialize: not initialized");
                    yield break;
                }

                _messaging.ServiceWorker.ExperimentalSetDeliveryMetricsExportedToBigQueryEnabled(true);
                _messaging.ServiceWorker.ExperimentalSetDeliveryMetricsExportedToBigQueryEnabled(false);
            }

            void OnMessageReceived(string message)
            {
                Debug.Log($"OnMessage: {message}");
            }
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_messaging == null)
                return;

            GUILayout.Label("Messaging:");
            GUILayout.Label($"- initialized: {_messaging.isInitialized}");
            if (_messaging.isInitialized)
            {
                GUILayout.Label($"- token: {_messagingToken}");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Get Token"))
                {
                    _messaging.GetToken((callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"GetToken: {callback.error}");
                            return;
                        }
                        _messagingToken = callback.result;
                    });
                }
                if (GUILayout.Button("Delete Token"))
                {
                    _messaging.DeleteToken((callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"DeleteToken: {callback.error}");
                            return;
                        }

                        if (callback.result == false)
                        {
                            Debug.LogError($"DeleteToken: token isn't deleted");
                            return;
                        }

                        _messagingToken = null;
                    });
                }
                GUILayout.EndHorizontal();
            }
        }
#endif
    }
}
