# ReactBridge WebGL Template

This template provides:
1. **React Native communication bridge** - Enables Unity ↔ React Native messaging
2. **Automatic build file naming** - Uses correct game-specific file names (no more manual edits!)

## Features

### Unity Template Variables (Auto-replaced during build)
- `{{{ PRODUCT_NAME }}}` → Game name (MemoryMatch, Solitaire, etc.)
- `{{{ LOADER_FILENAME }}}` → Correct loader file (MemoryMatch.loader.js)
- `{{{ DATA_FILENAME }}}` → Correct data file (MemoryMatch.data)
- `{{{ FRAMEWORK_FILENAME }}}` → Correct framework file (MemoryMatch.framework.js)
- `{{{ CODE_FILENAME }}}` → Correct WASM file (MemoryMatch.wasm)
- `{{{ COMPANY_NAME }}}` → Company name from Player Settings
- `{{{ PRODUCT_VERSION }}}` → Version from Player Settings

### React Native Bridge
- `window.SendMessageToReact(message)` - Called by Unity C# to send messages to React
- `window.ReactNativeWebView.postMessage()` - Sends messages to React Native WebView
- Fallback support for web testing via `window.parent.postMessage()`

## How It Works

When you build in Unity, all `{{{ VARIABLE }}}` placeholders are automatically replaced with the correct values based on your Player Settings.

**No more manual editing of index.html after every build!**

## Unity Setup

This template should already be selected. To verify:

1. Go to **File > Build Settings**
2. Select **WebGL** platform
3. Click **Player Settings** button
4. Under **Resolution and Presentation**, check that **WebGL Template** is set to **ReactBridge**

## Building

Just build normally - the template handles everything automatically:
1. File > Build Settings
2. Click **Build**
3. Choose destination folder
4. The generated index.html will have correct file references!
