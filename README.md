## UnityNativeEdit
Unity Native Input Plugin for both iOS and Android (Unity UI InputField compatible).
This means you don't need a separate 'Unity' Input box and you can use all native text functions such as `Select`, `Copy` and `Paste`

## Usage
1. Simply copy the files in `release/Assets` into your existing unity project asset folder.
2. Make empty Gameobject and attach ```PluginMsgHandler``` to your new GameObject
3. Attach ```NativeEditBox``` script to your UnityUI ```InputField```object.
4. Build and run on your android or ios device!


## Etc
1. NativeEditBox will work with delegate defined in your Unity UI InputField, `On Value Change` and `End Edit`
2. It's open source and free to use/redistribute!
3. Please refer to `demo` Unity project.
