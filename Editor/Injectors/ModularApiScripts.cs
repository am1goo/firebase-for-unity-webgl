using System;
using System.Collections.Generic;
using System.IO;

namespace FirebaseWebGL.Editor
{
    internal class ModularApiScripts
    {
        internal static IScripts Remote(Dictionary<string, Uri> source)
        {
            return new RemoteScripts(source);
        }

        internal static IScripts Bundle(Dictionary<string, Uri> source, string bundlePath)
        {
            return new BundleScripts(source, bundlePath);
        }

        internal interface IScripts
        {
            string this[string sdkName] { get; }
        }

        private sealed class RemoteScripts : IScripts
        {
            private readonly Dictionary<string, string> _remoteUrls = new Dictionary<string, string>();

            internal RemoteScripts(Dictionary<string, Uri> source)
            {
                foreach (var kv in source)
                {
                    var name = kv.Key;
                    var uri = kv.Value;
                    _remoteUrls[name] = uri.ToString();
                }
            }

            string IScripts.this[string sdkName]
            {
                get { return _remoteUrls[sdkName]; }
            }
        }

        private sealed class BundleScripts : IScripts
        {
            private readonly Dictionary<string, string> _filenames = new Dictionary<string, string>();
            private readonly Dictionary<string, string> _bundlePaths = new Dictionary<string, string>();

            internal BundleScripts(Dictionary<string, Uri> source, string bundlePath)
            {
                foreach (var kv in source)
                {
                    var name = kv.Key;
                    var uri = kv.Value;

                    var filename = Path.GetFileName(uri.LocalPath);
                    _filenames[name] = filename;
                    _bundlePaths[name] = Path.Combine(bundlePath, filename).Replace("\\", "/");
                }
            }

            string IScripts.this[string sdkName]
            {
                get { return _bundlePaths[sdkName]; }
            }
        }
    }
}
