import { useRef, useState, useCallback } from 'react';
import { Platform } from 'react-native';

interface GameState {
  score: number;
  moves: number;
  time: string;
  matches: number;
}

interface GameEndData {
  won: boolean;
  finalScore: number;
  finalTime: string;
}

interface UnityMessage {
  type: string;
  payload: any;
}

export const useUnityGame = () => {
  const webViewRef = useRef<any>(null);
  const [gameState, setGameState] = useState<GameState>({
    score: 0,
    moves: 0,
    time: '00:00',
    matches: 0,
  });
  const [gameEndData, setGameEndData] = useState<GameEndData | null>(null);
  const [isReady, setIsReady] = useState(false);

  // Handle messages from Unity
  const handleUnityMessage = useCallback((event: any) => {
    console.log('[Unity → React] Raw event received:', event);
    console.log('[Unity → React] Event data:', event.nativeEvent?.data);

    try {
      const message: UnityMessage = JSON.parse(event.nativeEvent.data);
      console.log('[Unity → React] Parsed message:', message);

      switch (message.type) {
        case 'gameReady':
          console.log('[Unity → React] ✅ Game Ready! Setting isReady to true');
          setIsReady(true);
          break;

        case 'gameState':
          console.log('[Unity → React] Game state update:', message.payload);
          setGameState(message.payload);
          break;

        case 'gameEnd':
          console.log('[Unity → React] Game ended:', message.payload);
          setGameEndData(message.payload);
          break;

        default:
          console.warn('[Unity → React] Unknown message type:', message.type);
      }
    } catch (error) {
      console.error('[Unity → React] Failed to parse message:', error, 'Raw data:', event.nativeEvent?.data);
    }
  }, []);

  // Send action to Unity
  const sendToUnity = useCallback((action: string, data?: any) => {
    console.log('[React → Unity] sendToUnity called:', { action, data, isReady });

    if (!isReady) {
      console.warn('[React → Unity] Unity not ready yet, ignoring message');
      return;
    }

    const message = JSON.stringify({
      action,
      data: data ? JSON.stringify(data) : ""
    });

    console.log('[React → Unity] Sending message:', message);

    if (Platform.OS === 'web') {
      // For web platform, use iframe postMessage
      const iframe = webViewRef.current as HTMLIFrameElement;

      if (iframe && iframe.contentWindow) {
        try {
          // Properly escape the message for JavaScript string literal (escape backslashes AND quotes)
          const escapedMessage = message.replace(/\\/g, '\\\\').replace(/'/g, "\\'");

          const script = `
            if (window.unityInstance) {
              window.unityInstance.SendMessage('ReactBridge', 'ReceiveFromReact', '${escapedMessage}');
              console.log('[iframe] Sent to Unity:', '${escapedMessage}');
            } else {
              console.error('[iframe] window.unityInstance not found!');
            }
          `;
          iframe.contentWindow.eval(script);
          iframe.contentWindow.focus(); // Keep Unity running
          console.log('[React → Unity] Message sent successfully');
        } catch (error) {
          console.error('[Unity] Failed to send message:', error);
        }
      } else {
        console.error('[React → Unity] iframe or contentWindow not available');
      }
    } else {
      // For native platforms, use WebView injectJavaScript
      const script = `
        if (window.unityInstance) {
          window.unityInstance.SendMessage('ReactBridge', 'ReceiveFromReact', '${message.replace(/'/g, "\\'")}');
        }
      `;
      webViewRef.current?.injectJavaScript(script);
    }
  }, [isReady]);

  // Game actions
  const newGame = useCallback(() => {
    sendToUnity('newGame');
    setGameEndData(null);
  }, [sendToUnity]);

  const restart = useCallback(() => {
    sendToUnity('restart');
    setGameEndData(null);
  }, [sendToUnity]);

  const undo = useCallback(() => {
    sendToUnity('undo');
  }, [sendToUnity]);

  const setDifficulty = useCallback((difficulty: string) => {
    console.log('trying to send string : ', difficulty);
    sendToUnity('setDifficulty', { difficulty });
  }, [sendToUnity]);

  return {
    webViewRef,
    gameState,
    gameEndData,
    isReady,
    handleUnityMessage,
    newGame,
    restart,
    undo,
    setDifficulty,
  };
};
