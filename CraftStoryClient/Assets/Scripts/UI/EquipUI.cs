using UnityEngine;
using UnityEngine.UI;

public class EquipUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text Level { get => FindChiled<Text>("Level"); }
    Text Exp { get => FindChiled<Text>("Exp"); }
    Text HP { get => FindChiled<Text>("HP"); }
    Text Damage { get => FindChiled<Text>("Damage"); }
    Text Defense { get => FindChiled<Text>("Defense"); }

    MyButton WeaponIcon { get => FindChiled<MyButton>("WeaponIcon"); }
    MyButton ArmorIcon { get => FindChiled<MyButton>("ArmorIcon"); }

    Transform Grid { get => FindChiled("Grid"); }
    EquipSkillCell[] cells;

    public override void Init()
    {
        base.Init();

        EquipLG.E.Init(this);

        Title.SetTitle("装備");
        Title.SetOnClose(Close);

        WeaponIcon.AddClickListener((index) => 
        {
            UICtl.E.OpenUI<EquipListUI>(UIType.EquipList, UIOpenType.None, ItemType.Weapon);
        });
        ArmorIcon.AddClickListener((index) =>
        {
            UICtl.E.OpenUI<EquipListUI>(UIType.EquipList, UIOpenType.None, ItemType.Armor);
        });

        cells = new EquipSkillCell[Grid.childCount];
        for (int i = 0; i < Grid.childCount; i++)
        {
            var cell = Grid.GetChild(i).GetComponent<EquipSkillCell>();
            if (cell == null)
                cell = Grid.GetChild(i).gameObject.AddComponent<EquipSkillCell>();
            
            cells[i] = cell;
        }
    }

    public override void Open()
    {
        base.Open();

        RefreshParameter();
        RefreshEquip();
    }

    /// <summary>
    /// パラメータ更新
    /// </summary>
    public void RefreshParameter()
    {
        Level.text = "レベル：" + DataMng.E.RuntimeData.Lv;

        int nextExp = ConfigMng.E.Character[PlayerCtl.E.Character.Parameter.Level].LvUpExp;
        Exp.text = string.Format("Exp：{0}/{1}", DataMng.E.RuntimeData.Exp, nextExp);

        HP.text = "HP：" + PlayerCtl.E.Character.Parameter.MaxHP;
        Damage.text = "攻撃力：" + PlayerCtl.E.Character.Parameter.Damage;
        Defense.text = "防御力：" + PlayerCtl.E.Character.Parameter.Defense;
    }

    /// <summary>
    /// 装備更新
    /// </summary>
    public void RefreshEquip()
    {
        foreach (var item in PlayerCtl.E.GetEquipedItems().Values)
        {
            switch ((ItemType)item.Config().Type)
            {
                case ItemType.Weapon: 
                    WeaponIcon.SetIcon(item.Config().IconResourcesPath);

                    if (item.AttachSkills != null)
                    {
                        for (int i = 0; i < cells.Length; i++)
                        {
                            if (item.AttachSkills.Length > i)
                            {
                                cells[i].Set(int.Parse(item.AttachSkills[i]));
                            }
                            else
                            {
                                cells[i].Set(-1);
                            }
                        }
                    }
                    break;

                case ItemType.Armor: 
                    ArmorIcon.SetIcon(item.Config().IconResourcesPath); 
                    break;
            }
        }
    }
}