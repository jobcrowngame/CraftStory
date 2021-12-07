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

    Text EquipmentHP { get => FindChiled<Text>("EquipmentHP"); }
    Text EquipmentDamage { get => FindChiled<Text>("EquipmentDamage"); }
    Text EquipmentDefense { get => FindChiled<Text>("EquipmentDefense"); }

    Text SkillHP { get => FindChiled<Text>("SkillHP"); }
    Text SkillDamage { get => FindChiled<Text>("SkillDamage"); }
    Text SkillDefense { get => FindChiled<Text>("SkillDefense"); }

    MyButton WeaponIcon { get => FindChiled<MyButton>("WeaponIcon"); }
    MyButton ArmorIcon { get => FindChiled<MyButton>("ArmorIcon"); }

    Transform Grid { get => FindChiled("Grid"); }
    EquipSkillCell[] cells;

    public override void Init()
    {
        base.Init();

        EquipLG.E.Init(this);

        Title.SetTitle("装備");
        Title.SetOnClose(() =>
        {
            Close();
            if (DataMng.E.MapData.Config.MapType == (int)MapType.Guide)
            {
                GuideLG.E.Next();
            }
        });

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


        // 冒険エリアの場合、時間を停止
        if (DataMng.E.RuntimeData.MapType == MapType.Brave)
        {
            PlayerCtl.E.Pause();
        }
    }

    public override void Close()
    {
        base.Close();
        PlayerCtl.E.Pause(false);
    }

    /// <summary>
    /// パラメータ更新
    /// </summary>
    public void RefreshParameter()
    {
        Level.text = "レベル：" + DataMng.E.RuntimeData.Lv;

        int nextExp = ConfigMng.E.Character[PlayerCtl.E.Character.Parameter.Level].LvUpExp;
        Exp.text = string.Format("Exp：{0}/{1}", (int)DataMng.E.RuntimeData.Exp, nextExp);

        HP.text = string.Format("HP      ：{0} <color=yellow>(+{1})</color>", 
            PlayerCtl.E.Character.Parameter.config.HP, 
            PlayerCtl.E.Character.Parameter.Equipment.HP + PlayerCtl.E.Character.Parameter.skillPar.HP);

        Damage.text = string.Format("攻撃力：{0} <color=yellow>(+{1})</color>",
            PlayerCtl.E.Character.Parameter.config.Damage,
            PlayerCtl.E.Character.Parameter.Equipment.Damage + PlayerCtl.E.Character.Parameter.skillPar.Damage);

        Defense.text = string.Format("防御力：{0} <color=yellow>(+{1})</color>",
            PlayerCtl.E.Character.Parameter.config.Defense,
            PlayerCtl.E.Character.Parameter.Equipment.Defense + PlayerCtl.E.Character.Parameter.skillPar.Defense);
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

        EquipmentHP.text = string.Format("HP      ：{0}", PlayerCtl.E.Character.Parameter.Equipment.HP);
        EquipmentDamage.text = string.Format("攻撃力：{0}", PlayerCtl.E.Character.Parameter.Equipment.Damage);
        EquipmentDefense.text = string.Format("防御力：{0}", PlayerCtl.E.Character.Parameter.Equipment.Defense);

        SkillHP.text = string.Format("HP      ：{0}", PlayerCtl.E.Character.Parameter.skillPar.HP);
        SkillDamage.text = string.Format("攻撃力：{0}", PlayerCtl.E.Character.Parameter.skillPar.Damage);
        SkillDefense.text = string.Format("防御力：{0}", PlayerCtl.E.Character.Parameter.skillPar.Defense);
    }
}