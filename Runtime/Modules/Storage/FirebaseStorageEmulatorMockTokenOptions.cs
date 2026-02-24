using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageEmulatorMockTokenOptions
    {
        [Preserve]
        public string mockUserToken { get; set; }
    }
}
