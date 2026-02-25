using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthPhoneAuthCredential : FirebaseAuthCredential
    {
        [Preserve]
        public string verificationId { get; set; }
        [Preserve]
        public string verificationCode { get; set; }
        [Preserve]
        public string phoneNumber { get; set; }
        [Preserve]
        public string temporaryProof { get; set; }
    }
}
