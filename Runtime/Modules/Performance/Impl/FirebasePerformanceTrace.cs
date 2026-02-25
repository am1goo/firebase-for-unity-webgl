using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FirebaseWebGL
{
    internal sealed class FirebasePerformanceTrace : IFirebasePerformanceTrace
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebasePerformance_Trace_start(int id);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebasePerformance_Trace_stop(int id);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebasePerformance_Trace_putAttribute(int id, string attr, string value);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebasePerformance_Trace_removeAttribute(int id, string attr);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebasePerformance_Trace_getAttribute(int id, string attr);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebasePerformance_Trace_getAttributes(int id);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebasePerformance_Trace_putMetric(int id, string name, int num);
        [DllImport("__Internal")]
        private static extern int FirebaseWebGL_FirebasePerformance_Trace_getMetric(int id, string name);
        [DllImport("__Internal")]
        private static extern string FirebaseWebGL_FirebasePerformance_Trace_incrementMetric(int id, string name, int num);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebasePerformance_Trace_record(int id, int startTime, int duration, string optionsAsJson);

        private readonly int _id;

        private readonly string _name;
        public string name => _name;

        private bool _isStarted;
        public bool isStarted => _isStarted;
        public bool isStopped => !_isStarted;

        internal FirebasePerformanceTrace(int id, string name)
        {
            _id = id;
            _name = name;
            _isStarted = false;
        }

        public void Start()
        {
            if (_isStarted)
                return;

            FirebaseWebGL_FirebasePerformance_Trace_start(_id);
            _isStarted = true;
        }

        public void Stop()
        {
            if (!_isStarted)
                return;

            FirebaseWebGL_FirebasePerformance_Trace_stop(_id);
            _isStarted = false;
        }

        public void PutAttribute(string attr, string value)
        {
            if (attr == null)
                throw new ArgumentNullException(nameof(attr));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            FirebaseWebGL_FirebasePerformance_Trace_putAttribute(_id, attr, value);
        }

        public void RemoteAttribute(string attr)
        {
            if (attr == null)
                throw new ArgumentNullException(nameof(attr));

            FirebaseWebGL_FirebasePerformance_Trace_removeAttribute(_id, attr);
        }

        public string GetAttribute(string attr)
        {
            if (attr == null)
                throw new ArgumentNullException(nameof(attr));

            return FirebaseWebGL_FirebasePerformance_Trace_getAttribute(_id, attr);
        }

        public IReadOnlyDictionary<string, string> GetAttributes()
        {
            var attributesAsJson = FirebaseWebGL_FirebasePerformance_Trace_getAttributes(_id);
            if (attributesAsJson == null)
                return null;

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(attributesAsJson);
        }

        public void PutMetric(string name, int num)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            FirebaseWebGL_FirebasePerformance_Trace_putMetric(_id, name, num);
        }

        public int GetMetric(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return FirebaseWebGL_FirebasePerformance_Trace_getMetric(_id, name);
        }

        public void IncrementMetric(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            IncrementMetric(name, num: 1);
        }

        public void IncrementMetric(string name, int num)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            FirebaseWebGL_FirebasePerformance_Trace_incrementMetric(_id, name, num);
        }

        public void Record(TimeSpan startTime, TimeSpan duration, FirebasePerformanceTraceRecordOptions options)
        {
            if (startTime.TotalMilliseconds <= 0)
                throw new ArgumentException($"{nameof(startTime)} should be positive");

            if (duration.TotalMilliseconds <= 0)
                throw new ArgumentException($"{nameof(duration)} should be positive");

            var optionsAsJson = options != null ? JsonConvert.SerializeObject(options) : null;
            FirebaseWebGL_FirebasePerformance_Trace_record(_id, (int)startTime.TotalMilliseconds, (int)duration.TotalMilliseconds, optionsAsJson);
        }
    }
}