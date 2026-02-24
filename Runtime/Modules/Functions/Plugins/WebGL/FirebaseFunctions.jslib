const firebaseFunctionsLibrary = {
	$firebaseFunctions: {
		sdk: undefined,
		api: undefined,
		callbacks: {},
		
		callableCounter: undefined,
		callableInstances: {},
	
		initialize: function() {
			const plugin = this;
			plugin.firebaseToUnity = window.firebaseToUnity;
			plugin.firebaseToUnityBytes = window.firebaseToUnityBytes;
			
			if (typeof sdk !== 'undefined') {
				console.error("[Firebase Functions] initialize: already initialized");
				return false;
			}
			
			try {
				plugin.sdk = document.firebaseSdk.functions;
				plugin.api = document.firebaseSdk.functionsApi;
				plugin.callableCounter = this.createCounter();
				console.log('[Firebase Functions] initialize: initialized');
				return true;
			}
			catch(error) {
				console.error(`[Firebase Functions] initialize: failed, error=${error}`);
				return false;
			}
		},
		
		connectFunctionsEmulator: function(host, port) {
			const plugin = this;
			try {
				plugin.api.connectFunctionsEmulator(plugin.sdk, host, port);
				console.log(`[Firebase Functions] connectFunctionsEmulator: host=${host}, port=${port}`);
				return true;
			}
			catch(error) {
				console.error(`[Firebase Functions] connectFunctionsEmulator: error=${error}`);
				return false;
			}
		},
		
		httpsCallable: function(name, options) {
			const plugin = this;
			try {
				const callable = plugin.api.httpsCallable(plugin.sdk, name, options);
				console.log(`[Firebase Functions] httpsCallable: name=${name}`);
				
				const callableId = plugin.callableCounter();
				plugin.callableInstances[callableId] = callable;
				return callableId;
			}
			catch(error) {
				console.error(`[Firebase Functions] httpsCallable: ${error}`);
				return 0;
			}
		},
		
		httpsCallableFromURL: function(url, options) {
			const plugin = this;
			try {
				const callable = plugin.api.httpsCallableFromURL(plugin.sdk, url, options);
				console.log(`[Firebase Functions] httpsCallableFromURL: url=${url}`);
				
				const callableId = plugin.callableCounter();
				plugin.callableInstances[callableId] = callable;
				return callableId;
			}
			catch(error) {
				console.error(`[Firebase Functions] httpsCallableFromURL: ${error}`);
				return 0;
			}
		},
		
		stream: function(callableId, requestData, requestId, callbackPtr) {
			const plugin = this;
			const callable = plugin.getCallable(callableId);
			
			const stream_async = async function() {
				const { stream, data } = await callable(requestData);
				if (typeof stream !== 'undefined') {
					for await (const chunk of stream) {
						//do nothing
					}
				}
				
				return await data;
			};
			
			console.log(`[Firebase Functions] stream: response requested`);
			stream_async().then(function(responseData) {
				console.log(`[Firebase Functions] stream: response received ${JSON.stringify(responseData)}`);
				plugin.firebaseToUnity(requestId, callbackPtr, true, responseData, null);
			}).catch(function(error) {
				console.error(`[Firebase Functions] stream: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			});
		},
		
		getCallable: function(callableId) {
			const plugin = this;
			if (callableId in plugin.callableInstances) {
				return plugin.callableInstances[callableId];
			}
			else {
				return null;
			}
		},
		
		createCounter: function() {
			let count = 0;

			return function counter() {
				count = count + 1;
				return count;
			};
		},
	},
	
	FirebaseWebGL_FirebaseFunctions_initialize: function() {
		return firebaseFunctions.initialize();
	},
	
	FirebaseWebGL_FirebaseFunctions_connectFunctionsEmulator: function(hostPtr, port) {
		const host = UTF8ToString(hostPtr);
		return firebaseFunctions.connectFunctionsEmulator(host, port);
	},
	
	FirebaseWebGL_FirebaseFunctions_httpsCallable: function(namePtr, optionsAsJsonPtr) {
		const name = UTF8ToString(namePtr);
		const optionsAsJson = UTF8ToString(optionsAsJsonPtr);
		const options = JSON.parse(optionsAsJson);
		return firebaseFunctions.httpsCallable(name, options);
	},
	
	FirebaseWebGL_FirebaseFunctions_httpsCallableFromURL: function(urlPtr, optionsAsJsonPtr) {
		const url = UTF8ToString(urlPtr);
		const optionsAsJson = UTF8ToString(optionsAsJsonPtr);
		const options = JSON.parse(optionsAsJson);
		return firebaseFunctions.httpsCallableFromURL(url, options);
	},
	
	FirebaseWebGL_FirebaseFunctions_Callable_stream: function(callableId, requestDataAsJsonPtr, requestId, callbackPtr) {
		var requestDataAsJson = UTF8ToString(requestDataAsJsonPtr);
		var requestData = JSON.parse(requestDataAsJson);
		firebaseFunctions.stream(callableId, requestData, requestId, callbackPtr);
	},
};

autoAddDeps(firebaseFunctionsLibrary, '$firebaseFunctions');
mergeInto(LibraryManager.library, firebaseFunctionsLibrary);