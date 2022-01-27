
using GoogleMobileAds.Api;
using System;

public class GoogleMobileAdsMng : Single<GoogleMobileAdsMng>
{
    private string adUnitId;
    private RewardedAd rewardedAd;

    private Action callBack;

    // Use this for initialization
    public override void Init()
    {
        Logger.Log("GoogleMobileAdsMng 初期化");
        //アプリ起動時に一度必ず実行（他のスクリプトで実行していたら不要）
        MobileAds.Initialize(initStatus => { });
    }
    private void RequestReward()
    {
#if UNITY_ANDROID
    adUnitId = "ca-app-pub-7823916958756862/4363280363";
#elif UNITY_IOS
    adUnitId = "ca-app-pub-7823916958756862/5564874911";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);
        //動画の視聴が完了したら「HandleUserEarnedReward」を呼ぶ
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }
    //動画の視聴が完了したら実行される（途中で閉じられた場合は呼ばれない）
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Logger.Log("報酬獲得！");

        if (callBack != null)
        {
            callBack();
        }

    }
    public void ShowReawrd(Action callBack)
    {
        this.callBack = callBack;

        //広告を表示
        RequestReward();

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
