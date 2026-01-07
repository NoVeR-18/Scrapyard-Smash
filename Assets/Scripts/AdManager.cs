using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private const string BannerAdUnitId = "ca-app-pub-5782449081814912/6706781010";
    private const string InterstitialAdUnitId = "ca-app-pub-5782449081814912/9081006373";
    private const string RewardedAdUnitId = "ca-app-pub-5782449081814912/1418138777";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private Action onInterstitialClosed;
    private Action onRewardEarned;

    #region UNITY

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MobileAds.Initialize(status =>
        {
            Debug.Log("AdMob AdMob initialized");

            RequestBanner();
            RequestRewarded();
            RequestInterstitial();
        });
    }

    #endregion

    #region BANNER

    private void RequestBanner()
    {
        if (bannerView != null)
            return;

        bannerView = new BannerView(BannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

        bannerView.OnBannerAdLoaded += () =>
            Debug.Log("AdMob Banner loaded");

        bannerView.OnBannerAdLoadFailed += error =>
            Debug.LogWarning("AdMob Banner failed: " + error);

        bannerView.LoadAd(new AdRequest());
    }

    #endregion

    #region INTERSTITIAL

    public void RequestInterstitial()
    {
        InterstitialAd.Load(InterstitialAdUnitId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogWarning("AdMob Interstitial failed: " + error);
                    return;
                }

                interstitialAd = ad;

                interstitialAd.OnAdFullScreenContentOpened += () =>
                    Debug.Log("AdMob Interstitial opened");

                interstitialAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("AdMob Interstitial closed");

                    onInterstitialClosed?.Invoke();
                    onInterstitialClosed = null;

                    interstitialAd.Destroy();
                    interstitialAd = null;

                    RequestInterstitial();
                };

                interstitialAd.OnAdFullScreenContentFailed += err =>
                {
                    Debug.LogWarning("AdMob Interstitial show failed: " + err);
                    interstitialAd = null;
                    RequestInterstitial();
                };

                Debug.Log("AdMob Interstitial loaded");
            });
    }

    public void ShowInterstitial(Action onClosed = null)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            onInterstitialClosed = onClosed;
            interstitialAd.Show();
        }
        else
        {
            onClosed?.Invoke();
        }
    }

    public bool IsInterstitialReady()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
    }

    #endregion

    #region REWARDED

    public void RequestRewarded()
    {
        RewardedAd.Load(RewardedAdUnitId, new AdRequest(),
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogWarning("AdMob Rewarded failed: " + error);
                    return;
                }

                rewardedAd = ad;

                rewardedAd.OnAdFullScreenContentOpened += () =>
                    Debug.Log("AdMob Rewarded opened");

                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("AdMob Rewarded closed");

                    rewardedAd.Destroy();
                    rewardedAd = null;

                    RequestRewarded();
                };

                rewardedAd.OnAdFullScreenContentFailed += err =>
                {
                    Debug.LogWarning("AdMob Rewarded show failed: " + err);
                    rewardedAd = null;
                    RequestRewarded();
                };

                Debug.Log("AdMob Rewarded loaded");
            });
    }

    public void ShowRewarded(Action onReward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            onRewardEarned = onReward;

            rewardedAd.Show(reward =>
            {
                Debug.Log("AdMob User earned reward");
                onRewardEarned?.Invoke();
                onRewardEarned = null;
            });
        }
        else
        {
            Debug.Log("AdMob Rewarded not ready");
        }
    }

    public bool IsRewardedReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    #endregion
}
