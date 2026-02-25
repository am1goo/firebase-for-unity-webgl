using System;

namespace FirebaseWebGL
{
    public sealed class FirebaseStorageReferenceDeletedException : Exception
    {
        public FirebaseStorageReferenceDeletedException(FirebaseStorageRef @ref) : base($"Firebase Storage reference '{@ref.fullPath}' from bucket '{@ref.bucket}' is already deleted")
        {
        }
    }
}