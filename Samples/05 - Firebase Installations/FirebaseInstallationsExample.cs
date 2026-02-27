using System.Collections;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseInstallationsExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseInstallations _installations;

        private string _installationId;
        private string _installationToken;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _installations = app.Installations;
            if (_installations == null)
            {
                Debug.LogError($"Start: {nameof(IFirebaseInstallations)} is not injected");
                yield break;
            }

            bool? initialized = null;
            _installations.Initialize((callback) =>
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

            _installations.OnIdChange(OnIdChange);

            void OnIdChange(string newId)
            {
                Debug.Log($"OnIdChange: newId={newId}");
            }
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_installations == null)
                return;

            GUILayout.Label("Installations:");
            GUILayout.Label($"- initialized: {_installations.isInitialized}");
            if (_installations.isInitialized)
            {
                GUILayout.Label($"- id: {_installationId}");
                if (GUILayout.Button("Get Id"))
                {
                    _installations.GetId((callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"GetId: {callback.error}");
                            return;
                        }
                        _installationId = callback.result;
                    });
                }
                GUILayout.Label($"- token: {_installationToken}");
                if (GUILayout.Button("Get Token"))
                {
                    _installations.GetToken(forceRefresh: true, (callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"GetToken: {callback.error}");
                            return;
                        }
                        _installationToken = callback.result;
                    });
                }

                if (GUILayout.Button("Delete Installations"))
                {
                    _installations.DeleteInstallations((callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"DeleteInstallations: {callback.error}");
                            return;
                        }

                        if (callback.result)
                        {
                            _installationId = null;
                            _installationToken = null;
                        }
                        Debug.Log($"DeleteInstallations: result={callback.result}");
                    });
                }
            }
        }
#endif
    }
}
