using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace FirebaseWebGL
{
    [Serializable]
    public sealed class FirebaseAuthIdTokenResult
    {
        [Preserve]
        public string authTime { get; set; }
        [Preserve]
        public ParsedToken claims { get; set; }
        [Preserve]
        public string expirationTime { get; set; }
        [Preserve]
        public string issuedAtTime { get; set; }
        [Preserve]
        public string signInProvider { get; set; }
        [Preserve]
        public string signInSecondFactor { get; set; }
        [Preserve]
        public string token { get; set; }

        [Serializable]
        public sealed class ParsedToken
        {
            [Preserve]
            public string auth_time { get; set; }
            [Preserve]
            public string exp { get; set; }
            [Preserve]
            public Firebase firebase { get; set; }
            [Preserve]
            public string iat { get; set; }
            [Preserve]
            public string sub { get; set; }

            [Serializable]
            public sealed class Firebase
            {
                [Preserve]
                public string sign_in_provider { get; set; }
                [Preserve]
                public string sign_in_second_factor { get; set; }
                [Preserve]
                public Dictionary<string, string> identities { get; set; }
            }
        }
    }
}
