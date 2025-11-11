import React from 'react';
import { Platform, View, StyleSheet } from 'react-native';
import { WebView } from 'react-native-webview';
import { GameType } from '../../types';

interface UnityFrameProps {
  webViewRef: React.RefObject<any>;
  onMessage: (event: any) => void;
  gameType: GameType;
  style?: any;
}

export const UnityFrame: React.FC<UnityFrameProps> = ({ webViewRef, onMessage, gameType, style }) => {
  // Map game type to Unity folder name
  const getUnityFolderName = (type: GameType): string => {
    switch (type) {
      case GameType.SOLITAIRE:
        return 'Solitaire';
      case GameType.MEMORY_MATCH:
        return 'MemoryMatch';
      default:
        console.warn('[UnityFrame] Unknown game type:', type);
        return 'Solitaire';
    }
  };

  if (Platform.OS === 'web') {
    // For web platform, use iframe
    React.useEffect(() => {
      console.log('[UnityFrame] Setting up message listener for web platform');
      console.log('[UnityFrame] Expected origin:', window.location.origin);

      const handleMessage = (event: MessageEvent) => {
        console.log('[UnityFrame] Message received from iframe:', {
          origin: event.origin,
          expectedOrigin: window.location.origin,
          data: event.data,
          source: event.source
        });

        // Only accept messages from our origin
        if (event.origin === window.location.origin) {
          console.log('[UnityFrame] ✅ Origin matches, forwarding to onMessage');
          onMessage({ nativeEvent: { data: event.data } });
        } else {
          console.warn('[UnityFrame] ❌ Origin mismatch, ignoring message');
        }
      };

      window.addEventListener('message', handleMessage);
      console.log('[UnityFrame] Message listener attached');

      return () => {
        console.log('[UnityFrame] Removing message listener');
        window.removeEventListener('message', handleMessage);
      };
    }, [onMessage]);

    // Construct Unity path based on game type and current location for GitHub Pages compatibility
    const getUnityPath = () => {
      const folderName = getUnityFolderName(gameType);
      const basePath = window.location.pathname.includes('/cardGameCollection')
        ? `/cardGameCollection/unity/${folderName}/index.html`
        : `/unity/${folderName}/index.html`;
      console.log('[UnityFrame] Using Unity path:', basePath);
      console.log('[UnityFrame] Game type:', gameType);
      console.log('[UnityFrame] Folder name:', folderName);
      return basePath;
    };

    const unityPath = getUnityPath();

    // Auto-focus iframe when it loads to ensure Unity starts immediately
    React.useEffect(() => {
      const iframe = webViewRef.current as HTMLIFrameElement;
      if (iframe) {
        const handleLoad = () => {
          console.log('[UnityFrame] iframe loaded, focusing...');
          iframe.contentWindow?.focus();
        };
        iframe.addEventListener('load', handleLoad);
        return () => iframe.removeEventListener('load', handleLoad);
      }
    }, [webViewRef]);

    return (
      <iframe
        ref={webViewRef as any}
        src={unityPath}
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
  const folderName = getUnityFolderName(gameType);
  const nativeUri = `http://localhost:8081/unity/${folderName}/index.html`;

  return (
    <WebView
      ref={webViewRef}
      source={{ uri: nativeUri }}
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
        console.log('WebView loaded:', nativeUri);
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
