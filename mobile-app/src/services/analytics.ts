import { Platform } from 'react-native';
import { logEvent, setUserId, setUserProperties } from 'firebase/analytics';
import { analytics as firebaseAnalytics } from '../config/firebase';

/**
 * Analytics Service
 * Tracks user events across web and mobile using Firebase Analytics
 *
 * Provides unified tracking across all platforms
 *
 * Current integrations:
 * - Firebase Analytics (events, user properties)
 *
 * Future integrations:
 * - Firebase Crashlytics (error tracking)
 * - AdMob (ad revenue tracking via Firebase)
 * - RevenueCat (IAP revenue tracking)
 */

interface AnalyticsEvent {
  name: string;
  properties?: Record<string, string | number | boolean>;
}

class AnalyticsService {
  private isEnabled: boolean = true;

  /**
   * Initialize analytics
   */
  initialize() {
    console.log('[Analytics] Firebase Analytics initialized for platform:', Platform.OS);
  }

  /**
   * Track a custom event
   */
  trackEvent(eventName: string, properties?: Record<string, string | number | boolean>) {
    if (!this.isEnabled) return;

    console.log('[Analytics] Event:', eventName, properties);

    if (Platform.OS === 'web' && firebaseAnalytics) {
      try {
        logEvent(firebaseAnalytics, eventName, properties as any);
      } catch (error) {
        console.error('[Analytics] Error logging event:', error);
      }
    } else {
      // For mobile, Firebase Analytics would work here too
      // For now just log
      console.log('[Mobile Analytics]', eventName, properties);
    }
  }

  /**
   * Track page view
   */
  trackPageView(pageName: string) {
    if (!this.isEnabled) return;

    console.log('[Analytics] Page view:', pageName);

    if (Platform.OS === 'web' && firebaseAnalytics) {
      try {
        logEvent(firebaseAnalytics, 'page_view', {
          page_title: pageName,
          page_location: window.location.href,
          page_path: window.location.pathname,
        });
      } catch (error) {
        console.error('[Analytics] Error logging page view:', error);
      }
    } else {
      this.trackEvent('screen_view', { screen_name: pageName });
    }
  }

  /**
   * Track game events
   */
  trackGameStart(gameType: string) {
    this.trackEvent('game_start', { game_type: gameType });
  }

  trackGameComplete(gameType: string, won: boolean, score: number, timeSeconds: number) {
    this.trackEvent('game_complete', {
      game_type: gameType,
      won: won.toString(),
      score,
      time_seconds: timeSeconds,
    });
  }

  trackGameQuit(gameType: string, timeSeconds: number) {
    this.trackEvent('game_quit', {
      game_type: gameType,
      time_seconds: timeSeconds,
    });
  }

  /**
   * Track app open
   */
  trackAppOpen() {
    this.trackEvent('app_open', {
      platform: Platform.OS,
      version: Platform.Version?.toString() || 'unknown',
    });
  }

  /**
   * Track language change
   */
  trackLanguageChange(language: string) {
    this.trackEvent('language_change', { language });
  }

  /**
   * Set user ID for analytics
   */
  setUser(userId: string) {
    if (Platform.OS === 'web' && firebaseAnalytics) {
      try {
        setUserId(firebaseAnalytics, userId);
        console.log('[Analytics] User ID set:', userId);
      } catch (error) {
        console.error('[Analytics] Error setting user ID:', error);
      }
    }
  }

  /**
   * Set user properties
   */
  setUserProperty(propertyName: string, propertyValue: string) {
    if (Platform.OS === 'web' && firebaseAnalytics) {
      try {
        setUserProperties(firebaseAnalytics, { [propertyName]: propertyValue });
        console.log('[Analytics] User property set:', propertyName, propertyValue);
      } catch (error) {
        console.error('[Analytics] Error setting user property:', error);
      }
    }
  }

  /**
   * Disable analytics (for privacy/GDPR)
   */
  disable() {
    this.isEnabled = false;
    console.log('[Analytics] Disabled');
  }

  /**
   * Enable analytics
   */
  enable() {
    this.isEnabled = true;
    console.log('[Analytics] Enabled');
  }
}

// Export singleton instance
export const analytics = new AnalyticsService();
