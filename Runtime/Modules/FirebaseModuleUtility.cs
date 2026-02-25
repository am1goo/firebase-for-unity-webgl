using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseWebGL
{
    internal static class FirebaseModuleUtility
    {
        public static void InvokeCallback<T>(Dictionary<int, Action<FirebaseCallback<T>>> callbacks, string json, bool doNotRemoveCallback = false)
        {
            var firebaseCallback = default(FirebaseCallback<T>);
            try
            {
                firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<T>>(json);
            }
            catch (Exception ex)
            {
                var scheme = new { requestId = 0 };
                var deserialized = JsonConvert.DeserializeAnonymousType(json, scheme);
                firebaseCallback = FirebaseCallback<T>.Error(deserialized.requestId, ex.Message);
            }

            if (!callbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                Debug.LogError($"{nameof(InvokeCallback)}: request with id '{firebaseCallback.requestId}' is not found");
                return;
            }

            try
            {
                callback?.Invoke(firebaseCallback);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                if (!doNotRemoveCallback)
                {
                    callbacks.Remove(firebaseCallback.requestId);
                }
            }
        }


        public static R InvokeCallback<T, R>(Dictionary<int, Func<FirebaseCallback<T>, R>> callbacks, string json, bool doNotRemoveCallback = false)
        {
            var firebaseCallback = default(FirebaseCallback<T>);
            try
            {
                firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<T>>(json);
            }
            catch (Exception ex)
            {
                var scheme = new { requestId = 0 };
                var deserialized = JsonConvert.DeserializeAnonymousType(json, scheme);
                firebaseCallback = FirebaseCallback<T>.Error(deserialized.requestId, ex.Message);
            }

            if (!callbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                Debug.LogError($"{nameof(InvokeCallback)}: request with id '{firebaseCallback.requestId}' is not found");
                return default;
            }

            try
            {
                if (callback != null)
                {
                    return callback.Invoke(firebaseCallback);
                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return default;
            }
            finally
            {
                if (!doNotRemoveCallback)
                {
                    callbacks.Remove(firebaseCallback.requestId);
                }
            }
        }
    }
}
