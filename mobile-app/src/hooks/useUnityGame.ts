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

      console.log('[Unity Message]', message.type, message.payload);

      switch (message.type) {
        case 'gameReady':
          setIsReady(true);
          console.log('✅ Unity game is ready');
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
      console.error('[Unity] Failed to parse message:', error, event.nativeEvent.data);
    }
  }, []);

  // Send action to Unity
  const sendToUnity = useCallback((action: string, data?: any) => {
    console.log('[1/6] sendToUnity called:', { action, data, isReady });

    if (!isReady) {
      console.warn('[Unity] Game not ready yet, queuing action:', action);
      // Could implement a queue here if needed
      return;
    }

    const message = JSON.stringify({ action, data: data || {} });
    console.log('[2/6] Message constructed:', message);
    console.log('[3/6] Platform:', Platform.OS);

    if (Platform.OS === 'web') {
      // For web platform, use iframe postMessage
      const iframe = webViewRef.current as HTMLIFrameElement;
      console.log('[4/6] iframe ref:', iframe ? 'exists' : 'null');
      console.log('[5/6] iframe.contentWindow:', iframe?.contentWindow ? 'exists' : 'null');

      if (iframe && iframe.contentWindow) {
        const script = `
          console.log('[Unity iframe] Executing script for action: ${action}');
          if (window.unityInstance) {
            console.log('[Unity iframe] Unity instance found, calling SendMessage');
            window.unityInstance.SendMessage('ReactBridge', 'ReceiveFromReact', '${message.replace(/'/g, "\\'")}');
            console.log('[Unity iframe] SendMessage called successfully');
          } else {
            console.error('[Unity iframe] Unity instance not found');
            console.log('[Unity iframe] window.unityInstance =', window.unityInstance);
          }
        `;
        console.log('[6/6] Executing script in iframe');
        try {
          iframe.contentWindow.eval(script);
          console.log('[6/6] Script executed successfully');
        } catch (error) {
          console.error('[6/6] Script execution failed:', error);
        }
      } else {
        console.error('[ERROR] iframe or contentWindow not available');
      }
    } else {
      // For native platforms, use WebView injectJavaScript
      const script = `
        if (window.unityInstance) {
          window.unityInstance.SendMessage('ReactBridge', 'ReceiveFromReact', '${message.replace(/'/g, "\\'")}');
        } else {
          console.error('Unity instance not found');
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
