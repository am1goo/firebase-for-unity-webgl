using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    public class FirebaseApp : IFirebaseApp, IDisposable
    {
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseApp_initalize();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseApp_deleteApp();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseApp_setLogLevel(int logLevel);

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }

        private bool _isDisposed;
        public bool isDisposed => _isDisposed;

        private IFirebaseAnalytics _analytics;
        public IFirebaseAnalytics Analytics => _analytics;

        private IFirebaseAppCheck _appCheck;
        public IFirebaseAppCheck AppCheck => _appCheck;

        private IFirebaseMessaging _messaging;
        public IFirebaseMessaging Messaging => _messaging;

        private IFirebaseRemoteConfig _remoteConfig;
        public IFirebaseRemoteConfig RemoteConfig => _remoteConfig;

        private IFirebaseInstallations _installations;
        public IFirebaseInstallations Installations => _installations;

        private IFirebasePerformance _performance;
        public IFirebasePerformance Performance => _performance;

        private IFirebaseStorage _storage;
        public IFirebaseStorage Storage => _storage;

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
            if (Application.isEditor)
            {
                _isInitialized = false;
                onInitialized?.Invoke(_isInitialized);
                return;
            }

            _isInitialized = FirebaseWebGL_FirebaseApp_initalize();
            onInitialized?.Invoke(_isInitialized);

            if (!_isInitialized)
            {
                Debug.LogError("FirebaseApp is not initialized, modules registration no needed anymore.");
                return;
            }

            if (settings.includeAuth)
            {
                //TODO: add FirebaseAuth initialization here
            }
            if (settings.includeAnalytics)
            {
                _analytics = new FirebaseAnalytics();
            }
            if (settings.includeAppCheck)
            {
                _appCheck = new FirebaseAppCheck();
            }
            if (settings.includeFirestore)
            {
                //TODO: add FirebaseFirestore initialization here
            }
            if (settings.includeMessaging)
            {
                var enableServiceWorker = settings.includeMessagingSettings.enableServiceWorker;
                _messaging = new FirebaseMessaging(enableServiceWorker);
            }
            if (settings.includeRemoteConfig)
            {
                _remoteConfig = new FirebaseRemoteConfig();
            }
            if (settings.includeInstallations)
            {
                _installations = new FirebaseInstallations();
            }
            if (settings.includePerformance)
            {
                _performance = new FirebasePerformance();
            }
            if (settings.includeStorage)
            {
                _storage = new FirebaseStorage();
            }
        }

        public void SetLogLevel(FirebaseAppLogLevel logLevel)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            FirebaseWebGL_FirebaseApp_setLogLevel((int)logLevel);
        }
    }
}
