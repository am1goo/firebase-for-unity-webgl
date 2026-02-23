using System;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceListResult
    {
        public string[] items { get; set; }
        public string nextPageToken { get; set; }
        public string[] prefixes { get; set; }
    }
}
