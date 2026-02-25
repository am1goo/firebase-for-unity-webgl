using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public class FirebaseAuthCredential
    {
        [Preserve]
        public string providerId { get; set; }
        [Preserve]
        public string signInMethod { get; set; }
    }
}
