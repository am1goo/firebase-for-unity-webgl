using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceUploadMetadata : FirebaseStorageReferenceSettableMetadata
    {
        [Preserve]
        public string md5Hash { get; set; }

        public override string ToString()
        {
            return $"{nameof(md5Hash)}={md5Hash}, {base.ToString()}";
        }
    }
}
