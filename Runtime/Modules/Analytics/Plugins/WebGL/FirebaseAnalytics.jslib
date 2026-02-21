const analyticsLibrary = {
	$analytics: {
		sdk: undefined,
		api: undefined,
		firebaseToUnity: undefined,
		
		initialize: function(requestId, callbackPtr) {
			this.firebaseToUnity = window.firebaseToUnity;
			
			if (typeof sdk !== 'undefined') {
				this.firebaseToUnity(requestId, callbackPtr, false, null, "already initialized");
				return;
			}
			this.sdk = document.firebaseSdk.analytics;
			this.api = document.firebaseSdk.analyticsApi;
			
			console.log(`initialize: requested`);
			this.api.isSupported(this.sdk).then(function(success) {
				console.log(`initialize: ${(success ? "initialized" : "not initialized")}`);
				this.firebaseToUnity(requestId, callbackPtr, true, success, null);
			}).catch(function(error) {
				console.error(`initialize: ${error}`);
				this.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getGoogleAnalyticsClientId: function(requestId, callbackPtr) {
			console.log(`getGoogleAnalyticsClientId: requested`);
			this.api.getGoogleAnalyticsClientId(this.sdk).then(function(clientId) {
				console.log(`getGoogleAnalyticsClientId: ${clientId}`);
				this.firebaseToUnity(requestId, callbackPtr, true, clientId, null);
			}).catch(function(error) {
				console.error(`getGoogleAnalyticsClientId: ${error}`);
				this.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		setAnalyticsCollectionEnabled: function(enabled) {
			this.api.setAnalyticsCollectionEnabled(this.sdk, enabled);
			console.log(`setAnalyticsCollectionEnabled: ${enabled}`);
		},
		
		setUserId: function(userId) {
			this.api.setUserId(this.sdk, userId);
			console.log(`setUserId: ${userId}`);
		},
		
		setUserProperties: function(properties) {
			this.api.setUserProperties(this.sdk, properties);
			console.log(`setUserProperties: ${JSON.stringify(properties)}`);
		},
		
		setDefaultEventParameters: function(parameters) {
			this.api.setDefaultEventParameters(this.sdk, parameters);
			console.log(`setDefaultEventParameters: ${JSON.stringify(parameters)}`);
		},
		
		setConsent: function(consent) {
			this.api.setConsent(this.sdk, consent);
			console.log(`setConsent: ${JSON.stringify(consent)}`);
		},
		
		logEvent: function(eventName, eventParams) {
			if (eventParams != null) {
				this.api.logEvent(this.sdk, eventName, eventParams);
				console.log(`logEvent: name=${eventName}, params=[${JSON.stringify(eventParams)}]`);
			}
			else {
				this.api.logEvent(this.sdk, eventName);
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

