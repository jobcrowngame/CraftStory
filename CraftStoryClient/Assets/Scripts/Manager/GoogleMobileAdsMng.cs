using GoogleMobileAds.Api;
using System;

public class GoogleMobileAdsMng : SingleMono<GoogleMobileAdsMng>
{
    private string adUnitId;
    private RewardedAd rewardedAd;

    private Action callBack;
    private bool HandleUserEarnedRewarded;

    // Use this for initialization
    public override void Init()
    {
        Logger.Log("GoogleMobileAdsMng 初期化");
        //アプリ起動時に一度必ず実行（他のスクリプトで実行していたら不要）
        MobileAds.Initialize(initStatus => { });

#if UNITY_ANDROID
        adUnitId = "ca-app-pub-7823916958756862/4363280363";
#elif UNITY_IOS
        adUnitId = "ca-app-pub-7823916958756862/5564874911";
#endif
        rewardedAd = new RewardedAd(adUnitId);


        rewardedAd.OnAdLoaded += OnAdLoaded;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }

    private void Update()
    {
        OnHandleUserEarnedReward();
    }

    /// <summary>
    /// 広告がロード出来た場合
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnAdLoaded(object sender, EventArgs args)
    {
        Logger.Log("OnAdLoaded");
    }

    /// <summary>
    /// 広告成功場合
    /// </summary>
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Logger.Log("報酬獲得！");

        HandleUserEarnedRewarded = true;
    }
    private void OnHandleUserEarnedReward()
    {
        if (HandleUserEarnedRewarded)
        {
            HandleUserEarnedRewarded = false;

            AdRequest request = new AdRequest.Builder().Build();
            rewardedAd.LoadAd(request);

            if (callBack != null)
                callBack();
        }
    }

    /// <summary>
    /// 広告を出す
    /// </summary>
    /// <param name="callBack"></param>
    public void ShowReawrd(Action callBack)
    {
        this.callBack = callBack;

        if (this.rewardedAd.IsLoaded())
        {
            AudioMng.E.StopBGM();
            this.rewardedAd.Show();
        }
        else
        {
            Logger.Warning("広告準備されてない！！！");
        }
    }
}
