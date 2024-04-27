using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class AdBanner : MonoBehaviour
{
    BannerView bannerView;

    void Awake()
    {

#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-1352849215432606/5968971307";
#else
        string adUnitId = "unexpected_platform";
#endif

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
    }

    void Start()
    {
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

}
