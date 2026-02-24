using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthCredential
    {
        [Preserve]
        public string providerId { get; set; }
        [Preserve]
        public FirebaseAuthSignInMethod signInMethod { get; set; }
    }
}
