#if UNITY_WEBGL
using HtmlAgilityPack;
using System;
using System.IO;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FirebaseWebGL
{
    public class FirebaseWebGLPostProcessBuildWithReport : IPostprocessBuildWithReport
    {
        public int callbackOrder => 999;

        private const string indexFilename = "index.html";
        private const string remarkValue = "injected-by-firebase-for-webgl-plugin";
        private static readonly Encoding utf8 = new UTF8Encoding(false);

        public void OnPostprocessBuild(BuildReport report)
        {
            InjectFirebaseScripts(report.summary.outputPath);
        }

        private static void InjectFirebaseScripts(string outputPath)
        {
            var fi = new FileInfo(Path.Combine(outputPath, indexFilename));
            if (fi.Exists == false)
                throw new Exception($"{indexFilename} is not found in output folder");

            var settings = FirebaseSettings.instance;
            if (settings == null)
                throw new Exception($"{nameof(FirebaseSettings)} file is not found in {nameof(Resources)} folder");

            var doc = new HtmlDocument();
            using (var fs = fi.OpenRead())
            {
                doc.Load(fs, utf8, false);
            }

            var html = doc.DocumentNode.SelectSingleNode("html");
            var body = html.SelectSingleNode("body");
            var scripts = body.SelectNodes("script");

            foreach (var script in scripts)
            {
                var remark = script.GetAttributeValue("remark", null);
                if (remark == null || remark != remarkValue)
                    continue;

                body.RemoveChild(script);
                break;
            }

            if (string.IsNullOrEmpty(settings.apiKey))
                throw new Exception($"{nameof(settings.apiKey)} is not defined");

            if (string.IsNullOrEmpty(settings.authDomain))
                throw new Exception($"{nameof(settings.authDomain)} is not defined");

            if (string.IsNullOrEmpty(settings.projectId))
                throw new Exception($"{nameof(settings.projectId)} is not defined");

            if (string.IsNullOrEmpty(settings.storageBucket))
                throw new Exception($"{nameof(settings.storageBucket)} is not defined");

            if (string.IsNullOrEmpty(settings.messagingSenderId))
                throw new Exception($"{nameof(settings.messagingSenderId)} is not defined");

            if (string.IsNullOrEmpty(settings.appId))
                throw new Exception($"{nameof(settings.appId)} is not defined");

            if (string.IsNullOrEmpty(settings.measurementId))
                throw new Exception($"{nameof(settings.measurementId)} is not defined");

            var textToInject = HtmlNode.CreateNode(GenerateText(settings));

            var nodeToInject = new HtmlNode(HtmlNodeType.Element, doc, 0);
            nodeToInject.Name = "script";
            nodeToInject.Attributes.Append("type", "module");
            nodeToInject.Attributes.Append("remark", remarkValue);
            nodeToInject.AppendChild(textToInject);
            body.AppendChild(nodeToInject);

            using (var fs = fi.OpenWrite())
            {
                fs.SetLength(0);
                fs.Seek(0, SeekOrigin.Begin);
                doc.Save(fs, utf8);
            }

            var fiServiceWorker = new FileInfo(Path.Combine(outputPath, "firebase-messaging-sw.js"));
            if (fiServiceWorker.Exists)
                fiServiceWorker.Delete();

            if (settings.includeMessaging)
            {
                using (var fs = fiServiceWorker.OpenWrite())
                {
                    //TODO: if you need support of onBackgroundMessage callback, feel free to create feature request if you need it
                    //https://github.com/firebase/quickstart-js/blob/master/messaging/firebase-messaging-sw.js
                    fs.Flush();
                }
            }
        }

        private static string GenerateText(FirebaseSettings settings)
        {
            var indent = "  ";

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(indent).AppendLine("// Import the functions you need from the SDKs you need");
            sb.Append(indent).AppendLine("import { initializeApp, setLogLevel } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-app.js\";");
            if (settings.includeAuth)
            {
                sb.Append(indent).AppendLine("import { getAuth } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-auth.js\";");
            }
            if (settings.includeAnalytics)
            {
                sb.Append(indent).AppendLine("import { getAnalytics, isSupported as isSupportedAnalytics, getGoogleAnalyticsClientId, logEvent, setAnalyticsCollectionEnabled, setConsent, setDefaultEventParameters, setUserId, setUserProperties } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-analytics.js\";");
            }
            if (settings.includeAppCheck)
            {
                sb.Append(indent).AppendLine("import { initializeAppCheck, getLimitedUseToken as getLimitedUseTokenAppCheck, getToken as getTokenAppCheck, onTokenChanged as onTokenChangedAppCheck, setTokenAutoRefreshEnabled as setTokenAutoRefreshEnabledAppCheck, CustomProvider, ReCaptchaEnterpriseProvider, ReCaptchaV3Provider } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-app-check.js\";");
            }
            if (settings.includeFirestore)
            {
                sb.Append(indent).AppendLine("import { getFirestore } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-firestore.js\";");
            }
            if (settings.includeMessaging)
            {
                sb.Append(indent).AppendLine("import { getMessaging, isSupported as isSupportedMessaging, getToken as getTokenMessaging, deleteToken, onMessage } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging.js\";");
                sb.Append(indent).AppendLine("import { getMessaging as getMessagingInSw, isSupported as isSupportedMessagingInSw, experimentalSetDeliveryMetricsExportedToBigQueryEnabled } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging-sw.js\";");
            }
            if (settings.includeRemoteConfig)
            {
                sb.Append(indent).AppendLine("import { getRemoteConfig, isSupported as isSupportedRemoteConfig, activate, ensureInitialized, fetchAndActivate, fetchConfig, getAll, getBoolean, getNumber, getString, getValue, onConfigUpdate, setCustomSignals, setLogLevel as setLogLevelRemoteConfig } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-remote-config.js\";");
            }
            if (settings.includeInstallations)
            {
                sb.Append(indent).AppendLine("import { getInstallations, deleteInstallations, getId as getIdInstallations, getToken as getTokenInstallations, onIdChange } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-installations.js\";");
            }
            if (settings.includePerformance)
            {
                sb.Append(indent).AppendLine("import { getPerformance, trace } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-performance.js\";");
            }
            if (settings.includeStorage)
            {
                sb.Append(indent).AppendLine("import { getStorage, connectStorageEmulator, deleteObject, getBlob, getBytes, getDownloadURL, getMetadata, getStream, list, listAll, ref, updateMetadata, uploadBytes, uploadBytesResumable, uploadString } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-storage.js\";");
            }
            sb.AppendLine();
            sb.Append(indent).AppendLine("const firebaseConfig = {");
            sb.Append(indent).Append(indent).AppendLine($"apiKey: \"{settings.apiKey}\",");
            sb.Append(indent).Append(indent).AppendLine($"authDomain: \"{settings.authDomain}\",");
            sb.Append(indent).Append(indent).AppendLine($"projectId: \"{settings.projectId}\",");
            sb.Append(indent).Append(indent).AppendLine($"storageBucket: \"{settings.storageBucket}\",");
            sb.Append(indent).Append(indent).AppendLine($"messagingSenderId: \"{settings.messagingSenderId}\",");
            sb.Append(indent).Append(indent).AppendLine($"appId: \"{settings.appId}\",");
            sb.Append(indent).Append(indent).AppendLine($"measurementId: \"{settings.measurementId}\",");
            sb.Append(indent).AppendLine("};");
            sb.AppendLine();
            sb.Append(indent).AppendLine("// Initialize Firebase");
            sb.Append(indent).AppendLine("const firebaseSdk = {}");
            sb.Append(indent).AppendLine("const app = initializeApp(firebaseConfig);");
            sb.Append(indent).AppendLine("firebaseSdk.app = app;");
            sb.Append(indent).AppendLine("firebaseSdk.appApi = { setLogLevel };");
            if (settings.includeAuth)
            {
                sb.Append(indent).AppendLine("firebaseSdk.auth = getAuth(app);");
            }
            if (settings.includeAnalytics)
            {
                sb.Append(indent).AppendLine("firebaseSdk.analytics = getAnalytics(app);");
                sb.Append(indent).AppendLine("firebaseSdk.analyticsApi = { isSupported: isSupportedAnalytics, getGoogleAnalyticsClientId, logEvent, setAnalyticsCollectionEnabled, setConsent, setDefaultEventParameters, setUserId, setUserProperties };");
            }
            if (settings.includeAppCheck)
            {
                var provider = settings.includeAppCheckSettings.providerType switch
                {
                    FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaV3 => "new ReCaptchaV3Provider(\'" + settings.includeAppCheckSettings.reCaptchaV3PublicKey + "\')",
                    FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaEnterprise => "new ReCaptchaEnterpriseProvider(\'" + settings.includeAppCheckSettings.reCaptchaEnterprisePublicKey + "\')",
                    _ => throw new Exception($"unsupported provider type {settings.includeAppCheckSettings.providerType}"),
                };
                var reCaptchaV3PublicKey = settings.includeAppCheckSettings.reCaptchaV3PublicKey;
                var isTokenAutoRefreshEnabled = settings.includeAppCheckSettings.isTokenAutoRefreshEnabled;
                sb.Append(indent).AppendLine("const appCheckOptions = { provider: " + provider + ", isTokenAutoRefreshEnabled: " + (isTokenAutoRefreshEnabled ? 1 : 0) + " };");
                sb.Append(indent).AppendLine("firebaseSdk.appCheck = initializeAppCheck(app, appCheckOptions);");
                sb.Append(indent).AppendLine("firebaseSdk.appCheckApi = { getLimitedUseToken: getLimitedUseTokenAppCheck, getToken: getTokenAppCheck, onTokenChanged: onTokenChangedAppCheck, setTokenAutoRefreshEnabled: setTokenAutoRefreshEnabledAppCheck };");
            }
            if (settings.includeFirestore)
            {
                sb.Append(indent).AppendLine("firebaseSdk.firestore = getFirestore(app);");
            }
            if (settings.includeMessaging)
            {
                sb.Append(indent).AppendLine("firebaseSdk.messaging = getMessaging(app);");
                sb.Append(indent).AppendLine("firebaseSdk.messagingApi = { isSupported: isSupportedMessaging, getToken: getTokenMessaging, deleteToken, onMessage };");
                sb.Append(indent).AppendLine("firebaseSdk.messagingSw = getMessagingInSw(app);");
                sb.Append(indent).AppendLine("firebaseSdk.messagingSwApi = { isSupported: isSupportedMessagingInSw, experimentalSetDeliveryMetricsExportedToBigQueryEnabled };");
            }
            if (settings.includeRemoteConfig)
            {
                sb.Append(indent).AppendLine("firebaseSdk.remoteConfig = getRemoteConfig(app);");
                sb.Append(indent).AppendLine("firebaseSdk.remoteConfigApi = { isSupported: isSupportedRemoteConfig, activate, ensureInitialized, fetchAndActivate, fetchConfig, getAll, getBoolean, getNumber, getString, getValue, onConfigUpdate, setCustomSignals, setLogLevel: setLogLevelRemoteConfig };");
            }
            if (settings.includeInstallations)
            {
                sb.Append(indent).AppendLine("firebaseSdk.installations = getInstallations(app);");
                sb.Append(indent).AppendLine("firebaseSdk.installationsApi = { deleteInstallations, getId: getIdInstallations, getToken: getTokenInstallations, onIdChange };");
            }
            if (settings.includePerformance)
            {
                sb.Append(indent).AppendLine("firebaseSdk.performance = getPerformance(app);");
                sb.Append(indent).AppendLine("firebaseSdk.performanceApi = { trace };");
            }
            if (settings.includeStorage)
            {
                var bucketUrl = settings.includeStorageSettings.bucketUrl;
                if (!string.IsNullOrWhiteSpace(bucketUrl))
                {
                    var uri = default(Uri);
                    try
                    {
                        uri = new Uri(bucketUrl);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{nameof(bucketUrl)} cannot be parsed as uri", ex);
                    }

                    if (uri.Scheme != "gs")
                        throw new Exception($"{nameof(bucketUrl)} should starts with 'gs://' scheme");

                    sb.Append(indent).AppendLine("firebaseSdk.storage = getStorage(app, \'" + uri.ToString() + "\');");
                }
                else
                {
                    sb.Append(indent).AppendLine("firebaseSdk.storage = getStorage(app);");
                }
                sb.Append(indent).AppendLine("firebaseSdk.storageApi = { connectStorageEmulator, deleteObject, getBlob, getBytes, getDownloadURL, getMetadata, getStream, list, listAll, ref, updateMetadata, uploadBytes, uploadBytesResumable, uploadString };");
            }
            sb.AppendLine(indent).AppendLine("document.firebaseSdk = firebaseSdk;");
            return sb.ToString();
        }
    }
}
#endif
