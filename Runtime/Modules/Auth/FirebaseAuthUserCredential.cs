using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthUserCredential
    {
        [Preserve]
        public string operationType { get; set; }
        [Preserve]
        public string providerId { get; set; }
        [Preserve]
        public FirebaseAuthUser user { get; set; }
    }
}
