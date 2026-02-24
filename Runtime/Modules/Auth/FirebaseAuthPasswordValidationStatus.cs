using System;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthPasswordValidationStatus
    {
        [Preserve]
        public bool containsLowercaseLetter { get; set; }
        [Preserve]
        public bool containsNonAlphanumericCharacter { get; set; }
        [Preserve]
        public bool containsNumericCharacter { get; set; }
        [Preserve]
        public bool containsUppercaseLetter { get; set; }
        [Preserve]
        public bool isValid { get; set; }
        [Preserve]
        public bool meetsMaxPasswordLength { get; set; }
        [Preserve]
        public bool meetsMinPasswordLength { get; set; }
        [Preserve]
        public FirebaseAuthPasswordPolicy passwordPolicy { get; set; }

        public override string ToString()
        {
            return $"containsLowercaseLetter={containsLowercaseLetter}, " +
                $"containsNonAlphanumericCharacter={containsNonAlphanumericCharacter}, " +
                $"containsNumericCharacter={containsNumericCharacter}, " +
                $"containsUppercaseLetter={containsUppercaseLetter}, " +
                $"isValid={isValid}, " +
                $"meetsMaxPasswordLength={meetsMaxPasswordLength}, " +
                $"meetsMinPasswordLength={meetsMinPasswordLength}, " +
                $"passwordPolicy=[{passwordPolicy}]";
        }
    }
}