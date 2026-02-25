const firebaseAuthLibrary = {
	$firebaseAuth: {
		sdk: undefined,
		api: undefined,
		callbacks: {},
	
		initialize: function() {
			const plugin = this;
			plugin.firebaseToUnity = window.firebaseToUnity;
			plugin.firebaseToUnityAndReturnInteger = window.firebaseToUnityAndReturnInteger;
			
			if (typeof sdk !== 'undefined') {
				console.error("[Firebase Auth] initialize: already initialized");
				return false;
			}
			
			try {
				plugin.sdk = document.firebaseSdk.auth;
				plugin.api = document.firebaseSdk.authApi;
				console.log('[Firebase Auth] initialize: initialized');
				return true;
			}
			catch(error) {
				console.error(`[Firebase Auth] initialize: failed, error=${error}`);
				return false;
			}
		},
		
		connectAuthEmulator: function(url, options) {
			const plugin = this;
			plugin.api.connectAuthEmulator(plugin.sdk, url, options);
		},
		
		currentUser: function() {
			const plugin = this;
			try {
				return plugin.sdk.currentUser;
			}
			catch(error) {
				console.error(`[Firebase Auth] currentUser: ${error}`);
				return null;
			}
		},
		
		languageCode: function() {
			const plugin = this;
			try {
				return plugin.sdk.languageCode;
			}
			catch(error) {
				console.error(`[Firebase Auth] languageCode: ${error}`);
				return null;
			}
		},
		
		tenantId: function() {
			const plugin = this;
			try {
				return plugin.sdk.tenantId;
			}
			catch(error) {
				console.error(`[Firebase Auth] tenantId: ${error}`);
				return null;
			}
		},
		
		applyActionCode: function(oobCode, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.applyActionCode(plugin.sdk, oobCode).then(function() {
					console.log('[Firebase Auth] applyActionCode');
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] applyActionCode: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] applyActionCode: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		checkActionCode: function(oobCode, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.checkActionCode(plugin.sdk, oobCode).then(function(actionCodeInfo) {
					console.log(`[Firebase Auth] checkActionCode: actionCodeInfo=${JSON.stringify(actionCodeInfo)}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, actionCodeInfo, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] checkActionCode: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] checkActionCode: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		confirmPasswordReset: function(oobCode, newPassword, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.confirmPasswordReset(plugin.sdk, oobCode, newPassword).then(function() {
					console.log('[Firebase Auth] confirmPasswordReset');
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] confirmPasswordReset: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] confirmPasswordReset: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		createUserWithEmailAndPassword: function(email, password, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.createUserWithEmailAndPassword(plugin.sdk, email, password).then(function(userCredential) {
					console.log('[Firebase Auth] createUserWithEmailAndPassword');
					plugin.firebaseToUnity(requestId, callbackPtr, true, userCredential, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] createUserWithEmailAndPassword: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] createUserWithEmailAndPassword: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		fetchSignInMethodsForEmail: function(email, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.fetchSignInMethodsForEmail(plugin.sdk, email).then(function(signInMethods) {
					console.log(`[Firebase Auth] fetchSignInMethodsForEmail: email=${email}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, signInMethods, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] fetchSignInMethodsForEmail: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] fetchSignInMethodsForEmail: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		initializeRecaptchaConfig: function(requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.initializeRecaptchaConfig(plugin.sdk).then(function() {
					console.log(`[Firebase Auth] initializeRecaptchaConfig`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] initializeRecaptchaConfig: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] initializeRecaptchaConfig: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		isSignInWithEmailLink: function(emailLink) {
			const plugin = this;
			try {
				console.log(`[Firebase Auth] isSignInWithEmailLink: emailLink=${emailLink}`);
				return plugin.api.isSignInWithEmailLink(plugin.sdk, emailLink);
			}
			catch(error) {
				console.error(`[Firebase Auth] isSignInWithEmailLink: ${error}`);
				return false;
			}
		},
		
		beforeAuthStateChanged: function(instanceId, callbackPtr) {
			const plugin = this;
			if (typeof plugin.callbacks.beforeAuthStateChangedUnsubscribe !== 'undefined') {
				plugin.callbacks.beforeAuthStateChangedUnsubscribe();
				plugin.callbacks.beforeAuthStateChangedUnsubscribe = null;
				console.log('[Firebase Auth] beforeAuthStateChanged: unsubscribed');
			}
			
			plugin.callbacks.beforeAuthStateChangedUnsubscribe = plugin.api.beforeAuthStateChanged(plugin.sdk, function(user) {
				console.log(`[Firebase Auth] beforeAuthStateChanged: user=${JSON.stringify(user)}`);
				const result = plugin.firebaseToUnityAndReturnInteger(instanceId, callbackPtr, true, user, null);
				if (!result)
					throw new Error('callback from Unity Engine is failured');
				
			}, function() {
				console.log(`[Firebase Auth] beforeAuthStateChanged: onAbort`);
			});
			console.log('[Firebase Auth] beforeAuthStateChanged: registered callback');
		},
		
		onAuthStateChanged: function(instanceId, callbackPtr) {
			const plugin = this;
			if (typeof plugin.callbacks.onAuthStateChangedUnsubscribe !== 'undefined') {
				plugin.callbacks.onAuthStateChangedUnsubscribe();
				plugin.callbacks.onAuthStateChangedUnsubscribe = null;
				console.log('[Firebase Auth] onAuthStateChanged: unsubscribed');
			}
			
			plugin.callbacks.onAuthStateChangedUnsubscribe = plugin.api.onAuthStateChanged(plugin.sdk, function(user) {
				console.log(`[Firebase Auth] onAuthStateChanged: user=${JSON.stringify(user)}`);
				plugin.firebaseToUnity(instanceId, callbackPtr, true, user, null);
			});
			console.log('[Firebase Auth] onAuthStateChanged: registered callback');
		},
		
		onIdTokenChanged: function(instanceId, callbackPtr) {
			const plugin = this;
			if (typeof plugin.callbacks.onIdTokenChangedUnsubscribe !== 'undefined') {
				plugin.callbacks.onIdTokenChangedUnsubscribe();
				plugin.callbacks.onIdTokenChangedUnsubscribe = null;
				console.log('[Firebase Auth] onIdTokenChanged: unsubscribed');
			}
			
			plugin.callbacks.onIdTokenChangedUnsubscribe = plugin.api.onIdTokenChanged(plugin.sdk, function(user) {
				console.log(`[Firebase Auth] onIdTokenChanged: user=${JSON.stringify(user)}`);
				plugin.firebaseToUnity(instanceId, callbackPtr, true, user, null);
			});
			console.log('[Firebase Auth] onIdTokenChanged: registered callback');
		},
		
		revokeAccessToken: function(token, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.revokeAccessToken(plugin.sdk, token).then(function() {
					console.log(`[Firebase Auth] revokeAccessToken`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] revokeAccessToken: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] revokeAccessToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		sendPasswordResetEmail: function(email, actionCodeSettings, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.sendPasswordResetEmail(plugin.sdk, email, actionCodeSettings).then(function() {
					console.log(`[Firebase Auth] sendPasswordResetEmail`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] sendPasswordResetEmail: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] sendPasswordResetEmail: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		sendSignInLinkToEmail: function(email, actionCodeSettings, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.sendSignInLinkToEmail(plugin.sdk, email, actionCodeSettings).then(function() {
					console.log(`[Firebase Auth] sendSignInLinkToEmail`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] sendSignInLinkToEmail: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] sendSignInLinkToEmail: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		signInAnonymously: function(requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.signInAnonymously(plugin.sdk).then(function(userCredential) {
					console.log(`[Firebase Auth] signInAnonymously`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, userCredential, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] signInAnonymously: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] signInAnonymously: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		signInWithCredential: function(credential, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.signInWithCredential(plugin.sdk, credential).then(function(userCredential) {
					console.log(`[Firebase Auth] signInWithCredential`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, userCredential, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] signInWithCredential: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] signInWithCredential: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		reauthenticateWithCredential: function(user, credential, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.reauthenticateWithCredential(plugin.sdk, user, credential).then(function(userCredential) {
					console.log(`[Firebase Auth] reauthenticateWithCredential`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, userCredential, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] reauthenticateWithCredential: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] reauthenticateWithCredential: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		signInWithCustomToken: function(customToken, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.signInWithCustomToken(plugin.sdk, customToken).then(function(userCredential) {
					console.log(`[Firebase Auth] signInWithCustomToken`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, userCredential, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] signInWithCustomToken: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] signInWithCustomToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		signInWithEmailAndPassword: function(email, password, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.signInWithEmailAndPassword(plugin.sdk, email, password).then(function(userCredential) {
					console.log(`[Firebase Auth] signInWithEmailAndPassword: email=${email}, password=xxxxxxxxx`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, userCredential, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] signInWithEmailAndPassword: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] signInWithEmailAndPassword: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		signInWithEmailLink: function(email, emailLink, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.signInWithEmailLink(plugin.sdk, email, emailLink).then(function(userCredential) {
					console.log(`[Firebase Auth] signInWithEmailLink: email=${email}, emailLink=${emailLink}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, userCredential, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] signInWithEmailLink: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] signInWithEmailLink: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		signOut: function(requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.signOut(plugin.sdk).then(function() {
					console.log(`[Firebase Auth] signOut`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] signOut: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] signOut: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		useDeviceLanguage: function() {
			const plugin = this;
			try {
				console.log(`[Firebase Auth] useDeviceLanguage`);
				return plugin.api.useDeviceLanguage(plugin.sdk);
			}
			catch(error) {
				console.error(`[Firebase Auth] useDeviceLanguage: error=${error}`);
				return false;
			}
		},
		
		updateCurrentUser: function(user, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.updateCurrentUser(plugin.sdk, user).then(function() {
					console.log(`[Firebase Auth] updateCurrentUser`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] updateCurrentUser: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] updateCurrentUser: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		validatePassword: function(password, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.validatePassword(plugin.sdk, password).then(function(passwordValidationStatus) {
					console.log(`[Firebase Auth] validatePassword`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, passwordValidationStatus, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] validatePassword: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] validatePassword: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		verifyPasswordResetCode: function(code, requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.verifyPasswordResetCode(plugin.sdk, password).then(function(email) {
					console.log(`[Firebase Auth] verifyPasswordResetCode`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, email, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] verifyPasswordResetCode: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] verifyPasswordResetCode: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		parseActionCodeURL: function(link) {
			const plugin = this;
			try {
				console.log(`[Firebase Auth] parseActionCodeURL: link=${link}`);
				return plugin.api.parseActionCodeURL(plugin.sdk, link);
			}
			catch(error) {
				console.error(`[Firebase Auth] parseActionCodeURL: error=${error}`);
				return null;
			}
		},
		
		getAdditionalUserInfo: function(credential) {
			const plugin = this;
			try {
				console.log(`[Firebase Auth] getAdditionalUserInfo: link=${JSON.stringify(link)}`);
				return plugin.api.getAdditionalUserInfo(plugin.sdk, credential);
			}
			catch(error) {
				console.error(`[Firebase Auth] getAdditionalUserInfo: error=${error}`);
				return null;
			}
		},
		
		deleteUser: function(uid, requestId, callbackPtr) {
			const plugin = this;
			try {
				const currentUser = plugin.sdk.currentUser;
				if (currentUser.uid != uid)
				{
					console.error(`[Firebase Auth] deleteUser: you're trying to delete a different user`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
					return;
				}
				
				currentUser.delete().then(function() {
					console.log(`[Firebase Auth] deleteUser: uid=${uid} deleted`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] deleteUser: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] deleteUser: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		getIdToken: function(uid, forceRefresh, requestId, callbackPtr) {
			const plugin = this;
			try {
				const currentUser = plugin.sdk.currentUser;
				if (currentUser.uid != uid)
				{
					console.error(`[Firebase Auth] getIdToken: you're trying to delete a different user`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
					return;
				}
				
				currentUser.getIdToken(forceRefresh).then(function(token) {
					console.log(`[Firebase Auth] getIdToken: token=${token}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, token, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] getIdToken: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] getIdToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		getIdTokenResult: function(uid, forceRefresh, requestId, callbackPtr) {
			const plugin = this;
			try {
				const currentUser = plugin.sdk.currentUser;
				if (currentUser.uid != uid)
				{
					console.error(`[Firebase Auth] getIdTokenResult: you're trying to delete a different user`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
					return;
				}
				
				currentUser.getIdTokenResult(forceRefresh).then(function(tokenResult) {
					console.log(`[Firebase Auth] getIdTokenResult: tokenResult=${JSON.stringify(tokenResult)}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, tokenResult, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] getIdTokenResult: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] getIdTokenResult: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		reload: function(uid, requestId, callbackPtr) {
			const plugin = this;
			try {
				const currentUser = plugin.sdk.currentUser;
				if (currentUser.uid != uid)
				{
					console.error(`[Firebase Auth] reload: you're trying to delete a different user`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
					return;
				}
				
				currentUser.reload().then(function() {
					console.log(`[Firebase Auth] reload: reloaded`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
				}).catch(function(error) {
					console.error(`[Firebase Auth] reload: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Auth] reload: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
	},
	
	FirebaseWebGL_FirebaseAuth_initialize: function() {
		return firebaseAuth.initialize();
	},
	
	FirebaseWebGL_FirebaseAuth_connectAuthEmulator: function(urlPtr, optionsAsJsonPtr) {
		const url = UTF8ToString(urlPtr);
		if (optionsAsJsonPtr != 0) {
			const optionsAsJson = UTF8ToString(optionsAsJsonPtr);
			const options = JSON.parse(optionsAsJson);
			return firebaseAuth.connectAuthEmulator(url, options);
		}
		else {
			return firebaseAuth.connectAuthEmulator(url);
		}
	},
	
	FirebaseWebGL_FirebaseAuth_currentUser: function() {
		const user = firebaseAuth.currentUser();
		if (user == null)
			return null;
		
		const userAsJson = JSON.stringify(user);
		return new stringToNewUTF8(userAsJson);
	},
	
	FirebaseWebGL_FirebaseAuth_languageCode: function() {
		const languageCode = firebaseAuth.languageCode();
		if (languageCode == null)
			return null;
		
		return new stringToNewUTF8(languageCode);
	},
	
	FirebaseWebGL_FirebaseAuth_tenantId: function() {
		const tenantId = firebaseAuth.tenantId();
		if (tenantId == null)
			return null;
		
		return new stringToNewUTF8(tenantId);
	},
	
	FirebaseWebGL_FirebaseAuth_applyActionCode: function(oobCodePtr, requestId, callbackPtr) {
		const oobCode = UTF8ToString(oobCodePtr);
		firebaseAuth.applyActionCode(oobCode, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_checkActionCode: function(oobCodePtr, requestId, callbackPtr) {
		const oobCode = UTF8ToString(oobCodePtr);
		firebaseAuth.checkActionCode(oobCode, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_confirmPasswordReset: function(oobCodePtr, newPasswordPtr, requestId, callbackPtr) {
		const oobCode = UTF8ToString(oobCodePtr);
		const newPassword = UTF8ToString(newPasswordPtr);
		firebaseAuth.confirmPasswordReset(oobCode, newPassword, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_createUserWithEmailAndPassword: function(emailPtr, passwordPtr, requestId, callbackPtr) {
		const email = UTF8ToString(emailPtr);
		const password = UTF8ToString(passwordPtr);
		firebaseAuth.createUserWithEmailAndPassword(email, password, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_fetchSignInMethodsForEmail: function(emailPtr, requestId, callbackPtr) {
		const email = UTF8ToString(emailPtr);
		firebaseAuth.fetchSignInMethodsForEmail(email, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_initializeRecaptchaConfig: function(requestId, callbackPtr) {
		firebaseAuth.initializeRecaptchaConfig(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_isSignInWithEmailLink: function(emailLink) {
		return firebaseAuth.isSignInWithEmailLink(emailLink);
	},
	
	FirebaseWebGL_FirebaseAuth_beforeAuthStateChanged: function(instanceId, callbackPtr) {
		firebaseAuth.beforeAuthStateChanged(instanceId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_onAuthStateChanged: function(instanceId, callbackPtr) {
		firebaseAuth.onAuthStateChanged(instanceId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_onIdTokenChanged: function(instanceId, callbackPtr) {
		firebaseAuth.onIdTokenChanged(instanceId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_revokeAccessToken: function(tokenPtr, requestId, callbackPtr) {
		const token = UTF8ToString(tokenPtr);
		firebaseAuth.revokeAccessToken(token, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_sendPasswordResetEmail: function(emailPtr, actionCodeSettingsAsJsonPtr, requestId, callbackPtr) {
		const email = UTF8ToString(emailPtr);
		const actionCodeSettingsAsJson = UTF8ToString(actionCodeSettingsAsJsonPtr);
		const actionCodeSettings = JSON.parse(actionCodeSettingsAsJson);
		firebaseAuth.sendPasswordResetEmail(email, actionCodeSettings, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_sendSignInLinkToEmail: function(emailPtr, actionCodeSettingsAsJsonPtr, requestId, callbackPtr) {
		const email = UTF8ToString(emailPtr);
		const actionCodeSettingsAsJson = UTF8ToString(actionCodeSettingsAsJsonPtr);
		const actionCodeSettings = JSON.parse(actionCodeSettingsAsJson);
		firebaseAuth.sendSignInLinkToEmail(email, actionCodeSettings, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_signInAnonymously: function(requestId, callbackPtr) {
		firebaseAuth.signInAnonymously(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_signInWithCredential: function(credentialAsJsonPtr, requestId, callbackPtr) {
		const credentialAsJson = UTF8ToString(credentialAsJsonPtr);
		const credential = JSON.parse(credentialAsJson);
		firebaseAuth.signInWithCredential(credential, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_reauthenticateWithCredential: function(userAsJsonPtr, credentialAsJsonPtr, requestId, callbackPtr) {
		const userAsJson = UTF8ToString(userAsJsonPtr);
		const user = JSON.parse(userAsJson);
		const credentialAsJson = UTF8ToString(credentialAsJsonPtr);
		const credential = JSON.parse(credentialAsJson);
		firebaseAuth.reauthenticateWithCredential(user, credential, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_signInWithCustomToken: function(customTokenPtr, requestId, callbackPtr) {
		const customToken = UTF8ToString(customTokenPtr);
		firebaseAuth.signInWithCustomToken(customToken, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_signInWithEmailAndPassword: function(emailPtr, passwordPtr, requestId, callbackPtr) {
		const email = UTF8ToString(emailPtr);
		const password = UTF8ToString(passwordPtr);
		firebaseAuth.signInWithEmailAndPassword(email, password, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_signInWithEmailLink: function(emailPtr, emailLinkPtr, requestId, callbackPtr) {
		const email = UTF8ToString(emailPtr);
		const emailLink = UTF8ToString(emailLinkPtr);
		firebaseAuth.signInWithEmailLink(email, emailLink, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_signOut: function(requestId, callbackPtr) {
		firebaseAuth.signOut(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_updateCurrentUser: function(userAsJsonPtr, requestId, callbackPtr) {
		const userAsJson = UTF8ToString(userAsJsonPtr);
		const user = UTF8ToString(userAsJson);
		firebaseAuth.updateCurrentUser(user, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_useDeviceLanguage: function() {
		firebaseAuth.useDeviceLanguage();
	},
	
	FirebaseWebGL_FirebaseAuth_validatePassword: function(passwordPtr, requestId, callbackPtr) {
		const password = UTF8ToString(passwordPtr);
		firebaseAuth.validatePassword(password, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_verifyPasswordResetCode: function(codePtr, requestId, callbackPtr) {
		const code = UTF8ToString(codePtr);
		firebaseAuth.verifyPasswordResetCode(code, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_parseActionCodeURL: function(linkPtr) {
		const link = UTF8ToString(linkPtr);
		const actionCodeUrl = firebaseAuth.parseActionCodeURL(link);
		const actionCodeUrlAsJson = JSON.stringify(actionCodeUrl);
		return stringToNewUTF8(actionCodeUrlAsJson);
	},
	
	FirebaseWebGL_FirebaseAuth_getAdditionalUserInfo: function(credentialAsJsonPtr) {
		const credentialAsJson = UTF8ToString(credentialAsJsonPtr);
		const credential = JSON.parse(credentialAsJson);
		
		const additionalUserInfo = firebaseAuth.getAdditionalUserInfo(credential);
		if (additionalUserInfo == null)
			return null;
		
		const additionalUserInfoAsJson = JSON.stringify(additionalUserInfo);
		return stringToNewUTF8(additionalUserInfoAsJson);
	},
	
	FirebaseWebGL_FirebaseAuth_User_deleteUser: function(uidPtr, requestId, callbackPtr) {
		const uid = UTF8ToString(uidPtr);
		firebaseAuth.deleteUser(uid, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_User_getIdToken: function(uidPtr, forceRefresh, requestId, callbackPtr) {
		const uid = UTF8ToString(uidPtr);
		firebaseAuth.getIdToken(uid, forceRefresh, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_User_getIdTokenResult: function(uidPtr, forceRefresh, requestId, callbackPtr) {
		const uid = UTF8ToString(uidPtr);
		firebaseAuth.getIdTokenResult(uid, forceRefresh, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAuth_User_reload: function(uidPtr, requestId, callbackPtr) {
		const uid = UTF8ToString(uidPtr);
		firebaseAuth.reload(uid, requestId, callbackPtr);
	},
};

autoAddDeps(firebaseAuthLibrary, '$firebaseAuth');
mergeInto(LibraryManager.library, firebaseAuthLibrary);