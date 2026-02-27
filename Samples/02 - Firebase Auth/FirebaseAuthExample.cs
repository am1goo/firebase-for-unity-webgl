using System;
using System.Collections;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseAuthExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseAuth _auth;

        private bool _emulatorConnected;
        private string _emulatorUrl = "http://127.0.0.1:9099";

        private bool _authAuthBlocked;
        private string _authIdToken;
        private FirebaseAuthIdTokenResult _authIdTokenResult;

        private const string authEmail = "user@test.com";
        private const string authPassword = "1234567890@ABC";

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _auth = app.Auth;
            if (_auth == null)
            {
                Debug.LogError($"Start: {nameof(IFirebaseAuth)} is not injected");
                yield break;
            }
            _auth.onLoggedUserChanged = (loggerUser) =>
            {
                Debug.Log($"OnLoggedUserChanged: uid={loggerUser?.uid}");
            };

            bool? initialized = null;
            _auth.Initialize((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"Initialize: {callback.error}");
                    return;
                }
                initialized = callback.result;
            });
            yield return new WaitUntil(() => initialized != null);
            if (initialized.Value == false)
            {
                Debug.LogError("Initialize: not initialized");
                yield break;
            }

            _auth.languageCode = "en";
            _auth.UseDeviceLanguage();
            _auth.BeforeAuthStateChanged(BeforeAuthStateChanged);
            _auth.BeforeAuthStateChanged(BeforeAuthStateChanged);
            _auth.OnAuthStateChanged(OnAuthStateChanged);
            _auth.OnAuthStateChanged(OnAuthStateChanged);
            _auth.OnIdTokenChanged(OnIdTokenChanged);
            _auth.OnIdTokenChanged(OnIdTokenChanged);

            bool BeforeAuthStateChanged(FirebaseAuthUser user)
            {
                Debug.Log($"BeforeAuthStateChanged: user={user}");
                return !_authAuthBlocked;
            }

            void OnAuthStateChanged(FirebaseAuthUser user)
            {
                Debug.Log($"OnAuthStateChanged: user={user}");
            }

            void OnIdTokenChanged(FirebaseAuthUser user)
            {
                Debug.Log($"OnIdTokenChanged: user={user}");
            }
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_auth == null)
                return;

            GUILayout.Label("Auth:");
            GUILayout.Label($"- initialized: {_auth.isInitialized}");
            if (_auth.isInitialized)
            {
                if (!_emulatorConnected)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Emulator Url:");
                    _emulatorUrl = GUILayout.TextField(_emulatorUrl);
                    GUILayout.EndHorizontal();

                    var prevEnabled = GUI.enabled;
                    GUI.enabled = !string.IsNullOrWhiteSpace(_emulatorUrl);
                    if (GUILayout.Button("Connect to Emulator"))
                    {
                        try
                        {
                            _auth.ConnectAuthEmulator(_emulatorUrl, options: new FirebaseAuthEmulatorOptions
                            {
                                disableWarnings = false,
                            });
                            _emulatorConnected = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            _emulatorConnected = false;
                        }
                    }
                    GUI.enabled = prevEnabled;
                }
                else
                {
                    GUILayout.Label($"- user: {_auth.loggedUser?.uid}");
                    GUILayout.Label($"- languageCode: {_auth.languageCode}");
                    GUILayout.Label($"- tenantId: {_auth.tenantId}");

                    var prevEnabled = GUI.enabled;
                    GUI.enabled = _auth.loggedUser == null;
                    _authAuthBlocked = GUILayout.Toggle(_authAuthBlocked, "Is Sign-in Blocked");
                    if (GUILayout.Button("Sign Anonymously"))
                    {
                        _auth.SignInAnonymously((callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignInAnonymously: {callback.error}");
                                return;
                            }

                            var user = callback.result.user;
                            Debug.Log($"SignInAnonymously: uid={user.uid}, displayName={user.displayName}");

                            _auth.loggedUser.UpdateEmail("test@test.xyz", (callback) =>
                            {
                                if (callback.success == false)
                                {
                                    Debug.LogError($"UpdateEmail: {callback.error}");
                                    return;
                                }

                                var updated = callback.result;
                                Debug.Log($"UpdateEmail: updated={updated}");
                            });

                            _auth.loggedUser.UpdateProfile("yobayo", null, (callback) =>
                            {
                                if (callback.success == false)
                                {
                                    Debug.LogError($"UpdateProfile: {callback.error}");
                                    return;
                                }

                                var updated = callback.result;
                                Debug.Log($"UpdateProfile: updated={updated}");
                            });

                            _auth.loggedUser.UpdatePassword("321456987", (callback) =>
                            {
                                if (callback.success == false)
                                {
                                    Debug.LogError($"UpdatePassword: {callback.error}");
                                    return;
                                }

                                var updated = callback.result;
                                Debug.Log($"UpdatePassword: updated={updated}");
                            });
                        });
                    }
                    if (GUILayout.Button("Sign with Email and Password"))
                    {
                        _auth.CreateUserWithEmailAndPassword(authEmail, authPassword, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                if (callback.error.Contains("auth/email-already-in-use"))
                                {
                                    _auth.SignInWithEmailAndPassword(authEmail, authPassword, (callback) =>
                                    {
                                        if (callback.success == false)
                                        {
                                            Debug.LogError($"SignInWithEmailAndPassword: {callback.error}");
                                            return;
                                        }

                                        var user = callback.result.user;
                                        Debug.Log($"SignInWithEmailAndPassword: uid={user.uid}, displayName={user.displayName}");
                                    });
                                }
                                else
                                {
                                    Debug.LogError($"CreateUserWithEmailAndPassword: {callback.error}");
                                    return;
                                }
                            }

                            var user = callback.result.user;
                            Debug.Log($"CreateUserWithEmailAndPassword: uid={user.uid}, displayName={user.displayName}");

                            _auth.SignOut((callback) =>
                            {
                                if (callback.success == false)
                                {
                                    Debug.LogError($"SignOut: {callback.error}");
                                    return;
                                }

                                Debug.Log($"SignOut: {callback.result}");

                                _auth.SignInWithEmailAndPassword(authEmail, authPassword, (callback) =>
                                {
                                    if (callback.success == false)
                                    {
                                        Debug.LogError($"SignInWithEmailAndPassword: {callback.error}");
                                        return;
                                    }

                                    var user = callback.result.user;
                                    Debug.Log($"SignInWithEmailAndPassword: uid={user.uid}, displayName={user.displayName}");
                                });
                            });
                        });
                    }
                    if (GUILayout.Button("Sign with Google"))
                    {
                        var providerId = FirebaseAuthProviders.google;
                        _auth.SignInWithPopup(providerId, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignInWithPopup: providerId={providerId}, error={callback.error}");
                                return;
                            }

                            Debug.Log($"SignInWithPopup: providerId={providerId}, signedIn={callback.result}");
                        });
                    }
                    if (GUILayout.Button("Sign with Apple"))
                    {
                        var providerId = FirebaseAuthProviders.apple;
                        _auth.SignInWithPopup(providerId, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignInWithPopup: providerId={providerId}, error={callback.error}");
                                return;
                            }

                            Debug.Log($"SignInWithPopup: providerId={providerId}, signedIn={callback.result}");
                        });
                    }
                    if (GUILayout.Button("Sign with GitHub"))
                    {
                        var providerId = FirebaseAuthProviders.github;
                        _auth.SignInWithPopup(providerId, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignInWithPopup: providerId={providerId}, error={callback.error}");
                                return;
                            }

                            Debug.Log($"SignInWithPopup: providerId={providerId}, signedIn={callback.result}");
                        });
                    }
                    if (GUILayout.Button("Sign with Twitter"))
                    {
                        var providerId = FirebaseAuthProviders.twitter;
                        _auth.SignInWithPopup(providerId, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignInWithPopup: providerId={providerId}, error={callback.error}");
                                return;
                            }

                            Debug.Log($"SignInWithPopup: providerId={providerId}, signedIn={callback.result}");
                        });
                    }
                    if (GUILayout.Button("Sign with Facebook"))
                    {
                        var providerId = FirebaseAuthProviders.facebook;
                        _auth.SignInWithPopup(providerId, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignInWithPopup: providerId={providerId}, error={callback.error}");
                                return;
                            }

                            Debug.Log($"SignInWithPopup: providerId={providerId}, signedIn={callback.result}");
                        });
                    }
                    if (GUILayout.Button("Sign with Microsoft"))
                    {
                        var providerId = FirebaseAuthProviders.microsoft;
                        _auth.SignInWithPopup(providerId, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignInWithPopup: providerId={providerId}, error={callback.error}");
                                return;
                            }

                            Debug.Log($"SignInWithPopup: providerId={providerId}, signedIn={callback.result}");
                        });
                    }
                    if (GUILayout.Button("Sign with Yahoo"))
                    {
                        var providerId = FirebaseAuthProviders.yahoo;
                        _auth.SignInWithPopup(providerId, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignInWithPopup: providerId={providerId}, error={callback.error}");
                                return;
                            }

                            Debug.Log($"SignInWithPopup: providerId={providerId}, signedIn={callback.result}");
                        });
                    }
                    if (GUILayout.Button("Fetch SignIn Methods"))
                    {
                        _auth.FetchSignInMethodsForEmail(authEmail, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"FetchSignInMethodsForEmail: {callback.error}");
                                return;
                            }

                            var signinMethods = callback.result;
                            Debug.Log($"FetchSignInMethodsForEmail: initialized={string.Join(", ", signinMethods)}");
                        });
                    }
                    GUI.enabled = _auth.loggedUser != null;
                    GUILayout.Label($"- user id token: {_authIdToken}");
                    if (GUILayout.Button("Get Id Token"))
                    {
                        _auth.loggedUser.GetIdToken(forceRefresh: true, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"GetIdToken: {callback.error}");
                                return;
                            }

                            _authIdToken = callback.result;
                            Debug.Log($"GetIdToken: token={_authIdToken}");
                        });
                    }
                    GUILayout.Label($"- user id token result: {_authIdTokenResult?.token}");
                    if (GUILayout.Button("Get Id Token Result"))
                    {
                        _auth.loggedUser.GetIdTokenResult(forceRefresh: true, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"GetIdTokenResult: {callback.error}");
                                return;
                            }

                            _authIdTokenResult = callback.result;
                            Debug.Log($"GetIdTokenResult: tokenResult={_authIdTokenResult}");
                        });
                    }
                    if (GUILayout.Button("Reload"))
                    {
                        _auth.loggedUser.Reload((callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"Reload: {callback.error}");
                                return;
                            }

                            var reloaded = callback.result;
                            Debug.Log($"Reload: reloaded={reloaded}");
                        });
                    }
                    if (GUILayout.Button("Delete User"))
                    {
                        _auth.loggedUser.DeleteUser((callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"DeleteUser: {callback.error}");
                                return;
                            }

                            var deleted = callback.result;
                            Debug.Log($"DeleteUser: deleted={deleted}");
                        });
                    }
                    if (GUILayout.Button("Sign Out"))
                    {
                        _auth.SignOut((callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"SignOut: {callback.error}");
                                return;
                            }

                            var signedOut = callback.result;
                            Debug.Log($"SignOut: signedOut={signedOut}");
                        });
                    }
                    GUI.enabled = prevEnabled;
                    if (GUILayout.Button("Initialize ReCAPTCHA"))
                    {
                        _auth.InitializeRecaptchaConfig((callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"InitializeRecaptchaConfig: {callback.error}");
                                return;
                            }

                            var initialized = callback.result;
                            Debug.Log($"InitializeRecaptchaConfig: initialized={initialized}");
                        });
                    }
                    if (GUILayout.Button("Validate Password"))
                    {
                        _auth.ValidatePassword(authPassword, (callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"ValidatePassword: {callback.error}");
                                return;
                            }

                            Debug.Log($"ValidatePassword: {callback.result}");
                        });
                    }
                }
            }
        }
#endif
    }
}
