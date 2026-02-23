using System;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceListOptions
    {
        public int? maxResults { get; set; }
        public string pageToken { get; set; }
    }
}
