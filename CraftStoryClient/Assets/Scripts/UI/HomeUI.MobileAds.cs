using UnityEngine;
using UnityEngine.UI;


public partial class HomeUI
{
    Transform MobileAds { get => FindChiled("MobileAds"); }
    Image MobileAdsIcon { get => FindChiled<Image>("Image", MobileAds); }
    Button MobileAdsBtn { get => FindChiled<Button>("Image", MobileAds); }
    Image TimerImage { get => FindChiled<Image>("Timer", MobileAds); }

    bool activeAds = false;
    float adsTimer = 0;
    const int adsEnactiveTimer = 30;

    private void InitMobileAds()
    {
        MobileAdsBtn.onClick.AddListener(() =>
        {
            MobileAdsMng.E.ShowMobileAds();
        });
    }

    public void UpdateMobileAds()
    {
        if (activeAds)
        {
            adsTimer += Time.deltaTime;

            TimerImage.fillAmount = adsTimer / adsEnactiveTimer;

            if (adsTimer > adsEnactiveTimer)
            {
                CloseMobileAds();
            }
        }
    }

    public void ShowMobileAds(MobileAdsMng.MobileAdsType adsType)
    {
        if (activeAds)
            return;

        Logger.Log("show ads " + adsType);

        activeAds = true;
        adsTimer = 0;
        MobileAdsMng.E.AdsType = adsType;

        MobileAds.gameObject.SetActive(true);

        string iconPath = "";
        switch (MobileAdsMng.E.AdsType)
        {
            case MobileAdsMng.MobileAdsType.AddItem: iconPath = ""; break;
            case MobileAdsMng.MobileAdsType.NotSleep: iconPath = ""; break;
            case MobileAdsMng.MobileAdsType.AddHunger: iconPath = ""; break;
            case MobileAdsMng.MobileAdsType.KillAll: iconPath = ""; break;
        }

        MobileAdsIcon.sprite = ReadResources<Sprite>(iconPath);
    }

    public void CloseMobileAds()
    {
        MobileAds.gameObject.SetActive(false);
        activeAds = false;
    }
}