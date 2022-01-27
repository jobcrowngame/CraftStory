﻿using System;
using UnityEngine;

using Random = UnityEngine.Random;

public class MobileAdsMng : SingleMono<MobileAdsMng>
{
    public MobileAdsType AdsType { get; set; }

    float timer;
    int deltaTime = 0;

    // レアアイテムプレゼント
    const int AddItemTimer = 300;

    DateTime beforKillAdsTime;

    int[] randomBonusArr = new int[] { 100,101,102,103,104,105,106,107,108 };

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer -= 1;

            Update1S();
        }
    }

    private void Update1S()
    {
        if (DataMng.E.RuntimeData.MapType != MapType.AreaMap)
            return;

        deltaTime++;

        // 5分ことでレアアイテムプレゼントを出す
        if (deltaTime % AddItemTimer == 0)
        {
            if (HomeLG.E.UI != null)
            {
                HomeLG.E.UI.ShowMobileAds(MobileAdsType.AddItem);
            }
        }
    }

    public void ShowMobileAds()
    {
        switch (AdsType)
        {
            case MobileAdsType.AddItem: ShowAdsAddItem(); break;
            case MobileAdsType.NotSleep: ShowAdsNotSleep(); break;
            case MobileAdsType.AddHunger: ShowAdsAddHunger(); break;
            case MobileAdsType.KillAll: ShowAdsKillAll(); break;
        }

        if (HomeLG.E.UI != null)
        {
            HomeLG.E.UI.CloseMobileAds();
        }
    }
    private void ShowAdsAddItem()
    {
        // 50パーセント
        if (Random.Range(0, 100) > 50)
            return;

        GoogleMobileAdsMng.E.ShowReawrd(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                int randomIndex = Random.Range(0, randomBonusArr.Length);
                AdventureCtl.E.AddBonus(randomBonusArr[randomIndex]);

                AudioMng.E.ShowBGM("bgm_02");
            }
        });
    }
    private void ShowAdsNotSleep()
    {
        if (DataMng.E.RuntimeData.MapType != MapType.AreaMap)
            return;

        // 50パーセント
        if (Random.Range(0, 100) > 50)
            return;

        GoogleMobileAdsMng.E.ShowReawrd(() =>
        {
            HomeLG.E.UI.FadeOutAndIn();

            AudioMng.E.ShowBGM("bgm_02");
        });
    }
    private void ShowAdsAddHunger()
    {
        if (DataMng.E.RuntimeData.MapType != MapType.AreaMap)
            return;

        // 50パーセント
        if (Random.Range(0, 100) > 50)
            return;

        GoogleMobileAdsMng.E.ShowReawrd(() =>
        {
            // 空腹度を回復
            HomeLG.E.UI.RecoveryHunger(100);

            AudioMng.E.ShowBGM("bgm_02");
        });
    }
    private void ShowAdsKillAll()
    {
        if (DataMng.E.RuntimeData.MapType != MapType.AreaMap)
            return;

        // ５分の間隔
        TimeSpan deltaTime = new TimeSpan(DateTime.Now.Ticks - beforKillAdsTime.Ticks);
        if (deltaTime.TotalSeconds < 300)
            return;

        // 20パーセント
        if (Random.Range(0, 100) > 20)
            return;

        GoogleMobileAdsMng.E.ShowReawrd(() =>
        {
            // 空腹度を回復
            CharacterCtl.E.KillAllMonster();

            beforKillAdsTime = DateTime.Now;

            AudioMng.E.ShowBGM("bgm_02");
        });
    }

    public enum MobileAdsType
    {
        /// <summary>
        /// レアアイテムプレゼント
        /// </summary>
        AddItem = 1,

        /// <summary>
        /// 寝なくても朝になる
        /// </summary>
        NotSleep = 2,

        /// <summary>
        /// 満腹度が回復する
        /// </summary>
        AddHunger = 3,

        /// <summary>
        /// 敵の殲滅
        /// </summary>
        KillAll = 4,
    }
}