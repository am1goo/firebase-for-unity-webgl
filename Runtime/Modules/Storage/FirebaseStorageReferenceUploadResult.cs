using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceUploadResult
    {
        [Preserve]
        public string reference {get;set; }
        [Preserve]
        public FirebaseStorageReferenceFullMetadata metadata { get; set; }
    }
}
