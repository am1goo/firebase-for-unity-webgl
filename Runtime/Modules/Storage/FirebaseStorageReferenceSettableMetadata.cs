using System.Collections.Generic;
using System.Linq;

namespace FirebaseWebGL
{
    public class FirebaseStorageReferenceSettableMetadata
    {
        public string cacheControl { get; set; }
        public string contentDisposition { get; set; }
        public string contentEncoding { get; set; }
        public string contentLanguage { get; set; }
        public string contentType { get; set; }
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