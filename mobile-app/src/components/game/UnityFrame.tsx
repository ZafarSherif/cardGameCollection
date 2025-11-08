import React from 'react';
import { Platform, View, StyleSheet } from 'react-native';
import { WebView } from 'react-native-webview';

interface UnityFrameProps {
  webViewRef: React.RefObject<any>;
  onMessage: (event: any) => void;
  style?: any;
}

export const UnityFrame: React.FC<UnityFrameProps> = ({ webViewRef, onMessage, style }) => {
  if (Platform.OS === 'web') {
    // For web platform, use iframe
    React.useEffect(() => {
      const handleMessage = (event: MessageEvent) => {
        // Only accept messages from our origin
        if (event.origin === window.location.origin) {
          onMessage({ nativeEvent: { data: event.data } });
        }
      };

      window.addEventListener('message', handleMessage);
      return () => window.removeEventListener('message', handleMessage);
    }, [onMessage]);

    return (
      <iframe
        ref={webViewRef as any}
        src="/unity/index.html"
        style={{
          width: '100%',
          height: '100%',
          border: 'none',
          backgroundColor: '#1a1a1a',
        }}
        title="Unity Game"
      />
    );
  }

  // For native platforms (iOS/Android), use WebView
  return (
    <WebView
      ref={webViewRef}
      source={{ uri: 'http://localhost:8081/unity/index.html' }}
      style={style}
      onMessage={onMessage}
      javaScriptEnabled
      domStorageEnabled
      allowsInlineMediaPlayback
      mediaPlaybackRequiresUserAction={false}
      onError={(syntheticEvent) => {
        const { nativeEvent } = syntheticEvent;
        console.error('WebView error:', nativeEvent);
      }}
      onLoadEnd={() => {
        console.log('WebView loaded');
      }}
    />
  );
};

const styles = StyleSheet.create({
  webview: {
    flex: 1,
    backgroundColor: '#1a1a1a',
  },
});
