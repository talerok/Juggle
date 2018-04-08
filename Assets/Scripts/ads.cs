using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class Ads : Interfaces.IAd {

    private InterstitialAd interstitial;
    // Use this for initialization
    public Ads(string appId, string adId) {
        MobileAds.Initialize(appId);
        interstitial = new InterstitialAd(adId);
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    public void Show()
    {
        //if (interstitial.IsLoaded()) interstitial.Show();
    }

    ~Ads()
    {
        interstitial.Destroy();
    }

}
