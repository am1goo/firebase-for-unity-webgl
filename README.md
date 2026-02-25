# Firebase for Unity WebGL
Naive implementation of the most popular Firebase modules for Web apps and games made by Unity Engine.
Google does not provide support for the Unity WebGL plugin, so I've decided to do it by myself.

## Features
- **An easy-to-setup** via one Firebase settings file, **no additional actions required**!
- **An easy-to-use API** similar to the official Firebase plugin for Unity (iOS/Android) with small differences.

## Which Firebase modules were included?
- [x] App
- [x] App-Check (limitations: no support for Custom Providers, ReCAPTCHA v3 and ReCAPTCHA Enterprise only)
- [x] Auth (limitations: no support for persistence and resolvers)
- [x] Analytics
- [x] Functions
- [x] Installations
- [x] Messaging (limitations: no support for Service Worker 'onBackgroundMessage')
- [x] Performance
- [x] Remote Config
- [x] Storage

## Why were these modules missed?
As I see the situation, next modules are not used very often in games (for hypercasual/casual games it doesn't needed at all, for midcore/hardcode games usually used self-hosted backends)
- [ ] AI
- [ ] Database
- [ ] Firestore

If I wrong, feel free to ping me and I will add these modules in the package as soon as possible.


## What's inside?
- Just few `*.cs` and `*.jslib` files [Source Code]
- `HtmlAgilityPack` library as a dependency [Editor]

## Installation
##### via Unity Package Manager
The latest version can be installed via [package manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) using following git URL:
```
https://github.com/am1goo/FirebaseWebGL-Unity.git#0.9.1
```

## Extensions
- **UniTask extensions** package: [com.am1goo.firebase.webgl.unitask](https://github.com/am1goo/FirebaseWebGL-Unity-UniTask)

## How to use
#### Create a Firebase App
```csharp
private FirebaseWebGL.FirebaseApp app;

void Awake()
{
    app = FirebaseWebGL.FirebaseApp.DefaultInstance();
}
```
#### Every installed module must first be initialized
```csharp
IEnumerator Start()
{
    if (app.Analytics != null)
    {
        bool? isInitialized;
        app.Analytics.Initialize((callback) =>
        {
            if (callback.success == false)
            {
                Debug.LogError($"Initialize: {callback.error}");
                yield break;
            }
            isInitialized = callback.result;
        });
        yield return new WaitUntil(() => initialized != null);
        Debug.Log($"Initialized: {isInitialized}");
    }
}
```

#### Do what you want as same as you do it in official plugin (or kind of similar way)
```csharp
...
    app.Analytics.LogEvent("my event");
...
```

## Tested in
- Unity 2020.3.x

## Contribute
Contribution in any form is very welcome. Bugs, feature requests or feedback can be reported in form of Issues.
