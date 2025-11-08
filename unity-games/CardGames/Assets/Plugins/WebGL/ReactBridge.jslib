mergeInto(LibraryManager.library, {
  SendMessageToReact: function(message) {
    var messageStr = UTF8ToString(message);

    if (typeof window.SendMessageToReact === 'function') {
      window.SendMessageToReact(messageStr);
    } else {
      console.error('[Unity] SendMessageToReact function not found in window scope');
    }
  }
});
