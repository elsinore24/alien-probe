[English](README.md) / [日本語](README.ja.md)

---

# Cubism Unity Components

Welcome to the open components of the Cubism SDK for Unity.

It is used in conjunction with the Live2D Cubism Core.

Go [here](https://www.live2d.com/download/cubism-sdk/download-unity/) if you're looking for the download page of the SDK package.

## License

Please read the [license](LICENSE.md) before use.

## Notices

Please read the [notices](NOTICE.md) before use.

## Compatibility with Cubism 5 new features and previous Cubism SDK versions

This SDK is compatible with Cubism 5.  
For SDK compatibility with new features in Cubism 5 Editor, please refer to [here](https://docs.live2d.com/en/cubism-sdk-manual/cubism-5-new-functions/).  
For compatibility with previous versions of Cubism SDK, please refer to [here](https://docs.live2d.com/en/cubism-sdk-manual/compatibility-with-cubism-5/).


## Structure

### Components

The components are grouped by their role, and this grouping is reflected in both the folder structure and namespaces.

#### Core Wrapper

Components and classes in this group are a shim layer for wrapping the unmanaged Cubism core library to C# and Unity and are located in `./Assets/Live2D/Cubism/Core`.

#### Framework

Components and classes in this group provide additional functionality like lip-syncing, as well as integration of "foreign" Cubism files with Unity. The feature to convert Cubism files into Prefabs and AnimationClips is here.All the framework code is located in `./Assets/Live2D/Cubism/Framework`.

#### Rendering

Components and classes in this group provide the functionality for rendering Cubism models using Unity functionality and are located in `./Assets/Live2D/Cubism/Rendering`.

### Editor Extensions

Unity Editor extensions are located in `./Assets/Live2D/Cubism/Editor`.

### Resources

Resources like shaders and other assets are located in `./Assets/Live2D/Cubism/Rendering/Resources`.

## Development environment

| Unity | Version |
| --- | --- |
| Latest | 6000.0.49f1 |
| LTS | 2022.3.61f1 |

| Library / Tool | Version |
| --- | --- |
| Android SDK / NDK | *2 |
| Visual Studio 2022 | 17.14.2 |
| Windows SDK | 11.0.26100.3916 |
| Xcode | 16.1 |

*2 Use libraries embedded with Unity or recommended.

| HarmonyOS NEXT Supported Tools | Version |
| --- | --- |
| Tuanjie | 1.5.2 |
| DevEco Studio *3 | 5.0.13.200 |

*3 Builds for HarmonyOS NEXT outside of China must be built through DevEco.

### C# compiler

Build using Roslyn or mcs compiler supported by Unity 2018.4 and above.

Note: The mcs compiler is deprecated and we only check the build.

Please refer to the following official documentation for the versions of C# you can use.

https://docs.unity3d.com/ja/2018.4/Manual/CSharpCompiler.html

## Tested environment

| Platform | Version |
| --- | --- |
| Android | 16 |
| iOS | 18.5 |
| iPadOS | 18.5 |
| Ubuntu | 24.04.1 |
| macOS | 15.5 |
| Windows 11 | 24H2 (*4) |
| Google Chrome | 131.0.6778.86 |
| Chrome OS x86_64 | 130.0.6723.126 |
| Chrome OS ARMv8 (*5) | 130.0.6723.126　|
| HarmonyOS NEXT | 5.0.0.102 |

*4 In Unity 6, we have not verified the operation of builds for UWP.
*5 This is a confirmation of operation with APK files for Android.

## Branches

If you're looking for the latest features and/or fixes, all development takes place in the `develop` branch.

The `master` branch is brought into sync with the `develop` branch once for every official SDK release.

## Usage

Simply copy all files under `./Assets` into the folder where the Live2D Cubism SDK is located in your Unity project.

### Unsafe Blocks

The Core wrapper requires unsafe code blocks to be allowed, and the C# project Unity creates is patched accordingly. If unsafe code isn't an option for you, currently the best way is to compile the components and drop that dll into your Unity project.

## Contributing

There are many ways to contribute to the project: logging bugs, submitting pull requests on this GitHub, and reporting issues and making suggestions in Live2D Community.

### Forking And Pull Requests

We very much appreciate your pull requests, whether they bring fixes, improvements, or even new features. To keep the main repository as clean as possible, create a personal fork and feature branches there as needed.

### Bugs

We are regularly checking issue-reports and feature requests at Live2D Community. Before filing a bug report, please do a search in Live2D Community to see if the issue-report or feature request has already been posted. If you find your issue already exists, make relevant comments and add your reaction.

### Suggestions

We're also interested in your feedback for the future of the SDK. You can submit a suggestion or feature request at Live2D Community. To make this process more effective, we're asking that you include more information to help define them more clearly.

## Coding Guidelines

### Naming

Try to stick to the [Microsoft guidelines](https://msdn.microsoft.com/en-us/library/ms229002(v=vs.110).aspx) whenever possible. We name private fields in lower-camelcase starting with an underscore.

### Style

- In Unity Editor extension, try to write expressive code with LINQ and all the other fancy stuff.
- Stay away from LINQ and prefer `for` over `foreach` anywhere else.
- Try to be explicit. Prefer `private void Update()` over `void Update()`.

## Forum

If you have any questions, please join the official Live2D forum and discuss with other users.

- [Live2D Creator's Forum](https://community.live2d.com/)
- [Live2D 公式クリエイターズフォーラム (Japanese)](https://creatorsforum.live2d.com/)
