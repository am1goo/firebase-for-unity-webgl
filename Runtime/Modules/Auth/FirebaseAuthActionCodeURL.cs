using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthActionCodeURL
    {
        [Preserve]
        public string apiKey { get; set; }
        [Preserve]
        public string code { get; set; }
        [Preserve]
        public string continueUrl { get; set; }
        [Preserve]
        public string languageCode { get; set; }
        [Preserve]
        public string operation { get; set; }
        [Preserve]
        public string tenantId { get; set; }
    }
}
