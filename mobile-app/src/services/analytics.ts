import { Platform } from 'react-native';

/**
 * Analytics Service
 * Tracks user events across web and mobile
 *
 * Web: Uses Plausible (privacy-friendly, no cookies)
 * Mobile: Uses Expo Analytics (or can integrate Firebase)
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
    console.log('[Analytics] Initialized for platform:', Platform.OS);
  }

  /**
   * Track a custom event
   */
  trackEvent(eventName: string, properties?: Record<string, string | number | boolean>) {
    if (!this.isEnabled) return;

    console.log('[Analytics] Event:', eventName, properties);

    if (Platform.OS === 'web') {
      this.trackWebEvent(eventName, properties);
    } else {
      this.trackMobileEvent(eventName, properties);
    }
  }

  /**
   * Track page view
   */
  trackPageView(pageName: string) {
    if (!this.isEnabled) return;

    console.log('[Analytics] Page view:', pageName);

    if (Platform.OS === 'web') {
      // Plausible automatically tracks page views
      // Just log for debugging
    } else {
      this.trackMobileEvent('screen_view', { screen_name: pageName });
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
   * Web analytics (Plausible)
   */
  private trackWebEvent(eventName: string, properties?: Record<string, string | number | boolean>) {
    // @ts-ignore - plausible is loaded via script tag
    if (typeof window !== 'undefined' && window.plausible) {
      // @ts-ignore
      window.plausible(eventName, { props: properties });
    }
  }

  /**
   * Mobile analytics
   */
  private trackMobileEvent(eventName: string, properties?: Record<string, string | number | boolean>) {
    // For now just log - can integrate Firebase Analytics later
    console.log('[Mobile Analytics]', eventName, properties);

    // TODO: Integrate Firebase Analytics or similar
    // firebaseAnalytics.logEvent(eventName, properties);
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
