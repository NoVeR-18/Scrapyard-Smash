using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    // Ad Unit IDs
    private const string BannerAdUnitId = "ca-app-pub-5782449081814912/6706781010";
    private const string InterstitialAdUnitId = "ca-app-pub-5782449081814912/9081006373";
    private const string RewardedAdUnitId = "ca-app-pub-5782449081814912/1418138777";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    public InterstitialAd InterstitialAd { get => interstitialAd; set => interstitialAd = value; }

    private Action _onRevardAddEarn;
    private Action _onInterstitialClosed;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialized.");
            RequestBanner();
            RequestInterstitial();
            RequestRewardedAd();
        });
    }

    #region BANNER

    public void RequestBanner()
    {
        bannerView = new BannerView(BannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

        bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        bannerView.OnBannerAdLoadFailed += OnBannerAdFailedToLoad;
        bannerView.OnAdClicked += OnBannerAdOpened;
        bannerView.OnAdFullScreenContentClosed += OnBannerAdClosed;

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }



    private void OnBannerAdLoaded()
    {
        Debug.Log("Banner ad loaded.");
    }

    private void OnBannerAdFailedToLoad(LoadAdError error)
    {
        Debug.LogWarning("Banner ad failed to load: " + error.ToString());
    }

    private void OnBannerAdOpened()
    {
        Debug.Log("Banner ad opened.");
    }

    private void OnBannerAdClosed()
    {
        Debug.Log("Banner ad closed.");
    }

    #endregion

    #region INTERSTITIAL

    public void RequestInterstitial()
    {
        Debug.Log("Requesting interstitial ad...");
        var adRequest = new AdRequest();
        InterstitialAd.Load(InterstitialAdUnitId, adRequest,
          (InterstitialAd ad, LoadAdError error) =>
          {
              Debug.Log("Interstitial ad load callback invoked.");
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("interstitial ad failed to load an ad " +
                                 "with error : " + error);
                  return;
              }

              Debug.Log("Interstitial ad loaded with response : "
                        + ad.GetResponseInfo());

              interstitialAd = ad;
          });

        interstitialAd.OnAdFullScreenContentFailed += OnInterstitialAdFailedToLoad;
        interstitialAd.OnAdFullScreenContentOpened += OnInterstitialAdOpened;
        interstitialAd.OnAdFullScreenContentClosed += OnInterstitialAdClosed;

    }

    public void ShowInterstitialAd(Action onAddClosed = null)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            _onInterstitialClosed = onAddClosed;
            interstitialAd.Show();
            _onInterstitialClosed.Invoke();
        }
        else
        {
            Debug.Log("Interstitial ad not loaded yet.");
            onAddClosed?.Invoke();
        }
    }

    private void OnInterstitialAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("Interstitial ad loaded.");
    }

    private void OnInterstitialAdFailedToLoad(AdError error)
    {
        Debug.LogWarning("Interstitial ad failed to load: " + error);
    }

    private void OnInterstitialAdOpened()
    {
        Debug.Log("Interstitial ad opened.");
    }

    private void OnInterstitialAdClosed()
    {
        Debug.Log("Interstitial ad closed.");
        _onInterstitialClosed?.Invoke();
        _onInterstitialClosed = null;

        RequestInterstitial();
    }

    public bool IsInterstitialAdReady()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
    }
    #endregion

    #region REWARDED

    public void RequestRewardedAd()
    {
        var adRequest = new AdRequest();
        RewardedAd.Load(RewardedAdUnitId, adRequest,
          (RewardedAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("Rewarded ad failed to load an ad " +
                                 "with error : " + error);
                  return;
              }

              Debug.Log("Rewarded ad loaded with response : "
                        + ad.GetResponseInfo());

              rewardedAd = ad;
          });

        rewardedAd.OnAdFullScreenContentOpened += OnRewardedAdOpened;
        rewardedAd.OnAdFullScreenContentClosed += OnRewardedAdClosed;
        rewardedAd.OnAdFullScreenContentClosed += OnUserEarnedReward;

    }

    public void ShowRewardedAd(Action onRewardEarned = null)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            _onRevardAddEarn = onRewardEarned;
            rewardedAd.Show((Reward) => { OnUserEarnedReward(); });
            _onRevardAddEarn?.Invoke();
        }
        else
        {
            Debug.Log("Rewarded ad not loaded yet.");
        }
    }

    private void OnRewardedAdLoaded(object sender, EventArgs args) => Debug.Log("Rewarded ad loaded.");

    private void OnRewardedAdFailedToLoad() => Debug.LogWarning("Rewarded ad failed to load: ");

    private void OnRewardedAdOpened() => Debug.Log("Rewarded ad opened.");

    private void OnRewardedAdClosed()
    {
        Debug.Log("Rewarded ad closed.");
        RequestRewardedAd(); // Reload for next time
    }

    public bool IsRewardedAdReady()
    {
        if (rewardedAd == null)
            Debug.Log("Rewarded ad is null.");
        Debug.Log("Checking if rewarded ad is ready." + rewardedAd.CanShowAd());
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    private void OnUserEarnedReward()
    {
        _onRevardAddEarn?.Invoke();

        _onRevardAddEarn = null;
        Debug.Log($"User earned reward:");
        // TODO: Grant in-game reward here
    }

    #endregion
}
