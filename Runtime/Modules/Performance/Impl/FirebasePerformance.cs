using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebasePerformance : IFirebasePerformance
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebasePerformance_initialize();
        [DllImport("__Internal")]
        private static extern int FirebaseWebGL_FirebasePerformance_trace(string name);

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;

        public Action<bool> onInitialized { get; set; }

        private readonly Dictionary<string, IFirebasePerformanceTrace> _cache = new Dictionary<string, IFirebasePerformanceTrace>();

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


            FirebaseWebGL_FirebasePerformance_initialize();
            _isInitialized = true;
            firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(_isInitialized));
        }

        public IFirebasePerformanceTrace Trace(string name)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            if (!_cache.TryGetValue(name, out var exist))
            {
                var intId = FirebaseWebGL_FirebasePerformance_trace(name);
                exist = new FirebasePerformanceTrace(intId, name);
                _cache.Add(name, exist);
            }
            return exist;
        }
    }
}
