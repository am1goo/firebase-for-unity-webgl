#if UNITY_WEBGL
using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
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
            const string indent = "  ";
            const string rootName = "firebaseSdk";
            var auth = new ModularApiInjector(rootName, "auth", "authApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-auth.js", new[]
            {
                "getAuth",
            }, (postfix) =>
            {
                return $"getAuth_{postfix}(app)";
            });
            var analytics = new ModularApiInjector(rootName, "analytics", "analyticsApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-analytics.js", new[]
            {
                "getAnalytics", "isSupported", "getGoogleAnalyticsClientId", "logEvent", "setAnalyticsCollectionEnabled", "setConsent", "setDefaultEventParameters", "setUserId", "setUserProperties",
            }, (postfix) =>
            {
                return $"getAnalytics_{postfix}(app)";
            });
            var appCheck = new ModularApiInjector(rootName, "appCheck", "appCheckApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-app-check.js", new[]
            {
                "initializeAppCheck", "getLimitedUseToken", "getToken", "onTokenChanged", "setTokenAutoRefreshEnabled", "CustomProvider", "ReCaptchaEnterpriseProvider", "ReCaptchaV3Provider"
            }, (postfix) =>
            {
                var provider = settings.includeAppCheckSettings.providerType switch
                {
                    FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaV3 => $"new ReCaptchaV3Provider_{postfix}(\'" + settings.includeAppCheckSettings.reCaptchaV3PublicKey + "\')",
                    FirebaseSettings.AppCheckSettings.ProviderType.ReCaptchaEnterprise => $"new ReCaptchaEnterpriseProvider_{postfix}(\'" + settings.includeAppCheckSettings.reCaptchaEnterprisePublicKey + "\')",
                    _ => throw new Exception($"unsupported provider type {settings.includeAppCheckSettings.providerType}"),
                };
                var reCaptchaV3PublicKey = settings.includeAppCheckSettings.reCaptchaV3PublicKey;
                var isTokenAutoRefreshEnabled = settings.includeAppCheckSettings.isTokenAutoRefreshEnabled;

                var injectOptions = "return { provider: " + provider + ", isTokenAutoRefreshEnabled: " + (isTokenAutoRefreshEnabled ? 1 : 0) + " };";
                return $"initializeAppCheck_{postfix}(app, function() {{ {injectOptions} }}())";
            });
            var firestore = new ModularApiInjector(rootName, "firestore", "firestoreApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-firestore.js", new[]
            {
                "getFirestore",
            }, (postfix) =>
            {
                return $"getFirestore_{postfix}(app)";
            });
            var messaging = new ModularApiInjector(rootName, "messaging", "messagingApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging.js", new[]
            {
                "getMessaging", "isSupported", "getToken", "deleteToken", "onMessage",
            }, (postfix) =>
            {
                return $"getMessaging_{postfix}(app)";
            });
            var messagingSw = new ModularApiInjector(rootName, "messagingSw", "messagingSwApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging-sw.js", new[]
            {
                "getMessaging", "isSupported", "experimentalSetDeliveryMetricsExportedToBigQueryEnabled",
            }, (postfix) =>
            {
                return $"getMessaging_{postfix}(app)";
            });
            var remoteConfig = new ModularApiInjector(rootName, "remoteConfig", "remoteConfigApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-remote-config.js", new[]
            {
                "getRemoteConfig", "isSupported", "activate", "ensureInitialized", "fetchAndActivate", "fetchConfig", "getAll", "getBoolean", "getNumber", "getString", "getValue", "onConfigUpdate", "setCustomSignals", "setLogLevel",
            }, (postfix) =>
            {
                return $"getRemoteConfig_{postfix}(app)";
            });
            var installations = new ModularApiInjector(rootName, "installations", "installationsApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-installations.js", new[]
            {
                "getInstallations", "deleteInstallations", "getId", "getToken", "onIdChange",
            }, (postfix) =>
            {
                return $"getInstallations_{postfix}(app)";
            });
            var performance = new ModularApiInjector(rootName, "performance", "performanceApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-performance.js", new[]
            {
                "getPerformance", "trace",
            }, (postfix) =>
            {
                return $"getPerformance_{postfix}(app)";
            });
            var storage = new ModularApiInjector(rootName, "storage", "storageApi", "https://www.gstatic.com/firebasejs/12.9.0/firebase-storage.js", new[]
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

                    return $"getStorage_{postfix}(app, \'" + uri.ToString() + "\')";
                }
                else
                {
                    return $"getStorage_{postfix}(app)";
                }
            });

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(indent).AppendLine("// Import the functions you need from the SDKs you need");
            sb.Append(indent).AppendLine("import { initializeApp, setLogLevel } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-app.js\";");
            if (settings.includeAuth)
            {
                sb.Append(indent).InjectImport(auth);
            }
            if (settings.includeAnalytics)
            {
                sb.Append(indent).InjectImport(analytics);
            }
            if (settings.includeAppCheck)
            {
                sb.Append(indent).InjectImport(appCheck);
            }
            if (settings.includeFirestore)
            {
                sb.Append(indent).InjectImport(firestore);
            }
            if (settings.includeMessaging)
            {
                sb.Append(indent).InjectImport(messaging);
                sb.Append(indent).InjectImport(messagingSw);
            }
            if (settings.includeRemoteConfig)
            {
                sb.Append(indent).InjectImport(remoteConfig);
            }
            if (settings.includeInstallations)
            {
                sb.Append(indent).InjectImport(installations);
            }
            if (settings.includePerformance)
            {
                sb.Append(indent).InjectImport(performance);
            }
            if (settings.includeStorage)
            {
                sb.Append(indent).InjectImport(storage);
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
                sb.Append(indent).InjectSdk(auth);
                sb.Append(indent).InjectApi(auth);
            }
            if (settings.includeAnalytics)
            {
                sb.Append(indent).InjectSdk(analytics);
                sb.Append(indent).InjectApi(analytics);
            }
            if (settings.includeAppCheck)
            {
                sb.Append(indent).InjectSdk(appCheck);
                sb.Append(indent).InjectApi(appCheck);
            }
            if (settings.includeFirestore)
            {
                sb.Append(indent).InjectSdk(firestore);
                sb.Append(indent).InjectApi(firestore);
            }
            if (settings.includeMessaging)
            {
                sb.Append(indent).InjectSdk(messaging);
                sb.Append(indent).InjectApi(messaging);
                sb.Append(indent).InjectSdk(messagingSw);
                sb.Append(indent).InjectApi(messagingSw);
            }
            if (settings.includeRemoteConfig)
            {
                sb.Append(indent).InjectSdk(remoteConfig);
                sb.Append(indent).InjectApi(remoteConfig);
            }
            if (settings.includeInstallations)
            {
                sb.Append(indent).InjectSdk(installations);
                sb.Append(indent).InjectApi(installations);
            }
            if (settings.includePerformance)
            {
                sb.Append(indent).InjectSdk(performance);
                sb.Append(indent).InjectApi(performance);
            }
            if (settings.includeStorage)
            {
                sb.Append(indent).InjectSdk(storage);
                sb.Append(indent).InjectApi(storage);
            }
            sb.AppendLine(indent).AppendLine("document.firebaseSdk = firebaseSdk;");
            return sb.ToString();
        }
    }

    internal class ModularApiInjector
    {
        private readonly string rootName;
        private readonly string sdkName;
        private readonly string apiName;
        private readonly string sourcePath;
        private readonly string[] injectedMethods;
        private readonly OnSdkInjector sdkInjector;

        public delegate string OnSdkInjector(string postfix);

        public ModularApiInjector(string rootName, string sdkName, string apiName, string sourcePath, string[] injectedMethods, OnSdkInjector sdkInjector)
        {
            this.rootName = rootName;
            this.sdkName = sdkName;
            this.apiName = apiName;
            this.sourcePath = sourcePath;
            this.injectedMethods = injectedMethods;
            this.sdkInjector = sdkInjector;
        }

        public StringBuilder InjectImport(StringBuilder sb)
        {
            var injectedMethods = string.Join(", ", this.injectedMethods.Select(x => $"{x} as {x}_{sdkName}"));
            return sb.AppendLine($"import {{ {injectedMethods} }} from \"{sourcePath}\";");
        }

        public StringBuilder InjectSdk(StringBuilder sb)
        {
            return sb.AppendLine($"{rootName}.{sdkName} = {sdkInjector(sdkName)};");
        }

        public StringBuilder InjectApi(StringBuilder sb)
        {
            var injectedMethods = string.Join(", ", this.injectedMethods.Select(x => $"{x}: {x}_{sdkName}"));
            return sb.AppendLine($"{rootName}.{apiName} = {{ {injectedMethods} }};");
        }
    }

    internal static class ModularApiInjectorExtensions
    {
        internal static StringBuilder InjectImport(this StringBuilder sb, ModularApiInjector injector)
        {
            return injector.InjectImport(sb);
        }

        internal static StringBuilder InjectSdk(this StringBuilder sb, ModularApiInjector injector)
        {
            return injector.InjectSdk(sb);
        }

        internal static StringBuilder InjectApi(this StringBuilder sb, ModularApiInjector injector)
        {
            return injector.InjectApi(sb);
        }
    }
}
#endif
