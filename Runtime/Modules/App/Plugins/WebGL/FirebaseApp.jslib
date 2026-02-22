const firebaseAppLibrary = {
	$firebaseApp: {
		sdk: undefined,
		api: undefined,
		
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
			this.api = document.firebaseSdk.appApi;
			console.log("[Firebase App] initialize: initialized");
		},
		
		deleteApp: function() {
			const plugin = this;
			plugin.sdk.deleteApp(plugin.sdk);
			console.log("[Firebase App] deleteApp: success");
		},
		
		setLogLevel: function(logLevel) {
			const plugin = this;
			plugin.api.setLogLevel(logLevel);
			console.log(`[Firebase App] setLogLevel: logLevel=${logLevel}`);
		},
	},
	
	FirebaseWebGL_FirebaseApp_initalize: function () {
		firebaseApp.initialize();
	},
	
	FirebaseWebGL_FirebaseApp_deleteApp: function () {
		firebaseApp.deleteApp();
	},
	
	FirebaseWebGL_FirebaseApp_setLogLevel: function(logLevelInt) {
		const conversion = function(value) {
			switch (value) {
				case 0:
					return 'debug';
				case 1:
					return 'verbose';
				case 2:
					return 'info';
				case 3:
					return 'warn';
				case 4:
					return 'error';
				case 5:
				default:
					return 'silent';
			}
		}
		const logLevel = conversion(logLevelInt);
		firebaseApp.setLogLevel(logLevel);
	},
};

autoAddDeps(firebaseAppLibrary, '$firebaseApp');
mergeInto(LibraryManager.library, firebaseAppLibrary);
