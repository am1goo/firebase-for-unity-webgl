using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebasePerformanceExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebasePerformance _performance;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _performance = app.Performance;
            if (_performance == null)
            {
                Debug.LogError($"Start: {nameof(IFirebasePerformance)} is not injected");
                yield break;
            }

            bool? initialized = null;
            _performance.Initialize((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"Initialize: {callback.error}");
                    return;
                }
                initialized = callback.result;
            });
            yield return new WaitUntil(() => initialized != null);
            if (initialized.Value == false)
            {
                Debug.LogError("Initialize: not initialized");
                yield break;
            }

            var trace = _performance.Trace("somewhere");
            Debug.Log($"Trace: {trace.name}");

            trace.Start();
            Debug.Log($"Trace Start: {trace.name}, isStarted={trace.isStarted}");

            var attrKey = "attrKey";
            trace.PutAttribute(attrKey, "valueForAttribute");
            var attrValue = trace.GetAttribute(attrKey);
            Debug.Log($"GetAttibute: {attrKey}={attrValue}");
            trace.RemoteAttribute(attrKey);

            var attrs = trace.GetAttributes();
            Debug.Log($"GetAttributes: {string.Join(", ", attrs.Select(x => $"{x.Key}={x.Value}"))}");

            var metricKey = "metricKey";
            trace.PutMetric(metricKey, 5);
            trace.IncrementMetric(metricKey);
            var metricValue = trace.GetMetric(metricKey);
            Debug.Log($"GetMetric: {metricKey}={metricValue}");

            trace.Record(TimeSpan.FromMilliseconds(1), TimeSpan.FromSeconds(1), null);
            yield return new WaitForSeconds(1);
            trace.Record(TimeSpan.FromMilliseconds(1), TimeSpan.FromSeconds(1), new FirebaseWebGL.FirebasePerformanceTraceRecordOptions
            {
                attributes = new Dictionary<string, string>
            {
                { "attr1", "value1" },
            },
                metrics = new Dictionary<string, int>
            {
                { "metric1", 123 },
            },
            });
            yield return new WaitForSeconds(1);

            trace.Stop();
            Debug.Log($"Trace Stop: {trace.name}, isStopped={trace.isStopped}");
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_performance == null)
                return;

            GUILayout.Label("Performance:");
            GUILayout.Label($"- initialized: {_performance.isInitialized}");
        }
#endif
    }
}
