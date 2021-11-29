using System;
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
    /// 持ち物ボタン
    /// </summary>
    Button BagBtn;

    /// <summary>
    /// センタ用のアイテム欄
    /// </summary>
    Transform btnsParent;

    /// <summary>
    /// ビルダーペンセル
    /// </summary>
    Transform BuilderPencil;
    /// </summary>
    Button BuilderBtn;
    /// <summary>
    /// キャンセルビルダーボタン
    /// </summary>
    Button BuilderPencilCancelBtn;

    /// <summary>
    /// 手に入るアイテム親
    /// </summary>
    Transform ItemDropParent;

    /// <summary>
    /// 設計図を使用する場合、コンソールWindow
    /// </summary>
    Transform Blueprint;
    /// <summary>
    /// 消耗するアイテムリストのサブ親
    /// </summary>
    Transform BlueprintCellGrid;
    /// <summary>
    /// 回転ボタン
    /// </summary>
    Button SpinBtn;
    /// <summary>
    /// ビルダーキャンセルボタン
    /// </summary>
    Button BlueprintCancelBtn;
    /// <summary>
    /// ビルダーボタン
    /// </summary>
    Button BuildBtn;

    /// <summary>
    /// ジャンプボタン
    /// </summary>
    Button Jump;
    /// <summary>
    /// 画面操作用　+ボタン
    /// </summary>
    MyButton PlussBtn;
    /// <summary>
    /// 画面操作用　-ボタン
    /// </summary>
    MyButton MinusBtn;

    /// <summary>
    /// びっくりマック
    /// </summary>
    Transform RedPoint;

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
    /// Fadein　時間幅
    /// </summary>
    private float fadeInTimeStep = 0.05f;

    #endregion

    private void Awake()
    {
        SceneName = FindChiled<MyText>("SceneName");
        FadeinImg = FindChiled<Image>("Fadein");
        MenuBtn = FindChiled<Button>("MenuBtn");
        MapBtn = FindChiled<Button>("MapBtn");
        BagBtn = FindChiled<Button>("BagBtn");
        btnsParent = FindChiled("Grid");
        BuilderPencil = FindChiled("BuilderPencil");
        BuilderBtn = FindChiled<Button>("BuilderBtn", BuilderPencil);
        BuilderPencilCancelBtn = FindChiled<Button>("BuilderPencilCancelBtn", BuilderPencil);
        ItemDropParent = FindChiled("ItemDropParent");
        Blueprint = FindChiled("Blueprint");
        BlueprintCellGrid = FindChiled("Content", Blueprint.gameObject);
        SpinBtn = FindChiled<Button>("SpinBtn", Blueprint);
        BlueprintCancelBtn = FindChiled<Button>("BlueprintCancelBtn", Blueprint);
        BuildBtn = FindChiled<Button>("BuildBtn", Blueprint);
        Jump = FindChiled<Button>("Jump");
        PlussBtn = FindChiled<MyButton>("PlussBtn");
        MinusBtn = FindChiled<MyButton>("MinusBtn");
        RedPoint = FindChiled("RedPoint");
        SpriteAnim = FindChiled("SpriteAnim");
        Title = FindChiled<Title2UI>("Title2");
        DebugBtn = FindChiled<Button>("DebugBtn");
        Clock = FindChiled<Image>("Clock");
    }

    public override void Init()
    {
        base.Init();
        HomeLG.E.Init(this);

        FadeinImg.enabled = true;

        MenuBtn.onClick.AddListener(() => 
        {
            var menu = UICtl.E.OpenUI<MenuUI>(UIType.Menu); 
            menu.Init();

            GuideLG.E.Next();
        });
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

        BuilderBtn.onClick.AddListener(CreateBlueprint);
        BuilderPencilCancelBtn.onClick.AddListener(CancelBuilderPencilCancelBtn);
        SpinBtn.onClick.AddListener(SpinBlueprint);
        BlueprintCancelBtn.onClick.AddListener(CancelUserBlueprint);
        BuildBtn.onClick.AddListener(BuildBlueprint);
        PlussBtn.AddClickingListener(() => { PlayerCtl.E.CameraCtl.ChangeCameraPos(1); });
        MinusBtn.AddClickingListener(() => { PlayerCtl.E.CameraCtl.ChangeCameraPos(-1); });
        Jump.onClick.AddListener(PlayerCtl.E.Jump);

        PlayerCtl.E.Joystick = FindChiled<SimpleInputNamespace.Joystick>("Joystick");
        PlayerCtl.E.ScreenDraggingCtl = FindChiled<ScreenDraggingCtl>("ScreenDraggingCtl");
        PlayerCtl.E.CameraCtl = Camera.main.GetComponent<CameraCtl>();

        SceneName.text = DataMng.E.MapData.Config.Name;

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
        RefreshRedPoint();
        RefreshItemBtns();

        RefreshUiByMapType();

        SpriteAnim.gameObject.SetActive(false);
        ShowMonsterNumberLeft();
    }

    public override void Open()
    {
        base.Open();

        NWMng.E.GetCoins((rp) =>
        {
            DataMng.GetCoins(rp);
            RefreshCoins();
        });

        SetSkills();

        // 10階まで行くタスク
        if (DataMng.E.MapData.Config.Floor >= 10)
            TaskMng.E.AddMainTaskCount(9);
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
        MenuBtn.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Brave);

        SceneName.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave);
        ItemDropParent.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave
            || DataMng.E.RuntimeData.MapType == MapType.Guide);
        Battle.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave
             || DataMng.E.RuntimeData.MapType == MapType.Test);
        
        Items.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home
            || DataMng.E.RuntimeData.MapType == MapType.Guide);

        Title.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Market);

        Clock.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home);
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
    /// 設計図を使うに必要なブロックリスト
    /// </summary>
    /// <param name="blueprint">設計図</param>
    public void AddBlueprintCostItems(BlueprintData blueprint)
    {
        ClearCell(BlueprintCellGrid);

        Dictionary<int, int> costs = new Dictionary<int, int>();
        foreach (var entity in blueprint.blocks)
        {
            // Obstacleは無視
            if ((EntityType)ConfigMng.E.Entity[entity.id].Type == EntityType.Obstacle)
                continue;

            if (costs.ContainsKey(entity.id))
            {
                costs[entity.id]++;
            }
            else
            {
                costs[entity.id] = 1;
            }
        }

        foreach (var key in costs.Keys)
        {
            var cell = AddCell<BlueprintCell>("Prefabs/UI/BlueprintCell", BlueprintCellGrid);
            if (cell == null)
                return;

            cell.Init(ConfigMng.E.Entity[key].ItemID, costs[key]);
        }
    }

    private void CreateBlueprint()
    {
        Logger.Log("BuilderBtn");

        PlayerCtl.E.BuilderPencil.CreateBlueprint();
    }
    private void CancelBuilderPencilCancelBtn()
    {
        Logger.Log("CancelBtn");

        PlayerCtl.E.BuilderPencil.CancelCreateBlueprint();
    }
    private void SpinBlueprint()
    {
        PlayerCtl.E.BuilderPencil.SpinBlueprint();
    }
    private void CancelUserBlueprint()
    {
        PlayerCtl.E.BuilderPencil.CancelUserBlueprint();
    }
    private void BuildBlueprint()
    {
        PlayerCtl.E.BuilderPencil.BuildBlueprint();
    }
    public void RefreshRedPoint()
    {
        RedPoint.gameObject.SetActive(CommonFunction.MenuRedPoint());
    }

    /// <summary>
    /// ビルダーペンセルコンソールを表し
    /// </summary>
    /// <param name="b"></param>
    public void ShowBuilderPencilBtn(bool b = true)
    {
        if (BuilderPencil != null)
            BuilderPencil.gameObject.SetActive(b);
    }

    /// <summary>
    /// 設計図使用場合のコンソールWindowを表し
    /// </summary>
    /// <param name="b"></param>
    public void ShowBlueprintBtn(bool b = true)
    {
        if (Blueprint != null)
            Blueprint.gameObject.SetActive(b);
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
    }

    /// <summary>
    /// マップのぴーちゃんのAnim
    /// </summary>
    public void ShowSpriteAnimation()
    {
        try
        {
            if (SpriteAnim != null)
            {
                SpriteAnim.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide &&
                (DataMng.E.RuntimeData.GuideEnd2 == 0 || MapLG.E.IsEquipTutorial() || DataMng.E.RuntimeData.GuideEnd5 == 0));
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message);
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

    /// <summary>
    /// ロックオン
    /// </summary>
    /// <param name="target">目標</param>
    public void LockUnTarget(Transform target)
    {
        PlayerCtl.E.CameraCtl.LockUnTarget(target);
    }
}
