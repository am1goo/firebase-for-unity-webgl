using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthActionCodeData
    {
        [Preserve]
        public string email { get; set; }
        [Preserve]
        public FirebaseAuthMultiFactorInfo multiFactorInfo { get; set; }
        [Preserve]
        public string previousEmail { get; set; }
    }
}
