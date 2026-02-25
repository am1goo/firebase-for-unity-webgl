using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseAuthLoggedUser : IFirebaseAuthLoggedUser
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_deleteUser(string uid, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_getIdToken(string uid, bool forceRefresh, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_getIdTokenResult(string uid, bool forceRefresh, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_reload(string uid, int requestId, FirebaseJsonCallbackDelegate callback);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<bool>>> _onBoolCallbacks = new Dictionary<int, Action<FirebaseCallback<bool>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthIdTokenResult>>> _onIdTokenResultCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthIdTokenResult>>>();

        public string uid => _user.uid;
        public string displayName => _user.displayName;
        public string email => _user.email;
        public string phoneNumber => _user.phoneNumber;
        public string photoURL => _user.photoURL;
        public string providerId => _user.providerId;
        public bool isAnonymous => _user.isAnonymous;
        public bool emailVerified => _user.emailVerified;

        private bool _isDeleted;
        public bool isDeleted => _isDeleted;

        private FirebaseAuthUser _user;

        internal FirebaseAuthLoggedUser(FirebaseAuthUser user)
        {
            _user = user;
        }

        internal void Reload(FirebaseAuthUser user)
        {
            _user = user;
        }

        public void DeleteUser(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);

                if (callback.success && callback.result)
                {
                    _isDeleted = true;
                }
            });

            FirebaseWebGL_FirebaseAuth_User_deleteUser(_user.uid, requestId, OnBoolCallback);
        }

        public void GetIdToken(bool forceRefresh, Action<FirebaseCallback<string>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_User_getIdToken(_user.uid, forceRefresh, requestId, OnStringCallback);
        }

        public void GetIdTokenResult(bool forceRefresh, Action<FirebaseCallback<FirebaseAuthIdTokenResult>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onIdTokenResultCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_User_getIdTokenResult(_user.uid, forceRefresh, requestId, OnIdTokenResultCallback);
        }

        public void Reload(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_User_reload(_user.uid, requestId, OnBoolCallback);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnBoolCallback(string json)
        {
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<bool>>(json);

            if (_onBoolCallbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                _onBoolCallbacks.Remove(firebaseCallback.requestId);
                try
                {
                    callback?.Invoke(firebaseCallback);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnStringCallback(string json)
        {
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<string>>(json);

            if (_onStringCallbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                _onStringCallbacks.Remove(firebaseCallback.requestId);
                try
                {
                    callback?.Invoke(firebaseCallback);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnIdTokenResultCallback(string json)
        {
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<FirebaseAuthIdTokenResult>>(json);

            if (_onIdTokenResultCallbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                _onIdTokenResultCallbacks.Remove(firebaseCallback.requestId);
                try
                {
                    callback?.Invoke(firebaseCallback);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}
