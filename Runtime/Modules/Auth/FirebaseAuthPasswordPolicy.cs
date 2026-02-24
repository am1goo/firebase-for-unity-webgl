using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthPasswordPolicy
    {
        [Preserve]
        public string allowedNonAlphanumericCharacters { get; set; }
        [Preserve]
        public object customStrengthOptions { get; set; }
        [Preserve]
        public string enforcementState { get; set; }
        [Preserve]
        public bool forceUpgradeOnSignin { get; set; }

        [Serializable]
        public sealed class CustomStrengthOptions
        {
            [Preserve]
            public int? minPasswordLength { get; set; }
            [Preserve]
            public int? maxPasswordLength { get; set; }
            [Preserve]
            public bool? containsLowercaseLetter { get; set; }
            [Preserve]
            public bool? containsUppercaseLetter { get; set; }
            [Preserve]
            public bool? containsNumericCharacter { get; set; }
            [Preserve]
            public bool? containsNonAlphanumericCharacter { get; set; }
        }
    }
}