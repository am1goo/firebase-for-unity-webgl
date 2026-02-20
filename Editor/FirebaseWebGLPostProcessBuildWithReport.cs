#if UNITY_WEBGL
using HtmlAgilityPack;
using System;
using System.IO;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

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

        var settings = FirebaseWebGLSettings.instance;
        if (settings == null)
            throw new Exception($"{nameof(FirebaseWebGLSettings)} file is not found in {nameof(Resources)} folder");

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
    }

    private static string GenerateText(FirebaseWebGLSettings settings)
    {
        var indent = "  ";

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.Append(indent).AppendLine("// Import the functions you need from the SDKs you need");
        sb.Append(indent).AppendLine("import { initializeApp } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-app.js\";");
        if (settings.includeAuth)
        {
            sb.Append(indent).AppendLine("import { getAuth } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-auth.js\";");
        }
        if (settings.includeAnalytics)
        {
            sb.Append(indent).AppendLine("import { getAnalytics } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-analytics.js\";");
        }
        if (settings.includeFirestore)
        {
            sb.Append(indent).AppendLine("import { getFirestore } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-firestore.js\";");
        }
        if (settings.includeMessaging)
        {
            sb.Append(indent).AppendLine("import { getMessaging } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-messaging.js\";");
        }
        if (settings.includeRemoteConfig)
        {
            sb.Append(indent).AppendLine("import { remote-config } from \"https://www.gstatic.com/firebasejs/12.9.0/firebase-remote-config.js\";");
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
        sb.Append(indent).AppendLine("const app = initializeApp(firebaseConfig);");
        if (settings.includeAuth)
        {
            sb.Append(indent).AppendLine("const auth = getAuth(app);");
        }
        if (settings.includeAnalytics)
        {
            sb.Append(indent).AppendLine("const analytics = getAnalytics(app);");
        }
        if (settings.includeFirestore)
        {
            sb.Append(indent).AppendLine("const firestore = getFirestore(app);");
        }
        if (settings.includeMessaging)
        {
            sb.Append(indent).AppendLine("const messaging = getMessaging(app);");
        }
        if (settings.includeRemoteConfig)
        {
            sb.Append(indent).AppendLine("const remoteConfig = getRemoteConfig(app);");
        }
        return sb.ToString();
    }
}
#endif
