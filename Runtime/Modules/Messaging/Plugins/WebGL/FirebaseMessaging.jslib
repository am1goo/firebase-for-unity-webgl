const firebaseMessagingLibrary = {
	$firebaseMessaging: {
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
			
			console.log(`[Firebase Messaging] initialize: requested`);
			plugin.api.isSupported(plugin.sdk).then(function(success) {
				if (success) {
					if ('serviceWorker' in navigator) {
						navigator.serviceWorker.register('./firebase-messaging-sw.js').then(function(registration) {
							console.log(`[Firebase Messaging] initialize: scope=${registration.scope}`);
							plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
						}).catch(function(error) {
							console.error(`[Firebase Messaging] initialize: ${error}`);
							plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
						});
					}
					else {
						const error = 'Firebase Messaging cannot be registered';
						console.error(`[Firebase Messaging] initialize: ${error}`);
						plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
					}
				}
				else {
					const error = 'Firebase Messaging is not supported';
					console.error(`[Firebase Messaging] initialize: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				}
			}).catch(function(error) {
				console.error(`[Firebase Messaging] initialize: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getToken: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.getToken(plugin.sdk).then(function(token) {
				console.log(`[Firebase Messaging] getToken: token=${token}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, token, null);
			}).catch(function(error) {
				console.error(`[Firebase Messaging] getToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		deleteToken: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.deleteToken(plugin.sdk).then(function(success) {
				console.log(`[Firebase Messaging] deleteToken: ${(success ? "deleted" : "not deleted")}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
			}).catch(function(error) {
				console.error(`[Firebase Messaging] deleteToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		onMessage: function(instanceId, callbackPtr) {
			const plugin = this;
			if (typeof plugin.callbacks.onMessageUnsubscribe !== 'undefined') {
				plugin.callbacks.onMessageUnsubscribe();
				plugin.callbacks.onMessageUnsubscribe = null;
				console.log('[Firebase Messaging] onMessage: unsubscribed');
			}
			
			if (callbackPtr == 0)
				return;
			
			plugin.callbacks.onMessageUnsubscribe = plugin.api.onMessage(plugin.sdk, function(payload) {
				plugin.firebaseToUnity(instanceId, callbackPtr, true, payload, null);
				console.log(`[Firebase Messaging] onMessage: payload=${payload}`);
			});
			console.log('[Firebase Messaging] onMessage: subscribed');
		},
	},
	
	FirebaseWebGL_FirebaseMessaging_initialize: function(requestId, callbackPtr) {
		firebaseMessaging.initialize(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseMessaging_getToken: function(requestId, callbackPtr) {
		firebaseMessaging.getToken(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseMessaging_deleteToken: function(requestId, callbackPtr) {
		firebaseMessaging.deleteToken(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseMessaging_onMessage: function(instanceId, callbackPtr) {
		firebaseMessaging.onMessage(instanceId, callbackPtr);
	},
};

const firebaseMessagingSWLibrary = {
	$firebaseMessagingSW: {
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
			
			console.log(`[Firebase Messaging Service Worker] initialize: requested`);
			plugin.api.isSupported(plugin.sdk).then(function(success) {
				if (success) {
					console.log(`[Firebase Messaging Service Worker] initialize: initialized`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
				}
				else {
					const error = 'Firebase Messaging Service Worker is not supported';
					console.error(`[Firebase Messaging Service Worker] initialize: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				}
			}).catch(function(error) {
				console.error(`[Firebase Messaging Service Worker] initialize: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		experimentalSetDeliveryMetricsExportedToBigQueryEnabled: function(enabled) {
			const plugin = this;
			plugin.api.experimentalSetDeliveryMetricsExportedToBigQueryEnabled(plugin.sdk, enabled);
			console.log(`[Firebase Messaging Service Worker] experimentalSetDeliveryMetricsExportedToBigQueryEnabled: enabled=${enabled}`);
		},
	},
	
	FirebaseWebGL_FirebaseMessaging_ServiceWorker_initialize: function(requestId, callbackPtr) {
		firebaseMessagingSW.initialize(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseMessaging_ServiceWorker_experimentalSetDeliveryMetricsExportedToBigQueryEnabled: function(enabled) {
		firebaseMessagingSW.experimentalSetDeliveryMetricsExportedToBigQueryEnabled(enabled);
	},
};

autoAddDeps(firebaseMessagingLibrary, '$firebaseMessaging');
mergeInto(LibraryManager.library, firebaseMessagingLibrary);

autoAddDeps(firebaseMessagingSWLibrary, '$firebaseMessagingSW');
mergeInto(LibraryManager.library, firebaseMessagingSWLibrary);

