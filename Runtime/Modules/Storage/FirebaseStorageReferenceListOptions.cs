using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceListOptions
    {
        [Preserve]
        public int? maxResults { get; set; }
        [Preserve]
        public string pageToken { get; set; }
    }
}
