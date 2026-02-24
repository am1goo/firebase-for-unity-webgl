using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthActionCodeSettings
    {
        [Preserve]
        public Android android { get; set; }
        [Preserve]
        public string dynamicLinkDomain { get; set; }
        [Preserve]
        public bool handleCodeInApp { get; set; }
        [Preserve]
        public IOS iOS { get; set; }
        [Preserve]
        public string linkDomain { get; set; }
        [Preserve]
        public string url { get; set; }

        [Serializable]
        public sealed class Android
        {
            [Preserve]
            public bool installApp { get; set; }
            [Preserve]
            public string minimumVersion { get; set; }
            [Preserve]
            public string packageName { get; set; }
        }

        [Serializable]
        public sealed class IOS
        {
            [Preserve]
            public string bundleId { get; set; }
        }
    }
}
