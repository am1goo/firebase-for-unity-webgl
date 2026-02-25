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
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_linkWithCredential(string uid, string credentialAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_linkWithPopup(string providerId, string customParametersAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_reauthenticateWithCredential(string uid, string credentialAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_reauthenticateWithPopup(string providerId, string customParametersAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_sendEmailVerification(string uid, string actionCodeSettingsAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_unlink(string uid, string providerId, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_updateEmail(string uid, string newEmail, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_updatePassword(string uid, string newPassword, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_updateProfile(string uid, string optionsAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_User_verifyBeforeUpdateEmail(string uid, string newEmail, string actionCodeSettingsAsJson, int requestId, FirebaseJsonCallbackDelegate callback);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<bool>>> _onBoolCallbacks = new Dictionary<int, Action<FirebaseCallback<bool>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthIdTokenResult>>> _onIdTokenResultCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthIdTokenResult>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthUser>>> _onUserCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthUser>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthUserCredential>>> _onUserCredentialCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthUserCredential>>>();

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

        public void LinkWithCredential(FirebaseAuthCredential credential, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var credentialAsJson = JsonConvert.SerializeObject(credential);
            FirebaseWebGL_FirebaseAuth_User_linkWithCredential(_user.uid, credentialAsJson, requestId, OnUserCredentialCallback);
        }

        public void LinkInWithPopup(string providerId, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            LinkInWithPopup(providerId, null, firebaseCallback);
        }

        public void LinkInWithPopup(string providerId, Dictionary<string, string> customParameters, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (providerId == null)
                throw new ArgumentNullException(nameof(providerId));

            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var customParametersAsJson = customParameters != null ? JsonConvert.SerializeObject(customParameters) : null;
            FirebaseWebGL_FirebaseAuth_User_linkWithPopup(providerId, customParametersAsJson, requestId, OnUserCredentialCallback);
        }

        public void ReauthenticateWithCredential(FirebaseAuthCredential credential, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var credentialAsJson = JsonConvert.SerializeObject(credential);
            FirebaseWebGL_FirebaseAuth_User_reauthenticateWithCredential(_user.uid, credentialAsJson, requestId, OnUserCredentialCallback);
        }

        public void ReauthenticateInWithPopup(string providerId, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            ReauthenticateInWithPopup(providerId, null, firebaseCallback);
        }

        public void ReauthenticateInWithPopup(string providerId, Dictionary<string, string> customParameters, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (providerId == null)
                throw new ArgumentNullException(nameof(providerId));

            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var customParametersAsJson = customParameters != null ? JsonConvert.SerializeObject(customParameters) : null;
            FirebaseWebGL_FirebaseAuth_User_reauthenticateWithPopup(providerId, customParametersAsJson, requestId, OnUserCredentialCallback);
        }

        public void SendEmailVerification(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            SendEmailVerification(null, firebaseCallback);
        }

        public void SendEmailVerification(FirebaseAuthActionCodeSettings actionCodeSettings, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var actionCodeSettingsAsJson = actionCodeSettings != null ? JsonConvert.SerializeObject(actionCodeSettings) : null;
            FirebaseWebGL_FirebaseAuth_User_sendEmailVerification(_user.uid, actionCodeSettingsAsJson, requestId, OnBoolCallback);
        }

        public void Unlink(string providerId, Action<FirebaseCallback<FirebaseAuthUser>> firebaseCallback)
        {
            if (providerId == null)
                throw new ArgumentNullException(nameof(providerId));

            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onUserCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_User_unlink(_user.uid, providerId, requestId, OnUserCallback);
        }

        public void UpdateEmail(string newEmail, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (newEmail == null)
                throw new ArgumentNullException(nameof(newEmail));

            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_User_updateEmail(_user.uid, newEmail, requestId, OnBoolCallback);
        }

        public void UpdatePassword(string newPassword, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (newPassword == null)
                throw new ArgumentNullException(nameof(newPassword));

            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_User_updatePassword(_user.uid, newPassword, requestId, OnBoolCallback);
        }

        public void UpdateProfile(string displayName, string photoURL, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var options = new Dictionary<string, string>();
            if (displayName != null)
            {
                options[nameof(displayName)] = displayName;
            }
            if (photoURL != null)
            {
                options[nameof(photoURL)] = photoURL;
            }

            var optionsAsJson = JsonConvert.SerializeObject(options);
            FirebaseWebGL_FirebaseAuth_User_updateProfile(_user.uid, optionsAsJson, requestId, OnBoolCallback);
        }

        public void VerifyBeforeUpdateEmail(string newEmail, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            VerifyBeforeUpdateEmail(newEmail, null, firebaseCallback);
        }

        public void VerifyBeforeUpdateEmail(string newEmail, FirebaseAuthActionCodeSettings actionCodeSettings, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (newEmail == null)
                throw new ArgumentNullException(nameof(newEmail));

            if (_isDeleted)
                throw new FirebaseAuthUserDeletedException(_user.uid);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var actionCodeSettingsAsJson = actionCodeSettings != null ? JsonConvert.SerializeObject(actionCodeSettings) : null;
            FirebaseWebGL_FirebaseAuth_User_verifyBeforeUpdateEmail(_user.uid, newEmail, actionCodeSettingsAsJson, requestId, OnBoolCallback);
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

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnUserCallback(string json)
        {
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<FirebaseAuthUser>>(json);

            if (_onUserCallbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                _onUserCallbacks.Remove(firebaseCallback.requestId);
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
        private static void OnUserCredentialCallback(string json)
        {
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<FirebaseAuthUserCredential>>(json);

            if (_onUserCredentialCallbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                _onUserCredentialCallbacks.Remove(firebaseCallback.requestId);
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
