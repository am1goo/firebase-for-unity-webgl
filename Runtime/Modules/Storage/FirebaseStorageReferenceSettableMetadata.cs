using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    public class FirebaseStorageReferenceSettableMetadata
    {
        [Preserve]
        public string cacheControl { get; set; }
        [Preserve]
        public string contentDisposition { get; set; }
        [Preserve]
        public string contentEncoding { get; set; }
        [Preserve]
        public string contentLanguage { get; set; }
        [Preserve]
        public string contentType { get; set; }
        [Preserve]
        public Dictionary<string, string> customMetadata { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(cacheControl)}={cacheControl}, " +
                $"{nameof(contentDisposition)}={contentDisposition}, " +
                $"{nameof(contentEncoding)}={contentEncoding}, " +
                $"{nameof(contentLanguage)}={contentLanguage}, " +
                $"{nameof(contentType)}={contentType}, " +
                $"{nameof(customMetadata)}=[{(customMetadata != null ? string.Join(',', customMetadata.Select(x => $"{x.Key}={x.Value}")) : "null")}]";
        }
    }
}