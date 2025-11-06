/**
 * Unity WebGL Bridge for React Native Communication
 * This plugin provides JavaScript functions that Unity can call
 */

mergeInto(LibraryManager.library, {

  /**
   * Send message from Unity to JavaScript/React Native
   * @param {string} messageType - Type of message (e.g., "GameReady", "ScoreUpdate")
   * @param {string} data - JSON string with message data
   */
  SendMessageToJS: function(messageType, data) {
    // Convert Unity strings (UTF16LE pointer) to JavaScript strings
    var type = UTF8ToString(messageType);
    var jsonData = UTF8ToString(data);

    console.log('[Unity → JS] Message:', type, jsonData);

    // Try to send to React Native if available
    if (window.ReactNativeWebView) {
      // For React Native WebView
      window.ReactNativeWebView.postMessage(JSON.stringify({
        type: type,
        data: jsonData
      }));
    } else if (window.parent && window.parent !== window) {
      // For iframe embedding (our case)
      window.parent.postMessage({
        source: 'unity',
        type: type,
        data: jsonData
      }, '*');
    } else {
      // Fallback: dispatch custom event
      window.dispatchEvent(new CustomEvent('UnityMessage', {
        detail: {
          type: type,
          data: jsonData
        }
      }));
    }
  }

});
