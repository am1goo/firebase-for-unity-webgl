using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseFunctions : IFirebaseFunctions
    {
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseFunctions_initialize();
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseFunctions_connectFunctionsEmulator(string host, int port);
        [DllImport("__Internal")]
        private static extern int FirebaseWebGL_FirebaseFunctions_httpsCallable(string name, string optionsAsJson);
        [DllImport("__Internal")]
        private static extern int FirebaseWebGL_FirebaseFunctions_httpsCallableFromURL(string url, string optionsAsJson);

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

            _isInitialized = FirebaseWebGL_FirebaseFunctions_initialize();
            onInitialized?.Invoke(_isInitialized);
            firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(_isInitialized));
        }

        public void ConnectFunctionsEmulator(string host, int port)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var connected = FirebaseWebGL_FirebaseFunctions_connectFunctionsEmulator(host, port);
            if (!connected)
                throw new Exception("unable to connect to emulator");

            //do nothing
        }

        public IFirebaseFunctionsHttpsCallable HttpsCallable(string name, FirebaseFunctionsHttpsCallableOptions options)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var optionsAsJson = JsonConvert.SerializeObject(options);
            var callableId = FirebaseWebGL_FirebaseFunctions_httpsCallable(name, optionsAsJson);
            if (callableId == 0)
                throw new Exception("httpsCallable is not created");

            return new FirebaseFunctionsHttpsCallable(callableId);
        }

        public IFirebaseFunctionsHttpsCallable HttpsCallableFromURL(string url, FirebaseFunctionsHttpsCallableOptions options)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var optionsAsJson = JsonConvert.SerializeObject(options);
            var callableId = FirebaseWebGL_FirebaseFunctions_httpsCallableFromURL(url, optionsAsJson);
            if (callableId == 0)
                throw new Exception("httpsCallableFromURL is not created");

            return new FirebaseFunctionsHttpsCallable(callableId);
        }
    }
}
