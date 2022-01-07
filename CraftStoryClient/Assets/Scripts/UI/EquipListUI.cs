using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipListUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    MyToggleGroupCtl ToggleBtns { get => FindChiled<MyToggleGroupCtl>("ToggleBtns"); }
    ScrollRect ScrollView { get => FindChiled<ScrollRect>("Scroll View"); }
    Transform Parent { get => FindChiled("Parent"); }

    Transform Default { get => FindChiled("Default"); }
    Transform Parameter { get => FindChiled("ParameterPlan"); }

    /// <summary>
    /// 装備
    /// </summary>
    MyButton EquipBtn { get => FindChiled<MyButton>("EquipBtn"); }

    /// <summary>
    /// 鑑定
    /// </summary>
    Button AppraisalBtn { get => FindChiled<Button>("AppraisalBtn"); }

    /// <summary>
    /// ソート
    /// </summary>
    MyButton SortBtn { get => FindChiled<MyButton>("SortBtn"); }

    Text HP { get => FindChiled<Text>("HP"); }
    Text Damage { get => FindChiled<Text>("Damage"); }
    Text Defense { get => FindChiled<Text>("Defense"); }

    public override void Init(object data)
    {
        base.Init(data);

        EquipListLG.E.Init(this);

        Title.SetTitle("装備一覧");
        Title.SetOnClose(Close);

        ToggleBtns.Init();
        ToggleBtns.OnValueChangeAddListener((index) =>
        {
            EquipListLG.E.Tag = (EquipListLG.TagType)index;
        });

        EquipBtn.onClick.AddListener(() =>
        {
            PlayerCtl.E.EquipEquipment(EquipListLG.E.SelectedItem.Data);
            Close();
            if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            {
                GuideLG.E.Next();
            }
        });
        AppraisalBtn.onClick.AddListener(() =>
        {
            EquipListLG.E.AppraisalEquipment(EquipListLG.E.SelectedItem);
            RefreshParameter();
        });
        SortBtn.onClick.AddListener(() =>
        {
            EquipListLG.E.SortUp = !EquipListLG.E.SortUp;
            SortBtn.SetIcon(EquipListLG.E.SortUp ? "Textures/menu_2d_023" : "Textures/menu_2d_022");
        });
    }

    public override void Open(object data)
    {
        base.Open(data);

        EquipListLG.E.itemType = (ItemType)data;

        SortBtn.SetIcon(EquipListLG.E.SortUp ? "Textures/menu_2d_023" : "Textures/menu_2d_022");
        ToggleBtns.SetValue((int)EquipListLG.E.Tag);

        EquipListLG.E.GetEquipmentList();

        RefreshRightUI();
    }

    public override void Close()
    {
        base.Close();

        EquipListLG.E.SelectedItem = null;
    }

    private void RefreshRightUI()
    {
        Default.gameObject.SetActive(EquipListLG.E.SelectedItem == null);
        Parameter.gameObject.SetActive(EquipListLG.E.SelectedItem != null);
    }

    public void RefreshCells(List<ItemEquipmentData> list)
    {
        ClearCell(Parent);
        ScrollView.verticalNormalizedPosition = 1;
        if (list == null)
            return;

        foreach (var item in list)
        {
            var cell = AddCell<EquipListCell>("Prefabs/UI/EquipListCell", Parent);
            cell.name = item.itemId.ToString();
            cell.Set(item);
        }
    }

    public void RefreshParameter()
    {
        if (EquipListLG.E.SelectedItem == null)
        {
            Logger.Warning("選択した装備がないです。");
            return;
        }

        EquipBtn.gameObject.SetActive(EquipListLG.E.SelectedItem.Data.IsLocked);
        AppraisalBtn.gameObject.SetActive(!EquipListLG.E.SelectedItem.Data.IsLocked);

        HP.text = "HP：" + EquipListLG.E.SelectedItem.Data.equipmentConfig.HP;
        Damage.text = "攻撃力：" + EquipListLG.E.SelectedItem.Data.equipmentConfig.Damage;
        Defense.text = "防御力：" + EquipListLG.E.SelectedItem.Data.equipmentConfig.Defnse;

        RefreshRightUI();

        OnEquiped(EquipListLG.E.SelectedItem.IsEquiped);
    }

    private void OnEquiped(bool b)
    {
        EquipBtn.enabled = !b;
        EquipBtn.GetComponent<Image>().color = b ? Color.grey : Color.white;
    }
}
