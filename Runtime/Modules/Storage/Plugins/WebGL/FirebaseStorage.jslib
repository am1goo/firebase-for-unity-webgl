const firebaseStorageLibrary = {
	$firebaseStorage: {
		sdk: undefined,
		api: undefined,
		callbacks: {},
	
		initialize: function() {
			const plugin = this;
			plugin.firebaseToUnity = window.firebaseToUnity;
			plugin.firebaseToUnityBytes = window.firebaseToUnityBytes;
			
			if (typeof sdk !== 'undefined') {
				console.error("[Firebase Storage] initialize: already initialized");
				return false;
			}
			
			try {
				plugin.sdk = document.firebaseSdk.storage;
				plugin.api = document.firebaseSdk.storageApi;
				console.log('[Firebase Storage] initialize: initialized');
				return true;
			}
			catch(error) {
				console.error(`[Firebase Storage] initialize: failed, error=${error}`);
				return false;
			}
		},
		
		connectStorageEmulator: function(host, port, options) {
			const plugin = this;
			plugin.api.connectStorageEmulator(plugin.sdk, host, port, options);
			console.log(`[Firebase Storage] connectStorageEmulator: host=${host}, port=${port}`);
		},
		
		ref: function(url) {
			const plugin = this;
			const refInstance = plugin.api.ref(plugin.sdk, url);
			
			const refSimplified = {
				bucket: refInstance.bucket,
				name: refInstance.name,
				fullPath: refInstance.fullPath,
			};
			const refSimplifiedAsJson = JSON.stringify(refSimplified);
			return stringToNewUTF8(refSimplifiedAsJson);
		},
		
		deleteObject: function(url) {
			const plugin = this;
			const ref = plugin.api.ref(plugin.sdk, url);
			try {
				plugin.api.deleteObject(ref);
				console.log(`[Firebase Storage] deleteObject: url=${url}`);
				return true;
			}
			catch(error) {
				console.error(`[Firebase Storage] deleteObject: ${error}`);
				return false;
			}
		},
		
		getBytes: function(url, maxDownloadSizeBytes, requestId, callbackPtr) {
			const plugin = this;
			const ref = plugin.api.ref(plugin.sdk, url);
			try {
				plugin.api.getBytes(ref, maxDownloadSizeBytes).then(function(bytes) {
					console.log(`[Firebase Storage] getBytes: length=${bytes.byteLength}`);
					plugin.firebaseToUnityBytes(requestId, callbackPtr, true, bytes, null);
				})
				.catch(function(error) {
					console.error(`[Firebase Storage] getBytes: ${error}`);
					plugin.firebaseToUnityBytes(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Storage] getBytes: ${error}`);
				plugin.firebaseToUnityBytes(requestId, callbackPtr, false, null, error);
			}
		},
		
		getDownloadURL: function(url, requestId, callbackPtr) {
			const plugin = this;
			const ref = plugin.api.ref(plugin.sdk, url);
			try {
				plugin.api.getDownloadURL(ref).then(function(url) {
					console.log(`[Firebase Storage] getDownloadURL: url=${url}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, url, null);
				})
				.catch(function(error) {
					console.error(`[Firebase Storage] getDownloadURL: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Storage] getDownloadURL: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		getMetadata: function(url, requestId, callbackPtr) {
			const plugin = this;
			const ref = plugin.api.ref(plugin.sdk, url);
			try {
				plugin.api.getMetadata(ref).then(function(metadata) {
					console.log(`[Firebase Storage] getMetadata: metadata=${JSON.stringify(metadata)}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, metadata, null);
				})
				.catch(function(error) {
					console.error(`[Firebase Storage] getMetadata: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Storage] getMetadata: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		updateMetadata: function(url, metadata, requestId, callbackPtr) {
			const plugin = this;
			const ref = plugin.api.ref(plugin.sdk, url);
			try {
				plugin.api.updateMetadata(ref, metadata).then(function(metadata) {
					console.log(`[Firebase Storage] updateMetadata: metadata=${JSON.stringify(metadata)}`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, metadata, null);
				})
				.catch(function(error) {
					console.error(`[Firebase Storage] updateMetadata: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Storage] updateMetadata: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		list: function(url, options, requestId, callbackPtr) {
			const plugin = this;
			const ref = plugin.api.ref(plugin.sdk, url);
			try {
				plugin.api.list(ref, options).then(function(result) {
					console.log(`[Firebase Storage] list: received`);
					const modified = {
						items:	result.items.map(x => x.fullPath),
						nextPageToken: result.nextPageToken,
						prefixes: result.prefixes.map(x => x.fullPath),
					};
					plugin.firebaseToUnity(requestId, callbackPtr, true, modified, null);
				})
				.catch(function(error) {
					console.error(`[Firebase Storage] list: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Storage] list: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		uploadBytes: function(url, data, metadata, requestId, callbackPtr) {
			const plugin = this;
			const ref = plugin.api.ref(plugin.sdk, url);
			try {
				plugin.api.uploadBytes(ref, data, metadata).then(function(result) {
					const modified = {
						ref: result.ref.fullPath,
						metadata: result.metadata,
					};
					console.log(`[Firebase Storage] uploadBytes: uploaded`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, modified, null);
				})
				.catch(function(error) {
					console.error(`[Firebase Storage] uploadBytes: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Storage] uploadBytes: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
		
		uploadString: function(url, value, format, metadata, requestId, callbackPtr) {
			const plugin = this;
			const ref = plugin.api.ref(plugin.sdk, url);
			try {
				plugin.api.uploadString(ref, value, format, metadata).then(function(result) {
					const modified = {
						reference: result.ref.fullPath,
						metadata: result.metadata,
					};
					console.log(`[Firebase Storage] uploadString: uploaded`);
					plugin.firebaseToUnity(requestId, callbackPtr, true, modified, null);
				})
				.catch(function(error) {
					console.error(`[Firebase Storage] uploadString: ${error}`);
					plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
				});
			}
			catch(error) {
				console.error(`[Firebase Storage] uploadString: ${error}`);
				plugin.firebaseToUnity(requestId, callbackPtr, false, null, error);
			}
		},
	},
	
	FirebaseWebGL_FirebaseStorage_initialize: function() {
		return firebaseStorage.initialize();
	},
	
	FirebaseWebGL_FirebaseStorage_connectStorageEmulator: function(hostPtr, port, optionsAsJsonPtr) {
		const host = UTF8ToString(hostPtr);
		if (optionsAsJsonPtr != 0) {
			const optionsAsJson = UTF8ToString(optionsAsJsonPtr);
			const options = JSON.parse(optionsAsJson);
			firebaseStorage.connectStorageEmulator(host, port, options);
		}
		else {
			firebaseStorage.connectStorageEmulator(host, port);
		}
	},
	
	FirebaseWebGL_FirebaseStorage_ref: function(urlPtr) {
		const url = UTF8ToString(urlPtr);
		return firebaseStorage.ref(url);
	},
	
	FirebaseWebGL_FirebaseStorage_Ref_deleteObject: function(urlPtr) {
		url = UTF8ToString(urlPtr);
		return firebaseStorage.deleteObject(url);
	},
	
	FirebaseWebGL_FirebaseStorage_Ref_getBytes: function(urlPtr, maxDownloadSizeBytes, requestId, callbackPtr) {
		url = UTF8ToString(urlPtr);
		firebaseStorage.getBytes(url, maxDownloadSizeBytes, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseStorage_Ref_getDownloadURL: function(urlPtr, requestId, callbackPtr) {
		url = UTF8ToString(urlPtr);
		firebaseStorage.getDownloadURL(url, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseStorage_Ref_getMetadata: function(urlPtr, requestId, callbackPtr) {
		url = UTF8ToString(urlPtr);
		firebaseStorage.getMetadata(url, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseStorage_Ref_updateMetadata: function(urlPtr, metadataAsJsonPtr, requestId, callbackPtr) {
		url = UTF8ToString(urlPtr);
		const metadataAsJson = UTF8ToString(metadataAsJsonPtr);
		const medadata = JSON.parse(metadataAsJson);
		firebaseStorage.updateMetadata(url, medadata, requestId, callbackPtr);
	},
	
	FirebaseWebGL_FirebaseStorage_Ref_list: function(urlPtr, optionsAsJsonPtr, requestId, callbackPtr) {
		url = UTF8ToString(urlPtr);
		if (optionsAsJsonPtr != 0) {
			const optionsAsJson = UTF8ToString(optionsAsJsonPtr);
			const options = JSON.parse(optionsAsJson);
			firebaseStorage.list(url, options, requestId, callbackPtr);
		}
		else {
			firebaseStorage.list(url, null, requestId, callbackPtr);
		}
	},
	
	FirebaseWebGL_FirebaseStorage_Ref_uploadBytes: function(urlPtr, dataPtr, dataSize, metadataAsJsonPtr, requestId, callbackPtr) {
		url = UTF8ToString(urlPtr);
		var data = new Uint8Array(dataSize);
		for (var i = 0; i < dataSize; i++) {
			data[i] = HEAPU8[dataPtr + i];
		}
		
		if (metadataAsJsonPtr != 0) {
			const metadataAsJson = UTF8ToString(metadataAsJsonPtr);
			const medadata = JSON.parse(metadataAsJson);
			firebaseStorage.uploadBytes(url, data, medadata, requestId, callbackPtr);	
		}
		else {
			firebaseStorage.uploadBytes(url, data, null, requestId, callbackPtr);	
		}
	},
	
	FirebaseWebGL_FirebaseStorage_Ref_uploadString: function(urlPtr, valuePtr, formatInt, metadataAsJsonPtr, requestId, callbackPtr) {
		url = UTF8ToString(urlPtr);
		value = UTF8ToString(valuePtr);
		const conversion = function(value) {
			switch (value) {
				case 0:
				default:
					return 'raw';
				case 1:
					return 'base64';
				case 2:
					return 'base64url';
				case 3:
					return 'data_url';
			}
		};

		const format = conversion(formatInt);
		if (metadataAsJsonPtr != 0) {
			const metadataAsJson = UTF8ToString(metadataAsJsonPtr);
			const medadata = JSON.parse(metadataAsJson);
			firebaseStorage.uploadString(url, value, format, medadata, requestId, callbackPtr);	
		}
		else {
			firebaseStorage.uploadString(url, value, format, null, requestId, callbackPtr);	
		}
	},
};

autoAddDeps(firebaseStorageLibrary, '$firebaseStorage');
mergeInto(LibraryManager.library, firebaseStorageLibrary);
