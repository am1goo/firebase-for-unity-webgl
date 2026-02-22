using FirebaseWebGL;
using UnityEditor;

[CustomEditor(typeof(FirebaseSettings))]
public sealed class FirebaseSettingsInspector : Editor
{
    private SerializedProperty _includeAuth;
    private SerializedProperty _includeAnalytics;
    private SerializedProperty _includeFirestore;
    private SerializedProperty _includeMessaging;
    private SerializedProperty _includeMessagingServiceWorker;
    private SerializedProperty _includeRemoteConfig;
    private SerializedProperty _includeInstallations;
    private SerializedProperty _includePerformance;

    private void OnEnable()
    {
        _includeAuth = serializedObject.FindProperty(nameof(_includeAuth));
        _includeAnalytics = serializedObject.FindProperty(nameof(_includeAnalytics));
        _includeFirestore = serializedObject.FindProperty(nameof(_includeFirestore));
        _includeMessaging = serializedObject.FindProperty(nameof(_includeMessaging));
        _includeMessagingServiceWorker = serializedObject.FindProperty(nameof(_includeMessagingServiceWorker));
        _includeRemoteConfig = serializedObject.FindProperty(nameof(_includeRemoteConfig));
        _includeInstallations = serializedObject.FindProperty(nameof(_includeInstallations));
        _includePerformance = serializedObject.FindProperty(nameof(_includePerformance));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(_includeAuth);
        EditorGUILayout.PropertyField(_includeAnalytics);
        EditorGUILayout.PropertyField(_includeFirestore);
        EditorGUILayout.PropertyField(_includeMessaging);
        if (_includeMessaging.boolValue)
        {
            EditorGUILayout.PropertyField(_includeMessagingServiceWorker);
        }
        EditorGUILayout.PropertyField(_includeRemoteConfig);
        EditorGUILayout.PropertyField(_includeInstallations);
        EditorGUILayout.PropertyField(_includePerformance);

        serializedObject.ApplyModifiedProperties();
    }
}
