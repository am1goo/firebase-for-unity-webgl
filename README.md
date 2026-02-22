# Firebase for Unity WebGL
Naive implementation of the most popular Firebase modules for Web apps and games made by Unity Engine.

## Features
- one settings file to setup Firebase environment, **no additional actions required**!
- **easy to use API** which similar to Firebase iOS/Android official plugin (with small differences).

## Which Firebase modules were included?
- [x] App
- [ ] App-Check
- [ ] Auth
- [x] Analytics
- [ ] Database
- [ ] Firestore
- [ ] Functions
- [x] Installations
- [x] Messaging (limitations: no support for Service Worker 'onBackgroundMessage')
- [x] Performance
- [x] Remote Config
- [ ] Storage

I hope that I'll add other modules as soon as possible.

## What's inside?
- C# and JavaScript files
- `HtmlAgilityPack` library as a dependency [Editor]

## Installation
##### via Unity Package Manager
The latest version can be installed via [package manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) using following git URL:
```
https://github.com/am1goo/FirebaseWebGL-Unity.git#0.5.1
```

## How to use
#### Create a Firebase App
```csharp
private FirebaseWebGL.FirebaseApp app;

void Awake()
{
    app = FirebaseWebGL.FirebaseApp.DefaultInstance();
}
```
#### Initialize installed modules
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
