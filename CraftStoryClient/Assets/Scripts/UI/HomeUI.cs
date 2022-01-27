﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public partial class HomeUI : UIBase
{
    #region 変数

    /// <summary>
    /// 入たマップ名
    /// </summary>
    MyText SceneName;

    /// <summary>
    /// Fadein のマスク
    /// </summary>
    Image FadeinImg;

    /// <summary>
    /// メニューボタン
    /// </summary>
    Button MenuBtn;

    /// <summary>
    /// マップボタン
    /// </summary>
    Button MapBtn;

    /// <summary>
    /// アイテム欄
    /// </summary>
    public Transform Items { get => FindChiled("Items"); }

    /// <summary>
    /// アイテム欄
    /// </summary>
    Transform ItemsGridMask { get => Items != null ? FindChiled("GridMask", Items.gameObject) : null; }

    /// <summary>
    /// 持ち物ボタン
    /// </summary>
    Button BagBtn;

    /// <summary>
    /// センタ用のアイテム欄
    /// </summary>
    Transform btnsParent;

    /// <summary>
    /// 手に入るアイテム親
    /// </summary>
    Transform ItemDropParent;

    /// <summary>
    /// 画面操作用　+ボタン
    /// </summary>
    MyButton PlussBtn;
    /// <summary>
    /// 画面操作用　-ボタン
    /// </summary>
    MyButton MinusBtn;

    /// <summary>
    /// スプリットAnimation
    /// </summary>
    Transform SpriteAnim;

    /// <summary>
    /// コインバー
    /// </summary>
    Title2UI Title;

    /// <summary>
    /// Debug ボタン
    /// </summary>
    Button DebugBtn;

    /// <summary>
    /// 選択用アイテム欄ボタンリスト
    /// </summary>
    List<HomeItemBtn> itemBtns;

    /// <summary>
    /// 時計
    /// </summary>
    Image Clock;

    /// <summary>
    /// タスク概要
    /// </summary>
    MyText TaskOverview;

    Text Memory1 {  get => FindChiled<Text>("Memory1"); }
    Text Memory2 {  get => FindChiled<Text>("Memory2"); }
    Text Memory3 {  get => FindChiled<Text>("Memory3"); }

    /// <summary>
    /// タスク概要テキスト(表示制御用)
    /// </summary>
    Transform TaskOverviewText;

    /// <summary>
    /// Fadein　時間幅
    /// </summary>
    private float fadeInTimeStep = 0.05f;

    #endregion

    private void Awake()
    {
        SceneName = FindChiled<MyText>("SceneName");
        FadeinImg = FindChiled<Image>("Fadein");
        MapBtn = FindChiled<Button>("MapBtn");
        BagBtn = FindChiled<Button>("BagBtn");
        btnsParent = FindChiled("Grid");
        ItemDropParent = FindChiled("ItemDropParent");
        PlussBtn = FindChiled<MyButton>("PlussBtn");
        MinusBtn = FindChiled<MyButton>("MinusBtn");
        SpriteAnim = FindChiled("SpriteAnim");
        Title = FindChiled<Title2UI>("Title2");
        DebugBtn = FindChiled<Button>("DebugBtn");
        Clock = FindChiled<Image>("Clock");
        TaskOverview = FindChiled<MyText>("TaskOverview");
        TaskOverviewText = FindChiled<Transform>("Text", TaskOverview.transform);
    }

    private void Update()
    {
        UpdateHunger();
        //RefreshMemoryInfo();
        UpdateMobileAds();
    }

    public override void Init()
    {
        base.Init();
        HomeLG.E.Init(this);

        InitBlueprint();
        InitHunger();
        InitMobileAds();

        FadeinImg.enabled = true;

        MapBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<MapUI>(UIType.Map);
            GuideLG.E.Next();
        });
        BagBtn.onClick.AddListener(() => 
        { 
            UICtl.E.OpenUI<BagUI>(UIType.Bag); 
        });
        DebugBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<DebugUI>(UIType.Debug);
        });

        AddItemBtns();
       
        PlussBtn.AddClickingListener(() => { PlayerCtl.E.CameraCtl.ChangeCameraPos(1); });
        MinusBtn.AddClickingListener(() => { PlayerCtl.E.CameraCtl.ChangeCameraPos(-1); });

        PlayerCtl.E.Joystick = FindChiled<SimpleInputNamespace.Joystick>("Joystick");
        PlayerCtl.E.ScreenDraggingCtl = FindChiled<ScreenDraggingCtl>("ScreenDraggingCtl");
        PlayerCtl.E.CameraCtl = Camera.main.GetComponent<CameraCtl>();

        if(DataMng.E.MapData != null) SceneName.text = DataMng.E.MapData.Config.Name;

        skills = new SkillCell[Battle.GetChild(0).childCount];
        for (int i = 0; i < Battle.GetChild(0).childCount; i++)
        {
            skills[i] = Battle.GetChild(0).GetChild(i).GetComponent<SkillCell>();
        }

        Title.Init();
        Title.ShowCoin(1);
        Title.ShowCoin(2);
        Title.ShowCoin(3);

        StartCoroutine(FadeIn());
        RefreshItemBtns();

        RefreshUiByMapType();

        SpriteAnim.gameObject.SetActive(false);
        ShowMonsterNumberLeft();
    }

    public override void Open()
    {
        base.Open();

        RefreshCoins();
        SetSkills();

        // 10階まで行くタスク
        if (DataMng.E.MapData != null && DataMng.E.MapData.Config.Floor >= 10)
            TaskMng.E.AddMainTaskCount(9);

        TaskOverview.text = "";
        Image toImage = TaskOverview.GetComponent<Image>();
        toImage.color = new Color(0f, 0f, 0f, 1 / 256f);
        PlayerCtl.E.Pause(false);

        LevelUp.gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        HomeLG.E.ClearItemStack();
    }

    public void ClickSkill(int index)
    {
        skills[index].OnClickSkillBtn();
    }

    /// <summary>
    /// コインを更新
    /// </summary>
    public void RefreshCoins()
    {
        Title.RefreshCoins();
    }

    /// <summary>
    /// マップによってUI更新
    /// </summary>
    private void RefreshUiByMapType()
    {
        SceneName.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave 
            || DataMng.E.RuntimeData.MapType == MapType.Event);
        Battle.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave
            || DataMng.E.RuntimeData.MapType == MapType.AreaMap
            || DataMng.E.RuntimeData.MapType == MapType.Event
            || DataMng.E.RuntimeData.MapType == MapType.Test);
        
        Items.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home
             || DataMng.E.RuntimeData.MapType == MapType.AreaMap
            || DataMng.E.RuntimeData.MapType == MapType.Guide
            || DataMng.E.RuntimeData.MapType == MapType.Market);

        MonsterNumberLeft.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave
            || DataMng.E.RuntimeData.MapType == MapType.Event
            || DataMng.E.RuntimeData.MapType == MapType.Test);

        Jump.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home
             || DataMng.E.RuntimeData.MapType == MapType.AreaMap);

        HungerBar.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home
             || DataMng.E.RuntimeData.MapType == MapType.AreaMap);

        if (Items.gameObject.activeSelf)
        {
            ItemsGridMask.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Market);
        }

        Title.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Market);

        Clock.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home
             || DataMng.E.RuntimeData.MapType == MapType.AreaMap);

        TaskOverview.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home);
        TaskOverviewText.gameObject.SetActive(false);
    }

    private void AddItemBtns()
    {
        itemBtns = new List<HomeItemBtn>();

        for (int i = 0; i < 6; i++)
        {
            var cell = AddCell<HomeItemBtn>("Prefabs/UI/ItemBtn", btnsParent);
            if (cell == null)
                return;

            cell.name = i.ToString();
            cell.Index = i;
            itemBtns.Add(cell);
        }
    }

    /// <summary>
    /// アイテム選択欄を更新
    /// </summary>
    public void RefreshItemBtns()
    {
        foreach (var item in itemBtns)
        {
            item.Refresh();
        }
    }

    /// <summary>
    /// アイテムを手に入る演出
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(HomeLG.BraveCellItem item)
    {
        var cell = AddCell<HomeGetItemCell>("Prefabs/UI/BraveCell", ItemDropParent);
        if (cell != null)
        {
            cell.Set(item);
        }

        // ポイントゲットの場合、表現を追加
        if (item.itemId == 9002)
        {
            ShowPointGet();
        }
    }

    public Vector2 GetBagIconPos()
    {
        return BagBtn.transform.position;
    }

    IEnumerator FadeIn()
    {
        //　Colorのアルファを0.1ずつ下げていく
        for (var i = 1f; i > 0; i -= 0.1f)
        {
            FadeinImg.color = new Color(0f, 0f, 0f, i);
            //　指定秒数待つ
            yield return new WaitForSeconds(fadeInTimeStep);
        }

        FadeinImg.gameObject.SetActive(false);
    }

    public void FadeOutAndIn()
    {
        StartCoroutine(IFadeOutAndIn());
    }
    IEnumerator IFadeOutAndIn()
    {
        TimeZoneMng.E.Stop();

        FadeinImg.color = new Color(0f, 0f, 0f, 0);
        FadeinImg.gameObject.SetActive(true);

        //　Colorのアルファを0.1ずつ上げていく
        for (var i = 0f; i < 1; i += 0.1f)
        {
            //if (i > 1f) i = 1f;
            FadeinImg.color = new Color(0f, 0f, 0f, i);
            //　指定秒数待つ
            yield return new WaitForSeconds(fadeInTimeStep);
        }

        WorldMng.E.GameTimeCtl.ResetTime();

        //　Colorのアルファを0.1ずつ下げていく
        for (var i = 1f; i > 0; i -= 0.1f)
        {
            FadeinImg.color = new Color(0f, 0f, 0f, i);
            //　指定秒数待つ
            yield return new WaitForSeconds(fadeInTimeStep);
        }

        FadeinImg.gameObject.SetActive(false);

        TimeZoneMng.E.Resume();
    }

    /// <summary>
    /// タスク概要アクティブ
    /// </summary>
    public void ActivateTaskOverview()
    {
        TaskOverviewText.gameObject.SetActive(true);
    }

    /// <summary>
    /// タスク概要更新
    /// </summary>
    public void RefreshTaskOverview()
    {
        Image toImage = TaskOverview.GetComponent<Image>();

        if (!TaskOverviewText.gameObject.activeSelf)
        {
            toImage.color = new Color(0f, 0f, 0f, 1 / 256f);
            return;
        }

        GameObject ChatFlg = GameObject.Find("effect_2d_010");

        if (ChatFlg != null && ChatFlg.activeSelf)
        {
            TaskOverview.text = "フィーが何か伝えたいみたいだよ！";
        }
        else
        {
            int chatId = ConfigMng.E.MainTask[TaskMng.E.MainTaskConfig.ID].StartChat;
            if (chatId == -1)
            {
                TaskOverview.text = "";
            }
            else
            {
                string overview = ConfigMng.E.Chat[chatId].Overview;
                TaskOverview.text = overview != "N" ? overview : "";
            }
        }

        if (TaskOverview.text == "")
        {
            toImage.color = new Color(0f, 0f, 0f, 1 / 256f);
        }
        else
        {
            toImage.color = new Color(0f, 0f, 0f, 100 / 256f);
        }
    }

    public void RefreshMemoryInfo()
    {
        if (PlayerCtl.E.Character != null)
        {
            Memory1.text = "プールとして用意してくれているメモリー使用量:" + (float)Profiler.GetTotalReservedMemoryLong() / (1024 * 1024 * 1024) + "GB";
            Memory2.text = "現在使用しているメモリー使用量:" + (float)Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024 * 1024) + "GB";
            Memory3.text = "MonoUsedSize：" + (float)Profiler.GetMonoUsedSizeLong() / (1024 * 1024 * 1024) + "GB";
        }
    }
}
