using System;

namespace FirebaseWebGL
{
    public interface IFirebaseStorageReference
    {
        string name { get; }
        string fullPath { get; }

        void DeleteObject();
        void GetBytes(Action<FirebaseCallback<byte[]>> firebaseCallback);
        void GetBytes(int maxDownloadSizeBytes, Action<FirebaseCallback<byte[]>> firebaseCallback);
        void GetDownloadURL(Action<FirebaseCallback<string>> firebaseCallback);
        void GetMetadata(Action<FirebaseCallback<FirebaseStorageReferenceFullMetadata>> firebaseCallback);
        void UpdateMetadata(FirebaseStorageReferenceFullMetadata metadata, Action<FirebaseCallback<FirebaseStorageReferenceFullMetadata>> firebaseCallback);
        void List(Action<FirebaseCallback<FirebaseStorageReferenceListResult>> firebaseCallback);
        void List(FirebaseStorageReferenceListOptions options, Action<FirebaseCallback<FirebaseStorageReferenceListResult>> firebaseCallback);
        void UploadBytes(byte[] data, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>> firebaseCallback);
        void UploadBytes(byte[] data, FirebaseStorageReferenceUploadMetadata metadata, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>> firebaseCallback);
        void UploadString(string value, FirebaseStorageReferenceStringFormat? format, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>> firebaseCallback);
        void UploadString(string value, FirebaseStorageReferenceStringFormat? format, FirebaseStorageReferenceUploadMetadata metadata, Action<FirebaseCallback<FirebaseStorageReferenceUploadResult>> firebaseCallback);
    }
}
