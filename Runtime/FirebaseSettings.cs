using System;
using UnityEngine;

namespace FirebaseWebGL
{
    [CreateAssetMenu(fileName = nameof(FirebaseSettings), menuName = "Firebase/WebGL Settings")]
    public class FirebaseSettings : ScriptableObject
    {
        [Header("General")]
        [SerializeField]
        private string _apiKey;
        public string apiKey => _apiKey;
        [SerializeField]
        private string _authDomain;
        public string authDomain => _authDomain;
        [SerializeField]
        private string _projectId;
        public string projectId => _projectId;
        [SerializeField]
        private string _storageBucket;
        public string storageBucket => _storageBucket;
        [SerializeField]
        private string _messagingSenderId;
        public string messagingSenderId => _messagingSenderId;
        [SerializeField]
        private string _appId;
        public string appId => _appId;
        [SerializeField]
        private string _measurementId;
        public string measurementId => _measurementId;
        [Header("Options")]
        //hidden fields
        [SerializeField, HideInInspector]
        private bool _includeAuth;
        public bool includeAuth => _includeAuth;
        [SerializeField, HideInInspector]
        private bool _includeAnalytics;
        public bool includeAnalytics => _includeAnalytics;
        [SerializeField, HideInInspector]
        private bool _includeAppCheck;
        public bool includeAppCheck => _includeAppCheck;
        [SerializeField, HideInInspector]
        private AppCheckSettings _includeAppCheckSettings;
        public AppCheckSettings includeAppCheckSettings => _includeAppCheckSettings;
        [SerializeField, HideInInspector]
        private bool _includeFirestore;
        public bool includeFirestore => _includeFirestore;
        [SerializeField, HideInInspector]
        private bool _includeMessaging;
        public bool includeMessaging => _includeMessaging;
        [SerializeField, HideInInspector]
        private MessagingSettings _includeMessagingSettings;
        public MessagingSettings includeMessagingSettings => _includeMessagingSettings;
        [SerializeField, HideInInspector]
        private bool _includeRemoteConfig;
        public bool includeRemoteConfig => _includeRemoteConfig;
        [SerializeField, HideInInspector]
        private bool _includeInstallations;
        public bool includeInstallations => _includeInstallations;
        [SerializeField, HideInInspector]
        private bool _includePerformance;
        public bool includePerformance => _includePerformance;
        [SerializeField, HideInInspector]
        private bool _includeStorage;
        public bool includeStorage => _includeStorage;
        [SerializeField, HideInInspector]
        private StorageSettings _includeStorageSettings;
        public StorageSettings includeStorageSettings => _includeStorageSettings;

        private static FirebaseSettings _instance;
        public static FirebaseSettings instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<FirebaseSettings>(nameof(FirebaseSettings));
                }
                return _instance;
            }
        }

        [Serializable]
        public sealed class AppCheckSettings
        {
            [SerializeField]
            private ProviderType _providerType;
            public ProviderType providerType => _providerType;
            [SerializeField]
            private string _reCaptchaV3PublicKey;
            public string reCaptchaV3PublicKey => _reCaptchaV3PublicKey;
            [SerializeField]
            private string _reCaptchaEnterprisePublicKey;
            public string reCaptchaEnterprisePublicKey => _reCaptchaEnterprisePublicKey;
            [SerializeField]
            private bool _isTokenAutoRefreshEnabled;
            public bool isTokenAutoRefreshEnabled => _isTokenAutoRefreshEnabled;

            public enum ProviderType : byte
            {
                ReCaptchaV3 = 0,
                ReCaptchaEnterprise = 1,
            }
        }

        [Serializable]
        public sealed class MessagingSettings
        {
            [SerializeField]
            private bool _enableServiceWorker;
            public bool enableServiceWorker => _enableServiceWorker;
        }

        [Serializable]
        public sealed class StorageSettings
        {
            [SerializeField]
            private string _bucketUrl;
            public string bucketUrl => _bucketUrl;
        }
    }
}
