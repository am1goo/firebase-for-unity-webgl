#if UNITY_WEBGL
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

            var scriptsMap = new Dictionary<string, Uri>
            {
                { "app", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-app.js") },
                { "auth", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-auth.js") },
                { "analytics", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-analytics.js") },
                { "appCheck", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-app-check.js") },
                { "firestore", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-firestore.js") },
                { "functions", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-functions.js") },
                { "messaging", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging.js") },
                { "messagingSw", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging-sw.js") },
                { "remoteConfig", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-remote-config.js") },
                { "installations", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-installations.js") },
                { "performance", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-performance.js") },
                { "storage", new Uri("https://www.gstatic.com/firebasejs/12.9.0/firebase-storage.js") }
            };

            var scriptsOnCDNs = ModularApiScripts.Remote(scriptsMap);
            //TODO: added 'bundled modules' here later
            var scriptsToInject = scriptsOnCDNs;

            var injectors = new List<ModularApiInjector>();
            if (true) //always have to add Firebase App module
            {
                var app = new ModularApiInjector(rootName, "app", "appApi", postfix: null, scriptsToInject["app"], new[]
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
                var auth = new ModularApiInjector(rootName, "auth", "authApi", postfix: "auth", scriptsToInject["auth"], new[]
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
                    var authProviders = new ModularApiInjector(rootName, "authProviders", null, postfix: null, null, null, (postfix) =>
                    {
                        return $"{{ {injectConfigs} }};";
                    });
                    injectors.Add(authProviders);
                }
            }
            if (settings.includeAnalytics)
            {
                var analytics = new ModularApiInjector(rootName, "analytics", "analyticsApi", postfix: "analytics", scriptsToInject["analytics"], new[]
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
                var appCheck = new ModularApiInjector(rootName, "appCheck", "appCheckApi", postfix: "appCheck", scriptsToInject["appCheck"], new[]
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
                var firestore = new ModularApiInjector(rootName, "functions", "functionsApi", postfix: "functions", scriptsToInject["functions"], new[]
                {
                    "getFunctions", "FunctionsError", "connectFunctionsEmulator" , "httpsCallable", "httpsCallableFromURL",
                }, (postfix) =>
                {
                    var injectOptions = $"\'{settings.includeFunctionsSettings.regionOnCustomDomain}\'";
                    return $"getFunctions{postfix}({rootName}.app, {injectOptions})";
                });
                injectors.Add(firestore);
            }
            if (settings.includeMessaging)
            {
                var messaging = new ModularApiInjector(rootName, "messaging", "messagingApi", postfix: "messaging", scriptsToInject["messaging"], new[]
                {
                    "getMessaging", "isSupported", "getToken", "deleteToken", "onMessage",
                }, (postfix) =>
                {
                    return $"getMessaging{postfix}({rootName}.app)";
                });
                injectors.Add(messaging);

                if (settings.includeMessagingSettings.enableServiceWorker)
                {
                    var messagingSw = new ModularApiInjector(rootName, "messagingSw", "messagingSwApi", postfix: "messagingSw", scriptsToInject["messagingSw"], new[]
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
                var remoteConfig = new ModularApiInjector(rootName, "remoteConfig", "remoteConfigApi", postfix: "remoteConfig", scriptsToInject["remoteConfig"], new[]
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
                var installations = new ModularApiInjector(rootName, "installations", "installationsApi", postfix: "installations", scriptsToInject["installations"], new[]
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
                var performance = new ModularApiInjector(rootName, "performance", "performanceApi", postfix: "performance", scriptsToInject["performance"], new[]
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
                var storage = new ModularApiInjector(rootName, "storage", "storageApi", postfix: "storage", scriptsToInject["storage"], new[]
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
