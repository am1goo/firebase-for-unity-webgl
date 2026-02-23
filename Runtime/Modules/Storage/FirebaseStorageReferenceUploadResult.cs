using System;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceUploadResult
    {
        public string reference {get;set;}
        public FirebaseStorageReferenceFullMetadata metadata { get; set; }
    }
}
