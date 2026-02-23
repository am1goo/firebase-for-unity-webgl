const firebasePerformanceLibrary = {
	$firebasePerformance: {
		sdk: undefined,
		api: undefined,
		callbacks: {},
		
		traceCounter: undefined,
		traceInstances: {},
		
		initialize: function() {
			const plugin = this;
			plugin.firebaseToUnity = window.firebaseToUnity;
			
			if (typeof sdk !== 'undefined') {
				console.error("[Firebase Performance] initialize: already initialized");
				return false;
			}
			
			try {
				plugin.sdk = document.firebaseSdk.performance;
				plugin.api = document.firebaseSdk.performanceApi;
				plugin.traceCounter = this.createCounter();
				console.log('[Firebase Performance] initialize: initialized');
				return true;
			}
			catch(error) {
				console.error(`[Firebase Performance] initialize: failed, error=${error}`);
				return false;
			}
		},
		
		trace: function(name) {
			const plugin = this;
			const traceInstance = plugin.api.trace(plugin.sdk, name);
			const traceId = plugin.traceCounter();
			plugin.traceInstances[traceId] = traceInstance;
			return traceId;
		},
		
		start: function(traceId) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			trace.start();
		},
		
		stop: function(traceId) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			trace.stop();
		},
		
		putAttribute: function(traceId, attr, value) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			trace.putAttribute(attr, value);
		},
		
		removeAttribute: function(traceId, attr) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			trace.removeAttribute(attr);
		},
		
		getAttribute: function(traceId, attr) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			const attrValue = trace.getAttribute(attr);
			
			var buffer = stringToNewUTF8(attrValue);
			setTimeout(function() {
				_free(buffer);
			}, 100);
			return buffer;
		},
		
		getAttributes: function(traceId) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			return trace.getAttributes();
		},
		
		putMetric: function(traceId, name, num) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			trace.putMetric(name, num);
		},
		
		getMetric: function(traceId, name) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			return trace.getMetric(name);
		},
		
		incrementMetric: function(traceId, name, num) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			trace.incrementMetric(name, num);
		},
		
		record: function(traceId, startTime, duration, options) {
			const plugin = this;
			const trace = plugin.getTrace(traceId);
			trace.record(startTime, duration, options);
		},
		
		getTrace: function(traceId) {
			const plugin = this;
			if (traceId in plugin.traceInstances) {
				return plugin.traceInstances[traceId];
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
	
	FirebaseWebGL_FirebasePerformance_initialize: function() {
		return firebasePerformance.initialize();
	},
	
	FirebaseWebGL_FirebasePerformance_trace: function(namePtr) {
		const name = UTF8ToString(namePtr);
		return firebasePerformance.trace(name);
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_start: function(traceId) {
		firebasePerformance.start(traceId);
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_stop: function(traceId) {
		firebasePerformance.stop(traceId);
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_putAttribute: function(traceId, attrPtr, valuePtr) {
		const attr = UTF8ToString(attrPtr);
		const value = UTF8ToString(valuePtr);
		firebasePerformance.putAttribute(traceId, attr, value);
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_removeAttribute: function(traceId, attrPtr) {
		const attr = UTF8ToString(attrPtr);
		firebasePerformance.removeAttribute(traceId, attr);
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_getAttribute: function(traceId, attrPtr) {
		const attr = UTF8ToString(attrPtr);
		const value = firebasePerformance.getAttribute(traceId, attr);
		
		var buffer = stringToNewUTF8(value);
		setTimeout(function() {
			_free(buffer);
		}, 100);
		return buffer;
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_getAttributes: function(traceId) {
		const attributes = firebasePerformance.getAttributes(traceId);
		const attributesAsJson = JSON.stringify(attributes);
		
		var buffer = stringToNewUTF8(attributesAsJson);
		setTimeout(function() {
			_free(buffer);
		}, 100);
		return buffer;
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_putMetric: function(traceId, namePtr, num) {
		const name = UTF8ToString(namePtr);
		firebasePerformance.putMetric(traceId, name, num);
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_getMetric: function(traceId, namePtr) {
		const name = UTF8ToString(namePtr);
		return firebasePerformance.getMetric(traceId, name);
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_incrementMetric: function(traceId, namePtr, num) {
		const name = UTF8ToString(namePtr);
		firebasePerformance.incrementMetric(traceId, name, num);
	},
	
	FirebaseWebGL_FirebasePerformance_Trace_record: function(traceId, startTime, duration, optionsAsJsonPtr) {
		if (optionsAsJsonPtr != 0) {
			const optionsAsJson = UTF8ToString(optionsAsJsonPtr);
			const options = JSON.parse(optionsAsJson);
			firebasePerformance.record(traceId, startTime, duration, options);
		}
		else {
			firebasePerformance.record(traceId, startTime, duration);
		}
	},
};

autoAddDeps(firebasePerformanceLibrary, '$firebasePerformance');
mergeInto(LibraryManager.library, firebasePerformanceLibrary);