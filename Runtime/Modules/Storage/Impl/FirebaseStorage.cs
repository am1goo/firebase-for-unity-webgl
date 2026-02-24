using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseStorage : IFirebaseStorage
    {
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseStorage_initialize();
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseStorage_connectStorageEmulator(string host, int port, string optionsAsJson);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseStorage_ref(string url);

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }

        public void Initialize(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (_isInitialized)
            {
                firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(_isInitialized));
                return;
            }

            if (Application.isEditor)
            {
                firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(false));
                return;
            }

            _isInitialized = FirebaseWebGL_FirebaseStorage_initialize();
            onInitialized?.Invoke(_isInitialized);
            firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(_isInitialized));
        }

        public void ConnectStorageEmulator(string host, int port, FirebaseStorageEmulatorMockTokenOptions options)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var optionsAsJson = options != null ? JsonConvert.SerializeObject(options) : null;
            var connected = FirebaseWebGL_FirebaseStorage_connectStorageEmulator(host, port, optionsAsJson);
            if (!connected)
                throw new Exception("unable to connect to emulator");

            //do nothing
        }

        public IFirebaseStorageReference Ref(string url)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            if (string.IsNullOrWhiteSpace(url))
                url = "/";

            var refInstanceAsJson = FirebaseWebGL_FirebaseStorage_ref(url);
            var refInstance = JsonConvert.DeserializeObject<FirebaseStorageRef>(refInstanceAsJson);
            return new FirebaseStorageReference(refInstance);
        }
    }
}
