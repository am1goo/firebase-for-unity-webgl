const analyticsLibrary = {
	$analytics: {
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
			
			console.log(`initialize: requested`);
			plugin.api.isSupported(plugin.sdk).then(function(success) {
				if (success) {
					console.log(`initialize: initialized`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
				}
				else {
					const error = 'Firebase Analytics is not supported';
					console.error(`initialize: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				}
			}).catch(function(error) {
				console.error(`initialize: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getGoogleAnalyticsClientId: function(requestId, callbackPtr) {
			const plugin = this;
			console.log(`getGoogleAnalyticsClientId: requested`);
			plugin.api.getGoogleAnalyticsClientId(plugin.sdk).then(function(clientId) {
				console.log(`getGoogleAnalyticsClientId: ${clientId}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, clientId, null);
			}).catch(function(error) {
				console.error(`getGoogleAnalyticsClientId: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		setAnalyticsCollectionEnabled: function(enabled) {
			const plugin = this;
			plugin.api.setAnalyticsCollectionEnabled(plugin.sdk, enabled);
			console.log(`setAnalyticsCollectionEnabled: ${enabled}`);
		},
		
		setUserId: function(userId) {
			const plugin = this;
			plugin.api.setUserId(plugin.sdk, userId);
			console.log(`setUserId: ${userId}`);
		},
		
		setUserProperties: function(properties) {
			const plugin = this;
			plugin.api.setUserProperties(plugin.sdk, properties);
			console.log(`setUserProperties: ${JSON.stringify(properties)}`);
		},
		
		setDefaultEventParameters: function(parameters) {
			const plugin = this;
			plugin.api.setDefaultEventParameters(plugin.sdk, parameters);
			console.log(`setDefaultEventParameters: ${JSON.stringify(parameters)}`);
		},
		
		setConsent: function(consent) {
			const plugin = this;
			plugin.api.setConsent(plugin.sdk, consent);
			console.log(`setConsent: ${JSON.stringify(consent)}`);
		},
		
		logEvent: function(eventName, eventParams) {
			const plugin = this;
			if (eventParams != null) {
				plugin.api.logEvent(plugin.sdk, eventName, eventParams);
				console.log(`logEvent: name=${eventName}, params=[${JSON.stringify(eventParams)}]`);
			}
			else {
				plugin.api.logEvent(plugin.sdk, eventName);
				console.log(`logEvent: name=${eventName}`);
			}
		},
	},	
	
	FirebaseWebGL_FirebaseAnalytics_initialize: function(requestId, callbackPtr) {
		analytics.initialize(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAnalytics_getGoogleAnalyticsClientId: function(requestId, callbackPtr) {
		analytics.getGoogleAnalyticsClientId(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setAnalyticsCollectionEnabled: function(enabled) {
		analytics.setAnalyticsCollectionEnabled(enabled);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setUserId: function (userIdPtr) {
		const userId = UTF8ToString(userIdPtr);
		analytics.setUserId(userId);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setUserProperties: function (propertiesAsJsonPtr) {
		const propertiesAsJson = UTF8ToString(propertiesAsJsonPtr);
		const properties = JSON.parse(propertiesAsJson);
		analytics.setUserProperties(properties);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setDefaultEventParameters: function (parametersAsJsonPtr) {
		const parametersAsJson = UTF8ToString(parametersAsJsonPtr);
		const parameters = JSON.parse(parametersAsJson);
		analytics.setDefaultEventParameters(parameters);
	},
	
	FirebaseWebGL_FirebaseAnalytics_setConsent: function (consentAsJsonPtr) {
		const consentAsJson = UTF8ToString(consentAsJsonPtr);
		const consent = JSON.parse(consentAsJson);
		analytics.setConsent(consent);
	},
	
	FirebaseWebGL_FirebaseAnalytics_logEvent: function (eventNamePtr, eventParamsPtr) {
		const eventName = UTF8ToString(eventNamePtr);
		const eventParams = eventParamsPtr != 0 ? UTF8ToString(eventParamsPtr) : null;
		analytics.logEvent(eventName, eventParams);
	},
};

autoAddDeps(analyticsLibrary, '$analytics');
mergeInto(LibraryManager.library, analyticsLibrary);

