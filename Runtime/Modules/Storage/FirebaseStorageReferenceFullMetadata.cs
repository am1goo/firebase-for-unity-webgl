using System;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceFullMetadata
    {
        public string bucket { get; set; }
        public string[] downloadTokens { get; set; }
        public string fullPath { get; set; }
        public string generation { get; set; }
        public string metageneration { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public string timeCreated { get; set; }
        public string updated { get; set; }

        public override string ToString()
        {
            return $"[{nameof(bucket)}={bucket}, " +
                $"{nameof(downloadTokens)}={downloadTokens?.Length ?? 0}, " +
                $"{nameof(fullPath)}={fullPath}, " +
                $"{nameof(generation)}={generation}, " +
                $"{nameof(metageneration)}={metageneration}, " +
                $"{nameof(name)}={name}, " +
                $"{nameof(size)}={size}, " +
                $"{nameof(timeCreated)}={timeCreated}, " +
                $"{nameof(updated)}={updated}]";
        }
    }
}
