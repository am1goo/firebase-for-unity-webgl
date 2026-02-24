using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public class FirebaseAuthUserInfo
    {
        [Preserve]
        public string displayName { get; set; }
        [Preserve]
        public string email { get; set; }
        [Preserve]
        public string phoneNumber { get; set; }
        [Preserve]
        public string photoURL { get; set; }
        [Preserve]
        public string providerId { get; set; }
        [Preserve]
        public string uid { get; set; }

        public override string ToString()
        {
            return $"displayName={displayName}, " +
                $"email={email}, " +
                $"phoneNumber={phoneNumber}, " +
                $"photoURL={photoURL}, " +
                $"providerId={providerId}, " +
                $"uid={uid}";
        }
    }
}
