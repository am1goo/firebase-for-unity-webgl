using System;

namespace FirebaseWebGL
{
    [Serializable]
    public struct FirebaseStorageRef
    {
        public string bucket { get; set; }
        public string fullPath { get; set; }
        public string name { get; set; }
    }
}
