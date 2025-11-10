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
      const handleMessage = (event: MessageEvent) => {
        // Only accept messages from our origin
        if (event.origin === window.location.origin) {
          onMessage({ nativeEvent: { data: event.data } });
        }
      };

      window.addEventListener('message', handleMessage);
      return () => window.removeEventListener('message', handleMessage);
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
