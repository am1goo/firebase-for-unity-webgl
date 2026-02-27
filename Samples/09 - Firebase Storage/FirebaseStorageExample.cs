using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FirebaseWebGL.Samples
{
    internal class FirebaseStorageExample : BaseExample
    {
#if UNITY_WEBGL
        private IFirebaseStorage _storage;

        protected override IEnumerator Start()
        {
            yield return base.Start();

            _storage = app.Storage;
            if (_storage == null)
            {
                Debug.LogError($"Start: {nameof(IFirebaseStorage)} is not injected");
                yield break;
            }

            bool? initialized = null;
            _storage.Initialize((callback) =>
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

            _storage.ConnectStorageEmulator("localhost", 9199, null);

            var rootRef = _storage.Ref("/");

            rootRef.List((callback) =>
            {
                if (callback.success == false)
                {
                    Debug.LogError($"List: {callback.error}");
                    return;
                }

                foreach (var item in callback.result.items)
                {
                    var itemRef = _storage.Ref(item);
                    itemRef.GetDownloadURL((callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"GetDownloadUrl: {itemRef.fullPath}, {callback.error}");
                            return;
                        }
                        Debug.Log($"GetDownloadUrl: {callback.result}");
                    });

                    itemRef.GetMetadata((callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"GetMetadata: {callback.error}");
                            return;
                        }
                        Debug.Log($"GetMetadata: {callback.result}");
                    });
                }
            });

            rootRef.DeleteObject();
        }

        protected override void OnDrawGUI()
        {
            base.OnDrawGUI();

            if (_storage == null)
                return;

            GUILayout.Label("Storage:");
            GUILayout.Label($"- initialized: {_storage.isInitialized}");
            if (_storage.isInitialized)
            {
                if (GUILayout.Button("Test upload bytes"))
                {
                    var uploadBytes = new byte[128];
                    var fileName = Path.GetFileName("abcdefg0123456789.bytes");
                    var fileRef = _storage.Ref(fileName);
                    fileRef.UploadBytes(uploadBytes, (callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"UploadBytes: {fileRef.fullPath}, {callback.error}");
                            return;
                        }

                        var fileMetadata = callback.result;
                        Debug.Log($"UploadBytes: {fileRef.fullPath} uploaded, metadata is {fileMetadata}");

                        fileRef.GetMetadata((callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"GetMetadata: {fileRef.fullPath}, {callback.error}");
                                return;
                            }

                            fileRef.GetBytes((callback) =>
                            {
                                if (callback.success == false)
                                {
                                    Debug.LogError($"GetBytes: {fileRef.fullPath}, {callback.error}");
                                    return;
                                }

                                var downloadedBytes = callback.result;
                                if (!Enumerable.SequenceEqual(uploadBytes, downloadedBytes))
                                {
                                    Debug.LogError($"GetBytes: {fileRef.fullPath}, uploaded bytes are not equal to downloaded bytes");
                                    return;
                                }

                                Debug.Log($"GetBytes: {fileRef.fullPath}, downloaded bytes are equal to uploaded bytes");

                                fileRef.DeleteObject();
                                Debug.Log($"GetBytes: {fileRef.fullPath} is deleted");
                            });
                        });
                    });
                }

                if (GUILayout.Button("Test upload string"))
                {
                    var alphabet = "abcdefghijklmnoprstuwxyz0123456798";
                    var sb = new StringBuilder();
                    for (int i = 0; i < 128; ++i)
                    {
                        var c = alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
                        sb.Append(c);
                    }
                    var uploadStr = sb.ToString();

                    var fileName = Path.GetFileName("abcdefg9876543210.txt");
                    var fileRef = _storage.Ref(fileName);
                    fileRef.UploadString(uploadStr, null, (callback) =>
                    {
                        if (callback.success == false)
                        {
                            Debug.LogError($"UploadString: {fileRef.fullPath}, {callback.error}");
                            return;
                        }

                        var fileMetadata = callback.result;
                        Debug.Log($"UploadString: {fileRef.fullPath} uploaded, metadata is {fileMetadata}");

                        fileRef.GetMetadata((callback) =>
                        {
                            if (callback.success == false)
                            {
                                Debug.LogError($"GetMetadata: {fileRef.fullPath}, {callback.error}");
                                return;
                            }

                            fileRef.GetBytes((callback) =>
                            {
                                if (callback.success == false)
                                {
                                    Debug.LogError($"GetBytes: {fileRef.fullPath}, {callback.error}");
                                    return;
                                }

                                var bytes = callback.result;
                                var downloadedStr = Encoding.ASCII.GetString(bytes);
                                if (uploadStr != downloadedStr)
                                {
                                    Debug.LogError($"GetBytes: {fileRef.fullPath}, uploadStr={uploadStr}, downloadedStr={downloadedStr}");
                                    return;
                                }

                                Debug.Log($"GetBytes: {fileRef.fullPath}, downloadedStr is {downloadedStr}");

                                fileRef.DeleteObject();
                                Debug.Log($"GetBytes: {fileRef.fullPath} is deleted");
                            });
                        });
                    });
                }
            }
        }
#endif
    }
}
