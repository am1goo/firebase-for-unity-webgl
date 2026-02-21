using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseCallback<T>
    {
        [Preserve]
        public int requestId { get; set; }
        [Preserve]
        public bool success { get; set; }
        [Preserve]
        public T result { get; set; }
        [Preserve]
        public string error { get; set; }

        [Preserve]
        private FirebaseCallback()
        {
        }

        [Preserve]
        public FirebaseCallback(T result)
        {
            this.requestId = 0;
            this.success = true;
            this.result = result;
            this.error = null;
        }

        public bool IsCompletedSynchronously()
        {
            return requestId == 0;
        }
    }
}
