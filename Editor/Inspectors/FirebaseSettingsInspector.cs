using System;
using UnityEditor;
using UnityEngine;

namespace FirebaseWebGL.Editor
{
    [CustomEditor(typeof(FirebaseSettings))]
    public sealed class FirebaseSettingsInspector : UnityEditor.Editor
    {
        private SerializedProperty _includeAuth;
        private SerializedProperty _includeAnalytics;
        private SerializedProperty _includeAppCheck;
        private SerializedProperty _includeAppCheckSettings;
        private SerializedProperty _includeFunctions;
        private SerializedProperty _includeFunctionsSettings;
        private SerializedProperty _includeMessaging;
        private SerializedProperty _includeMessagingSettings;
        private SerializedProperty _includeRemoteConfig;
        private SerializedProperty _includeInstallations;
        private SerializedProperty _includePerformance;
        private SerializedProperty _includeStorage;
        private SerializedProperty _includeStorageSettings;

        private void OnEnable()
        {
            _includeAuth = serializedObject.FindProperty(nameof(_includeAuth));
            _includeAnalytics = serializedObject.FindProperty(nameof(_includeAnalytics));
            _includeAppCheck = serializedObject.FindProperty(nameof(_includeAppCheck));
            _includeAppCheckSettings = serializedObject.FindProperty(nameof(_includeAppCheckSettings));
            _includeFunctions = serializedObject.FindProperty(nameof(_includeFunctions));
            _includeFunctionsSettings = serializedObject.FindProperty(nameof(_includeFunctionsSettings));
            _includeMessaging = serializedObject.FindProperty(nameof(_includeMessaging));
            _includeMessagingSettings = serializedObject.FindProperty(nameof(_includeMessagingSettings));
            _includeRemoteConfig = serializedObject.FindProperty(nameof(_includeRemoteConfig));
            _includeInstallations = serializedObject.FindProperty(nameof(_includeInstallations));
            _includePerformance = serializedObject.FindProperty(nameof(_includePerformance));
            _includeStorage = serializedObject.FindProperty(nameof(_includeStorage));
            _includeStorageSettings = serializedObject.FindProperty(nameof(_includeStorageSettings));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(_includeAuth);
            EditorGUILayout.PropertyField(_includeAnalytics);
            EditorGUILayout.PropertyField(_includeAppCheck);
            if (_includeAppCheck.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_includeAppCheckSettings, includeChildren: true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(_includeFunctions);
            if (_includeFunctions.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_includeFunctionsSettings, includeChildren: true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(_includeMessaging);
            if (_includeMessaging.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_includeMessagingSettings, includeChildren: true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(_includeRemoteConfig);
            EditorGUILayout.PropertyField(_includeInstallations);
            EditorGUILayout.PropertyField(_includePerformance);
            EditorGUILayout.PropertyField(_includeStorage);
            if (_includeStorage.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_includeStorageSettings, includeChildren: true);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        [CustomPropertyDrawer(typeof(FirebaseSettings.AppCheckSettings))]
        sealed class AppCheckSettingsDrawer : PropertyDrawer
        {
            private static readonly FirebaseSettings.AppCheckSettings.ProviderType[] providerTypes = (FirebaseSettings.AppCheckSettings.ProviderType[])Enum.GetValues(typeof(FirebaseSettings.AppCheckSettings.ProviderType));

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var height = 0.0f;
                
                var providerType = property.FindPropertyRelative("_providerType");
                var providerTypeValue = providerTypes[providerType.enumValueIndex];
                height += EditorGUI.GetPropertyHeight(providerType);

                switch (providerTypeValue)
                {
                    case FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaV3:
                        var reCaptchaV3PublicKey = property.FindPropertyRelative("_reCaptchaV3PublicKey");
                        height += EditorGUI.GetPropertyHeight(reCaptchaV3PublicKey);
                        break;

                    case FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaEnterprise:
                        var reCaptchaEnterprisePublicKey = property.FindPropertyRelative("_reCaptchaEnterprisePublicKey");
                        height += EditorGUI.GetPropertyHeight(reCaptchaEnterprisePublicKey);
                        break;
                }

                var isTokenAutoRefreshEnabled = property.FindPropertyRelative("_isTokenAutoRefreshEnabled");
                height += EditorGUI.GetPropertyHeight(isTokenAutoRefreshEnabled);

                return height;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var r = position;

                var providerType = property.FindPropertyRelative("_providerType");
                var providerTypeValue = providerTypes[providerType.enumValueIndex];
                r.height = EditorGUI.GetPropertyHeight(providerType);
                EditorGUI.PropertyField(r, providerType);
                r.y += r.height;

                switch (providerTypeValue)
                {
                    case FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaV3:
                        var reCaptchaV3PublicKey = property.FindPropertyRelative("_reCaptchaV3PublicKey");
                        r.height = EditorGUI.GetPropertyHeight(reCaptchaV3PublicKey);
                        EditorGUI.PropertyField(r, reCaptchaV3PublicKey);
                        r.y += r.height;
                        break;

                    case FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaEnterprise:
                        var reCaptchaEnterprisePublicKey = property.FindPropertyRelative("_reCaptchaEnterprisePublicKey");
                        r.height = EditorGUI.GetPropertyHeight(reCaptchaEnterprisePublicKey);
                        EditorGUI.PropertyField(r, reCaptchaEnterprisePublicKey);
                        r.y += r.height;
                        break;
                }

                var isTokenAutoRefreshEnabled = property.FindPropertyRelative("_isTokenAutoRefreshEnabled");
                r.height = EditorGUI.GetPropertyHeight(isTokenAutoRefreshEnabled);
                EditorGUI.PropertyField(r, isTokenAutoRefreshEnabled);
                r.y += r.height;
            }
        }

        [CustomPropertyDrawer(typeof(FirebaseSettings.FunctionsSettings))]
        sealed class FunctionsSettingsDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var height = 0.0f;

                var regionOrCustomDomain = property.FindPropertyRelative("_regionOnCustomDomain");
                height += EditorGUI.GetPropertyHeight(regionOrCustomDomain);

                return height;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var r = position;

                var regionOrCustomDomain = property.FindPropertyRelative("_regionOnCustomDomain");
                r.height = EditorGUI.GetPropertyHeight(regionOrCustomDomain);
                EditorGUI.PropertyField(r, regionOrCustomDomain);
                r.y += r.height;
            }
        }

        [CustomPropertyDrawer(typeof(FirebaseSettings.MessagingSettings))]
        sealed class MessagingSettingsDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var height = 0.0f;

                var enableServiceWorker = property.FindPropertyRelative("_enableServiceWorker");
                height += EditorGUI.GetPropertyHeight(enableServiceWorker);

                return height;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var r = position;

                var enableServiceWorker = property.FindPropertyRelative("_enableServiceWorker");
                r.height = EditorGUI.GetPropertyHeight(enableServiceWorker);
                EditorGUI.PropertyField(r, enableServiceWorker);
                r.y += r.height;
            }
        }

        [CustomPropertyDrawer(typeof(FirebaseSettings.StorageSettings))]
        sealed class StorageSettingsDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var height = 0.0f;

                var bucketUrl = property.FindPropertyRelative("_bucketUrl");
                height += EditorGUI.GetPropertyHeight(bucketUrl);

                return height;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var r = position;

                var bucketUrl = property.FindPropertyRelative("_bucketUrl");
                r.height = EditorGUI.GetPropertyHeight(bucketUrl);
                EditorGUI.PropertyField(r, bucketUrl);
                r.y += r.height;
            }
        }
    }
}
