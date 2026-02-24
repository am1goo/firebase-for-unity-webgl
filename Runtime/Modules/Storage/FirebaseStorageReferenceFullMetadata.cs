using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseStorageReferenceFullMetadata
    {
        [Preserve]
        public string bucket { get; set; }
        [Preserve]
        public string[] downloadTokens { get; set; }
        [Preserve]
        public string fullPath { get; set; }
        [Preserve]
        public string generation { get; set; }
        [Preserve]
        public string metageneration { get; set; }
        [Preserve]
        public string name { get; set; }
        [Preserve]
        public int size { get; set; }
        [Preserve]
        public string timeCreated { get; set; }
        [Preserve]
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
