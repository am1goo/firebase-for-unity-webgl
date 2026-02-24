using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseFunctionsHttpsCallable : IFirebaseFunctionsHttpsCallable
    {
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseFunctions_Callable_stream(int callableId, string requestDataAsJson, int request, FirebaseJsonCallbackDelegate callback);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();

        private readonly int _id;

        public FirebaseFunctionsHttpsCallable(int id)
        {
            _id = id;
        }

        public void Request<RESP>(Action<FirebaseCallback<RESP>> firebaseCallback)
        {
            Request(FirebaseVoid.Void, firebaseCallback);
        }

        public void Request<REQ, RESP>(REQ requestData, Action<FirebaseCallback<RESP>> firebaseCallback)
        {
            if (requestData == null)
                throw new ArgumentException($"{nameof(requestData)} is null");

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                try
                {
                    if (callback.success == false)
                    {
                        firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, callback.error));
                        return;
                    }

                    var type = typeof(RESP);
                    TypeCode typeCode = Type.GetTypeCode(type);
                    switch (typeCode)
                    {
                        case TypeCode.Byte:
                            if (byte.TryParse(callback.result, out var resultAsByte) && resultAsByte is RESP respAsByte)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsByte));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.SByte:
                            if (sbyte.TryParse(callback.result, out var resultAsSByte) && resultAsSByte is RESP respAsSByte)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsSByte));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.Int16:
                            if (short.TryParse(callback.result, out var resultAsInt16) && resultAsInt16 is RESP respAsInt16)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsInt16));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.UInt16:
                            if (ushort.TryParse(callback.result, out var resultAsUInt16) && resultAsUInt16 is RESP respAsUInt16)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsUInt16));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.Int32:
                            if (int.TryParse(callback.result, out var resultAsInt32) && resultAsInt32 is RESP respAsInt32)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsInt32));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.UInt32:
                            if (uint.TryParse(callback.result, out var resultAsUInt32) && resultAsUInt32 is RESP respAsUInt32)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsUInt32));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.Int64:
                            if (long.TryParse(callback.result, out var resultAsInt64) && resultAsInt64 is RESP respAsInt64)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsInt64));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.UInt64:
                            if (ulong.TryParse(callback.result, out var resultAsUInt64) && resultAsUInt64 is RESP respAsUInt64)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsUInt64));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.Single:
                            if (float.TryParse(callback.result, out var resultAsSingle) && resultAsSingle is RESP respAsSingle)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsSingle));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.Double:
                            if (double.TryParse(callback.result, out var resultAsDouble) && resultAsDouble is RESP respAsDouble)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsDouble));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.Decimal:
                            if (decimal.TryParse(callback.result, out var resultAsDecimal) && resultAsDecimal is RESP respAsDecimal)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsDecimal));
                            }
                            else
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"string '{callback.result}' cannot be parsed as {typeCode}"));
                            }
                            break;

                        case TypeCode.String:
                            if (callback.result is RESP respAsString)
                            {
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, respAsString));
                            }
                            else
                            {
                                var actualType = callback.result?.GetType().Name ?? "null";
                                var expectedType = type.Name;
                                firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, $"wrong response type, actual={actualType}, expected={expectedType}"));
                            }
                            break;

                        case TypeCode.Object:
                            var response = JsonConvert.DeserializeObject<RESP>(callback.result);
                            firebaseCallback?.Invoke(FirebaseCallback<RESP>.Success(requestId, response));
                            break;

                        default:
                            firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, FirebaseCallbackErrors.FunctionsUnsupportedResponseDataType));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    firebaseCallback?.Invoke(FirebaseCallback<RESP>.Error(requestId, ex.Message));
                }
            });

            var requestDataAsJson = JsonConvert.SerializeObject(requestData);
            FirebaseWebGL_FirebaseFunctions_Callable_stream(_id, requestDataAsJson, requestId, OnStringCallback);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnStringCallback(string json)
        {
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<string>>(json);

            if (_onStringCallbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                _onStringCallbacks.Remove(firebaseCallback.requestId);
                try
                {
                    callback?.Invoke(firebaseCallback);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}
