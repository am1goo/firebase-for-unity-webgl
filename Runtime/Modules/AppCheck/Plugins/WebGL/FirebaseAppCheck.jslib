const firebaseAppCheckLibrary = {
	$firebaseAppCheck: {
		sdk: undefined,
		api: undefined,
		callbacks: {},
	
		initialize: function() {
			const plugin = this;
			plugin.firebaseToUnity = window.firebaseToUnity;
			
			if (typeof sdk !== 'undefined') {
				console.error("[Firebase AppCheck] initialize: already initialized");
				return false;
			}
			
			try {
				plugin.sdk = document.firebaseSdk.appCheck;
				plugin.api = document.firebaseSdk.appCheckApi;
				console.log('[Firebase AppCheck] initialize: initialized');
				return true;
			}
			catch(error) {
				console.error(`[Firebase AppCheck] initialize: failed, error=${error}`);
				return false;
			}
		},
		
		getLimitedUseToken: function(requestId, callbackPtr) {
			const plugin = this;
			try {
				plugin.api.getLimitedUseToken(plugin.sdk).then(function(limitedUseTokenResult) {
					const token = limitedUseTokenResult.token;
					console.log(`[Firebase AppCheck] getLimitedUseToken: limitedUseToken=${token}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, token, null);
				}).catch(function(error) {
					console.error(`[Firebase AppCheck] getLimitedUseToken: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase AppCheck] getLimitedUseToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		getToken: function(forceRefresh, requestId, callbackPtr) {
			const plugin = this;
			try{
				plugin.api.getToken(plugin.sdk, forceRefresh).then(function(tokenResult) {
					const token = tokenResult.token;
					console.log(`[Firebase AppCheck] getToken: token=${token}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, token, null);
				}).catch(function(error) {
					console.error(`[Firebase AppCheck] getToken: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase AppCheck] getToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		onTokenChanged: function(instanceId, callbackPtr) {
			const plugin = this;
			if (typeof plugin.callbacks.onTokenChangedUnsubscribe !== 'undefined') {
				plugin.callbacks.onTokenChangedUnsubscribe();
				plugin.callbacks.onTokenChangedUnsubscribe = null;
				console.log('[Firebase AppCheck] onTokenChanged: unsubscribed');
			}
			
			plugin.callbacks.onTokenChangedUnsubscribe = plugin.api.onTokenChanged(plugin.sdk, {
				next: (tokenResult) => {
					const token = tokenResult.token;
					plugin.firebaseToUnity(instanceId, callbackPtr, true, token, null);
					console.log(`[Firebase AppCheck] onTokenChanged: ${updatedKeys}`);
				},
				error: (error) => {
					console.error(`[Firebase AppCheck] onTokenChanged: error=${error}`);
					plugin.firebaseToUnity(instanceId, callbackPtr, false, null, error);
				},
				complete: () => {
					console.log("[Firebase AppCheck] onTokenChanged: listening stopped.");
				}
			});
			console.log('[Firebase AppCheck] onTokenChanged: subscribed');
		},
		
		setTokenAutoRefreshEnabled: function(isTokenAutoRefreshEnabled) {
			const plugin = this;
			plugin.api.setTokenAutoRefreshEnabled(plugin.sdk, isTokenAutoRefreshEnabled);
			console.log(`[Firebase AppCheck] setTokenAutoRefreshEnabled: isTokenAutoRefreshEnabled=${isTokenAutoRefreshEnabled}`);
		},
	},
	
	FirebaseWebGL_FirebaseAppCheck_initialize: function() {
		return firebaseAppCheck.initialize();
	},
	
	FirebaseWebGL_FirebaseAppCheck_getLimitedUseToken: function(requestId, callbackPtr) {
		firebaseAppCheck.getLimitedUseToken(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAppCheck_getToken: function(forceRefresh, requestId, callbackPtr) {
		firebaseAppCheck.getToken(forceRefresh, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAppCheck_onTokenChanged: function(instanceId, callbackPtr) {
		firebaseAppCheck.onTokenChanged(instanceId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAppCheck_setTokenAutoRefreshEnabled: function(isTokenAutoRefreshEnabled) {
		firebaseAppCheck.setTokenAutoRefreshEnabled(isTokenAutoRefreshEnabled);
	},
};

autoAddDeps(firebaseAppCheckLibrary, '$firebaseAppCheck');
mergeInto(LibraryManager.library, firebaseAppCheckLibrary);
