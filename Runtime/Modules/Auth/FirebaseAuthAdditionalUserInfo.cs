using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthAdditionalUserInfo
    {
        [Preserve]
        public bool isNewUser { get; set; }
        [Preserve]
        public Dictionary<string, object> profile { get; set; }
        [Preserve]
        public string providerId { get; set; }
        [Preserve]
        public string username { get; set; }
    }
}
