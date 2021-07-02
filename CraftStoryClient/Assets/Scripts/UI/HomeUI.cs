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

    MyButton PlussBtn { get => FindChiled<MyButton>("PlussBtn"); }
    MyButton MinusBtn { get => FindChiled<MyButton>("MinusBtn"); }

    List<HomeItemBtn> itemBtns;

    private float fadeInTime = 0.05f;

    private void Start()
    {
        WorldMng.E.CreateGameObjects();
        UICtl.E.AddUI(this, UIType.Home);

        if (PlayDescriptionLG.E.IsFirst)
        {
            UICtl.E.OpenUI<PlayDescriptionUI>(UIType.PlayDescription);
        }

        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            PlayerCtl.E.CameraCtl.ChangeCameraPos(1);
        }
        if (Input.GetKey(KeyCode.F6))
        {
            PlayerCtl.E.CameraCtl.ChangeCameraPos(-1);
        }
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
            menu.Init(MenuUI.MenuUIType.Home);
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

        PlayerCtl.E.Joystick = FindChiled<SimpleInputNamespace.Joystick>("Joystick");
        PlayerCtl.E.ScreenDraggingCtl = FindChiled<ScreenDraggingCtl>("ScreenDraggingCtl");
        PlayerCtl.E.CameraCtl = Camera.main.GetComponent<CameraCtl>();

        NWMng.E.GetItemList((rp) =>
        {
            DataMng.GetItems(rp[0]);
        });
        NWMng.E.GetCoins((rp) =>
        {
            DataMng.GetCoins(rp[0]);
        });

        StartCoroutine(FadeIn());
    }

    private void AddItemBtns()
    {
        itemBtns = new List<HomeItemBtn>();

        for (int i = 0; i < 6; i++)
        {
            var cell = AddCell<HomeItemBtn>("Prefabs/UI/ItemBtn", btnsParent);
            if (cell == null)
                return;

            cell.Index = i;
            itemBtns.Add(cell);
        }
    }
    public void AddBlueprintCostItems(List<MapBlockData> blocks)
    {
        ClearCell(BlueprintCellGrid);

        Dictionary<int, int> costs = new Dictionary<int, int>();
        foreach (var item in blocks)
        {
            if (costs.ContainsKey(item.ItemID))
            {
                costs[item.ItemID]++;
            }
            else
            {
                costs[item.ItemID] = 1;
            }
        }

        foreach (var key in costs.Keys)
        {
            var cell = AddCell<BlueprintCell>("Prefabs/UI/BlueprintCell", BlueprintCellGrid);
            if (cell == null)
                return;

            cell.Init(key, costs[key]);
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
