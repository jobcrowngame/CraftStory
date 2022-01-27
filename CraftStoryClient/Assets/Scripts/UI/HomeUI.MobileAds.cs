using System;
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

            TimerImage.fillAmount = (float)(adsEnactiveTimer - adsTimer) / adsEnactiveTimer;

            if (adsTimer > adsEnactiveTimer)
            {
                CloseMobileAds();
            }
        }
    }

    public void ShowMobileAds(MobileAdsMng.MobileAdsType adsType)
    {
        if (DataMng.E.RuntimeData.MapType != MapType.AreaMap)
            return;

        if (activeAds)
            return;

        if (!CheckShowByPercent(adsType))
            return;

        Logger.Log("show ads " + adsType);

        activeAds = true;
        adsTimer = 0;
        MobileAdsMng.E.AdsType = adsType;

        MobileAds.gameObject.SetActive(true);

        string iconPath = "";
        switch (MobileAdsMng.E.AdsType)
        {
            case MobileAdsMng.MobileAdsType.AddItem: iconPath = "Textures/button_2d_055"; break;
            case MobileAdsMng.MobileAdsType.NotSleep: iconPath = "Textures/button_2d_056"; break;
            case MobileAdsMng.MobileAdsType.AddHunger: iconPath = "Textures/button_2d_057"; break;
            case MobileAdsMng.MobileAdsType.KillAll: iconPath = "Textures/button_2d_058"; break;
        }

        MobileAdsIcon.sprite = ReadResources<Sprite>(iconPath);
    }
    private bool CheckShowByPercent(MobileAdsMng.MobileAdsType adsType)
    {
        int percent = 0;
        switch (adsType)
        {
            case MobileAdsMng.MobileAdsType.AddItem:
            case MobileAdsMng.MobileAdsType.NotSleep:
            case MobileAdsMng.MobileAdsType.AddHunger: 
                percent = 50; break;

            case MobileAdsMng.MobileAdsType.KillAll: 
                percent = 20;

                // ５分の間隔
                TimeSpan deltaTime = new TimeSpan(DateTime.Now.Ticks - MobileAdsMng.E.BeforKillAdsTime.Ticks);
                if (deltaTime.TotalSeconds < 300)
                    percent = -1;
                break;
        }

        return UnityEngine.Random.Range(0, 100) <= percent;
    }

    public void CloseMobileAds()
    {
        MobileAds.gameObject.SetActive(false);
        activeAds = false;
    }
}