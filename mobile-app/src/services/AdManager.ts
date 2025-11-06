/**
 * Ad Manager - Centralized ad handling
 * Manages when and where ads are shown
 */

export class AdManager {
  private static gamesPlayedSinceAd = 0;
  private static readonly INTERSTITIAL_FREQUENCY = 3; // Show every 3 games

  /**
   * Check if interstitial should be shown
   */
  static shouldShowInterstitial(): boolean {
    this.gamesPlayedSinceAd++;

    if (this.gamesPlayedSinceAd >= this.INTERSTITIAL_FREQUENCY) {
      this.gamesPlayedSinceAd = 0;
      return true;
    }

    return false;
  }

  /**
   * Show interstitial ad (to be implemented with AdMob)
   */
  static async showInterstitial(): Promise<void> {
    // TODO: Implement with react-native-google-mobile-ads
    console.log('[AdManager] Would show interstitial ad');

    // Example:
    // const interstitial = InterstitialAd.createForAdRequest('ad-unit-id');
    // await interstitial.load();
    // await interstitial.show();
  }

  /**
   * Show rewarded ad for bonus coins
   */
  static async showRewardedAd(onReward: (coins: number) => void): Promise<void> {
    // TODO: Implement with react-native-google-mobile-ads
    console.log('[AdManager] Would show rewarded ad');

    // Example:
    // const rewarded = RewardedAd.createForAdRequest('ad-unit-id');
    // await rewarded.load();
    // rewarded.addAdEventListener('onUserEarnedReward', () => {
    //   onReward(50); // Give 50 coins
    // });
    // await rewarded.show();

    // For now, just give reward (testing)
    setTimeout(() => onReward(50), 1000);
  }

  /**
   * Show banner ad
   */
  static showBanner(): void {
    // TODO: Implement with react-native-google-mobile-ads
    console.log('[AdManager] Would show banner ad');
  }

  /**
   * Hide banner ad
   */
  static hideBanner(): void {
    console.log('[AdManager] Would hide banner ad');
  }
}
