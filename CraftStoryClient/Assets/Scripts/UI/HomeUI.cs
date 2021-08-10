using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UIBase
{
    Image FadeinImg;

    Button MenuBtn;
    Button BagBtn;

    Transform btnsParent;

    Transform BuilderPencil;
    Button BuilderBtn;
    Button BuilderPencilCancelBtn;

    Transform Blueprint;
    Transform BlueprintCellGrid;
    Button SpinBtn;
    Button BlueprintCancelBtn;
    Button BuildBtn;

    Button Jump { get => FindChiled<Button>("Jump"); }
    MyButton PlussBtn { get => FindChiled<MyButton>("PlussBtn"); }
    MyButton MinusBtn { get => FindChiled<MyButton>("MinusBtn"); }

    Transform RedPoint { get => FindChiled("RedPoint"); }

    List<HomeItemBtn> itemBtns;

    private float fadeInTime = 0.05f;

    private void Start()
    {
        DataMng.E.RuntimeData.MapType = MapType.Home;
        WorldMng.E.CreateGameObjects();
        WorldMng.E.GameTimeCtl.Active = true;

        UICtl.E.AddUI(this, UIType.Home);

        if (NoticeLG.E.IsFirst)
        {
            UICtl.E.OpenUI<NoticeUI>(UIType.Notice);
            NoticeLG.E.IsFirst = false;
        }

        if (PlayDescriptionLG.E.IsFirst)
        {
            UICtl.E.OpenUI<PlayDescriptionUI>(UIType.PlayDescription);
            PlayDescriptionLG.E.IsFirst = false;
        }

        Init();

        RefreshItemBtns();
    }

    public override void Init()
    {
        base.Init();
        HomeLG.E.Init(this);

        FadeinImg = FindChiled<Image>("Fadein");
        FadeinImg.enabled = true;

        MenuBtn = FindChiled<Button>("MenuBtn");
        MenuBtn.onClick.AddListener(() => 
        {
            var menu = UICtl.E.OpenUI<MenuUI>(UIType.Menu); 
            menu.Init();
        });

        BagBtn = FindChiled<Button>("BagBtn");
        BagBtn.onClick.AddListener(() => { UICtl.E.OpenUI<BagUI>(UIType.Bag); });

        btnsParent = FindChiled("Grid");
        AddItemBtns();

        BuilderPencil = FindChiled("BuilderPencil");
        BuilderBtn = FindChiled<Button>("BuilderBtn", BuilderPencil);
        BuilderBtn.onClick.AddListener(CreateBlueprint);
        BuilderPencilCancelBtn = FindChiled<Button>("BuilderPencilCancelBtn", BuilderPencil);
        BuilderPencilCancelBtn.onClick.AddListener(CancelBuilderPencilCancelBtn);

        Blueprint = FindChiled("Blueprint");
        BlueprintCellGrid = FindChiled("Content", Blueprint.gameObject);
        SpinBtn = FindChiled<Button>("SpinBtn", Blueprint);
        SpinBtn.onClick.AddListener(SpinBlueprint);
        BlueprintCancelBtn = FindChiled<Button>("BlueprintCancelBtn", Blueprint);
        BlueprintCancelBtn.onClick.AddListener(CancelUserBlueprint);
        BuildBtn = FindChiled<Button>("BuildBtn", Blueprint);
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

    public void ShowBuilderPencilBtn(bool b = true)
    {
        if (BuilderPencil != null)
            BuilderPencil.gameObject.SetActive(b);
    }
    public void ShowBlueprintBtn(bool b = true)
    {
        if (Blueprint != null)
            Blueprint.gameObject.SetActive(b);
    }

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
            yield return new WaitForSeconds(fadeInTime);
        }

        FadeinImg.gameObject.SetActive(false);
    }
}
