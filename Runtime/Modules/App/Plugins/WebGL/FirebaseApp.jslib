const firebaseAppLibrary = {
	$firebaseApp: {
		sdk: undefined,
		api: undefined,
		
		firebaseToUnity: function(requestId, callbackPtr, success, result, e) {
			var error = (e instanceof Error ? e.message : e);
			var args = { 
				requestId: requestId,
				success: success,
				result: result,
				error: error
			};
			var json = JSON.stringify(args);
			var jsonBuffer = stringToNewUTF8(json);
			try {
				{{{ makeDynCall('vi', 'callbackPtr') }}} (jsonBuffer);
			}
			finally {
				_free(jsonBuffer);
			}
		},
		
		firebaseToUnityBytes: function(requestId, callbackPtr, success, bytes, e) {
			var error = (e instanceof Error ? e.message : e);
			var args = { 
				requestId: requestId,
				success: success,
				result: null,
				error: error
			};
			var json = JSON.stringify(args);
			var jsonBuffer = stringToNewUTF8(json);
			
			var bytesArray = new Uint8Array(bytes);
			var bytesBuffer = _malloc(bytesArray.length * bytesArray.BYTES_PER_ELEMENT);
			HEAPU8.set(bytesArray, bytesBuffer);
			try {
				{{{ makeDynCall('viii', 'callbackPtr') }}} (jsonBuffer, bytesBuffer, bytesArray.length);
			}
			finally {
				_free(jsonBuffer);
				_free(bytesBuffer);
			}
		},
		
		initialize: function() {
			window.firebaseToUnity = this.firebaseToUnity;
			window.firebaseToUnityBytes = this.firebaseToUnityBytes;
			
			if (typeof sdk !== 'undefined') {
				console.error("[Firebase App] initialize: already initialized");
				return false;
			}
			
			if (typeof document.firebaseSdk === 'undefined') {
				console.error("[Firebase App] initialize: missing SDK");
				return false;
			}
					
			try {
				this.sdk = document.firebaseSdk.app;
				this.api = document.firebaseSdk.appApi;
				console.log("[Firebase App] initialize: initialized");
				return true;
			}
			catch(error) {
				console.error(`[Firebase App] initialize: failed, error=${error}`);
				return false;
			}
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
		return firebaseApp.initialize();
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
