using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthActionCodeInfo
    {
        [Preserve]
        public FirebaseAuthActionCodeData data { get; set; }
        [Preserve]
        public FirebaseAuthActionCodeOperation operation { get; set; }
    }
}
