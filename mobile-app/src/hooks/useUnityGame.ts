import { useRef, useState, useCallback } from 'react';
import { Platform } from 'react-native';

interface GameState {
  score: number;
  moves: number;
  time: string;
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
  });
  const [gameEndData, setGameEndData] = useState<GameEndData | null>(null);
  const [isReady, setIsReady] = useState(false);

  // Handle messages from Unity
  const handleUnityMessage = useCallback((event: any) => {
    try {
      const message: UnityMessage = JSON.parse(event.nativeEvent.data);

      switch (message.type) {
        case 'gameReady':
          setIsReady(true);
          break;

        case 'gameState':
          setGameState(message.payload);
          break;

        case 'gameEnd':
          setGameEndData(message.payload);
          break;

        default:
          console.warn('[Unity] Unknown message type:', message.type);
      }
    } catch (error) {
      console.error('[Unity] Failed to parse message:', error);
    }
  }, []);

  // Send action to Unity
  const sendToUnity = useCallback((action: string, data?: any) => {
    if (!isReady) return;

    const message = JSON.stringify({ action, data: data || {} });

    if (Platform.OS === 'web') {
      // For web platform, use iframe postMessage
      const iframe = webViewRef.current as HTMLIFrameElement;

      if (iframe && iframe.contentWindow) {
        try {
          const script = `
            if (window.unityInstance) {
              window.unityInstance.SendMessage('ReactBridge', 'ReceiveFromReact', '${message.replace(/'/g, "\\'")}');
            }
          `;
          iframe.contentWindow.eval(script);
          iframe.contentWindow.focus(); // Keep Unity running
        } catch (error) {
          console.error('[Unity] Failed to send message:', error);
        }
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

  return {
    webViewRef,
    gameState,
    gameEndData,
    isReady,
    handleUnityMessage,
    newGame,
    restart,
    undo,
  };
};
