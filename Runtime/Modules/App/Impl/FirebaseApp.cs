using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    public class FirebaseApp : IFirebaseApp, IDisposable
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseApp_initalize();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseApp_deleteApp();

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }

        private bool _isDisposed;
        public bool isDisposed => _isDisposed;

        private IFirebaseAnalytics _analytics;
        public IFirebaseAnalytics Analytics => _analytics;

        private IFirebaseMessaging _messaging;
        public IFirebaseMessaging Messaging => _messaging;

        ~FirebaseApp()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            OnDispose(disposing);
            _isDisposed = true;
        }

        private void OnDispose(bool disposing)
        {
            if (Application.isEditor)
                return;

            try
            {
                FirebaseWebGL_FirebaseApp_deleteApp();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static FirebaseApp DefaultInstance()
        {
            var settings = FirebaseSettings.instance;
            if (settings == null)
                throw new Exception($"{nameof(FirebaseSettings)} file is not found in {nameof(Resources)} folder");

            return new FirebaseApp(settings);
        }

        public FirebaseApp(FirebaseSettings settings)
        { 
            if (settings.includeAuth)
            {
                //TODO: add FirebaseAuth initialization here
            }
            if (settings.includeAnalytics)
            {
                _analytics = new FirebaseAnalytics();
            }
            if (settings.includeFirestore)
            {
                //TODO: add FirebaseFirestore initialization here
            }
            if (settings.includeMessaging)
            {
                _messaging = new FirebaseMessaging(settings.includeMessagingServiceWorker);
            }
            if (settings.includeRemoteConfig)
            {
                //TODO: add FirebaseRemoteConfig initialization here
            }

            if (Application.isEditor)
            {
                _isInitialized = false;
                onInitialized?.Invoke(_isInitialized);
                return;
            }

            try
            {
                FirebaseWebGL_FirebaseApp_initalize();
                _isInitialized = true;
                onInitialized?.Invoke(_isInitialized);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                _isInitialized = false;
                onInitialized?.Invoke(_isInitialized);
            }
        }
    }
}
