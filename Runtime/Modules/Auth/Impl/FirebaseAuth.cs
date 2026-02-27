using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseAuth : IFirebaseAuth
    {
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseAuth_initialize();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_connectAuthEmulator(string url, string optionsAsJson);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseAuth_currentUser();
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseAuth_languageCode();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_languageCodeSet(string locale);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseAuth_tenantId();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_applyActionCode(string oobCode, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_checkActionCode(string oobCode, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_confirmPasswordReset(string oobCode, string newPassword, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_createUserWithEmailAndPassword(string email, string password, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_fetchSignInMethodsForEmail(string email, int requestId, FirebaseJsonCallbackDelegate callback);
        //getMultiFactorResolver
        //getRedirectResult
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_initializeRecaptchaConfig(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseAuth_isSignInWithEmailLink(string emailLink);
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseAuth_beforeAuthStateChanged(int instanceId, FirebaseJsonFunctionDelegate<bool> callback);
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseAuth_onAuthStateChanged(int instanceId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseAuth_onIdTokenChanged(int instanceId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_revokeAccessToken(string token, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_sendPasswordResetEmail(string email, string actionCodeSettingsAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_sendSignInLinkToEmail(string email, string actionCodeSettingsAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        //setPersistence
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_signInAnonymously(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_signInWithCredential(string credentialAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_signInWithCustomToken(string customToken, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_signInWithEmailAndPassword(string email, string password, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_signInWithEmailLink(string email, string emailLink, int requestId, FirebaseJsonCallbackDelegate callback);
        //signInWithPhoneNumber
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_signInWithPopup(string providerId, string customParametersAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        //signInWithRedirect
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_signOut(int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_updateCurrentUser(string userAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseAuth_useDeviceLanguage();
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_validatePassword(string password, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseAuth_verifyPasswordResetCode(string code, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseAuth_parseActionCodeURL(string link);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebaseAuth_getAdditionalUserInfo(string credentialAsJson);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<bool>>> _onBoolCallbacks = new Dictionary<int, Action<FirebaseCallback<bool>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<string[]>>> _onStringArrayCallbacks = new Dictionary<int, Action<FirebaseCallback<string[]>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthUserCredential>>> _onUserCredentialCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthUserCredential>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthActionCodeInfo>>> _onActionCodeInfoCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthActionCodeInfo>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthPasswordValidationStatus>>> _onPasswordValidationStatusCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthPasswordValidationStatus>>>();
        private static readonly Dictionary<int, Func<FirebaseCallback<FirebaseAuthUser>, bool>> _onBeforeAuthStateChangeCallbacks = new Dictionary<int, Func<FirebaseCallback<FirebaseAuthUser>, bool>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthUser>>> _onAuthStateChangeCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthUser>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseAuthUser>>> _onIdTokenChangeCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseAuthUser>>>();

        private bool _isInitialized;
        public bool isInitialized => _isInitialized;
        public Action<bool> onInitialized { get; set; }

        public Func<FirebaseAuthUser, bool> onBeforeAuthStateChanged { get; set; }
        public Action<FirebaseAuthUser> onAuthStateChanged { get; set; }
        public Action<FirebaseAuthUser> onIdTokenChanged { get; set; }

        private bool _isRecaptchaInitialized;
        public bool isRecaptchaInitialized => _isRecaptchaInitialized;

        private FirebaseAuthLoggedUser _loggedUser;
        public IFirebaseAuthLoggedUser loggedUser => _loggedUser;

        private IFirebaseAuth.OnLoggerUserChangedDelegate _onLoggedUserChanged;
        public IFirebaseAuth.OnLoggerUserChangedDelegate onLoggedUserChanged { get => _onLoggedUserChanged; set => _onLoggedUserChanged = value; }

        private readonly int _instanceId;

        public FirebaseAuth()
        {
            _instanceId = _requests.NextId();
        }

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

            _isInitialized = FirebaseWebGL_FirebaseAuth_initialize();
            OnInitialized();
            onInitialized?.Invoke(_isInitialized);
            firebaseCallback?.Invoke(FirebaseCallback<bool>.Success(_isInitialized));
        }

        private void OnInitialized()
        {
            OnAuthStateChanged((Action<FirebaseAuthUser>)null);

            var currentUserAsJson = FirebaseWebGL_FirebaseAuth_currentUser();
            var currentUser = currentUserAsJson != null ? JsonConvert.DeserializeObject<FirebaseAuthUser>(currentUserAsJson) : null;
            SetCurrentUser(currentUser);
        }

        private void SetCurrentUser(FirebaseAuthUser authorizedUser)
        {
            if (authorizedUser != null)
            {
                if (_loggedUser != null && _loggedUser.uid == authorizedUser.uid)
                {
                    _loggedUser.Reload(authorizedUser);
                }
                else
                {
                    _loggedUser = new FirebaseAuthLoggedUser(authorizedUser);
                    onLoggedUserChanged?.Invoke(_loggedUser);
                }
            }
            else
            {
                if (_loggedUser != null)
                {
                    _loggedUser = null;
                    onLoggedUserChanged?.Invoke(_loggedUser);
                }
            }
        }

        public void ConnectAuthEmulator(string url, FirebaseAuthEmulatorOptions options)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var optionsAsJson = options != null ? JsonConvert.SerializeObject(options) : null;
            FirebaseWebGL_FirebaseAuth_connectAuthEmulator(url, optionsAsJson);
        }

        public string languageCode
        {
            get
            {
                if (!_isInitialized)
                    throw new FirebaseModuleNotInitializedException(this);

                return FirebaseWebGL_FirebaseAuth_languageCode();
            }
            set
            {
                if (!_isInitialized)
                    throw new FirebaseModuleNotInitializedException(this);

                FirebaseWebGL_FirebaseAuth_languageCodeSet(value);
            }
        }

        public string tenantId
        {
            get
            {
                if (!_isInitialized)
                    throw new FirebaseModuleNotInitializedException(this);

                return FirebaseWebGL_FirebaseAuth_tenantId();
            }
        }

        public void ApplyActionCode(string oobCode, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (oobCode == null)
                throw new ArgumentNullException(nameof(oobCode));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_applyActionCode(oobCode, requestId, OnBoolCallback);
        }

        public void CheckActionCode(string oobCode, Action<FirebaseCallback<FirebaseAuthActionCodeInfo>> firebaseCallback)
        {
            if (oobCode == null)
                throw new ArgumentNullException(nameof(oobCode));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onActionCodeInfoCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_checkActionCode(oobCode, requestId, OnActionCodeInfoCallback);
        }

        public void ConfirmPasswordReset(string oobCode, string newPassword, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (oobCode == null)
                throw new ArgumentNullException(nameof(oobCode));

            if (newPassword == null)
                throw new ArgumentNullException(nameof(newPassword));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_confirmPasswordReset(oobCode, newPassword, requestId, OnBoolCallback);
        }

        public void CreateUserWithEmailAndPassword(string email, string password, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_createUserWithEmailAndPassword(email, password, requestId, OnUserCredentialCallback);
        }

        public void FetchSignInMethodsForEmail(string email, Action<FirebaseCallback<string[]>> firebaseCallback)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onStringArrayCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_fetchSignInMethodsForEmail(email, requestId, OnStringArrayCallback);
        }

        public void InitializeRecaptchaConfig(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            if (_isRecaptchaInitialized)
            {
                firebaseCallback?.Invoke(FirebaseCallback<bool>.Error(FirebaseCallbackErrors.AuthRecaptchaAlreadInitialized));
                return;
            }

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);

                if (callback.success && callback.result)
                {
                    _isRecaptchaInitialized = true;
                }
            });

            FirebaseWebGL_FirebaseAuth_initializeRecaptchaConfig(requestId, OnBoolCallback);
        }

        public bool IsSignInWithEmailLink(string emailLink)
        {
            if (emailLink == null)
                throw new ArgumentNullException(nameof(emailLink));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            return FirebaseWebGL_FirebaseAuth_isSignInWithEmailLink(emailLink);
        }

        public void BeforeAuthStateChanged(Func<FirebaseAuthUser, bool> onBeforeAuthStateChanged)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            this.onBeforeAuthStateChanged = onBeforeAuthStateChanged;

            _onBeforeAuthStateChangeCallbacks[_instanceId] = (callback) =>
            {
                if (callback.success)
                {
                    if (this.onBeforeAuthStateChanged != null)
                    {
                        var user = callback.result;
                        return this.onBeforeAuthStateChanged.Invoke(user);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            };

            FirebaseWebGL_FirebaseAuth_beforeAuthStateChanged(_instanceId, OnBeforeAuthStateChangedCallback);
        }

        public void OnAuthStateChanged(Action<FirebaseAuthUser> onAuthStateChanged)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            this.onAuthStateChanged = onAuthStateChanged;

            _onAuthStateChangeCallbacks[_instanceId] = (callback) =>
            {
                if (callback.success)
                {
                    OnAuthStateChanged(callback.result);
                    this.onAuthStateChanged?.Invoke(callback.result);
                }
            };

            FirebaseWebGL_FirebaseAuth_onAuthStateChanged(_instanceId, OnAuthStateChangedCallback);
        }

        private void OnAuthStateChanged(FirebaseAuthUser currentUser)
        {
            SetCurrentUser(currentUser);
        }

        public void OnIdTokenChanged(Action<FirebaseAuthUser> onIdTokenChanged)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            this.onIdTokenChanged = onIdTokenChanged;

            _onIdTokenChangeCallbacks[_instanceId] = (callback) =>
            {
                if (callback.success)
                {
                    this.onIdTokenChanged?.Invoke(callback.result);
                }
            };

            FirebaseWebGL_FirebaseAuth_onIdTokenChanged(_instanceId, OnIdTokenChangedCallback);
        }

        public void RevokeAccessToken(string token, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_revokeAccessToken(token, requestId, OnBoolCallback);
        }

        public void SendPasswordResetEmail(string email, FirebaseAuthActionCodeSettings actionCodeSettings, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            if (actionCodeSettings == null)
                throw new ArgumentNullException(nameof(actionCodeSettings));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var actionCodeSettingsAsJson = JsonConvert.SerializeObject(actionCodeSettings);
            FirebaseWebGL_FirebaseAuth_sendPasswordResetEmail(email, actionCodeSettingsAsJson, requestId, OnBoolCallback);
        }

        public void SendSignInLinkToEmail(string email, FirebaseAuthActionCodeSettings actionCodeSettings, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            if (actionCodeSettings == null)
                throw new ArgumentNullException(nameof(actionCodeSettings));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var actionCodeSettingsAsJson = JsonConvert.SerializeObject(actionCodeSettings);
            FirebaseWebGL_FirebaseAuth_sendSignInLinkToEmail(email, actionCodeSettingsAsJson, requestId, OnBoolCallback);
        }

        public void SignInAnonymously(Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_signInAnonymously(requestId, OnUserCredentialCallback);
        }

        public void SignInWithCredential(FirebaseAuthCredential credential, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var credentialAsJson = JsonConvert.SerializeObject(credential);
            FirebaseWebGL_FirebaseAuth_signInWithCredential(credentialAsJson, requestId, OnUserCredentialCallback);
        }

        public void SignInWithCustomToken(string customToken, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (customToken == null)
                throw new ArgumentNullException(nameof(customToken));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_signInWithCustomToken(customToken, requestId, OnUserCredentialCallback);
        }

        public void SignInWithEmailAndPassword(string email, string password, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_signInWithEmailAndPassword(email, password, requestId, OnUserCredentialCallback);
        }

        public void SignInWithEmailLink(string email, string emailLink, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            if (emailLink == null)
                throw new ArgumentNullException(nameof(emailLink));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_signInWithEmailLink(email, emailLink, requestId, OnUserCredentialCallback);
        }

        public void SignInWithPopup(string providerId, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            SignInWithPopup(providerId, null, firebaseCallback);
        }

        public void SignInWithPopup(string providerId, Dictionary<string, string> customParameters, Action<FirebaseCallback<FirebaseAuthUserCredential>> firebaseCallback)
        {
            if (providerId == null)
                throw new ArgumentNullException(nameof(providerId));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onUserCredentialCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var customParametersAsJson = customParameters != null ? JsonConvert.SerializeObject(customParameters) : null;
            FirebaseWebGL_FirebaseAuth_signInWithPopup(providerId, customParametersAsJson, requestId, OnUserCredentialCallback);
        }

        public void SignOut(Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_signOut(requestId, OnBoolCallback);
        }

        public void UpdateCurrentUser(FirebaseAuthUser user, Action<FirebaseCallback<bool>> firebaseCallback)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onBoolCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var userAsJson = JsonConvert.SerializeObject(user);
            FirebaseWebGL_FirebaseAuth_updateCurrentUser(userAsJson, requestId, OnBoolCallback);
        }

        public bool UseDeviceLanguage()
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            return FirebaseWebGL_FirebaseAuth_useDeviceLanguage();
        }

        public void ValidatePassword(string password, Action<FirebaseCallback<FirebaseAuthPasswordValidationStatus>> firebaseCallback)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onPasswordValidationStatusCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_validatePassword(password, requestId, OnPasswordValidationStatusCallback);
        }

        public void VerifyPasswordResetCode(string code, Action<FirebaseCallback<string>> firebaseCallback)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseAuth_verifyPasswordResetCode(code, requestId, OnStringCallback);
        }

        public FirebaseAuthActionCodeURL ParseActionCodeURL(string link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var actionCodeUrlAsJson = FirebaseWebGL_FirebaseAuth_parseActionCodeURL(link);
            return JsonConvert.DeserializeObject<FirebaseAuthActionCodeURL>(actionCodeUrlAsJson);
        }

        public FirebaseAuthAdditionalUserInfo GetAdditionalUserInfo(FirebaseAuthUserCredential credential)
        {
            if (!_isInitialized)
                throw new FirebaseModuleNotInitializedException(this);

            var credentialAsJson = JsonConvert.SerializeObject(credential);
            var additionalUserInfoAsJson = FirebaseWebGL_FirebaseAuth_getAdditionalUserInfo(credentialAsJson);
            return JsonConvert.DeserializeObject<FirebaseAuthAdditionalUserInfo>(additionalUserInfoAsJson);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnBoolCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onBoolCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnStringCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onStringCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnStringArrayCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onStringArrayCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnActionCodeInfoCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onActionCodeInfoCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnUserCredentialCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onUserCredentialCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnPasswordValidationStatusCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onPasswordValidationStatusCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static bool OnBeforeAuthStateChangedCallback(string json)
        {
            return FirebaseModuleUtility.InvokeCallback(_onBeforeAuthStateChangeCallbacks, json, doNotRemoveCallback: true);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnAuthStateChangedCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onAuthStateChangeCallbacks, json, doNotRemoveCallback: true);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnIdTokenChangedCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onIdTokenChangeCallbacks, json, doNotRemoveCallback: true);
        }
    }
}
