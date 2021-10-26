using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class HomeUI : UIBase
{
    #region �ϐ�

    /// <summary>
    /// �����}�b�v��
    /// </summary>
    MyText SceneName { get => FindChiled<MyText>("SceneName"); }

    /// <summary>
    /// Fadein �̃}�X�N
    /// </summary>
    Image FadeinImg { get => FindChiled<Image>("Fadein"); }

    /// <summary>
    /// ���j���[�{�^��
    /// </summary>
    Button MenuBtn { get => FindChiled<Button>("MenuBtn"); }

    /// <summary>
    /// �}�b�v�{�^��
    /// </summary>
    Button MapBtn { get => FindChiled<Button>("MapBtn"); }

    /// <summary>
    /// �A�C�e����
    /// </summary>
    Transform Items { get => FindChiled("Items"); }

    /// <summary>
    /// �������{�^��
    /// </summary>
    Button BagBtn { get => FindChiled<Button>("BagBtn"); }

    /// <summary>
    /// �Z���^�p�̃A�C�e����
    /// </summary>
    Transform btnsParent { get => FindChiled("Grid"); }

    /// <summary>
    /// �r���_�[�y���Z��
    /// </summary>
    Transform BuilderPencil { get => FindChiled("BuilderPencil"); }
    /// <summary>
    /// �r���_�[�{�^��
    /// </summary>
    Button BuilderBtn { get => FindChiled<Button>("BuilderBtn", BuilderPencil); }
    /// <summary>
    /// �L�����Z���r���_�[�{�^��
    /// </summary>
    Button BuilderPencilCancelBtn { get => FindChiled<Button>("BuilderPencilCancelBtn", BuilderPencil); }

    /// <summary>
    /// ��ɓ���A�C�e���e
    /// </summary>
    Transform ItemDropParent { get => FindChiled("ItemDropParent"); }

    /// <summary>
    /// �݌v�}���g�p����ꍇ�A�R���\�[��Window
    /// </summary>
    Transform Blueprint { get => FindChiled("Blueprint"); }
    /// <summary>
    /// ���Ղ���A�C�e�����X�g�̃T�u�e
    /// </summary>
    Transform BlueprintCellGrid { get => FindChiled("Content", Blueprint.gameObject); }
    /// <summary>
    /// ��]�{�^��
    /// </summary>
    Button SpinBtn { get => FindChiled<Button>("SpinBtn", Blueprint); }
    /// <summary>
    /// �r���_�[�L�����Z���{�^��
    /// </summary>
    Button BlueprintCancelBtn { get => FindChiled<Button>("BlueprintCancelBtn", Blueprint); }
    /// <summary>
    /// �r���_�[�{�^��
    /// </summary>
    Button BuildBtn { get => FindChiled<Button>("BuildBtn", Blueprint); }

    /// <summary>
    /// �W�����v�{�^��
    /// </summary>
    Button Jump { get => FindChiled<Button>("Jump"); }
    /// <summary>
    /// ��ʑ���p�@+�{�^��
    /// </summary>
    MyButton PlussBtn { get => FindChiled<MyButton>("PlussBtn"); }
    /// <summary>
    /// ��ʑ���p�@-�{�^��
    /// </summary>
    MyButton MinusBtn { get => FindChiled<MyButton>("MinusBtn"); }

    /// <summary>
    /// �т�����}�b�N
    /// </summary>
    Transform RedPoint { get => FindChiled("RedPoint"); }

    /// <summary>
    /// �X�v���b�gAnimation
    /// </summary>
    Transform SpriteAnim { get => FindChiled("SpriteAnim"); }

    /// <summary>
    /// �R�C���o�[
    /// </summary>
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }

    /// <summary>
    /// �I��p�A�C�e�����{�^�����X�g
    /// </summary>
    List<HomeItemBtn> itemBtns;

    /// <summary>
    /// Fadein�@���ԕ�
    /// </summary>
    private float fadeInTimeStep = 0.05f;

    #endregion

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
        ShowSpriteAnimation();
        RefreshItemBtns();

        RefreshUiByMapType();
    }

    public override void Open()
    {
        base.Open();

        Title.RefreshCoins();
    }

    /// <summary>
    /// �R�C�����X�V
    /// </summary>
    public void RefreshCoins()
    {
        Title.RefreshCoins();
    }

    /// <summary>
    /// �}�b�v�ɂ����UI�X�V
    /// </summary>
    private void RefreshUiByMapType()
    {
        SceneName.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave);
        ItemDropParent.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave);
        Battle.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Brave);
        
        Items.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Home
            || DataMng.E.RuntimeData.MapType == MapType.Guide);

        Title.gameObject.SetActive(DataMng.E.RuntimeData.MapType == MapType.Market);
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
    /// �݌v�}���g���ɕK�v�ȃu���b�N���X�g
    /// </summary>
    /// <param name="blueprint">�݌v�}</param>
    public void AddBlueprintCostItems(BlueprintData blueprint)
    {
        ClearCell(BlueprintCellGrid);

        Dictionary<int, int> costs = new Dictionary<int, int>();
        foreach (var entity in blueprint.blocks)
        {
            // Obstacle�͖���
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
    /// �r���_�[�y���Z���R���\�[����\��
    /// </summary>
    /// <param name="b"></param>
    public void ShowBuilderPencilBtn(bool b = true)
    {
        if (BuilderPencil != null)
            BuilderPencil.gameObject.SetActive(b);
    }

    /// <summary>
    /// �݌v�}�g�p�ꍇ�̃R���\�[��Window��\��
    /// </summary>
    /// <param name="b"></param>
    public void ShowBlueprintBtn(bool b = true)
    {
        if (Blueprint != null)
            Blueprint.gameObject.SetActive(b);
    }

    /// <summary>
    /// �A�C�e���I�𗓂��X�V
    /// </summary>
    public void RefreshItemBtns()
    {
        foreach (var item in itemBtns)
        {
            item.Refresh();
        }
    }

    /// <summary>
    /// �A�C�e������ɓ��鉉�o
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
    /// �}�b�v�̂ҁ[������Anim
    /// </summary>
    public void ShowSpriteAnimation()
    {
        SpriteAnim.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide && DataMng.E.RuntimeData.GuideEnd2 == 0);
    }

    public Vector2 GetBagIconPos()
    {
        return BagBtn.transform.position;
    }

    IEnumerator FadeIn()
    {
        //�@Color�̃A���t�@��0.1�������Ă���
        for (var i = 1f; i > 0; i -= 0.1f)
        {
            FadeinImg.color = new Color(0f, 0f, 0f, i);
            //�@�w��b���҂�
            yield return new WaitForSeconds(fadeInTimeStep);
        }

        FadeinImg.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���b�N�I��
    /// </summary>
    /// <param name="target">�ڕW</param>
    public void LockUnTarget(Transform target)
    {
        PlayerCtl.E.CameraCtl.LockUnTarget(target);
    }
}
