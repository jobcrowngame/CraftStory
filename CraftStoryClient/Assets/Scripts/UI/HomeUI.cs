using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UIBase
{
    #region 変数

    /// <summary>
    /// Fadein のマスク
    /// </summary>
    Image FadeinImg { get => FindChiled<Image>("Fadein"); }

    /// <summary>
    /// メニューボタン
    /// </summary>
    Button MenuBtn { get => FindChiled<Button>("MenuBtn"); }

    /// <summary>
    /// 持ち物ボタン
    /// </summary>
    Button BagBtn { get => FindChiled<Button>("BagBtn"); }

    /// <summary>
    /// センタ用のアイテム欄
    /// </summary>
    Transform btnsParent { get => FindChiled("Grid"); }

    /// <summary>
    /// ビルダーペンセル
    /// </summary>
    Transform BuilderPencil { get => FindChiled("BuilderPencil"); }
    /// <summary>
    /// ビルダーボタン
    /// </summary>
    Button BuilderBtn { get => FindChiled<Button>("BuilderBtn", BuilderPencil); }
    /// <summary>
    /// キャンセルビルダーボタン
    /// </summary>
    Button BuilderPencilCancelBtn { get => FindChiled<Button>("BuilderPencilCancelBtn", BuilderPencil); }

    /// <summary>
    /// 設計図を使用する場合、コンソールWindow
    /// </summary>
    Transform Blueprint { get => FindChiled("Blueprint"); }
    /// <summary>
    /// 消耗するアイテムリストのサブ親
    /// </summary>
    Transform BlueprintCellGrid { get => FindChiled("Content", Blueprint.gameObject); }
    /// <summary>
    /// 回転ボタン
    /// </summary>
    Button SpinBtn { get => FindChiled<Button>("SpinBtn", Blueprint); }
    /// <summary>
    /// ビルダーキャンセルボタン
    /// </summary>
    Button BlueprintCancelBtn { get => FindChiled<Button>("BlueprintCancelBtn", Blueprint); }
    /// <summary>
    /// ビルダーボタン
    /// </summary>
    Button BuildBtn { get => FindChiled<Button>("BuildBtn", Blueprint); }

    /// <summary>
    /// ジャンプボタン
    /// </summary>
    Button Jump { get => FindChiled<Button>("Jump"); }
    /// <summary>
    /// 画面操作用　+ボタン
    /// </summary>
    MyButton PlussBtn { get => FindChiled<MyButton>("PlussBtn"); }
    /// <summary>
    /// 画面操作用　-ボタン
    /// </summary>
    MyButton MinusBtn { get => FindChiled<MyButton>("MinusBtn"); }

    /// <summary>
    /// びっくりマック
    /// </summary>
    Transform RedPoint { get => FindChiled("RedPoint"); }

    /// <summary>
    /// 選択用アイテム欄ボタンリスト
    /// </summary>
    List<HomeItemBtn> itemBtns;

    /// <summary>
    /// Fadein　時間幅
    /// </summary>
    private float fadeInTimeStep = 0.05f;

    #endregion

    private void Start()
    {
        // 今のマップタイプを設定
        DataMng.E.RuntimeData.MapType = MapType.Home;
        WorldMng.E.CreateGameObjects();
        WorldMng.E.GameTimeCtl.Active = true;

        UICtl.E.AddUI(this, UIType.Home);

        if (DataMng.E.RuntimeData.MapType == MapType.Home && NoticeLG.E.IsFirst)
        {
            UICtl.E.OpenUI<NoticeUI>(UIType.Notice);
            NoticeLG.E.IsFirst = false;
        }

        Init();

        RefreshItemBtns();
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
        BagBtn.onClick.AddListener(() => 
        { 
            UICtl.E.OpenUI<BagUI>(UIType.Bag); 
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

        NWMng.E.GetItems(null);
        NWMng.E.GetCoins((rp) =>
        {
            DataMng.GetCoins(rp);
        });

        StartCoroutine(FadeIn());
        RefreshRedPoint();
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
    public void AddBlueprintCostItems(BlueprintData blueprint)
    {
        ClearCell(BlueprintCellGrid);

        Dictionary<int, int> costs = new Dictionary<int, int>();
        foreach (var entity in blueprint.blocks)
        {
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
}
