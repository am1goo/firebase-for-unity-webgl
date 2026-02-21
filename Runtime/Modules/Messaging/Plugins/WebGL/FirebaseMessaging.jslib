const messagingLibrary = {
	$messaging: {
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
			plugin.sdk = document.firebaseSdk.messaging;
			plugin.api = document.firebaseSdk.messagingApi;
			
			console.log(`initialize: requested`);
			plugin.api.isSupported(plugin.sdk).then(function(success) {
				if (success) {
					if ('serviceWorker' in navigator) {
						navigator.serviceWorker.register('./firebase-messaging-sw.js').then((registration) => {
							console.log(`initialize: scope=${registration.scope}`);
							plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
						}).catch((error) => {
							console.error(`initialize: ${error}`);
							plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
						});
					}
					else {
						const error = 'Firebase Messaging Service Worker cannot be registered';
						console.error(`initialize: ${error}`);
						plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
					}
				}
				else {
					const error = 'Firebase Messaging is not supported';
					console.error(`initialize: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				}
			}).catch(function(error) {
				console.error(`initialize: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getToken: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.getToken(plugin.sdk).then(function(token) {
				console.log(`getToken: token=${token}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, token, null);
			}).catch(function(error) {
				console.error(`getToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		deleteToken: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.deleteToken(plugin.sdk).then(function(success) {
				console.log(`deleteToken: ${(success ? "deleted" : "not deleted")}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
			}).catch(function(error) {
				console.error(`deleteToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		onMessage: function(instanceId, callbackPtr) {
			const plugin = this;
			if (typeof plugin.callbacks.onMessageUnsubscribe !== 'undefined') {
				plugin.callbacks.onMessageUnsubscribe();
				plugin.callbacks.onMessageUnsubscribe = null;
				console.log('onMessage: unsubscribed');
			}
			
			if (callbackPtr == 0)
				return;
			
			plugin.callbacks.onMessageUnsubscribe = plugin.api.onMessage(plugin.sdk, function(payload) {
				plugin.firebaseToUnity(instanceId, callbackPtr, true, payload, null);
				console.log(`onMessage: payload=${payload}`);
			});
			console.log('onMessage: subscribed');
		},
	},
	
	FirebaseWebGL_FirebaseMessaging_initialize: function(requestId, callbackPtr) {
		messaging.initialize(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseMessaging_getToken: function(requestId, callbackPtr) {
		messaging.getToken(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseMessaging_deleteToken: function(requestId, callbackPtr) {
		messaging.deleteToken(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseMessaging_onMessage: function(instanceId, callbackPtr) {
		messaging.onMessage(instanceId, callbackPtr);
	},
};

const messagingSWLibrary = {
	$messagingSW: {
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
			plugin.sdk = document.firebaseSdk.messagingSw;
			plugin.api = document.firebaseSdk.messagingSwApi;
			
			console.log(`initialize: requested`);
			plugin.api.isSupported(plugin.sdk).then(function(success) {
				if (success) {
					console.log(`initialize: initialized`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
				}
				else {
					const error = 'Firebase Messaging is not supported';
					console.error(`initialize: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				}
			}).catch(function(error) {
				console.error(`initialize: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		onBackgroundMessage: function(instanceId, callbackPtr) {
			const plugin = this;
			if (typeof plugin.callbacks.onMessageUnsubscribe !== 'undefined') {
				plugin.callbacks.onMessageUnsubscribe();
				plugin.callbacks.onMessageUnsubscribe = null;
				console.log('onBackgroundMessage: unsubscribed');
			}
			
			if (callbackPtr == 0)
				return;
			
			plugin.callbacks.onMessageUnsubscribe = plugin.api.onBackgroundMessage(plugin.sdk, function(payload) {
				plugin.firebaseToUnity(instanceId, callbackPtr, true, payload, null);
				console.log(`onBackgroundMessage: payload=${payload}`);
			});
			console.log('onBackgroundMessage: subscribed');
		},
		
		experimentalSetDeliveryMetricsExportedToBigQueryEnabled: function(enabled) {
			const plugin = this;
			plugin.api.experimentalSetDeliveryMetricsExportedToBigQueryEnabled(plugin.sdk, enabled);
			console.log(`experimentalSetDeliveryMetricsExportedToBigQueryEnabled: enabled=${enabled}`);
		},
	},
	
	FirebaseWebGL_FirebaseMessaging_ServiceWorker_initialize: function(requestId, callbackPtr) {
		messagingSW.initialize(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseMessaging_ServiceWorker_experimentalSetDeliveryMetricsExportedToBigQueryEnabled: function(enabled) {
		messagingSW.experimentalSetDeliveryMetricsExportedToBigQueryEnabled(enabled);
	},
	
	FirebaseWebGL_FirebaseMessaging_ServiceWorker_onBackgroundMessage: function(instanceId, callbackPtr) {
		messagingSW.onBackgroundMessage(instanceId, callbackPtr);
	},
};

autoAddDeps(messagingLibrary, '$messaging');
mergeInto(LibraryManager.library, messagingLibrary);

autoAddDeps(messagingSWLibrary, '$messagingSW');
mergeInto(LibraryManager.library, messagingSWLibrary);

