using System.Collections;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseFunctionsExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseFunctions _functions;

        private string _appCheckLimitedUseToken;
        private string _appCheckToken;

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

            _functions.ConnectFunctionsEmulator("localhost", 5001);
            var options = new FirebaseFunctionsHttpsCallableOptions { limitedUseAppCheckTokens = false };
            var callable = _functions.HttpsCallable("helloWorld", options);
            callable.Request<string>((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"Request ({nameof(callable)}): error={callback.error}");
                    return;
                }

                Debug.Log($"Request ({nameof(callable)}): {callback.result}");
            });
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_functions == null)
                return;

            GUILayout.Label("Functions:");
            GUILayout.Label($"- initialized: {_functions.isInitialized}");
        }
#endif
    }
}
