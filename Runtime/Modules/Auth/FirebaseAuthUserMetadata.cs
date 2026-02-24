using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthUserMetadata
    {
        [Preserve]
        public string creationTime { get; set; }
        [Preserve]
        public string lastSignInTime { get; set; }

        public override string ToString()
        {
            return $"creationTime={creationTime}, lastSignInTime={lastSignInTime}";
        }
    }
}
