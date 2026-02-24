using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthUser : FirebaseAuthUserInfo
    {
        [Preserve]
        public bool emailVerified { get; set; }
        [Preserve]
        public bool isAnonymous { get; set; }
        [Preserve]
        public FirebaseAuthUserMetadata metadata { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, emailVerified={emailVerified}, isAnonymous={isAnonymous}, metadata=[{metadata}]";
        }
    }
}
