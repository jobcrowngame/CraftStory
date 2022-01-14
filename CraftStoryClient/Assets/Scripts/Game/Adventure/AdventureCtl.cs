using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 冒険コンソル
/// </summary>
public class AdventureCtl : Single<AdventureCtl>
{
    // 一時的手にいれたボーナス
    private List<int> bonusList;
    public List<int> BonusList { get => bonusList; }

    // 手に入れた経験値
    int curExp = 0;
    public int CurExp { get => curExp; }

    int timer;

    private void UpdateBySeconds()
    {
        timer++;

        AddBuff();
    }

    public override void Init()
    {
        base.Init();

        bonusList = new List<int>();
        timer = 0;

        TimeZoneMng.E.AddTimerEvent03(UpdateBySeconds);
    }

    // ボーナス追加
    public void AddBonus(int id)
    {
        bonusList.Add(id);
        HomeLG.E.AddBonus(id);

        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
            DataMng.E.AddBonus(id);
    }

    // ボーナスリストクリア
    public void Clear()
    {
        bonusList.Clear();
        curExp = 0;

        TimeZoneMng.E.RemoveTimerEvent03(UpdateBySeconds);
    }

    /// <summary>
    /// 冒険をクリア
    /// </summary>
    /// <param name="callback"></param>
    public void GetBonus(Action callback)
    {
        // 手に入れたアイテムがない場合、通信しない
        if (BonusList.Count <= 0 || DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            callback();
            return;
        }

        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
        {
            GuideLG.E.Next();
        }

        var ui = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox);
        ui.AddBonus(BonusList);
        ui.SetCallBack(() =>
        {
            NWMng.E.GetItems(() =>
            {
                callback();
            });
        });
    }

    /// <summary>
    /// 経験値追加
    /// </summary>
    /// <param name="count"></param>
    public void AddExp(int count)
    {
        curExp += count;

        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            LocalDataMng.E.Data.UserDataT.exp += count;
            CheckLevelUp();
        }
    }

    private void CheckLevelUp()
    {
        var lvUpExp = ConfigMng.E.Character[LocalDataMng.E.Data.UserDataT.lv].LvUpExp;
        if (LocalDataMng.E.Data.UserDataT.exp >= lvUpExp)
        {
            LocalDataMng.E.Data.UserDataT.lv++;
            LocalDataMng.E.Data.UserDataT.exp -= lvUpExp;

            // add effect
            HomeLG.E.UI.OnLevelUp();

            // パラメータ更新
            WorldMng.E.CharacterCtl.getPlayer().Parameter.LevelUp(LocalDataMng.E.Data.UserDataT.lv);
        }
    }

    private void AddBuff()
    {
        if (DataMng.E.RuntimeData.MapType != MapType.Brave &&
            DataMng.E.RuntimeData.MapType != MapType.Event)
            return;

        if (timer % SettingMng.CreateAdventureBuffStep == 0)
        {
            AddAdventureBuff(1);
        }
    }

    /// <summary>
    /// 冒険BUFFを追加
    /// </summary>
    /// <param name="id"></param>
    public void AddAdventureBuff(int id)
    {
        if (DataMng.E.MapData == null)
            return;

        var config = ConfigMng.E.AdventureBuff[id];

        var pos = MapCtl.GetGroundPos(DataMng.E.MapData, -1, -1, 0);
        var cell = CommonFunction.Instantiate<AdventureBuffCell>(config.ResourcesPath, null, pos);
        cell.Set(config);
    }
}