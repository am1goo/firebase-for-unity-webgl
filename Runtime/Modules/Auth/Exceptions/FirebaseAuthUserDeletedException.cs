using System;

namespace FirebaseWebGL
{
    public sealed class FirebaseAuthUserDeletedException : Exception
    {
        public FirebaseAuthUserDeletedException(string uid) : base($"Firebase Auth user with uid '{uid}' is already deleted")
        {

        }
    }
}