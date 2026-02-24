using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceListResult
    {
        [Preserve]
        public string[] items { get; set; }
        [Preserve]
        public string nextPageToken { get; set; }
        [Preserve]
        public string[] prefixes { get; set; }
    }
}
