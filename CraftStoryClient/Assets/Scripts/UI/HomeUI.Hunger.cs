﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public partial class HomeUI
{
    /// <summary>
    /// ジャンプボタン
    /// </summary>
    MyButton Jump { get => FindChiled<MyButton>("Jump"); }

    Slider HungerBar { get => FindChiled<Slider>("HungerBar"); }

    float timer;

    private int Hunger
    {
        get => DataMng.E.UserData.Hunger;
        set
        {
            if (value > SettingMng.MaxHunger)
                value = SettingMng.MaxHunger;

            if (value < 0)
                value = 0;

            DataMng.E.UserData.Hunger = value;

            IsHunger(value == 0);

            HungerBar.value = value;

            CheckJumpState();
        }
    }

    private void FixedUpdateHunger()
    {
        timer -= 0.02f;

        if (timer <= 0)
        {
            HungerChange(-1);
            timer = SettingMng.CostHumgerTimer;
        }
    }

    public void InitHunger()
    {
        PlayerCtl.E.Character.HpCtl.ShowHungerBar(false);
        PlayerCtl.E.Character.MoveSpeed = 1;

        Jump.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home);
        HungerBar.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home);

        Jump.onClick.AddListener(OnJump);

        Hunger = DataMng.E.UserData.Hunger;
        timer = SettingMng.CostHumgerTimer;

        CheckJumpState();
    }

    private void OnJump()
    {
        if (PlayerCtl.E.Character.Behavior == BehaviorType.Jump)
            return;

        HungerChange(-SettingMng.JumCostHumger);
        PlayerCtl.E.Jump();
    }

    private void CheckJumpState()
    {
        bool canJump = Hunger >= SettingMng.JumCostHumger;
        Jump.Enable(canJump);
    }

    private void IsHunger(bool b)
    {
        if (DataMng.E.RuntimeData.MapType == MapType.Home)
        {
            PlayerCtl.E.Character.HpCtl.ShowHungerBar(b);
            PlayerCtl.E.Character.MoveSpeed = b ? 0.5f : 1;
        }
    }

    /// <summary>
    /// 空腹度変化
    /// </summary>
    /// <param name="value"></param>
    private void HungerChange(int value)
    {
        Hunger += value;
    }

    /// <summary>
    /// 空腹度を回復
    /// </summary>
    /// <param name="value"></param>
    public void RecoveryHunger(int value)
    {
        Hunger += value;

        PlayerCtl.E.Character.Behavior = BehaviorType.EatFood;

        var effect = EffectMng.E.AddUIEffect<EffectBase>(transform, HungerBar.transform.position, EffectType.Gacha);
        effect.Init();
    }
}