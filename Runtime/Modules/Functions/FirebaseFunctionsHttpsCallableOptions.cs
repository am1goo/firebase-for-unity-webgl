using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseFunctionsHttpsCallableOptions
    {
        [Preserve]
        public bool limitedUseAppCheckTokens { get; set; }
        [Preserve]
        public int timeout { get; set; } = 70000;
    }
}