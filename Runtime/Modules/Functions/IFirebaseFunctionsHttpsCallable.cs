using System;

namespace FirebaseWebGL
{
    public interface IFirebaseFunctionsHttpsCallable
    {
        void Request<RESP>(Action<FirebaseCallback<RESP>> firebaseCallback);
        void Request<REQ, RESP>(REQ data, Action<FirebaseCallback<RESP>> firebaseCallback);
    }
}