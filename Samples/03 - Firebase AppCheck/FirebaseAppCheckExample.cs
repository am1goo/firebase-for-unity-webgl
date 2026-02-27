using System.Collections;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseAppCheckExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseAppCheck _appCheck;

        private string _appCheckLimitedUseToken;
        private string _appCheckToken;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _appCheck = app.AppCheck;
            if (_appCheck == null)
            {
                Debug.LogError($"Start: {nameof(IFirebaseAppCheck)} is not injected");
                yield break;
            }

            bool? initialized = null;
            _appCheck.Initialize((callback) =>
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

            _appCheck.SetTokenAutoRefreshEnabled(false);
            _appCheck.SetTokenAutoRefreshEnabled(true);

            _appCheck.OnTokenChanged(OnTokenChanged);
            _appCheck.OnTokenChanged(null);
            _appCheck.OnTokenChanged(OnTokenChanged);

            void OnTokenChanged(string token)
            {
                Debug.Log($"OnTokenChanged: {token}");
            }
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_appCheck == null)
                return;

            GUILayout.Label("AppCheck:");
            GUILayout.Label($"- initialized: {_appCheck.isInitialized}");
            if (_appCheck.isInitialized)
            {
                GUILayout.Label($"- limited use token: {_appCheckLimitedUseToken}");
                if (GUILayout.Button("Get Limited Use Token"))
                {
                    _appCheck.GetLimitedUseToken((callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"GetLimitedUseToken: {callback.error}");
                            return;
                        }
                        _appCheckLimitedUseToken = callback.result;
                    });
                }
                GUILayout.Label($"- token: {_appCheckToken}");
                if (GUILayout.Button("Get Token"))
                {
                    _appCheck.GetToken(forceRefresh: true, (callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"GetToken: {callback.error}");
                            return;
                        }
                        _appCheckToken = callback.result;
                    });
                }
            }
        }
#endif
    }
}
