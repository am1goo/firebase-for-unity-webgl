const remoteConfigLibrary = {
	$remoteConfig: {
		sdk: undefined,
		api: undefined,
		callbacks: {},
		
		initialize: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.firebaseToUnity = window.firebaseToUnity;
			
			if (typeof sdk !== 'undefined') {
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, "already initialized");
				return;
			}
			plugin.sdk = document.firebaseSdk.remoteConfig;
			plugin.api = document.firebaseSdk.remoteConfigApi;
			
			console.log(`[Firebase Remote Config] initialize: requested`);
			plugin.api.isSupported(plugin.sdk).then(function(success) {
				if (success) {
					console.log('[Firebase Remote Config] initialize: initialized');
					plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
				}
				else {
					const error = 'Firebase Remote Config is not supported';
					console.error(`[Firebase Remote Config] initialize: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				}
			}).catch(function(error) {
				console.error(`[Firebase Remote Config] initialize: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		activate: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.activate(plugin.sdk).then(function(success) {
				console.log(`[Firebase Remote Config] activate: ${(success ? 'activated' : 'not activated')}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
			}).catch(function(error) {
				console.error(`[Firebase Remote Config] activate: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		ensureInitialized: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.ensureInitialized(plugin.sdk).then(function() {
				console.log('[Firebase Remote Config] ensureInitialized');
				plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
			}).catch(function(error) {
				console.error(`[Firebase Remote Config] ensureInitialized: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		fetchAndActivate: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.fetchAndActivate(plugin.sdk).then(function(success) {
				console.log(`[Firebase Remote Config] fetchAndActivate: ${(success ? 'activated' : 'not activated')}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
			}).catch(function(error) {
				console.error(`[Firebase Remote Config] fetchAndActivate: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		fetchConfig: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.fetchConfig(plugin.sdk).then(function() {
				console.log('[Firebase Remote Config] fetchConfig');
				plugin.firebaseToUnity(requestId, callbackPtr, true, true, null);
			}).catch(function(error) {
				console.error(`[Firebase Remote Config] fetchConfig: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getBoolean: function(key) {
			const plugin = this;
			return plugin.api.getBoolean(plugin.sdk, key);
		},
		
		getInteger: function(key) {
			const plugin = this;
			return plugin.api.getNumber(plugin.sdk, key);
		},
		
		getString: function(key) {
			const plugin = this;
			return plugin.api.getString(plugin.sdk, key);
		},
		
		onConfigUpdate: function(instanceId, callbackPtr) {
			const plugin = this;
			if (typeof plugin.callbacks.onConfigUpdateUnsubscribe !== 'undefined') {
				plugin.callbacks.onConfigUpdateUnsubscribe();
				plugin.callbacks.onConfigUpdateUnsubscribe = null;
				console.log('[Firebase Remote Config] onConfigUpdate: unsubscribed');
			}
			
			if (callbackPtr == 0)
				return;
			
			plugin.callbacks.onConfigUpdateUnsubscribe = plugin.api.onConfigUpdate(plugin.sdk, {
				next: (configUpdate) => {
					const updatedKeys = Array.from(configUpdate.getUpdatedKeys());
					plugin.firebaseToUnity(instanceId, callbackPtr, true, updatedKeys, error);
					console.log(`[Firebase Remote Config] onConfigUpdate: ${updatedKeys}`);
				},
				error: (error) => {
					console.error(`[Firebase Remote Config] onConfigUpdate: error=${error}`);
					plugin.firebaseToUnity(instanceId, callbackPtr, false, null, error);
				},
				complete: () => {
					console.log("[Firebase Remote Config] onConfigUpdate: listening stopped.");
				}
			});
			console.log('[Firebase Remote Config] onConfigUpdate: subscribed');
		},
		
		setCustomSignals: function(customSignals) {
			const plugin = this;
			plugin.api.setCustomSignals(plugin.sdk, customSignals);
			console.log(`[Firebase Remote Config] setCustomSignals: ${JSON.stringify(customSignals)}`);
		},
		
		setLogLevel: function(logLevel) {
			const plugin = this;
			plugin.api.setLogLevel(plugin.sdk, logLevel);
			console.log(`[Firebase Remote Config] setLogLevel: ${logLevel}`);
		}
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_initialize: function(requestId, callbackPtr) {
		remoteConfig.initialize(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_activate: function(requestId, callbackPtr) {
		remoteConfig.activate(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_ensureInitialized: function(requestId, callbackPtr) {
		remoteConfig.ensureInitialized(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_fetchAndActivate: function(requestId, callbackPtr) {
		remoteConfig.fetchAndActivate(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_fetchConfig: function(requestId, callbackPtr) {
		remoteConfig.fetchConfig(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_getBoolean: function(keyPtr) {
		const key = UTF8ToString(keyPtr);
		return remoteConfig.getBoolean(key);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_getInteger: function(keyPtr) {
		const key = UTF8ToString(keyPtr);
		return remoteConfig.getInteger(key);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_getString: function(keyPtr) {
		const key = UTF8ToString(keyPtr);
		const value = remoteConfig.getString(key);
		
		var buffer = stringToNewUTF8(value);
		setTimeout(() => {
			_free(buffer);
		}, 100);
		return buffer;
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_onConfigUpdate: function(instanceId, callbackPtr) {
		remoteConfig.onConfigUpdate(instanceId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_setCustomSignals: function(customSignalsAsJsonPtr) {
		const customSignalsAsJson = UTF8ToString(customSignalsAsJsonPtr);
		const customSignals = JSON.parse(customSignalsAsJson);
		remoteConfig.setCustomSignals(customSignals);
	},
	
	FirebaseWebGL_FirebaseRemoteConfig_setLogLevel: function(logLevelInt) {
		const conversion = function(value) {
			switch (value) {
				case 0:
					return 'debug';
				case 1:
					return 'error';
				case 2:
				default:
					return 'silent';
			}
		};
		const logLevel = conversion(logLevelInt);
		remoteConfig.setLogLevel(logLevel);
	},
};

autoAddDeps(remoteConfigLibrary, '$remoteConfig');
mergeInto(LibraryManager.library, remoteConfigLibrary);