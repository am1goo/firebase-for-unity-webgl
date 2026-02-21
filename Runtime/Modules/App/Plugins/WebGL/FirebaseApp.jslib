const appLibrary = {
	$app: {
		sdk: undefined,
		
		firebaseToUnity: function(requestId, callbackPtr, success, result, e) {
			var error = (e instanceof Error ? e.message : e);
			var args = { requestId, success, result, error };
			var json = JSON.stringify(args);
			var buffer = stringToNewUTF8(json);
			try {
				{{{ makeDynCall('vi', 'callbackPtr') }}} (buffer);
			}
			finally {
				_free(buffer);
			}
		},
		
		initialize: function() {
			window.firebaseToUnity = this.firebaseToUnity;
			
			if (typeof sdk !== 'undefined') {
				return;
			}
			
			this.sdk = document.firebaseSdk.app;
		},
		
		deleteApp: function() {
			this.sdk.deleteApp(this.sdk);
			console.log("deleteApp: success");
		},
	},
	
	FirebaseWebGL_FirebaseApp_initalize: function () {
		app.initialize();
	},
	
	FirebaseWebGL_FirebaseApp_deleteApp: function () {
		app.deleteApp();
	},
};

autoAddDeps(appLibrary, '$app');
mergeInto(LibraryManager.library, appLibrary);
