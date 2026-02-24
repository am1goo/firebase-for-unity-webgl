using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthEmulatorOptions
    {
        [Preserve]
        public bool disableWarnings { get; set; }
    }
}
