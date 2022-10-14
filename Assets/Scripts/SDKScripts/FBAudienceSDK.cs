using AudienceNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBAudienceSDK : MonoBehaviour
{
    [Header("FacebookAudience Keys")]
    public string interstitialID;
    public string rewardedID;

    private SDKController SDKController;
    private InterstitialAd interstitialAd;
    private RewardedVideoAd rewardedAd;

    private bool isInterLoaded;
    private bool isRewardLoaded;

    private void Awake()
    {
        AudienceNetworkAds.Initialize();
    }

    private void Start()
    {
        SDKController = GetComponent<SDKController>();
      //  init();
    }

    /// <summary>
    /// Иниц Событий и переменных класса
    /// </summary>
    private void init()
    {
        isInterLoaded = false;
        isRewardLoaded = false;
        interstitialAd = new InterstitialAd(interstitialID);
        interstitialAd.Register(gameObject);
        rewardedAd = new RewardedVideoAd(rewardedID);
        rewardedAd.Register(gameObject);

        #region INTERSTITIAL EVENT
        interstitialAd.InterstitialAdDidLoad = delegate ()
        {
            SDKController.Message = "Facebook Interstitial ad loaded.";
            isInterLoaded = true;
            // didClose = false;
        };
        interstitialAd.InterstitialAdDidFailWithError = delegate (string error)
        {
            SDKController.Message = "Interstitial ad failed to load. " + error;
        };
        interstitialAd.InterstitialAdWillLogImpression = delegate ()
        {
            Debug.Log("Facebook Interstitial ad logged impression.");
            SDKController.Message = "Facebook Interstitial ad logged impression.";
        };
        interstitialAd.InterstitialAdDidClick = delegate ()
        {
            SDKController.Message = "Facebook Interstitial ad clicked";
            Debug.Log("Facebook Interstitial ad clicked.");
        };
        interstitialAd.InterstitialAdDidClose = delegate ()
        {
            Debug.Log("Facebook Interstitial ad did close.");
            SDKController.Message = "Facebook Interstitial ad did close";
            //  didClose = true;
            if (interstitialAd != null)
            {
                interstitialAd.Dispose();
            }
        };

        #endregion

        #region REWARDED EVENT
        // Set delegates to get notified on changes or when the user interacts with the ad.
        rewardedAd.RewardedVideoAdDidLoad = delegate ()
        {
            Debug.Log("RewardedVideo ad loaded.");
            isRewardLoaded = true;
            //didClose = false;
            string isAdValid = rewardedAd.IsValid() ? "valid" : "invalid";
            SDKController.Message = "Ad loaded and is " + isAdValid + ". Click show to present!";
        };
        rewardedAd.RewardedVideoAdDidFailWithError = delegate (string error)
        {
            Debug.Log("RewardedVideo ad failed to load with error: " + error);
            SDKController.Message = "RewardedVideo ad failed to load. Check console for details.";
        };
        rewardedAd.RewardedVideoAdWillLogImpression = delegate ()
        {
            Debug.Log("RewardedVideo ad logged impression.");
        };
        rewardedAd.RewardedVideoAdDidClick = delegate ()
        {
            Debug.Log("RewardedVideo ad clicked.");
        };
        rewardedAd.RewardedVideoAdDidSucceed = delegate ()
        {
            Debug.Log("Rewarded video ad validated by server");
        };

        rewardedAd.RewardedVideoAdDidFail = delegate ()
        {
            Debug.Log("Rewarded video ad not validated, or no response from server");
        };

        rewardedAd.RewardedVideoAdDidClose = delegate ()
        {
            Debug.Log("Rewarded video ad did close.");
            // didClose = true;
            if (rewardedAd != null)
            {
                rewardedAd.Dispose();
            }
        };
        #endregion

        interstitialAd.LoadAd();
        rewardedAd.LoadAd();
    }

    public void ShowInterstitial()
    {
        interstitialAd.Show();
        isInterLoaded = false;
        SDKController.Message = "Facebook Inter Show";
        interstitialAd.LoadAd();
    }

    public void ShowRewarded()
    {
        rewardedAd.Show();
        isRewardLoaded = false;
        SDKController.Message = "Facebook reward Show";
        rewardedAd.LoadAd();
    }

    public bool IsInterLoaded()
    {
        return isInterLoaded;
    }

    public bool IsRewardLoaded()
    {
        return isRewardLoaded;
    }
}
