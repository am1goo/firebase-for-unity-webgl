using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public struct FirebaseStorageRef
    {
        [Preserve]
        public string bucket { get; set; }
        [Preserve]
        public string fullPath { get; set; }
        [Preserve]
        public string name { get; set; }
    }
}
