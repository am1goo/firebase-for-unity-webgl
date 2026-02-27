using System.Collections;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal abstract class BaseExample : MonoBehaviour
    {
#if UNITY_WEBGL
        private FirebaseApp _app;
        protected FirebaseApp app => _app;

        protected virtual void Awake()
        {
            _app = FirebaseApp.DefaultInstance();
            if (!_app.isInitialized)
            {
                Debug.LogError($"Start: Firebase SDK is not initialized is not initialized. Be sure that you setup {nameof(FirebaseSettings)}.asset file in {nameof(Resources)} folder.");
                Destroy(gameObject);
                return;
            }

            _app.SetLogLevel(FirebaseAppLogLevel.Debug);
        }

        protected virtual IEnumerator Start()
        {
            yield break;
        }

        private Vector2 _scrollPosition;

        private void OnGUI()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            OnDrawGUI();
            GUILayout.EndScrollView();
        }

        protected virtual void OnDrawGUI()
        {
            //do nothing
        }
#endif
    }
}
