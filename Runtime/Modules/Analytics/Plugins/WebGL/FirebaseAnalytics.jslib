const firebaseAnalyticsLibrary = {
	$firebaseAnalytics: {
		sdk: undefined,
		api: undefined,
		firebaseToUnity: undefined,
		
		initialize: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.firebaseToUnity = window.firebaseToUnity;
			
			if (typeof sdk !== 'undefined') {
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, "already initialized");
				return;
			}
			plugin.sdk = document.firebaseSdk.analytics;
			plugin.api = document.firebaseSdk.analyticsApi;
			
			console.log(`[Firebase Analytics] initialize: requested`);
			plugin.api.isSupported(plugin.sdk).then(function(success) {
				if (success) {
					console.log(`[Firebase Analytics] initialize: initialized`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
				}
				else {
					const error = 'Firebase Analytics is not supported';
					console.error(`[Firebase Analytics] initialize: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				}
			}).catch(function(error) {
				console.error(`[Firebase Analytics] initialize: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getGoogleAnalyticsClientId: function(requestId, callbackPtr) {
			const plugin = this;
			console.log(`[Firebase Analytics] getGoogleAnalyticsClientId: requested`);
			plugin.api.getGoogleAnalyticsClientId(plugin.sdk).then(function(clientId) {
				console.log(`[Firebase Analytics] getGoogleAnalyticsClientId: ${clientId}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, clientId, null);
			}).catch(function(error) {
				console.error(`[Firebase Analytics] getGoogleAnalyticsClientId: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		setAnalyticsCollectionEnabled: function(enabled) {
			const plugin = this;
			plugin.api.setAnalyticsCollectionEnabled(plugin.sdk, enabled);
			console.log(`[Firebase Analytics] setAnalyticsCollectionEnabled: ${enabled}`);
		},
		
		setUserId: function(userId) {
			const plugin = this;
			plugin.api.setUserId(plugin.sdk, userId);
			console.log(`[Firebase Analytics] setUserId: ${userId}`);
		},
		
		setUserProperties: function(properties) {
			const plugin = this;
			plugin.api.setUserProperties(plugin.sdk, properties);
			console.log(`[Firebase Analytics] setUserProperties: ${JSON.stringify(properties)}`);
		},
		
		setDefaultEventParameters: function(parameters) {
			const plugin = this;
			plugin.api.setDefaultEventParameters(plugin.sdk, parameters);
			console.log(`[Firebase Analytics] setDefaultEventParameters: ${JSON.stringify(parameters)}`);
		},
		
		setConsent: function(consent) {
			const plugin = this;
			plugin.api.setConsent(plugin.sdk, consent);
			console.log(`[Firebase Analytics] setConsent: ${JSON.stringify(consent)}`);
		},
		
		logEvent: function(eventName, eventParams) {
			const plugin = this;
			if (eventParams != null) {
				plugin.api.logEvent(plugin.sdk, eventName, eventParams);
				console.log(`[Firebase Analytics] logEvent: name=${eventName}, params=[${JSON.stringify(eventParams)}]`);
			}
			else {
				plugin.api.logEvent(plugin.sdk, eventName);
				console.log(`[Firebase Analytics] logEvent: name=${eventName}`);
			}
		},
	},	
	
	FirebaseWebGL_FirebaseAnalytics_initialize: function(requestId, callbackPtr) {
		firebaseAnalytics.initialize(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAnalytics_getGoogleAnalyticsClientId: function(requestId, callbackPtr) {
		firebaseAnalytics.getGoogleAnalyticsClientId(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setAnalyticsCollectionEnabled: function(enabled) {
		firebaseAnalytics.setAnalyticsCollectionEnabled(enabled);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setUserId: function (userIdPtr) {
		const userId = UTF8ToString(userIdPtr);
		firebaseAnalytics.setUserId(userId);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setUserProperties: function (propertiesAsJsonPtr) {
		const propertiesAsJson = UTF8ToString(propertiesAsJsonPtr);
		const properties = JSON.parse(propertiesAsJson);
		firebaseAnalytics.setUserProperties(properties);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setDefaultEventParameters: function (parametersAsJsonPtr) {
		const parametersAsJson = UTF8ToString(parametersAsJsonPtr);
		const parameters = JSON.parse(parametersAsJson);
		firebaseAnalytics.setDefaultEventParameters(parameters);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setConsent: function (consentAsJsonPtr) {
		const consentAsJson = UTF8ToString(consentAsJsonPtr);
		const consent = JSON.parse(consentAsJson);
		firebaseAnalytics.setConsent(consent);
	},
	
	FirebaseWebGL_FirebaseAnalytics_logEvent: function (eventNamePtr, eventParamsPtr) {
		const eventName = UTF8ToString(eventNamePtr);
		const eventParams = eventParamsPtr != 0 ? UTF8ToString(eventParamsPtr) : null;
		firebaseAnalytics.logEvent(eventName, eventParams);
	},
};

autoAddDeps(firebaseAnalyticsLibrary, '$firebaseAnalytics');
mergeInto(LibraryManager.library, firebaseAnalyticsLibrary);

