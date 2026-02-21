using System;

namespace FirebaseWebGL
{
    public sealed class FirebaseModuleNotInitializedException : Exception
    {
        public FirebaseModuleNotInitializedException(IFirebaseModule module) : this(module.GetType())
        {
        }

        public FirebaseModuleNotInitializedException(Type moduleType) : this(moduleType.Name)
        {
        }

        public FirebaseModuleNotInitializedException(string moduleName) : base($"firebase module '{moduleName}' is not initialzed")
        {

        }
    }
}
