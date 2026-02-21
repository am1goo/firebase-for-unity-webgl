const installationsLibrary = {
	$installations: {
		sdk: undefined,
		api: undefined,
		callbacks: {},
		
		initialize: function() {
			const plugin = this;
			plugin.firebaseToUnity = window.firebaseToUnity;
			
			if (typeof sdk !== 'undefined') {
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, "already initialized");
				return;
			}
			plugin.sdk = document.firebaseSdk.installations;
			plugin.api = document.firebaseSdk.installationsApi;
			console.log('[Firebase Installations] initialize: initialized');
		},
		
		deleteInstallations: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.deleteInstallations(plugin.sdk).then(function(success) {
				console.log(`[Firebase Installations] deleteInstallations: ${(success ? 'deleted' : 'not deleted')}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, success, null);
			}).catch(function(error) {
				console.error(`[Firebase Installations] deleteInstallations: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getId: function(requestId, callbackPtr) {
			const plugin = this;
			plugin.api.getId(plugin.sdk).then(function(id) {
				console.log(`[Firebase Installations] getId: id=${id}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, id, null);
			}).catch(function(error) {
				console.error(`[Firebase Installations] getId: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getToken: function(forceRefresh, requestId, callbackPtr) {
			const plugin = this;
			plugin.api.getToken(plugin.sdk, forceRefresh).then(function(token) {
				console.log(`[Firebase Installations] getToken: token=${token}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, token, null);
			}).catch(function(error) {
				console.error(`[Firebase Installations] getToken: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		onIdChange: function(instanceId, callbackPtr) {
			const plugin = this;
			plugin.api.onIdChange(plugin.sdk, function(newId) {
				console.log(`[Firebase Installations] onIdChange: newId=${newId}`);
				plugin.firebaseToUnity(instanceId, callbackPtr, true, newId, null);
			});
			console.log('[Firebase Installations] onIdChange: registered callback');
		},
	},
	
	FirebaseWebGL_FirebaseInstallations_initialize: function() {
		installations.initialize();
	},
	
	FirebaseWebGL_FirebaseInstallations_deleteInstallations: function(requestId, callbackPtr) {
		installations.deleteInstallations(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseInstallations_getId: function(requestId, callbackPtr) {
		installations.getId(requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseInstallations_getToken: function(forceRefresh, requestId, callbackPtr) {
		installations.getToken(forceRefresh, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseInstallations_onIdChange: function(instanceId, callbackPtr) {
		installations.onIdChange(instanceId, callbackPtr);
	},
};

autoAddDeps(installationsLibrary, '$installations');
mergeInto(LibraryManager.library, installationsLibrary);