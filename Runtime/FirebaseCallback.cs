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

        private const int synchronizedId = 0;

        [Preserve]
        private FirebaseCallback()
        {
        }

        [Preserve]
        public static FirebaseCallback<T> Success(T result)
        {
            return Success(synchronizedId, result);
        }

        [Preserve]
        public static FirebaseCallback<T> Success(int requestId, T result)
        {
            return new FirebaseCallback<T>
            {
                requestId = requestId,
                success = true,
                result = result,
                error = null,
            };
        }

        public static FirebaseCallback<T> Error(string error)
        {
            return Error(synchronizedId, error);
        }

        public static FirebaseCallback<T> Error(int requestId, string error)
        {
            return new FirebaseCallback<T>
            {
                requestId = requestId,
                success = false,
                result = default,
                error = error
            };
        }

        public bool IsCompletedSynchronously()
        {
            return requestId == synchronizedId;
        }
    }
}
