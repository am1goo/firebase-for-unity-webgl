using AOT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FirebaseWebGL
{
    internal sealed class FirebaseStorageReference : IFirebaseStorageReference
    {
        [DllImport("__Internal")]
        private static extern bool FirebaseWebGL_FirebaseStorage_Ref_deleteObject(string url);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseStorage_Ref_getBytes(string url, int maxDownloadSizeBytes, int requestId, FirebaseJsonAndBytesCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseStorage_Ref_getDownloadURL(string url, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseStorage_Ref_getMetadata(string url, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseStorage_Ref_updateMetadata(string url, string metadataAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseStorage_Ref_list(string url, string optionsAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseStorage_Ref_uploadBytes(string url, byte[] data, int dataSize, string metadataAsJson, int requestId, FirebaseJsonCallbackDelegate callback);
        [DllImport("__Internal")]
        private static extern void FirebaseWebGL_FirebaseStorage_Ref_uploadString(string url, string value, int format, string metadataAsJson, int requestId, FirebaseJsonCallbackDelegate callback);

        private static readonly FirebaseRequests _requests = new FirebaseRequests();
        private static readonly Dictionary<int, Action<FirebaseCallback<string>>> _onStringCallbacks = new Dictionary<int, Action<FirebaseCallback<string>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<byte[]>>> _onBytesCallbacks = new Dictionary<int, Action<FirebaseCallback<byte[]>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseStorageReferenceFullMetadata>>> _onMetadataCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseStorageReferenceFullMetadata>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseStorageReferenceListResult>>> _onListResultCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseStorageReferenceListResult>>>();
        private static readonly Dictionary<int, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>>> _onUploadResultCallbacks = new Dictionary<int, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>>>();

        private bool _isDeleted;
        public bool isDeleted => _isDeleted;

        private readonly FirebaseStorageRef _ref;
        private readonly bool _isRootRef;

        public string name => _ref.name;
        public string fullPath => _ref.fullPath;

        internal FirebaseStorageReference(FirebaseStorageRef @ref)
        {
            _ref = @ref;
            _isRootRef = string.IsNullOrEmpty(_ref.fullPath);
            _isDeleted = false;
        }

        public void DeleteObject()
        {
            if (_isDeleted)
                throw new FirebaseStorageReferenceDeletedException(_ref);

            if (_isRootRef)
            {
                //root object cannot be deleted
                return;
            }

            var deleted = FirebaseWebGL_FirebaseStorage_Ref_deleteObject(_ref.fullPath);
            if (!deleted)
                throw new Exception("cannot be deleted");

            _isDeleted = true;
        }

        public void GetBytes(Action<FirebaseCallback<byte[]>> firebaseCallback)
        {
            GetBytes(maxDownloadSizeBytes: int.MaxValue, firebaseCallback);
        }

        public void GetBytes(int maxDownloadSizeBytes, Action<FirebaseCallback<byte[]>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseStorageReferenceDeletedException(_ref);

            if (_isRootRef)
            {
                firebaseCallback?.Invoke(FirebaseCallback<byte[]>.Error(FirebaseCallbackErrors.StorageOperationUnavailableForRootRef));
                return;
            }

            if (maxDownloadSizeBytes <= 0)
            {
                firebaseCallback?.Invoke(FirebaseCallback<byte[]>.Error(FirebaseCallbackErrors.StorageMaxDownloadSizeMustBeGreaterThanZero));
                return;
            }

            var requestId = _requests.NextId();
            _onBytesCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseStorage_Ref_getBytes(_ref.fullPath, maxDownloadSizeBytes, requestId, OnBytesCallback);
        }

        public void GetDownloadURL(Action<FirebaseCallback<string>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseStorageReferenceDeletedException(_ref);

            if (_isRootRef)
            {
                firebaseCallback?.Invoke(FirebaseCallback<string>.Error(FirebaseCallbackErrors.StorageOperationUnavailableForRootRef));
                return;
            }

            var requestId = _requests.NextId();
            _onStringCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseStorage_Ref_getDownloadURL(_ref.fullPath, requestId, OnStringCallback);
        }

        public void GetMetadata(Action<FirebaseCallback<FirebaseStorageReferenceFullMetadata>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseStorageReferenceDeletedException(_ref);

            if (_isRootRef)
            {
                firebaseCallback?.Invoke(FirebaseCallback<FirebaseStorageReferenceFullMetadata>.Error(FirebaseCallbackErrors.StorageOperationUnavailableForRootRef));
                return;
            }

            var requestId = _requests.NextId();
            _onMetadataCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            FirebaseWebGL_FirebaseStorage_Ref_getMetadata(_ref.fullPath, requestId, OnMetadataCallback);
        }

        public void UpdateMetadata(FirebaseStorageReferenceFullMetadata metadata, Action<FirebaseCallback<FirebaseStorageReferenceFullMetadata>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseStorageReferenceDeletedException(_ref);

            if (_isRootRef)
            {
                firebaseCallback?.Invoke(FirebaseCallback<FirebaseStorageReferenceFullMetadata>.Error(FirebaseCallbackErrors.StorageOperationUnavailableForRootRef));
                return;
            }

            var requestId = _requests.NextId();
            _onMetadataCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var metadataAsJson = JsonConvert.SerializeObject(metadata);
            FirebaseWebGL_FirebaseStorage_Ref_updateMetadata(_ref.fullPath, metadataAsJson, requestId, OnMetadataCallback);
        }

        public void List(Action<FirebaseCallback<FirebaseStorageReferenceListResult>> firebaseCallback)
        {
            List(options: null, firebaseCallback);
        }

        public void List(FirebaseStorageReferenceListOptions options, Action<FirebaseCallback<FirebaseStorageReferenceListResult>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseStorageReferenceDeletedException(_ref);

            var requestId = _requests.NextId();
            _onListResultCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var optionsAsJson = options != null ? JsonConvert.SerializeObject(options) : null;
            FirebaseWebGL_FirebaseStorage_Ref_list(_ref.fullPath, optionsAsJson, requestId, OnListResultCallback);
        }

        public void UploadBytes(byte[] data, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>> firebaseCallback)
        {
            UploadBytes(data, metadata: null, firebaseCallback);
        }

        public void UploadBytes(byte[] data, FirebaseStorageReferenceUploadMetadata metadata, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseStorageReferenceDeletedException(_ref);

            var requestId = _requests.NextId();
            _onUploadResultCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var metadataAsJson = metadata != null ? JsonConvert.SerializeObject(metadata) : null;
            FirebaseWebGL_FirebaseStorage_Ref_uploadBytes(_ref.fullPath, data, data.Length, metadataAsJson, requestId, OnUploadResultCallback);
        }

        public void UploadString(string value, FirebaseStorageReferenceStringFormat? format, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>> firebaseCallback)
        {
            UploadString(value, format, metadata: null, firebaseCallback);
        }

        public void UploadString(string value, FirebaseStorageReferenceStringFormat? format, FirebaseStorageReferenceUploadMetadata metadata, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>> firebaseCallback)
        {
            if (_isDeleted)
                throw new FirebaseStorageReferenceDeletedException(_ref);

            var requestId = _requests.NextId();
            _onUploadResultCallbacks.Add(requestId, (callback) =>
            {
                firebaseCallback?.Invoke(callback);
            });

            var formatInt = format.HasValue ? (int)format.Value : -1;
            var metadataAsJson = metadata != null ? JsonConvert.SerializeObject(metadata) : null;
            FirebaseWebGL_FirebaseStorage_Ref_uploadString(_ref.fullPath, value, formatInt, metadataAsJson, requestId, OnUploadResultCallback);
        }

        public override string ToString()
        {
            return _ref.fullPath;
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnStringCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onStringCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnMetadataCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onMetadataCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnListResultCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onListResultCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonCallbackDelegate))]
        private static void OnUploadResultCallback(string json)
        {
            FirebaseModuleUtility.InvokeCallback(_onUploadResultCallbacks, json);
        }

        [MonoPInvokeCallback(typeof(FirebaseJsonAndBytesCallbackDelegate))]
        private static void OnBytesCallback(string json, int bytesPointer, int bytesLength)
        {
            byte[] bytes = new byte[bytesLength];
            Marshal.Copy(new IntPtr(bytesPointer), bytes, 0, bytesLength);
            OnBytesCallback(json, bytes);
        }

        private static void OnBytesCallback(string json, byte[] bytes)
        { 
            var firebaseCallback = JsonConvert.DeserializeObject<FirebaseCallback<byte[]>>(json);
            firebaseCallback.result = bytes;

            if (_onBytesCallbacks.TryGetValue(firebaseCallback.requestId, out var callback))
            {
                _onBytesCallbacks.Remove(firebaseCallback.requestId);
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
