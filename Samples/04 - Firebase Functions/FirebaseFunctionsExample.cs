using System;
using System.Collections;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseFunctionsExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseFunctions _functions;

        private bool _emulatorConnected;
        private string _emulatorHost = "127.0.0.1";
        private int _emulatorPort = 5001;

        private string _log = string.Empty;
        private int _logCounter = 0;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _functions = app.Functions;
            if (_functions == null)
            {
                Debug.LogError($"Start: {nameof(IFirebaseFunctions)} is not injected");
                yield break;
            }

            bool? initialized = null;
            _functions.Initialize((callback) =>
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
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_functions == null)
                return;

            GUILayout.Label("Functions:");
            GUILayout.Label($"- initialized: {_functions.isInitialized}");

            if (_functions.isInitialized)
            {
                if (!_emulatorConnected)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Emulator Host:");
                    _emulatorHost = GUILayout.TextField(_emulatorHost);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Emulator Port:");
                    var emulatorPortString = GUILayout.TextField(_emulatorPort.ToString());
                    if (int.TryParse(emulatorPortString, out var emulatorPort))
                    {
                        _emulatorPort = emulatorPort;
                    }
                    GUILayout.EndHorizontal();

                    var prevEnabled = GUI.enabled;
                    GUI.enabled = !string.IsNullOrWhiteSpace(_emulatorHost) && _emulatorPort > 0;
                    if (GUILayout.Button("Connect to Emulator"))
                    {
                        try
                        {
                            _functions.ConnectFunctionsEmulator(_emulatorHost, _emulatorPort);
                            _emulatorConnected = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            _emulatorConnected = false;
                        }
                    }
                    GUI.enabled = prevEnabled;
                }
                else
                {
                    if (GUILayout.Button("Invoke \"Hello World\" function"))
                    {
                        var options = new FirebaseFunctionsHttpsCallableOptions { limitedUseAppCheckTokens = false };
                        var callable = _functions.HttpsCallable("helloWorld", options);
                        callable.Request<string>((callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"Request ({nameof(callable)}): error={callback.error}");
                                return;
                            }

                            _log += $"{(_logCounter + 1)}: {callback.result}";
                            _log += Environment.NewLine;
                            _logCounter++;
                        });
                    }
                    GUILayout.TextArea(_log);
                }
            }
        }
#endif
    }
}
