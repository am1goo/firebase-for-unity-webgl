using System;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceUploadMetadata : FirebaseStorageReferenceSettableMetadata
    {
        public string md5Hash { get; set; }

        public override string ToString()
        {
            return $"{nameof(md5Hash)}={md5Hash}, {base.ToString()}";
        }
    }
}
