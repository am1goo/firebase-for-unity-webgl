#if UNITY_WEBGL
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FirebaseWebGL.Editor
{
    public class FirebaseWebGLPostProcessBuildWithReport : IPostprocessBuildWithReport
    {
        public int callbackOrder => 999;

        private const string indexFilename = "index.html";
        private const string remarkValue = "injected-by-firebase-for-webgl-plugin";
        private const string bundledFolder = "./FirebaseBundle";
        private const string indent = "  ";
        private const string rootName = "firebaseSdk";
        private const string envSettingsPathKey = "FIREBASE_WEBGL_SETTINGS_PATH";
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

            var settings = default(FirebaseSettings);
            var firebaseSettingsPath = Environment.GetEnvironmentVariable(envSettingsPathKey);
            if (!string.IsNullOrWhiteSpace(firebaseSettingsPath))
            {
                firebaseSettingsPath = firebaseSettingsPath.Trim();
                if (!firebaseSettingsPath.EndsWith(".asset"))
                    firebaseSettingsPath += ".asset";
                settings = AssetDatabase.LoadAssetAtPath<FirebaseSettings>(firebaseSettingsPath);
                if (settings == null)
                    throw new Exception($"{nameof(FirebaseSettings)} file is not loaded at path '{firebaseSettingsPath}' (provided by environment argument {envSettingsPathKey})");
            }

            if (settings == null)
            {
                settings = Resources.Load<FirebaseSettings>(nameof(FirebaseSettings));
                if (settings == null)
                {
                    Debug.LogWarning($"{nameof(InjectFirebaseScripts)}: {nameof(FirebaseSettings)} file is not found in {nameof(Resources)} folder, injecting Firebase SDK is skipped.");
                    return;
                }
            }
            
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

            var scriptsMap = new Dictionary<string, Uri>
            {
                { FirebaseModuleNames.app, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-app.js") },
                { FirebaseModuleNames.auth, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-auth.js") },
                { FirebaseModuleNames.analytics, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-analytics.js") },
                { FirebaseModuleNames.appCheck, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-app-check.js") },
                { FirebaseModuleNames.firestore, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-firestore.js") },
                { FirebaseModuleNames.functions, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-functions.js") },
                { FirebaseModuleNames.messaging, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging.js") },
                { FirebaseModuleNames.messagingSw, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging-sw.js") },
                { FirebaseModuleNames.remoteConfig, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-remote-config.js") },
                { FirebaseModuleNames.installations, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-installations.js") },
                { FirebaseModuleNames.performance, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-performance.js") },
                { FirebaseModuleNames.storage, new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-storage.js") }
            };

            var scriptsOnCDNs = ModularApiScripts.Remote(scriptsMap);
            //TODO: added 'bundled modules' here later
            var scriptsToInject = scriptsOnCDNs;

            var injectors = new List<ModularApiInjector>();
            if (true) //always have to add Firebase App module
            {
                var sdkName = FirebaseModuleNames.app;
                var app = new ModularApiInjector(rootName, sdkName, postfix: null, scriptsToInject[sdkName], new[]
                {
                    "initializeApp", "setLogLevel",
                }, (postfix) =>
                {
                    var injectConfig = $"{{ apiKey: \"{settings.apiKey}\", authDomain: \"{settings.authDomain}\", projectId: \"{settings.projectId}\", storageBucket: \"{settings.storageBucket}\", messagingSenderId: \"{settings.messagingSenderId}\", appId: \"{settings.appId}\", measurementId: \"{settings.measurementId}\" }}";
                    return $"initializeApp{postfix}({injectConfig})";
                });
                injectors.Add(app);
            }
            if (settings.includeAuth)
            {
                var sdkName = FirebaseModuleNames.auth;
                var auth = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "getAuth", "ActionCodeOperation", "ActionCodeURL", "AuthCredential", "AuthErrorCodes", "EmailAuthCredential", "EmailAuthProvider", "FacebookAuthProvider", "FactorId", "GithubAuthProvider", "GoogleAuthProvider", "OAuthCredential", "OAuthProvider", "OperationType", "PhoneAuthCredential", "PhoneAuthProvider", "PhoneMultiFactorGenerator", "ProviderId", "RecaptchaVerifier", "SAMLAuthProvider", "SignInMethod", "TotpMultiFactorGenerator", "TotpSecret", "TwitterAuthProvider", "applyActionCode", "beforeAuthStateChanged", "browserCookiePersistence", "browserLocalPersistence", "browserPopupRedirectResolver", "browserSessionPersistence", "checkActionCode", "confirmPasswordReset", "connectAuthEmulator", "createUserWithEmailAndPassword", "debugErrorMap", "deleteUser", "fetchSignInMethodsForEmail", "getAdditionalUserInfo", "getIdToken", "getIdTokenResult", "getMultiFactorResolver", "getRedirectResult", "inMemoryPersistence", "indexedDBLocalPersistence", "initializeAuth", "initializeRecaptchaConfig", "isSignInWithEmailLink", "linkWithCredential", "linkWithPhoneNumber", "linkWithPopup", "linkWithRedirect", "multiFactor", "onAuthStateChanged", "onIdTokenChanged", "parseActionCodeURL", "reauthenticateWithCredential", "reauthenticateWithPhoneNumber", "reauthenticateWithPopup", "reauthenticateWithRedirect", "reload", "revokeAccessToken", "sendEmailVerification", "sendPasswordResetEmail", "sendSignInLinkToEmail", "setPersistence", "signInAnonymously", "signInWithCredential", "signInWithCustomToken", "signInWithEmailAndPassword", "signInWithEmailLink", "signInWithPhoneNumber", "signInWithPopup", "signInWithRedirect", "signOut", "unlink", "updateCurrentUser", "updateEmail", "updatePassword", "updatePhoneNumber", "updateProfile", "useDeviceLanguage", "validatePassword", "verifyBeforeUpdateEmail", "verifyPasswordResetCode",
                }, (postfix) =>
                {
                    return $"getAuth{postfix}({rootName}.app)";
                });
                injectors.Add(auth);

                var providerConfigs = new List<string>();
                if (settings.includeAuthSettings.useGoogleAuthProvider)
                {
                    var providerSettings = settings.includeAuthSettings.useGoogleAuthProviderSettings;
                    var providerConfig = $"signWithGoogle: {{ provider: new GoogleAuthProvider(), scopes: [{string.Join(',', providerSettings.scopes.Select(x => $"\"{x}\""))}] }}";
                    providerConfigs.Add(providerConfig);
                }
                if (settings.includeAuthSettings.useAppleAuthProvider)
                {
                    var providerSettings = settings.includeAuthSettings.useAppleAuthProviderSettings;
                    var providerConfig = $"signWithApple: {{ provider: new OAuthProvider('apple.com'), scopes: [{string.Join(',', providerSettings.scopes.Select(x => $"\"{x}\""))}] }}";
                    providerConfigs.Add(providerConfig);
                }
                if (settings.includeAuthSettings.useFacebookAuthProvider)
                {
                    var providerSettings = settings.includeAuthSettings.useFacebookAuthProviderSettings;
                    var providerConfig = $"facebook: {{ provider: new FacebookAuthProvider(), scopes: [{string.Join(',', providerSettings.scopes.Select(x => $"\"{x}\""))}] }}";
                    providerConfigs.Add(providerConfig);
                }
                if (settings.includeAuthSettings.useGithubAuthProvider)
                {
                    var providerSettings = settings.includeAuthSettings.useGithubAuthProviderSettings;
                    var providerConfig = $"github: {{ provider: new GithubAuthProvider(), scopes: [{string.Join(',', providerSettings.scopes.Select(x => $"\"{x}\""))}] }}";
                    providerConfigs.Add(providerConfig);
                }
                if (settings.includeAuthSettings.useTwitterAuthProvider)
                {
                    var providerSettings = settings.includeAuthSettings.useTwitterAuthProviderSettings;
                    var providerConfig = $"twitter: {{ provider: new TwitterAuthProvider(), scopes: [{string.Join(',', providerSettings.scopes.Select(x => $"\"{x}\""))}] }}";
                    providerConfigs.Add(providerConfig);
                }
                if (settings.includeAuthSettings.useMicrosoftAuthProvider)
                {
                    var providerSettings = settings.includeAuthSettings.useMicrosoftAuthProviderSettings;
                    var providerConfig = $"microsoft: {{ provider: new OAuthProvider('microsoft.com'), scopes: [{string.Join(',', providerSettings.scopes.Select(x => $"\"{x}\""))}] }}";
                    providerConfigs.Add(providerConfig);
                }
                if (settings.includeAuthSettings.useYahooAuthProvider)
                {
                    var providerSettings = settings.includeAuthSettings.useYahooAuthProviderSettings;
                    var providerConfig = $"yahoo: {{ provider: new OAuthProvider('yahoo.com'), scopes: [{string.Join(',', providerSettings.scopes.Select(x => $"\"{x}\""))}] }}";
                    providerConfigs.Add(providerConfig);
                }

                if (providerConfigs.Count > 0)
                {
                    var injectConfigs = string.Join(", ", providerConfigs);
                    var authProviders = new ModularApiInjector(rootName, "authProviders", postfix: null, null, null, (postfix) =>
                    {
                        return $"{{ {injectConfigs} }};";
                    });
                    injectors.Add(authProviders);
                }
            }
            if (settings.includeAnalytics)
            {
                var sdkName = FirebaseModuleNames.analytics;
                var analytics = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "getAnalytics", "isSupported", "getGoogleAnalyticsClientId", "logEvent", "setAnalyticsCollectionEnabled", "setConsent", "setDefaultEventParameters", "setUserId", "setUserProperties",
                }, (postfix) =>
                {
                    return $"getAnalytics{postfix}({rootName}.app)";
                });
                injectors.Add(analytics);
            }
            if (settings.includeAppCheck)
            {
                var sdkName = FirebaseModuleNames.appCheck;
                var appCheck = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "initializeAppCheck", "getLimitedUseToken", "getToken", "onTokenChanged", "setTokenAutoRefreshEnabled", "CustomProvider", "ReCaptchaEnterpriseProvider", "ReCaptchaV3Provider"
                }, (postfix) =>
                {
                    var provider = settings.includeAppCheckSettings.providerType switch
                    {
                        FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaV3 => $"new ReCaptchaV3Provider(\'" + settings.includeAppCheckSettings.reCaptchaV3PublicKey + "\')",
                        FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaEnterprise => $"new ReCaptchaEnterpriseProvider(\'" + settings.includeAppCheckSettings.reCaptchaEnterprisePublicKey + "\')",
                        _ => throw new Exception($"unsupported provider type {settings.includeAppCheckSettings.providerType}"),
                    };
                    var reCaptchaV3PublicKey = settings.includeAppCheckSettings.reCaptchaV3PublicKey;
                    var isTokenAutoRefreshEnabled = settings.includeAppCheckSettings.isTokenAutoRefreshEnabled;

                    var injectOptions = $"{{ provider: {provider}, isTokenAutoRefreshEnabled: {(isTokenAutoRefreshEnabled ? 1 : 0)} }}";
                    return $"initializeAppCheck{postfix}({rootName}.app, {injectOptions})";
                });
                injectors.Add(appCheck);
            }
            if (settings.includeFunctions)
            {
                var sdkName = FirebaseModuleNames.functions;
                var functions = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "getFunctions", "FunctionsError", "connectFunctionsEmulator" , "httpsCallable", "httpsCallableFromURL",
                }, (postfix) =>
                {
                    var injectOptions = $"\'{settings.includeFunctionsSettings.regionOnCustomDomain}\'";
                    return $"getFunctions{postfix}({rootName}.app, {injectOptions})";
                });
                injectors.Add(functions);
            }
            if (settings.includeMessaging)
            {
                var sdkName = FirebaseModuleNames.messaging;
                var messaging = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "getMessaging", "isSupported", "getToken", "deleteToken", "onMessage",
                }, (postfix) =>
                {
                    return $"getMessaging{postfix}({rootName}.app)";
                });
                injectors.Add(messaging);

                if (settings.includeMessagingSettings.enableServiceWorker)
                {
                    var sdkNameSw = FirebaseModuleNames.messagingSw;
                    var messagingSw = new ModularApiInjector(rootName, sdkNameSw, postfix: sdkNameSw, scriptsToInject[sdkNameSw], new[]
                    {
                        "getMessaging", "isSupported", "experimentalSetDeliveryMetricsExportedToBigQueryEnabled",
                    }, (postfix) =>
                    {
                        return $"getMessaging{postfix}({rootName}.app)";
                    });
                    injectors.Add(messagingSw);
                }
            }
            if (settings.includeRemoteConfig)
            {
                var sdkName = FirebaseModuleNames.remoteConfig;
                var remoteConfig = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "getRemoteConfig", "isSupported", "activate", "ensureInitialized", "fetchAndActivate", "fetchConfig", "getAll", "getBoolean", "getNumber", "getString", "getValue", "onConfigUpdate", "setCustomSignals", "setLogLevel",
                }, (postfix) =>
                {
                    return $"getRemoteConfig{postfix}({rootName}.app)";
                });
                injectors.Add(remoteConfig);
            }

            if (settings.includeInstallations)
            {
                var sdkName = FirebaseModuleNames.installations;
                var installations = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "getInstallations", "deleteInstallations", "getId", "getToken", "onIdChange",
                }, (postfix) =>
                {
                    return $"getInstallations{postfix}({rootName}.app)";
                });
                injectors.Add(installations);
            }

            if (settings.includePerformance)
            {
                var sdkName = FirebaseModuleNames.performance;
                var performance = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "getPerformance", "trace",
                }, (postfix) =>
                {
                    return $"getPerformance{postfix}({rootName}.app)";
                });
                injectors.Add(performance);
            }

            if (settings.includeStorage)
            {
                var sdkName = FirebaseModuleNames.storage;
                var storage = new ModularApiInjector(rootName, sdkName, postfix: sdkName, scriptsToInject[sdkName], new[]
                {
                    "getStorage", "connectStorageEmulator", "deleteObject", "getBlob", "getBytes", "getDownloadURL", "getMetadata", "getStream", "list", "ref", "updateMetadata", "uploadBytes", "uploadBytesResumable", "uploadString",
                }, (postfix) =>
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

                        return $"getStorage{postfix}({rootName}.app, \'" + uri.ToString() + "\')";
                    }
                    else
                    {
                        return $"getStorage{postfix}({rootName}.app)";
                    }
                });
                injectors.Add(storage);
            }

            var textToInject = HtmlNode.CreateNode(GenerateText(settings, injectors));
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

            var bundleFolderInfo = new DirectoryInfo(Path.Combine(outputPath, bundledFolder));
            if (bundleFolderInfo.Exists)
                bundleFolderInfo.Delete(recursive: true);

            var serviceWorkerInfo = new FileInfo(Path.Combine(outputPath, "firebase-messaging-sw.js"));
            if (serviceWorkerInfo.Exists)
                serviceWorkerInfo.Delete();

            if (settings.includeMessaging)
            {
                using (var fs = serviceWorkerInfo.OpenWrite())
                {
                    //TODO: if you need support of onBackgroundMessage callback, feel free to create feature request if you need it
                    //https://github.com/firebase/quickstart-js/blob/master/messaging/firebase-messaging-sw.js
                    fs.Flush();
                }
            }
        }

        private static string GenerateText(FirebaseSettings settings, IReadOnlyList<ModularApiInjector> injectors)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(indent).AppendLine("// Import the functions you need from the SDKs you need");
            foreach (var injector in injectors)
            {
                if (injector.importSupported)
                    sb.Append(indent).InjectImport(injector);
            }
            sb.AppendLine();
            sb.Append(indent).AppendLine("// Initialize Firebase");
            sb.Append(indent).AppendLine($"const {rootName} = {{ }}");
            foreach (var injector in injectors)
            {
                if (injector.sdkSupported)
                    sb.Append(indent).InjectSdk(injector);

                if (injector.apiSupported)
                    sb.Append(indent).InjectApi(injector);
            }
            sb.AppendLine(indent).AppendLine($"document.{rootName} = {rootName};");
            return sb.ToString();
        }
    }
}
#endif
