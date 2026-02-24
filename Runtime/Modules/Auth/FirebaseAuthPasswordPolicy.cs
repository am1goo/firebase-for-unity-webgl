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
        public CustomStrengthOptions customStrengthOptions { get; set; }
        [Preserve]
        public string enforcementState { get; set; }
        [Preserve]
        public bool forceUpgradeOnSignin { get; set; }

        public override string ToString()
        {
            return $"allowedNonAlphanumericCharacters={allowedNonAlphanumericCharacters}, " +
                $"customStrengthOptions=[{customStrengthOptions}], " +
                $"enforcementState={enforcementState}, " +
                $"forceUpgradeOnSignin={forceUpgradeOnSignin}";
        }

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

            public override string ToString()
            {
                return $"minPasswordLength={minPasswordLength}, " +
                    $"maxPasswordLength={maxPasswordLength}, " +
                    $"containsLowercaseLetter={containsLowercaseLetter}, " +
                    $"containsUppercaseLetter={containsUppercaseLetter}, " +
                    $"containsNumericCharacter={containsNumericCharacter}, " +
                    $"containsNonAlphanumericCharacter={containsNonAlphanumericCharacter}";
            }
        }
    }
}